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
        private PredictionEnginePool<SentimentObservation, SentimentPrediction> _predictionEnginePool;

        public PredictionController(PredictionEnginePool<SentimentObservation, SentimentPrediction> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpPost()]
        public ActionResult<SentimentPrediction> GetSentiment(SentimentObservation input)
        {
            return _predictionEnginePool.Predict(input);
        }
    }
}
