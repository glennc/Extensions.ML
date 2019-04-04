using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Extensions.ML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private PredictionEnginePool<SentimentIssue, SentimentPrediction> _predictionEnginePool;

        public PredictionController(PredictionEnginePool<SentimentIssue, SentimentPrediction> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpGet()]
        public ActionResult<SentimentPrediction> GetSentiment([FromQuery]SentimentIssue input)
        {
            return _predictionEnginePool.Predict(input);
        }

        [HttpGet("/new")]
        public ActionResult<SentimentPrediction> GetNewSentiment([FromQuery]SentimentIssue input)
        {
            return _predictionEnginePool.Predict("newModel", input);
        }
    }
}
