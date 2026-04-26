using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminTicketsPage : Page
{
    public AdminTicketsPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminTicketsViewModel(nav);
    }
}