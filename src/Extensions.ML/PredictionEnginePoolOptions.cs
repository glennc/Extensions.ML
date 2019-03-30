using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.ML
{
    public class PredictionEnginePoolOptions<TData, TPrediction> where TData : class where TPrediction : class, new()
    {
        public Func<MLContext, ITransformer> CreateModel { get; set; }
    }
}
