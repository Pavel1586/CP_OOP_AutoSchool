using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminUserSchedulePage : Page
{
    public AdminUserSchedulePage(IPageNavigationService nav, int userId)
    {
        InitializeComponent();
        DataContext = new AdminUserScheduleViewModel(nav, userId);
    }
}