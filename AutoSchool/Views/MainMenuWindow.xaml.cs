using System.Windows;

namespace AutoSchool.Views
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var w = new SettingsWindow
            {
                Owner = this
            };
            w.ShowDialog();
        }
    }
}