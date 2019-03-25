using BenchmarkDotNet.Attributes;
using Extensions.ML;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace micro
{
    [CoreJob]
    [SimpleJob(launchCount: 3)]
    public class PredictionBenchmark
    {
        //private MLContext _context;
        //private ITransformer _trainedModel;
        //private PooledPredictionEngine<SentimentIssue, SentimentPrediction> _pooled;

        //[GlobalSetup]
        //public void Setup()
        //{
        //    _context = new MLContext();
        //    using (FileStream stream = new FileStream("SentimentModel.zip", FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        _trainedModel = _context.Model.Load(stream);
        //    }

        //    _pooled = new PooledPredictionEngine<SentimentIssue, SentimentPrediction>("SentimentModel.zip");
        //}

        //[Benchmark]
        //public void CreateAndPredict()
        //{
        //    var engine = _trainedModel.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(_context);
        //    engine.Predict(new SentimentIssue { Text = "Hello" });
        //}

        //[Benchmark]
        //public void PooledPredict()
        //{
        //    //TODO: This is presumably only really testing the case where we re-use the
        //    //same object over and over.
        //    _pooled.Predict(new SentimentIssue { Text = "Hello" });
        //}
    }

}