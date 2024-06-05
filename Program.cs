public class Program
{
    public static async Task Main(string[] args)
    {
        var predictionModel = new CryptoPrediction();
        await predictionModel.TrainModelAsync();

        var apiService = new ApiService();
        var latestData = (await apiService.GetCryptoDataAsync())[0];  // Pegando o primeiro dado como exemplo

        predictionModel.GiveAdvice(latestData);
    }
}
