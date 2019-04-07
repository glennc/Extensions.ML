﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var timer = Stopwatch.StartNew();
            Logger.FileReloadBegin(_logger, _filePath);

            var previousToken = Interlocked.Exchange(ref _reloadToken, new ModelReloadToken());
            lock (_lock)
            {
                LoadModel();
                Logger.ReloadingFile(_logger, _filePath, timer.Elapsed);
            }
            previousToken.OnReload();
            timer.Stop();
            Logger.FileReloadEnd(_logger, _filePath, timer.Elapsed);
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

        internal static class EventIds
        {
            public static readonly EventId FileReloadBegin = new EventId(100, "FileReloadBegin");
            public static readonly EventId FileReloadEnd = new EventId(101, "FileReloadEnd");
            public static readonly EventId FileReload = new EventId(102, "FileReload");
        }

        private static class Logger
        {
            private static readonly Action<ILogger, string, Exception> _fileLoadBegin = LoggerMessage.Define<string>(
                LogLevel.Debug,
                EventIds.FileReloadBegin,
                "File reload for '{filePath}'");

            private static readonly Action<ILogger, string, double, Exception> _fileLoadEnd = LoggerMessage.Define<string, double>(
                LogLevel.Debug,
                EventIds.FileReloadEnd,
                "File reload for '{filePath}' completed after {ElapsedMilliseconds}ms");

            private static readonly Action<ILogger, string, double, Exception> _fileReLoad = LoggerMessage.Define<string, double>(
                LogLevel.Information,
                EventIds.FileReloadEnd,
                "Reloading file '{filePath}' completed after {ElapsedMilliseconds}ms");

            public static void FileReloadBegin(ILogger logger, string filePath)
            {
                _fileLoadBegin(logger, filePath, null);
            }

            public static void FileReloadEnd(ILogger logger, string filePath, TimeSpan duration)
            {
                _fileLoadEnd(logger, filePath, duration.TotalMilliseconds, null);
            }

            public static void ReloadingFile(ILogger logger, string filePath, TimeSpan duration)
            {
                _fileReLoad(logger, filePath, duration.TotalMilliseconds, null);
            }
        }

    }
}