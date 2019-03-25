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
        private IPredictionEngine<SentimentObservation, SentimentPrediction> _predictionEngine;

        public PredictionController(IPredictionEngine<SentimentObservation, SentimentPrediction> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        [HttpGet("/{input}")]
        public ActionResult<SentimentPrediction> Get(string input)
        {
            //TODO: Do a model binding talk to Eric about making sure these are always POCOs and people can do that.
            return _predictionEngine.Predict(new SentimentObservation { SentimentText = input });
        }
    }
}
