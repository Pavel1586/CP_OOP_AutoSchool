using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Views;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService = new AuthService();
        private bool _showErrors;

        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); AuthError = ""; ValidateFirstName(); }
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); AuthError = ""; ValidateLastName(); }
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); AuthError = ""; ValidateEmail(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); AuthError = ""; ValidatePassword(); ValidateConfirmPassword(); }
        }

        private string _confirmPassword = "";
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); AuthError = ""; ValidateConfirmPassword(); }
        }

        private string _firstNameError = "";
        public string FirstNameError { get => _firstNameError; private set { _firstNameError = value; OnPropertyChanged(); } }

        private string _lastNameError = "";
        public string LastNameError { get => _lastNameError; private set { _lastNameError = value; OnPropertyChanged(); } }

        private string _emailError = "";
        public string EmailError { get => _emailError; private set { _emailError = value; OnPropertyChanged(); } }

        private string _passwordError = "";
        public string PasswordError { get => _passwordError; private set { _passwordError = value; OnPropertyChanged(); } }

        private string _confirmPasswordError = "";
        public string ConfirmPasswordError { get => _confirmPasswordError; private set { _confirmPasswordError = value; OnPropertyChanged(); } }

        private string _authError = "";
        public string AuthError { get => _authError; private set { _authError = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand BackCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
            BackCommand = new RelayCommand(Back);

            LocalizationManager.LanguageChanged += (_, __) =>
            {
                ValidateFirstName();
                ValidateLastName();
                ValidateEmail();
                ValidatePassword();
                ValidateConfirmPassword();

                if (!string.IsNullOrWhiteSpace(AuthError))
                    AuthError = Loc.T("Err_UserExists");
            };
        }

        private void ValidateFirstName()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(FirstName)) { FirstNameError = ""; return; }
            FirstNameError = string.IsNullOrWhiteSpace(FirstName) ? Loc.T("Err_EnterFirstName") : "";
        }

        private void ValidateLastName()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(LastName)) { LastNameError = ""; return; }
            LastNameError = string.IsNullOrWhiteSpace(LastName) ? Loc.T("Err_EnterLastName") : "";
        }

        private void ValidateEmail()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(Email)) { EmailError = ""; return; }

            if (string.IsNullOrWhiteSpace(Email)) EmailError = Loc.T("Err_EnterEmail");
            else if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) EmailError = Loc.T("Err_InvalidEmail");
            else EmailError = "";
        }

        private void ValidatePassword()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(Password)) { PasswordError = ""; return; }

            var (ok, msg) = ValidatePasswordRules(Password);
            PasswordError = ok ? "" : msg;
        }

        private void ValidateConfirmPassword()
        {
            if (!_showErrors && string.IsNullOrWhiteSpace(ConfirmPassword)) { ConfirmPasswordError = ""; return; }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ConfirmPasswordError = Loc.T("Err_ConfirmPassword");
                return;
            }

            ConfirmPasswordError = Password == ConfirmPassword ? "" : Loc.T("Err_PasswordsNotMatch");
        }

        private static (bool ok, string message) ValidatePasswordRules(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return (false, Loc.T("Err_EnterPassword"));
            if (password.Length < 8 || password.Length > 20) return (false, Loc.T("Err_PwdLen"));

            if (!Regex.IsMatch(password, @"^[\u0021-\u007E]+$")) return (false, Loc.T("Err_PwdAscii"));
            if (!Regex.IsMatch(password, @"[A-Z]")) return (false, Loc.T("Err_PwdUpper"));
            if (!Regex.IsMatch(password, @"[a-z]")) return (false, Loc.T("Err_PwdLower"));
            if (!Regex.IsMatch(password, @"\d")) return (false, Loc.T("Err_PwdDigit"));
            if (!Regex.IsMatch(password, @"[^A-Za-z0-9]")) return (false, Loc.T("Err_PwdSpec"));

            return (true, "");
        }

        private bool ValidateAll()
        {
            _showErrors = true;
            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidatePassword();
            ValidateConfirmPassword();

            return string.IsNullOrEmpty(FirstNameError)
                && string.IsNullOrEmpty(LastNameError)
                && string.IsNullOrEmpty(EmailError)
                && string.IsNullOrEmpty(PasswordError)
                && string.IsNullOrEmpty(ConfirmPasswordError);
        }

        private void Register(object? parameter)
        {
            if (!ValidateAll()) return;

            try
            {
                bool success = _authService.Register(
                    FirstName.Trim(),
                    LastName.Trim(),
                    Email.Trim(),
                    Password);

                if (!success)
                {
                    AuthError = Loc.T("Err_UserExists");
                    return;
                }

                var w = new LoginWindow();
                w.Show();
                if (parameter is Window currentWindow) currentWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Registration error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back(object? parameter)
        {
            var w = new LoginWindow();
            w.Show();
            if (parameter is Window currentWindow) currentWindow.Close();
        }
    }
}