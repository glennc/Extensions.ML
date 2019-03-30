using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public static class PredictionEnginePoolExtensions
    {
        public static TPrediction Predict<TData, TPrediction>(this PredictionEnginePool<TData, TPrediction> predictionEnginePool, TData dataSample) where TData : class where TPrediction : class, new()
        {
            return predictionEnginePool.Predict(dataSample, string.Empty);
        }

        public static TPrediction Predict<TData, TPrediction>(this PredictionEnginePool<TData, TPrediction> predictionEnginePool, TData dataSample, string modelName) where TData : class where TPrediction : class, new()
        {
            var predictionEngine = predictionEnginePool.GetPredictionEngine(modelName);

            try
            {
                return predictionEngine.Predict(dataSample);
            }
            finally
            {
                predictionEnginePool.ReturnPredictionEngine(predictionEngine, modelName);
            }
        }
    }
}
