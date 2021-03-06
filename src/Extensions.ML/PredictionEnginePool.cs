﻿using Microsoft.Extensions.Options;
using Microsoft.ML;
using System;
using System.Collections.Concurrent;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly PoolLoader<TData,TPrediction> _defaultEnginePool;
        private readonly Dictionary<string, PoolLoader<TData, TPrediction>> _namedPools;

        public PredictionEnginePool(IServiceProvider serviceProvider,
                                    IOptions<MLContextOptions> mlContextOptions,
                                    IOptionsFactory<PredictionEnginePoolOptions<TData, TPrediction>> predictionEngineOptions)
        {
            _mlContextOptions = mlContextOptions.Value;
            _predictionEngineOptions = predictionEngineOptions;
            _serviceProvider = serviceProvider;

            var defaultOptions = _predictionEngineOptions.Create(string.Empty);

            if (defaultOptions.ModelLoader != null)
            {
                _defaultEnginePool = new PoolLoader<TData, TPrediction>(_serviceProvider, defaultOptions);
            }

            _namedPools = new Dictionary<string, PoolLoader<TData, TPrediction>>();
        }

        /// <summary>
        /// Get the Model used to create the pooled PredictionEngine.
        /// </summary>
        /// 
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ITransformer GetModel(string modelName)
        {
            return _namedPools[modelName].Loader.GetModel();
        }

        /// <summary>
        /// Get the Model used to create the pooled PredictionEngine.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ITransformer GetModel()
        {
            return _defaultEnginePool.Loader.GetModel();
        }

        public PredictionEngine<TData, TPrediction> GetPredictionEngine()
        {
            return GetPredictionEngine(string.Empty);
        }

        public PredictionEngine<TData, TPrediction> GetPredictionEngine(string modelName)
        {
            if (_namedPools.ContainsKey(modelName))
            {
                return _namedPools[modelName].PredictionEnginePool.Get();
            }

            //This is the case where someone has used string.Empty to get the default model.
            //We can throw all the time, but it seems reasonable that we would just do what 
            //they are expecting if they know that an empty string means default.
            if (string.IsNullOrEmpty(modelName))
            {
                if (_defaultEnginePool == null)
                {
                    throw new ArgumentException("You need to configure a default, not named, model before you use this method.");
                }

               return _defaultEnginePool.PredictionEnginePool.Get();
            }

            //Here we are in the world of named models where the model hasn't been built yet.
            var options = _predictionEngineOptions.Create(modelName);
            var pool = new PoolLoader<TData, TPrediction>(_serviceProvider, options);
            _namedPools.Add(modelName, pool);
            return pool.PredictionEnginePool.Get();
        }

        public void ReturnPredictionEngine(PredictionEngine<TData, TPrediction> engine)
        {
            ReturnPredictionEngine(string.Empty, engine);
        }

        public void ReturnPredictionEngine(string modelName, PredictionEngine<TData, TPrediction> engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            if (string.IsNullOrEmpty(modelName))
            {
                _defaultEnginePool.PredictionEnginePool.Return(engine);
            }
            else
            {
                _namedPools[modelName].PredictionEnginePool.Return(engine);
            }
        }
    }
}
