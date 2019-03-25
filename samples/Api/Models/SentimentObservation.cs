using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class SentimentObservation
    {
        [Required]
        [ColumnName("Text")]
        public string SentimentText { get; set; }

    }
}
