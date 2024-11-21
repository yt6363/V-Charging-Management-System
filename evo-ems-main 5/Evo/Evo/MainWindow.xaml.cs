using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evo.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using static Evo.StaticData.EvLibrary;
using static Evo.StaticFunctions.WebFetch;

namespace Evo;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window {
  // Global Data
  public static readonly IReadOnlyList<EvCar> Library = GenerateLibrary();
  public static readonly Task<List<ApiResponse>> LiveResponse = ProcessLiveData(version: "v1");
  public static int LocationIndex = 0;

  public MainWindow() {
    this.InitializeComponent();

    // Use new Windows title style
    this.ExtendsContentIntoTitleBar = true;

    // Auto open priority order page
    NavigateTo("priorityOrderPage", null);
  }
}

/// <summary>
/// Logic for navigation
/// </summary>
public sealed partial class MainWindow : Window {
  // List of pages
  private readonly List<(string Tag, Type Page)> _pages = new()
  {
    ("priorityOrderPage", typeof(PriorityOrderPage)),
    ("settingsPage", typeof(SettingsPage)),
  };

  // Retrieve the tag of the pressed menu item from XAML
  private void MainNavbar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
    if (args.IsSettingsSelected) {
      NavigateTo("settingsPage", args.RecommendedNavigationTransitionInfo);
    }
    else if (args.SelectedItemContainer is not null) {
      var navItemTag = args.SelectedItemContainer.Tag.ToString();
      NavigateTo(navItemTag, args.RecommendedNavigationTransitionInfo);
    }
  }

  // Navigate to the pressed menu item by referencing the tag in C#
  private void NavigateTo(string navItemTag, NavigationTransitionInfo recommendedNavigationTransitionInfo) {
    Type page = null;
    var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
    page = item.Page;
    var preNavPageType = ContentFrame.CurrentSourcePageType;

    // Only navigate if the selected page isn't currently loaded.
    if (page is not null && !Type.Equals(preNavPageType, page)) {
      ContentFrame.Navigate(page, null, recommendedNavigationTransitionInfo);
    }
  }
}
