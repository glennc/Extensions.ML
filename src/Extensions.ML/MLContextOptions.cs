using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public class MLContextOptions
    {
        public MLContext MLContext { get; set; } = new MLContext();
    }

    public class PostMLContextOptionsConfiguration : IPostConfigureOptions<MLContextOptions>
    {
        private readonly ILogger<MLContext> _logger;

        public PostMLContextOptionsConfiguration(ILogger<MLContext> logger)
        {
            _logger = logger;
        }

        public void PostConfigure(string name, MLContextOptions options)
        {
            options.MLContext.Log += Log;
        }

        private void Log(object sender, LoggingEventArgs e)
        {
            _logger.LogTrace(e.Message);
        }
    }
}
