using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.ML;

namespace Extensions.ML
{
    internal class UriModelLoader : ModelLoader, IDisposable
    {
        //TODO: THis should be able to be removed for HeaderNames.ETag
        private const string ETAG_HEADER = "ETag";
        private readonly MLContext _context;
        private TimeSpan? _timerPeriod;
        private Uri _uri;
        private ITransformer _model;
        private ModelReloadToken _reloadToken;
        private Timer _reloadTimer;
        private object _reloadTimerLock = new object();
        private string _eTag;
        private readonly ILogger _logger;
        private CancellationTokenSource _stopping;
        private bool _started;
        private int _timeoutMilliseconds = 60000;

        public UriModelLoader(IOptions<MLContextOptions> contextOptions, ILogger<UriModelLoader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = contextOptions.Value?.MLContext;
            _reloadToken = new ModelReloadToken();
            _stopping = new CancellationTokenSource();
            _started = false;
        }

        internal void Start(Uri uri, TimeSpan period)
        {
            _timerPeriod = period;
            _uri = uri;
            if (LoadModel().GetAwaiter().GetResult())
            {
                StartReloadTimer();
            }
            _started = true;
        }

        private async void ReloadTimerTick(object state)
        {
            StopReloadTimer();

            await RunAsync();

            StartReloadTimer();
        }

        internal bool IsStopping => _stopping.IsCancellationRequested;

        internal async Task RunAsync()
        {
            CancellationTokenSource cancellation = null;
            //TODO: Switch to ValueStopWatch
            var duration = Stopwatch.StartNew();
            try
            {
                cancellation = CancellationTokenSource.CreateLinkedTokenSource(_stopping.Token);
                cancellation.CancelAfter(_timeoutMilliseconds);
                Logger.UriReloadBegin(_logger, _uri);

                var eTagMatches = await MatchEtag(_uri, _eTag);
                if (!eTagMatches)
                {
                    await LoadModel();
                    var previousToken = Interlocked.Exchange(ref _reloadToken, new ModelReloadToken());
                    previousToken.OnReload();
                }

                Logger.UriReloadEnd(_logger, _uri, duration.Elapsed);
            }
            catch (OperationCanceledException) when (IsStopping)
            {
                // This is a cancellation - if the app is shutting down we want to ignore it.
            }
            catch (Exception ex)
            {
                Logger.UriReloadError(_logger, _uri, duration.Elapsed, ex);
            }
            finally
            {
                cancellation.Dispose();
            }
        }

        internal virtual async Task<bool> MatchEtag(Uri uri, string eTag)
        {
            using (var client = new HttpClient())
            {
                var headRequest = new HttpRequestMessage(HttpMethod.Head, uri);
                var resp = await client.SendAsync(headRequest);

                return resp.Headers.GetValues(ETAG_HEADER).First() != eTag;
            }
        }

        internal void StartReloadTimer()
        {
            lock (_reloadTimerLock)
            {
                if (_reloadTimer == null)
                {
                    _reloadTimer = new Timer(ReloadTimerTick, this, _timerPeriod.Value.Milliseconds, Timeout.Infinite);
                }
            }
        }

        internal void StopReloadTimer()
        {
            lock (_reloadTimerLock)
            {
                _reloadTimer.Dispose();
                _reloadTimer = null;
            }
        }

        internal virtual async Task<bool> LoadModel()
        {
            //TODO: We probably need some sort of retry policy for this.
            try
            {
                using (var client = new HttpClient())
                {
                    var resp = await client.GetAsync(_uri);
                    using (var stream = await resp.Content.ReadAsStreamAsync())
                    {
                        _model = _context.Model.Load(stream);
                    }

                    if (resp.Headers.Contains(ETAG_HEADER))
                    {
                        _eTag = resp.Headers.GetValues(ETAG_HEADER).First();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading {uri}", _uri);
                throw;
            }
        }

        public override ITransformer GetModel()
        {
            if (!_started) throw new InvalidOperationException("Start must be called on a ModelLoader before it can be used.");

            return _model;
        }

        public override IChangeToken GetReloadToken()
        {
            if (!_started) throw new InvalidOperationException("Start must be called on a ModelLoader before it can be used.");

            return _reloadToken;
        }

        public void Dispose()
        {
            _reloadTimer?.Dispose();
        }

        internal static class EventIds
        {
            public static readonly EventId UriReloadBegin = new EventId(100, "UriReloadBegin");
            public static readonly EventId UriReloadEnd = new EventId(101, "UriReloadEnd");
            public static readonly EventId UriReloadError = new EventId(102, "UriReloadError");
            public static readonly EventId UriReloadTimeout = new EventId(103, "UriReloadTimeout");
        }

        private static class Logger
        {
            private static readonly Action<ILogger, Uri, Exception> _uriReloadBegin = LoggerMessage.Define<Uri>(
                LogLevel.Debug,
                EventIds.UriReloadBegin,
                "URI reload '{uri}'");

            private static readonly Action<ILogger, Uri, double, Exception> _uriReloadEnd = LoggerMessage.Define<Uri, double>(
                LogLevel.Debug,
                EventIds.UriReloadEnd,
                "URI reload '{uri}' completed after {ElapsedMilliseconds}ms");

            private static readonly Action<ILogger, Uri, double, Exception> _uriReloadError = LoggerMessage.Define<Uri, double>(
                LogLevel.Error,
                EventIds.UriReloadError,
                "URI reload for {uri} threw an unhandled exception after {ElapsedMilliseconds}ms");

            private static readonly Action<ILogger, Uri, double, Exception> _uriReloadTimeout = LoggerMessage.Define<Uri, double>(
                LogLevel.Error,
                EventIds.UriReloadTimeout,
                "URI reload for {uri} was canceled after {ElapsedMilliseconds}ms");

            public static void UriReloadBegin(ILogger logger, Uri uri)
            {
                _uriReloadBegin(logger, uri, null);
            }

            public static void UriReloadEnd(ILogger logger, Uri uri, TimeSpan duration)
            {
                _uriReloadEnd(logger, uri, duration.TotalMilliseconds, null);
            }

            public static void UriReloadError(ILogger logger, Uri uri, TimeSpan duration, Exception exception)
            {
                _uriReloadError(logger, uri, duration.TotalMilliseconds, exception);
            }

            public static void UriReloadTimeout(ILogger logger, Uri uri, TimeSpan duration)
            {
                _uriReloadTimeout(logger, uri, duration.TotalMilliseconds, null);
            }
        }
    }
}
