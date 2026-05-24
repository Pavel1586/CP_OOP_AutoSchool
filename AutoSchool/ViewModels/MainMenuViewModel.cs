using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Views;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        public string WelcomeText =>
            UserSession.CurrentUser != null
                ? Loc.F("WelcomeFormat", UserSession.CurrentUser.FirstName)
                : Loc.T("WelcomeGeneric");

        public string InstructorText =>
            UserSession.CurrentUser?.Instructor != null
                ? Loc.F("YourInstructorFormat",
                        UserSession.CurrentUser.Instructor.FirstName,
                        UserSession.CurrentUser.Instructor.LastName)
                : Loc.T("InstructorNotSelected");

        public ICommand OpenTicketsCommand { get; }
        public ICommand OpenHistoryCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand OpenInstructorCommand { get; }
        public ICommand OpenTheoryScheduleCommand { get; }

        public MainMenuViewModel()
        {
            OpenTicketsCommand = new RelayCommand(OpenTickets);
            OpenHistoryCommand = new RelayCommand(OpenHistory);
            LogoutCommand = new RelayCommand(Logout);
            OpenInstructorCommand = new RelayCommand(OpenInstructor);
            OpenTheoryScheduleCommand = new RelayCommand(OpenTheorySchedule);

            LocalizationManager.LanguageChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(WelcomeText));
                OnPropertyChanged(nameof(InstructorText));
            };
        }

        private void OpenTickets(object? parameter)
        {
            var w = new TopicSelectionWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }

        private void OpenHistory(object? parameter)
        {
            var w = new ResultsHistoryWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }

        private void Logout(object? parameter)
        {
            UserSession.CurrentUser = null;
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }

        private void OpenInstructor(object? parameter)
        {
            var w = new InstructorSelectionWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }

        private void OpenTheorySchedule(object? parameter)
        {
            var w = new TheoryScheduleWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }
    }
}