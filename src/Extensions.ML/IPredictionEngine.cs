using Microsoft.Data.DataView;

namespace Extensions.ML
{
    public interface IPredictionEngine<TData, TPrediction>
    {
        TPrediction Predict(TData dataSample);

        IDataView PredictMany(IDataView testDataView);
    }
}