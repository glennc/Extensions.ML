using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public interface IPredictionEnginePoolBuilder<TData, TPrediction> where TData : class where TPrediction : class, new()
    {
        IServiceCollection Services { get; }

        string ModelName { get; }
    }

    public class DefaultPredictionEnginePoolBuilder<TData, TPrediction> : IPredictionEnginePoolBuilder<TData, TPrediction> where TData : class where TPrediction : class, new()
    {
        public DefaultPredictionEnginePoolBuilder(IServiceCollection services, string modelName)
        {
            Services = services ?? throw new ArgumentException(nameof(services));
            ModelName = modelName;      
        }
        public IServiceCollection Services { get; private set; }

        public string ModelName { get; private set; }
    }
}
