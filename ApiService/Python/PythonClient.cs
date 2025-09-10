using System.Text;
using System.Text.Json;

namespace ApiService.Python;

public class PythonClient(HttpClient httpClient)
{

    public async Task<string> CalculateCorrelation(object correlationRequest)
    {
        var json = JsonSerializer.Serialize(correlationRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("/analytics/correlation", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GenerateForecast(object forecastRequest)
    {
        var json = JsonSerializer.Serialize(forecastRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("/analytics/forecast", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool> CheckHealth()
    {
        try
        {
            var response = await httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
