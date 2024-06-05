using Newtonsoft.Json;
using System.Net.Http.Headers;

public class ApiService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<List<CryptoData>> GetCryptoDataAsync()
    {
        try
        {
            // Adicione uma chave de API se necessário
            string apiKey = "YOUR_API_KEY";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.GetAsync("https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=100&page=1&sparkline=false");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var cryptoDataList = JsonConvert.DeserializeObject<List<CryptoData>>(content);

            // Transform the data as needed (e.g., parse dates, normalize values)
            return cryptoDataList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<CryptoData>();
        }
    }
}
