using Microsoft.Extensions.ObjectPool;
using Microsoft.ML;

namespace Extensions.ML
{
    public class PredictionEnginePoolPolicy<TData, TPrediction> : IPooledObjectPolicy<PredictionEngine<TData, TPrediction>>
                    where TData : class
                    where TPrediction : class, new()
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public PredictionEnginePoolPolicy(MLContext mlContext, ITransformer model)
        {
            _mlContext = mlContext;
            _model = model;
        }

        public PredictionEngine<TData, TPrediction> Create()
        {
            return _model.CreatePredictionEngine<TData, TPrediction>(_mlContext);
        }

        public bool Return(PredictionEngine<TData, TPrediction> obj)
        {
            return obj != null;
        }
    }
}