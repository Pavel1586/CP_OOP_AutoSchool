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
                ? $"Здравствуйте, {UserSession.CurrentUser.FirstName}!"
                : "Здравствуйте!";

        public ICommand OpenTicketsCommand { get; }
        public ICommand OpenHistoryCommand { get; }   // <-- добавили
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
        }

        private void OpenTickets(object? parameter)
        {
            var w = new TopicSelectionWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }

        private void OpenHistory(object? parameter)   // <-- добавили
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

        public string InstructorText =>
            UserSession.CurrentUser?.Instructor != null
                ? $"Ваш инструктор: {UserSession.CurrentUser.Instructor.FirstName} {UserSession.CurrentUser.Instructor.LastName}"
                : "Инструктор не выбран";
    }
}