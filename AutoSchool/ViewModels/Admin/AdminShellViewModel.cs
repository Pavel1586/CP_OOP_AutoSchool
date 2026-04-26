using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Services.Navigation;
using AutoSchool.Views;
using AutoSchool.Views.Admin;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels.Admin;

public class AdminShellViewModel : BaseViewModel
{
    private readonly IPageNavigationService _nav;

    public ICommand OpenTicketsCommand { get; }
    public ICommand OpenQuestionsCommand { get; }
    public ICommand OpenResultsCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand OpenUsersCommand { get; }
    public ICommand OpenAnalyticsCommand { get; }
    public ICommand OpenInstructorsCommand { get; }

    public AdminShellViewModel(IPageNavigationService nav)
    {
        _nav = nav;

        OpenTicketsCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketsPage(_nav)));
        OpenQuestionsCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketSelectForQuestionsPage(_nav)));
        OpenResultsCommand = new RelayCommand(_ => _nav.Navigate(new AdminResultsPage(_nav)));
        LogoutCommand = new RelayCommand(Logout);
        OpenResultsCommand = new RelayCommand(_ => _nav.Navigate(new AdminResultsPage(_nav)));
        OpenAnalyticsCommand = new RelayCommand(_ => _nav.Navigate(new AutoSchool.Views.Admin.AdminAnalyticsPage(_nav)));
        OpenUsersCommand = new RelayCommand(_ => _nav.Navigate(new AutoSchool.Views.Admin.AdminUsersPage(_nav)));
        OpenInstructorsCommand = new RelayCommand(_ => _nav.Navigate(new AutoSchool.Views.Admin.AdminInstructorsPage(_nav)));
    }

    private void Logout(object? parameter)
    {
        UserSession.CurrentUser = null;

        var w = new LoginWindow();
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }
}