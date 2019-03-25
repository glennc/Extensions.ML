using BenchmarkDotNet.Attributes;
using Extensions.ML;
using Microsoft.ML;
using System;
using System.IO;

namespace micro
{
    [CoreJob]
    public class CreationBenchmark
    {
        private MLContext _context;
        private ITransformer _trainedModel;

        [GlobalSetup]
        public void Setup()
        {
            _context = new MLContext();
            using (FileStream stream = new FileStream("SentimentModel.zip", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _trainedModel = _context.Model.Load(stream);
            }
        }

        [Benchmark]
        public PredictionEngine<SentimentIssue, SentimentPrediction> CreatePredictionEngine()
        {
            return _trainedModel.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(_context);
        }
    }

}