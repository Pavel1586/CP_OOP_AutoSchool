using System.Windows;

namespace AutoSchool.Views.Admin;

public partial class TopicEditWindow : Window
{
    public string TopicName => NameBox.Text.Trim();

    public TopicEditWindow(string? name = null)
    {
        InitializeComponent();
        NameBox.Text = name ?? "";
        NameBox.Focus();
        NameBox.SelectAll();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TopicName))
        {
            MessageBox.Show("Введите название темы.");
            return;
        }
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}