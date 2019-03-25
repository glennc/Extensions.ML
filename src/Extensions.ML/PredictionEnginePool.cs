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
        private readonly PredictionEnginePoolOptions<TData, TPrediction> _predictionEngineOptions;
        private readonly ObjectPool<PredictionEngine<TData, TPrediction>> _predictionEnginePool;

        public PredictionEnginePool(IOptions<MLContextOptions> mlContextOptions, IOptions<PredictionEnginePoolOptions<TData, TPrediction>> predictionEngineOptions)
        {
            _mlContextOptions = mlContextOptions.Value;
            _predictionEngineOptions = predictionEngineOptions.Value;
 
            Model = _predictionEngineOptions.CreateModel(_mlContextOptions.MLContext);

            var predictionEnginePolicy = new PredictionEnginePoolPolicy<TData, TPrediction>(_mlContextOptions.MLContext, Model);
            _predictionEnginePool = new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(predictionEnginePolicy);
        }


        public ITransformer Model { get; private set; }

        public PredictionEngine<TData, TPrediction> GetPredictionEngine()
        {
            return _predictionEnginePool.Get();
        }

        public void ReturnPredictionEngine(PredictionEngine<TData, TPrediction> engine)
        {
            _predictionEnginePool.Return(engine);
        }
    }
}
