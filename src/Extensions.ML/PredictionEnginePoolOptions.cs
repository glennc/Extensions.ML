namespace Extensions.ML
{
    public class PredictionEnginePoolOptions<TData, TPrediction>
        where TData : class
        where TPrediction : class, new()
    {
        public ModelLoader ModelLoader { get; set; }
    }
}