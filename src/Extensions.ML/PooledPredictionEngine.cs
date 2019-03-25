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
    public class PooledPredictionEngine<TData, TPrediction> : IPredictionEngine<TData,TPrediction>
                        where TData : class
                        where TPrediction : class, new()
    {
        private readonly ObjectPool<PredictionEngine<TData, TPrediction>> _predictionEnginePool;
        private readonly MLContextOptions _mlContextOptions;
        private readonly PredictionEngineOptions<TData, TPrediction> _predictionEngineOptions;

        public PooledPredictionEngine(IOptions<MLContextOptions> mlContextOptions, IOptions<PredictionEngineOptions<TData, TPrediction>> predictionEngineOptions)
        {
            _mlContextOptions = mlContextOptions.Value;
            _predictionEngineOptions = predictionEngineOptions.Value;
  
            using (var fileStream = File.OpenRead(_predictionEngineOptions.ModelPath))
            {
                Model = _mlContextOptions.MLContext.Model.Load(fileStream);
            }

            var predictionEnginePolicy = new PooledPredictionEnginePolicy<TData, TPrediction>(_mlContextOptions.MLContext, Model);
            _predictionEnginePool = new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(predictionEnginePolicy);
        }


        public ITransformer Model { get; private set; }

        public TPrediction Predict(TData dataSample)
        {
            var predictionEngine = _predictionEnginePool.Get();

            try
            {
               return predictionEngine.Predict(dataSample);
            }
            finally
            {
                _predictionEnginePool.Return(predictionEngine);
            }
        }

        public IDataView PredictMany(IDataView testDataView)
        {
            return Model.Transform(testDataView);
        }
    }
}
