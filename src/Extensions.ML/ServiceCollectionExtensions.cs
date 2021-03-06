﻿using Microsoft.Extensions.DependencyInjection;
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

        public static IPredictionEnginePoolBuilder<TData, TPrediction> AddPredictionEnginePool<TData, TPrediction>(this IServiceCollection services) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngineServices<TData, TPrediction>();
            return new DefaultPredictionEnginePoolBuilder<TData, TPrediction>(services, string.Empty);
        }

        internal static IServiceCollection AddPredictionEngineServices<TData, TPrediction>(this IServiceCollection services) where TData : class where TPrediction : class, new()
        {
            services.AddLogging();
            services.AddOptions();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<MLContextOptions>, PostMLContextOptionsConfiguration>());
            services.AddSingleton<PredictionEnginePool<TData, TPrediction>, PredictionEnginePool<TData, TPrediction>>();
            return services;
        }

    }
}
