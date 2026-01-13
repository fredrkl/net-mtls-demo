public class VayapayClient
{
    private readonly HttpClient _httpClient;

    public VayapayClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetDataAsync()
    {
        var response = await _httpClient.GetAsync("https://api.vayapay.com/data");
        //response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
