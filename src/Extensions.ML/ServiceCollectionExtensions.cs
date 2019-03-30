using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Extensions.ML
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, string modelPath) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(modelPath, string.Empty);
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, Stream modelStream) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(modelStream, string.Empty);
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, Stream modelStream, string modelName) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(modelName, options => options.CreateModel = (mlContext) => {
                    return mlContext.Model.Load(modelStream);
            });
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, string modelPath, string modelName) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(modelName, options => options.CreateModel = (mlContext) => {
                using (var fileStream = File.OpenRead(modelPath))
                {
                    return mlContext.Model.Load(fileStream);
                }
            });
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, Uri modelUri, string modelName) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(modelName,  options => options.CreateModel = (mlContext) => {
                using (var client = new HttpClient())
                {
                    using (var stream = client.GetStreamAsync(modelUri).GetAwaiter().GetResult())
                    {
                        return mlContext.Model.Load(stream);
                    }

                }
            });
            return services;
        }

        public static void FromUri<TData, TPrediction>(this PredictionEnginePoolOptions<TData, TPrediction> options, string modelUri) where TData : class where TPrediction : class, new()
        {
            options.FromUri(new Uri(modelUri));
        }

        public static void FromUri<TData, TPrediction>(this PredictionEnginePoolOptions<TData, TPrediction> options, Uri modelUri) where TData : class where TPrediction : class, new()
        {
            options.CreateModel = (mlContext) =>
            {
                using (var client = new HttpClient())
                {
                    using (var stream = client.GetStreamAsync(modelUri).GetAwaiter().GetResult())
                    {
                        return mlContext.Model.Load(stream);
                    }
                }
            };
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, string name, Action<PredictionEnginePoolOptions<TData, TPrediction>> configure) where TData : class where TPrediction : class, new()
        {
            services.AddLogging();
            services.AddOptions();

            services.Configure(name, configure);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<MLContextOptions>, PostMLContextOptionsConfiguration>());
            services.AddSingleton<PredictionEnginePool<TData, TPrediction>, PredictionEnginePool<TData, TPrediction>>();
            return services;
        }
    }
}
