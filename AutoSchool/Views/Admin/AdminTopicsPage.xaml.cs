using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminTopicsPage : Page
{
    public AdminTopicsPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminTopicsViewModel(nav);
    }
}