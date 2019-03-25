using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public class PredictionEnginePoolOptions<TData, TPrediction> where TData : class where TPrediction : class, new()
    {
        public Func<MLContext, ITransformer> CreateModel { get; set; }
    }
}
