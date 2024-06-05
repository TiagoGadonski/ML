using Microsoft.ML;
using Microsoft.ML.Data;

public class CryptoPrediction
{
    private ITransformer _model;
    private MLContext _context;

    public CryptoPrediction()
    {
        _context = new MLContext();
    }

    public async Task TrainModelAsync()
    {
        var dataView = await LoadDataFromApiAsync();
        var dataProcessPipeline = _context.Transforms.Concatenate("Features", nameof(CryptoData.Open), nameof(CryptoData.High), nameof(CryptoData.Low), nameof(CryptoData.Volume))
                                                     .Append(_context.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(CryptoData.Close)))
                                                     .Append(_context.Transforms.NormalizeMinMax("Features"));

        var trainer = _context.Regression.Trainers.Sdca();
        var trainingPipeline = dataProcessPipeline.Append(trainer);

        _model = trainingPipeline.Fit(dataView);
    }

    private async Task<IDataView> LoadDataFromApiAsync()
    {
        var apiService = new ApiService();
        var data = await apiService.GetCryptoDataAsync();
        return _context.Data.LoadFromEnumerable(data);
    }

    public float Predict(CryptoData data)
    {
        var predictionEngine = _context.Model.CreatePredictionEngine<CryptoData, CryptoDataPrediction>(_model);
        var prediction = predictionEngine.Predict(data);
        return prediction.Close;
    }

    public void GiveAdvice(CryptoData data)
    {
        var prediction = Predict(data);
        if (prediction > data.Close)
        {
            Console.WriteLine("A previsão indica que o preço vai subir. Considere comprar.");
        }
        else
        {
            Console.WriteLine("A previsão indica que o preço vai cair. Considere vender.");
        }
    }
}

public class CryptoDataPrediction
{
    [ColumnName("Score")]
    public float Close { get; set; }
}
