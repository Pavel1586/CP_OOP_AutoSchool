using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminAnalyticsPage : Page
{
    private readonly IPageNavigationService _nav;

    public AdminAnalyticsPage(IPageNavigationService nav)
    {
        InitializeComponent();

        _nav = nav;
        DataContext = new AdminAnalyticsViewModel(nav);

        Loaded += AdminAnalyticsPage_Loaded;
        Unloaded += AdminAnalyticsPage_Unloaded;
    }

    private void AdminAnalyticsPage_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        ForceReapplyTabStyles();
    }

    private void AdminAnalyticsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
    }

    private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Background, ForceReapplyTabStyles);
    }

    private void ForceReapplyTabStyles()
    {
        if (DetailsTabs == null) return;
        var style = Application.Current.TryFindResource("AnalyticsTabItemStyle") as Style;
        if (style != null)
        {
            DetailsTabs.ItemContainerStyle = null;
            DetailsTabs.ItemContainerStyle = style;
        }
        DetailsTabs.InvalidateVisual();
        DetailsTabs.UpdateLayout();
    }
}