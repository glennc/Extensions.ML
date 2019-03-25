using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.ML
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, string modelPath) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(options => options.ModelPath = modelPath);
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, Action<PredictionEngineOptions<TData, TPrediction>> configure) where TData : class where TPrediction : class, new()
        {
            services.Configure(configure);
            services.AddSingleton<IPredictionEngine<TData, TPrediction>, PooledPredictionEngine<TData, TPrediction>>();
            return services;
        }
    }
}
