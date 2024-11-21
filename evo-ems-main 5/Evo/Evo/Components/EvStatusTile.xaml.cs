using System;
using System.Collections.Generic;
using System.Linq;
using Evo.StaticFunctions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static Evo.StaticData.EvLibrary;

namespace Evo.Components;

/// <summary>
/// Status Tile UI Variables
/// </summary>
public sealed partial class EvStatusTile : UserControl
{
  private int InitialSoc { get; set; }
  private int PresentSoc { get; set; }
  private int Selected { get; set; }
  private IReadOnlyList<EvCar> Library { get; set; }
  private List<bool> SlectedEvFeatures { get; set; }
  private List<string> AutocompleteList { get; set; }
}

/// <summary>
/// Initialize
/// </summary>
public sealed partial class EvStatusTile : UserControl
{
  public EvStatusTile()
  {
    InitializeCarList();
    this.InitializeComponent();
    InitializeDefaultValues();
  }

  private void InitializeCarList()
  {
    // Load data about cars from StaticData via JSON
    Library = MainWindow.Library;
    SlectedEvFeatures = new List<bool>() {
      false, // solar
      false, // fast charge
      false  // rapid charge
     };

    // Generate List<string> for auto-complete
    AutocompleteList = new List<string>();
    for (int i = 0; i < Library.Count; i++)
    {
      AutocompleteList.Add($"{Library[i].Make} {Library[i].Model} ({Library[i].Year})");
    }
  }

  private void InitializeDefaultValues()
  {
    // Components which fire ValueChanged
    InitialChargeNumberBox.Value = 0;
    InitialChargeProgressBar.Value = 0;
    PresentChargeNumberBox.Value = 0;
    PresentChargeProgressBar.Value = 0;
    EvMoneyNumberBox.Value = 0.00;

    // Set current time
    string time = DateTime.Now.ToString("HH:mm:ss");
    List<int> timeSpan = time.Split(':').Select(int.Parse).ToList();
    ArrivalTimePicker.SelectedTime = new TimeSpan(timeSpan[0], timeSpan[1], timeSpan[2]);
    DepartureTimePicker.SelectedTime = new TimeSpan(timeSpan[0], timeSpan[1], timeSpan[2]);

    // Components which are affected by ValueChanged
    MaxRangeTextBlock.Text = String.Empty;
    PriorityTextBlock.Text = String.Empty;
    CurrentRangeTextBlock.Text = "📝";
    CompletionTimeTextBlock.Text = "📝";
    EnergyTransferredTextBlock.Text = "📝";
  }
}

/// <summary>
/// UI logic
/// </summary>
public sealed partial class EvStatusTile : UserControl
{
  // Auto-suggest list generation
  private void EvAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
  {
    var suitableItems = new List<string>();
    var splitText = sender.Text.ToLower().Split(" ");

    foreach (string car in AutocompleteList)
    {
      var found = splitText.All((key) => { return car.ToLower().Contains(key); });
      if (found) { suitableItems.Add(car); }
    }

    if (suitableItems.Count == 0) { suitableItems.Add("No results found"); }
    sender.ItemsSource = suitableItems;
  }

  // Handle event of car selection -> update stuff in backend and frontend
  private void EvAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
  {
    string selectedEv = args.SelectedItem.ToString();

    // Update global selection id
    try
    {
      Selected = AutocompleteList.IndexOf(selectedEv);

      // Update car details in backend and frontend
      SolarRoofCheckBox.IsChecked
        = SlectedEvFeatures[0]
        = Library[Selected].IsSolar;

      FastChargeCheckBox.IsChecked
        = SlectedEvFeatures[1]
        = Library[Selected].IsFastCharge;

      // Update the parameters on the information pane
      UpdateInformantionPane();
    }
    catch
    {
      // for when 'No result found' is selected by user
      // do nothing
    }
  }

  // Checkbox handlers
  private void SolarRoofCheckBox_Click(object sender, RoutedEventArgs e)
  {
    SlectedEvFeatures[0] = (bool)SolarRoofCheckBox.IsChecked;
  }

  private void FastChargeCheckBox_Click(object sender, RoutedEventArgs e)
  {
    SlectedEvFeatures[1] = (bool)FastChargeCheckBox.IsChecked;
  }

  private void RapidChargeCheckBox_Click(object sender, RoutedEventArgs e)
  {
    SlectedEvFeatures[2] = (bool)FastChargeCheckBox.IsChecked;
  }

  // Update the input progress bar according to the battery percentage
  private void InitialChargeNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
  {
    bool isParsable = int.TryParse(InitialChargeNumberBox.Value.ToString(), out int currentSoc);

    if (isParsable) { InitialChargeProgressBar.Value = InitialSoc = currentSoc; }
    else { InitialChargeProgressBar.Value = 0; }

    // Update the parameters on the information pane
    UpdateInformantionPane();

    // Update the Present charge such that it is always at least as high as the initial charge
    PresentChargeNumberBox_ValueChanged(null, null);
  }

  // Update the Present progress bar according to the battery percentage
  private void PresentChargeNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
  {
    bool isParsable = int.TryParse(PresentChargeNumberBox.Value.ToString(), out int currentSoc);

    if (isParsable)
    {
      int higherSoc = Math.Max(InitialSoc, currentSoc);
      PresentChargeProgressBar.Value = higherSoc;
      PresentChargeNumberBox.Value = higherSoc;
      PresentSoc = (int)PresentChargeNumberBox.Value;
    }
    else { PresentChargeProgressBar.Value = 0; }

    // Update the parameters on the information pane
    UpdateInformantionPane();
  }
}

/// <summary>
/// Information pane stats
/// </summary>
public sealed partial class EvStatusTile : UserControl
{
  private void UpdateInformantionPane()
  {
    // Prevent autofire at startup
    if (!ArchiveButton.IsLoaded) { return; }

    // Completion time
    double fullChargeTime = Library[Selected].FullChargeTime;
    double minutestToFullCharge = ChargeInfo.MinutesToChargeDelta(100 - PresentSoc, fullChargeTime);
    CompletionTimeTextBlock.Text = ChargeInfo.ReadableTime(minutestToFullCharge, "Fully charged");

    // Range
    int maxRange = Library[Selected].Range;
    int currentRange = ChargeInfo.RangeOnCurrentSoc(PresentSoc, maxRange);
    MaxRangeTextBlock.Text = ChargeInfo.ReadableMaxRange(maxRange);
    CurrentRangeTextBlock.Text = ChargeInfo.ReadableCurrentRange(currentRange);
    if (currentRange == maxRange) { MaxRangeTextBlock.Text = String.Empty; }

    // Energy transferred
    double capacity = Library[Selected].BatteryCapacity;
    double energyTransferred = ChargeInfo.EnergyTransferred(PresentSoc - InitialSoc, capacity);
    EnergyTransferredTextBlock.Text = $"{energyTransferred:0.00} kWh";

    // Time charged
    double timeCharged = DepartureTimePicker.Time.TotalMinutes - ArrivalTimePicker.Time.TotalMinutes;
    TimeOfUseTextBlock.Text = ChargeInfo.ReadableTime(timeCharged, "0 minute");

    // Revenue
    RevenueTextBlock.Text = $"₹ {ChargeInfo.TotalRevenue(energyTransferred):0.00}";

    // Priority
    int slotId = Priority.GenerateTimeSlot(new DateTime() + ArrivalTimePicker.Time).Id;
    double priority = Priority.GeneratePriority(soc: InitialSoc, fee: EvMoneyNumberBox.Value);
    string parsedPriority = priority.ToString().Substring(1);
    if (parsedPriority.Length > 6) { parsedPriority = parsedPriority.Substring(0, 5); }
    PriorityTextBlock.Text = $"{slotId}{parsedPriority}";
  }
}

/// <summary>
/// Time picker operations
/// </summary>
public sealed partial class EvStatusTile : UserControl
{
  // Set current time as departure time
  private void UpdateTimeButton_Click(object sender, RoutedEventArgs e)
  {
    string time = DateTime.Now.ToString("HH:mm:ss");
    List<int> timeSpan = time.Split(':').Select(int.Parse).ToList();
    DepartureTimePicker.SelectedTime = new TimeSpan(timeSpan[0], timeSpan[1], timeSpan[2]);
    UpdateInformantionPane();
  }


  private void ArrivalTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
  {
    UpdateInformantionPane();
  }

  private void DepartureTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
  {
    UpdateInformantionPane();
  }

  private void EvMoneyNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args) {
    UpdateInformantionPane();
  }
}
