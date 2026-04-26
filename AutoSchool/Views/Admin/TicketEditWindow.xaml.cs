using System.Windows;

namespace AutoSchool.Views.Admin;

public partial class TicketEditWindow : Window
{
    public string TicketTitle => TitleBox.Text.Trim();

    public TicketEditWindow(string? title = null)
    {
        InitializeComponent();
        TitleBox.Text = title ?? "";
        TitleBox.Focus();
        TitleBox.SelectAll();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TicketTitle))
        {
            MessageBox.Show("Введите название билета.");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}