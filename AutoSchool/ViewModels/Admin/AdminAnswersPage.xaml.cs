using System.Windows.Controls;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class AdminAnswersPage : Page
{
    public AdminAnswersPage(IPageNavigationService nav, int ticketId, int questionId)
    {
        InitializeComponent();
        DataContext = new AdminAnswersViewModel(nav, ticketId, questionId);
    }
}