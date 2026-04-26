using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            if (DataContext is LoginViewModel vm)
            {
                PasswordBox.PasswordChanged += (_, __) => vm.Password = PasswordBox.Password;
            }
        }
    }
}