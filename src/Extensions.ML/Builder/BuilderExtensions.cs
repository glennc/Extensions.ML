using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public static class extensions
    {

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromUri<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string uri) where TData : class where TPrediction : class, new()
        {
            return builder.FromUri(string.Empty, new Uri(uri));
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromUri<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string modelName, string uri) where TData : class where TPrediction : class, new()
        {
            return builder.FromUri(modelName, new Uri(uri));
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromUri<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string modelName, Uri uri) where TData : class where TPrediction : class, new()
        {
            return builder.FromUri(modelName, uri, TimeSpan.FromMinutes(5));
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromUri<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string modelName, string uri, TimeSpan period) where TData : class where TPrediction : class, new()
        {
            return builder.FromUri(modelName, new Uri(uri), period);
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromUri<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string modelName, Uri uri, TimeSpan period) where TData : class where TPrediction : class, new()
        {
            builder.Services.AddTransient<UriModelLoader, UriModelLoader>();
            builder.Services.AddOptions<PredictionEnginePoolOptions<TData, TPrediction>>(modelName)
                    .Configure<UriModelLoader>((opt, loader) =>
                    {
                        loader.Start(uri, period);
                        opt.ModelLoader = loader;
                    });
            return builder;
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromFile<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string filePath) where TData : class where TPrediction : class, new()
        {
            return builder.FromFile(string.Empty, filePath, true);
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromFile<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string modelName, string filePath) where TData : class where TPrediction : class, new()
        {
            return builder.FromFile(modelName, filePath, true);
        }

        public static IPredictionEnginePoolBuilder<TData, TPrediction> FromFile<TData, TPrediction>(this IPredictionEnginePoolBuilder<TData, TPrediction> builder, string modelName, string filePath, bool watchForChanges) where TData : class where TPrediction : class, new()
        {
            builder.Services.AddTransient<FileModelLoader, FileModelLoader>();
            builder.Services.AddOptions<PredictionEnginePoolOptions<TData, TPrediction>>(modelName)
                    .Configure<FileModelLoader>((options, loader) =>
                    {
                        loader.Start(filePath, watchForChanges);
                        options.ModelLoader = loader;
                    });
            return builder;
        }
    }
}
