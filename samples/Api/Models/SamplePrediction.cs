using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class SamplePrediction
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public Boolean Prediction { get; set; }
        public float[] Score { get; set; }
    }
}
