using System.Windows;
using System.Windows.Controls;
using AutoSchool.Infrastructure;

namespace AutoSchool.Views.Controls
{
    public partial class ThemeSwitcher : UserControl
    {
        public ThemeSwitcher()
        {
            InitializeComponent();
        }

        private void Light_Click(object sender, RoutedEventArgs e)
            => ThemeManager.Apply(AppTheme.Light);

        private void Dark_Click(object sender, RoutedEventArgs e)
            => ThemeManager.Apply(AppTheme.Dark);
    }
}