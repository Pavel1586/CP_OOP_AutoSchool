using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Views;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService = new AuthService();

        private bool _showErrors;

        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                AuthError = "";
                ValidateEmail();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                AuthError = "";
                ValidatePassword();
            }
        }

        private string _emailError = "";
        public string EmailError
        {
            get => _emailError;
            private set { _emailError = value; OnPropertyChanged(); }
        }

        private string _passwordError = "";
        public string PasswordError
        {
            get => _passwordError;
            private set { _passwordError = value; OnPropertyChanged(); }
        }

        private string _authError = "";
        public string AuthError
        {
            get => _authError;
            private set { _authError = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            OpenRegisterCommand = new RelayCommand(OpenRegister);

            LocalizationManager.LanguageChanged += (_, __) =>
            {
                // пересчитать тексты ошибок при смене языка
                ValidateEmail();
                ValidatePassword();
                if (!string.IsNullOrWhiteSpace(AuthError))
                    AuthError = Loc.T("Msg_WrongCredentials");
            };
        }

        private void ValidateEmail()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email)) EmailError = Loc.T("Err_EnterEmail");
            else if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) EmailError = Loc.T("Err_InvalidEmail");
            else EmailError = "";
        }

        private void ValidatePassword()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "";
                return;
            }

            PasswordError = string.IsNullOrWhiteSpace(Password) ? Loc.T("Err_EnterPassword") : "";
        }

        private bool ValidateAll()
        {
            _showErrors = true;
            ValidateEmail();
            ValidatePassword();
            return string.IsNullOrEmpty(EmailError) && string.IsNullOrEmpty(PasswordError);
        }

        private void Login(object? parameter)
        {
            if (!ValidateAll()) return;

            try
            {
                var user = _authService.Login(Email.Trim(), Password);
                if (user == null)
                {
                    AuthError = Loc.T("Msg_WrongCredentials");
                    return;
                }

                UserSession.CurrentUser = user;

                Window nextWindow = user.RoleId == 1 ? new AutoSchool.Views.AdminWindow() : new MainMenuWindow();
                nextWindow.Show();

                if (parameter is Window currentWindow) currentWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Login error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRegister(object? parameter)
        {
            var w = new RegisterWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }
    }
}