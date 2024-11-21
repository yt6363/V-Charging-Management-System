namespace Evo.StaticFunctions;

public static class StationInfo {
  /// <summary>
  /// Calculates the monthly fixed grid charge based on the maximum demand
  /// </summary>
  /// <remarks>
  /// Formulae used: tariff approved by commission for LT-6 (C)
  /// </remarks>
  /// <param name="maxDemand">Maximum demand in kW (for LT), or kVA (for HT)</param>
  /// <param name="isHT">Supply type: HT/LT</param>
  /// <returns>Monthly fixed grid charge (rupees)</returns>
  public static double FixedGirdCharges(int maxDemand, bool isHT) {
    // High tension supply
    if (isHT) { return maxDemand * 200; }

    // Low tension supply
    if (maxDemand < 50) { return maxDemand * 70; }
    return (50 * 70) + ((maxDemand - 50) * 170);
  }
}

