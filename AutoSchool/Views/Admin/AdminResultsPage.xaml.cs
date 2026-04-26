using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminResultsPage : Page
{
    public AdminResultsPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminResultsViewModel(nav);
    }
}