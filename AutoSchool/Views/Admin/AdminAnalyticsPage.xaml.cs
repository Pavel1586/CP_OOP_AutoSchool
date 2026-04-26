using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminAnalyticsPage : Page
{
    public AdminAnalyticsPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminAnalyticsViewModel(nav);
    }
}