using System;
using System.Text;

namespace Evo.StaticFunctions;

/*
// ------------------------- Range -------------------------
*/

public static partial class ChargeInfo
{
  /// <summary>
  /// Estimate the range on current state of charge
  /// </summary>
  /// <param name="soc">State of charge</param>
  /// <param name="maxRange">Range of car on full charge</param>
  /// <returns>Range of car on current state of charge (km)</returns>
  public static int RangeOnCurrentSoc(int soc, int maxRange)
  {
    return (maxRange * soc / 100);
  }

  /// <summary>
  /// Formats the current range (km)
  /// </summary>
  /// <param name="currentRange">Range of car on current state of charge</param>
  /// <returns>Formatted string of current range</returns>
  public static string ReadableCurrentRange(int currentRange)
  {
    return $"{currentRange} km";
  }

  /// <summary>
  /// Formats the max range (km)
  /// </summary>
  /// <param name="maxRange">Range of car on 100% charge</param>
  /// <returns>Formatted string of maximum range</returns>
  public static string ReadableMaxRange(int maxRange)
  {
    return $"({maxRange} km on full)";
  }
}

/*
// ------------------------- Time -------------------------
*/

public static partial class ChargeInfo
{
  /// <summary>
  /// Calculates the time required (in minutes) to increase state of charge by a certain percentage
  /// assuming a linear charging model
  /// </summary>
  /// <param name="delta">Increment in state of charge</param>
  /// <param name="fullChargeTime">Time required to charge EV from 0% to 100%</param>
  /// <returns>Time required to increment state of charge by delta (minutes)</returns>
  public static double MinutesToChargeDelta(int delta, double fullChargeTime)
  {
    return fullChargeTime * delta / 100;
  }

  /// <summary>
  /// Converts minutes to a readable format -> x hour(s) and y minute(s)
  /// </summary>
  /// <remarks>
  /// Input value is casted as an integer leading to minor loss of information.
  /// If minutes == 0, then the string "Fully Charged" is returned
  /// </remarks>
  /// <param name="minutes">Time in minutes</param>
  /// <returns>Time in "x hour(s) and y minute(s)" format</returns>
  public static string ReadableTime(double minutes, String zeroResponse)
  {
    int hour = Math.DivRem((int)minutes, 60, out int minute);
    if (hour == 0 && minute == 0) { return zeroResponse; }

    StringBuilder chargetime = new();
    if (hour == 1) { chargetime.Append($"{hour} hour"); }
    if (hour > 1) { chargetime.Append($"{hour} hours"); }
    if (minute == 1) { chargetime.Append($" {minute} minute"); }
    if (minute > 1) { chargetime.Append($" {minute} minutes"); }
    return chargetime.ToString();
  }
}

/*
// ------------------------- Energy -------------------------
*/

public static partial class ChargeInfo
{
  /// <summary>
  /// Todo: deprecated this function
  /// </summary>
  /// <param name="soc"></param>
  /// <param name="capacity"></param>
  /// <returns></returns>s
  public static double EnergyTransferred(int soc, double capacity)
  {
    double energyTransferred = Math.Round(capacity * soc / 100, 2);
    return energyTransferred;
  }
}

/*
// ------------------------- Tariff -------------------------
*/

public static partial class ChargeInfo
{
  /// <summary>
  /// Apply the monsoon season tax of +/- modifier based on time of day
  /// </summary>
  /// <returns>Season and time of day adjusted tariff</returns>
  private static double SeasonAdjustedTariff()
  {
    DateTime current = DateTime.Now;
    double tariff = MainWindow.LiveResponse.Result[MainWindow.LocationIndex].ElectricityRate;
    bool isMonsoonSeason = current.Month >= 6 && current.Month <= 9;

    // Return without modification
    if (!isMonsoonSeason) { return tariff; }

    // Return with appropriate busy season modifier
    double busySeasonModifier = MainWindow.LiveResponse.Result[MainWindow.LocationIndex].SeasonalModifier;
    if (current.Hour >= 18 && current.Hour < 22) { tariff += busySeasonModifier; }
    else if (current.Hour >= 22 || current.Hour <= 6) { tariff -= busySeasonModifier; }
    return tariff;
  }
}

/*
// ------------------------- Revenue -------------------------
*/

public static partial class ChargeInfo
{
  /// <summary>
  /// Calculates the sum losses associated with charging an individual EV
  /// </summary>
  /// <param name="units">Energy transferred</param>
  /// <returns>Total loss (rupees)</returns>
  public static double TotalRevenue(double units)
  {
    // 1. Apply busy season modifier (includes base electricity rate)
    double revenue = units * SeasonAdjustedTariff();

    // 2. Add base usage cost (30 rupees)
    revenue += 30;

    // 3. Apply tax
    revenue *= 1 + (MainWindow.LiveResponse.Result[MainWindow.LocationIndex].Tax / 100);

    // Return total loss adjusted for paisa to rupee conversion 
    return revenue / 100;
  }
}
