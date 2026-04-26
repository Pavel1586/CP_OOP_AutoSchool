using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminUsersPage : Page
{
    public AdminUsersPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminUsersViewModel(nav);
    }
}