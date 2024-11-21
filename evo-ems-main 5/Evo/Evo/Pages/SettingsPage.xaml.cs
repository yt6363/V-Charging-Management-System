using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;

namespace Evo.Pages;


/// <summary>
/// Invoke settings page
/// </summary>
public sealed partial class SettingsPage : Page
{
  readonly ObservableCollection<String> locations = new();
  public SettingsPage()
  {
    this.InitializeComponent();

    // Generate list of locations from the JSON response
    foreach (var metadata in MainWindow.LiveResponse.Result) { locations.Add(metadata.Location); }

    // Update value to globle location index
    LocationComboBox.SelectedIndex = MainWindow.LocationIndex;

    // Update data on screen based on the selected global location index
    UpdateData(MainWindow.LocationIndex);
  }

  // Display the metadata of selected location (index)
  private void UpdateData(int index)
  {
    var json = MainWindow.LiveResponse.Result[index];
    Tax.Text = json.Tax.ToString();
    ElectricityRate.Text = json.ElectricityRate.ToString();
    LowerFixedCharge.Text = json.FixedChargeLow.ToString();
    SeasonalModifier.Text = json.SeasonalModifier.ToString();
    HigherFixedCharge.Text = json.FixedChargeHigh.ToString();
  }

  private void LocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    MainWindow.LocationIndex = LocationComboBox.SelectedIndex;
    UpdateData(MainWindow.LocationIndex);
  }
}
