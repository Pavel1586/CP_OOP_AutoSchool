using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminInstructorsPage : Page
{
    public AdminInstructorsPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminInstructorsViewModel(nav);
    }
}