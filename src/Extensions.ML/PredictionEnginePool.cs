using Microsoft.Data.DataView;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Extensions.ML
{
    public class PredictionEnginePool<TData, TPrediction>
                        where TData : class
                        where TPrediction : class, new()
    {
        private readonly MLContextOptions _mlContextOptions;
        private readonly IOptionsFactory<PredictionEnginePoolOptions<TData, TPrediction>> _predictionEngineOptions;
        private readonly ITransformer _defaultModel;
        private readonly ObjectPool<PredictionEngine<TData, TPrediction>> _defaultPredictionEnginePool;
        private readonly Dictionary<string, ObjectPool<PredictionEngine<TData, TPrediction>>> _namedPools;

        public PredictionEnginePool(IOptions<MLContextOptions> mlContextOptions, IOptionsFactory<PredictionEnginePoolOptions<TData, TPrediction>> predictionEngineOptions)
        {
            _mlContextOptions = mlContextOptions.Value;
            _predictionEngineOptions = predictionEngineOptions;


            var defaultOptions = _predictionEngineOptions.Create(string.Empty);

            if (defaultOptions.CreateModel != null)
            {
                _defaultModel = defaultOptions.CreateModel(_mlContextOptions.MLContext);
                var predictionEnginePolicy = new PredictionEnginePoolPolicy<TData, TPrediction>(_mlContextOptions.MLContext, _defaultModel);
                _defaultPredictionEnginePool = new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(predictionEnginePolicy);
            }
            _namedPools = new Dictionary<string, ObjectPool<PredictionEngine<TData, TPrediction>>>();
        }

        public PredictionEngine<TData, TPrediction> GetPredictionEngine()
        {
            if (_defaultPredictionEnginePool == null)
            {
                throw new ArgumentException("You need to configure a default, not named, model before you use this method.");
            }

            return _defaultPredictionEnginePool.Get();
        }

        public void ReturnPredictionEngine(PredictionEngine<TData, TPrediction> engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            if (_defaultPredictionEnginePool == null)
            {
                throw new ArgumentException("You need to configure a default, not named, model before you use this method.");
            }

            _defaultPredictionEnginePool.Return(engine);
        }

        public PredictionEngine<TData, TPrediction> GetPredictionEngine(string modelName)
        {
            if (_namedPools.ContainsKey(modelName))
            {
                return _namedPools[modelName].Get();
            }

            var options = _predictionEngineOptions.Create(modelName);
            var model = options.CreateModel(_mlContextOptions.MLContext);
            var pool = new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(new PredictionEnginePoolPolicy<TData, TPrediction>(_mlContextOptions.MLContext, model));

            _namedPools.Add(modelName, pool);
            return pool.Get();
        }

        public void ReturnPredictionEngine(PredictionEngine<TData, TPrediction> engine, string modelName)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            _namedPools[modelName].Return(engine);
        }
    }
}
