using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public static class PredictionEnginePoolExtensions
    {
        public static TPrediction Predict<TData, TPrediction>(this PredictionEnginePool<TData, TPrediction> predictionEnginePool, TData dataSample) where TData : class where TPrediction : class, new()
        {
            return predictionEnginePool.Predict(string.Empty, dataSample);
        }

        public static TPrediction Predict<TData, TPrediction>(this PredictionEnginePool<TData, TPrediction> predictionEnginePool, string modelName, TData dataSample) where TData : class where TPrediction : class, new()
        {
            var predictionEngine = predictionEnginePool.GetPredictionEngine(modelName);

            try
            {
                return predictionEngine.Predict(dataSample);
            }
            finally
            {
                predictionEnginePool.ReturnPredictionEngine(modelName, predictionEngine);
            }
        }
    }
}
