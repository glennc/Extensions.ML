using Api.Models;
using Extensions.ML;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DetoxController : ControllerBase
    {
        private PredictionEnginePool<SampleObservation, SamplePrediction> _predictionEnginePool;

        public DetoxController(PredictionEnginePool<SampleObservation, SamplePrediction> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpGet()]
        public ActionResult<SamplePrediction> GetSentiment([FromQuery]SampleObservation input)
        {
            return _predictionEnginePool.Predict(input);
        }
    }
}
