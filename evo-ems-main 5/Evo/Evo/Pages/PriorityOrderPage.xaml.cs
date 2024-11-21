using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Evo.Pages;

/// <summary>
/// Invoke priority order application
/// </summary>
public sealed partial class PriorityOrderPage : Page
{
  // Observable collection to enable item repeater functionality
  public ObservableCollection<string> EvInfoItems { get; set; }

  public PriorityOrderPage()
  {
    EvInfoItems = new ObservableCollection<string>() { "1" }; // placeholder
    this.InitializeComponent();
  }

  // Command bar function: add new Ev to the list
  private void AddButton_Click(object sender, RoutedEventArgs e)
  {
    EvInfoItems.Add("1");

    // Re-enable remove button
    RemoveButton.IsEnabled = true;
  }

  // Command bar function: remove the last Ev from the list
  private void RemoveButton_Click(object sender, RoutedEventArgs e)
  {
    int itemCount = EvInfoItems.Count;
    EvInfoItems.RemoveAt(itemCount - 1);

    // Disable remove button if no elements are left on screen after Remove
    if (itemCount == 1) { RemoveButton.IsEnabled = false; }
  }
}
