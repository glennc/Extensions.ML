using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public class PredictionEngineOptions<TData, TPrediction> where TData : class where TPrediction : class, new()
    {
        public string ModelPath { get; set; }
    }
}
