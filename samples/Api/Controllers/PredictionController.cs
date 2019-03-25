using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Extensions.ML;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private PredictionEnginePool<SentimentObservation, SentimentPrediction> _predictionEngine;

        public PredictionController(PredictionEnginePool<SentimentObservation, SentimentPrediction> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        [HttpPost()]
        public ActionResult<SentimentPrediction> GetSentiment([FromBody]SentimentObservation input)
        {
            return _predictionEngine.Predict(input);
        }
    }
}
