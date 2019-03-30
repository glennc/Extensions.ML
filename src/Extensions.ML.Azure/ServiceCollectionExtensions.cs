using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.ML.Azure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, string name, string connectionString, string fileName) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(name, options => options.CreateModel = (context) => BlobModelCreator.CreateModel(context, connectionString, fileName));
            return services;
        }
    }
}
