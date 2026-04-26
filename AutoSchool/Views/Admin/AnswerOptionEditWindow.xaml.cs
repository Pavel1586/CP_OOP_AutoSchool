using System.Windows;

namespace AutoSchool.Views.Admin;

public partial class AnswerOptionEditWindow : Window
{
    public string OptionText => TextBox.Text.Trim();

    public AnswerOptionEditWindow(string? text = null)
    {
        InitializeComponent();
        TextBox.Text = text ?? "";
        TextBox.Focus();
        TextBox.SelectAll();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(OptionText))
        {
            MessageBox.Show("Введите текст варианта ответа.");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}