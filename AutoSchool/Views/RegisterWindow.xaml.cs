using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            if (DataContext is RegisterViewModel vm)
            {
                PasswordBox.PasswordChanged += (_, __) => vm.Password = PasswordBox.Password;
                ConfirmPasswordBox.PasswordChanged += (_, __) => vm.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }
    }
}