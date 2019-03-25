using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class SentimentObservation
    {
        [ColumnName("Text")]
        public string SentimentText { get; set; }

    }
}
