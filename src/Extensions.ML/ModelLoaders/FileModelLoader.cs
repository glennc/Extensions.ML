using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.ML;

namespace Extensions.ML
{
    internal class FileModelLoader : ModelLoader, IDisposable
    {
        private ILogger<FileModelLoader> _logger;
        private string _filePath;
        private FileSystemWatcher _watcher;
        private ModelReloadToken _reloadToken;
        private ITransformer _model;

        private MLContext _context;

        private object _lock = new object();

        public FileModelLoader(IOptions<MLContextOptions> contextOptions, ILogger<FileModelLoader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = contextOptions.Value?.MLContext ?? throw new ArgumentNullException(nameof(contextOptions));
        }

        public void Start(string filePath, bool watchFile)
        {
            _filePath = filePath;
            _reloadToken = new ModelReloadToken();

            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The provided model file {filePath} doesn't exist.");
            }

            var directory = Path.GetDirectoryName(filePath);

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            var file = Path.GetFileName(filePath);

            LoadModel();

            if (watchFile)
            {
                _watcher = new FileSystemWatcher(directory, file);
                _watcher.EnableRaisingEvents = true;
                _watcher.Changed += _watcher_Changed;
            }
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Reloading model from file {filePath}", _filePath);

            var previousToken = Interlocked.Exchange(ref _reloadToken, new ModelReloadToken());
            lock (_lock)
            {
                LoadModel();
            }
            previousToken.OnReload();
        }

        public override IChangeToken GetReloadToken()
        {
            if (_reloadToken == null) throw new InvalidOperationException("Start must be called on a ModelLoader before it can be used.");
            return _reloadToken;
        }

        public override ITransformer GetModel()
        {
            if (_model == null) throw new InvalidOperationException("Start must be called on a ModelLoader before it can be used.");

            return _model;
        }

        //internal virtual for testing purposes.
        internal virtual void LoadModel()
        {
            //Sleep to avoid some file system locking issues
            //TODO: The same thing occurs in configuration reload
            //we should make sure the sleeps are the same.
            Thread.Sleep(10);
            using (var fileStream = File.OpenRead(_filePath))
            {
                _model = _context.Model.Load(fileStream);
            }
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }

    }
}
