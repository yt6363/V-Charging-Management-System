using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Evo.StaticFunctions;

public struct WebFetch
{
  // Initialize web helper HTTP client
  public static readonly HttpClient client = new();

  // Download the live JSON data from GitHub
  private static async Task<string> FetchLiveJson(string apiVersion)
  {
    try
    {
      string repo = "https://raw.githubusercontent.com/Az-21/evo-ems-web/main/data/";
      return await client.GetStringAsync($"{repo}{apiVersion}.json");
    }
    catch
    {
      // Return a default value
      return "[{\"location\": \"India > Karnataka\",\"tax\":5,\"fixedChargeLow\":0.097,\"fixedChargeHigh\":0.236,\"electricityRate\":500\"seasonalModifier\":1}]";
    }
  }

  // API response model
  public struct ApiResponse
  {
    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("tax")]
    public double Tax { get; set; }

    [JsonPropertyName("fixedChargeLow")]
    public double FixedChargeLow { get; set; }

    [JsonPropertyName("fixedChargeHigh")]
    public double FixedChargeHigh { get; set; }

    [JsonPropertyName("electricityRate")]
    public double ElectricityRate { get; set; }

    [JsonPropertyName("seasonalModifier")]
    public double SeasonalModifier { get; set; }
  }

  // Parse API response
  public static async Task<List<ApiResponse>> ProcessLiveData(string version)
  {
    string json = await FetchLiveJson(version);
    return JsonSerializer.Deserialize<List<ApiResponse>>(json);
  }
}
