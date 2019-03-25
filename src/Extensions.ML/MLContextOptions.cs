using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public class MLContextOptions
    {
        public MLContext MLContext { get; set; } = new MLContext();
    }
}
