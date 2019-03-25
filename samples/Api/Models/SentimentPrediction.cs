using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class SentimentPrediction
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public bool IsToxic { get; set; }

        //Question:Isn't this column redundant? What we tell customers to do here?
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
