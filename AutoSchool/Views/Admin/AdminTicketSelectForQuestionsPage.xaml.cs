using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminTicketSelectForQuestionsPage : Page
{
    public AdminTicketSelectForQuestionsPage(IPageNavigationService nav)
    {
        InitializeComponent();
        DataContext = new AdminTicketSelectForQuestionsViewModel(nav);
    }
}