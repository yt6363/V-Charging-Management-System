using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evo.StaticData;

public struct EvLibrary
{
  /// <summary>
  /// Object model of an EV car
  /// </summary>
  public struct EvCar
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("make")]
    public string Make { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("batteryCapacity")]
    public double BatteryCapacity { get; set; }

    [JsonPropertyName("chargingCapacity")]
    public double ChargingCapacity { get; set; }

    [JsonPropertyName("time")]
    public double FullChargeTime { get; set; }

    [JsonPropertyName("range")]
    public int Range { get; set; }

    [JsonPropertyName("isFastCharge")]
    public bool IsFastCharge { get; set; }

    [JsonPropertyName("isHybrid")]
    public bool IsHybrid { get; set; }

    [JsonPropertyName("isSolar")]
    public bool IsSolar { get; set; }
  }

  /// <summary>
  /// Reads the JSON file from a fixed relative path
  /// </summary>
  /// <returns>JSON as string</returns>
  private static string ReadJsonAsString()
  {
    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string filePath = $"{currentDirectory}/StaticData/evLibrary.json";
    string json = File.ReadAllText(filePath);

    return json;
  }

  /// <summary>
  /// Converts a valid JSON to a list of immutable EvCar objects
  /// </summary>
  /// <returns>EV library as list of objects</returns>
  public static IReadOnlyList<EvCar> GenerateLibrary()
  {
    string json = ReadJsonAsString();
    return JsonSerializer.Deserialize<List<EvCar>>(json).ToImmutableList();
  }
}
