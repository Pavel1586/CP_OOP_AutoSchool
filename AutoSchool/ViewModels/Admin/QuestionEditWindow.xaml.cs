using System.Windows;

namespace AutoSchool.Views.Admin;

public partial class QuestionEditWindow : Window
{
    public string QuestionText => QuestionBox.Text.Trim();
    public string Explanation => ExplanationBox.Text.Trim();

    public QuestionEditWindow(string? text = null, string? explanation = null)
    {
        InitializeComponent();
        QuestionBox.Text = text ?? "";
        ExplanationBox.Text = explanation ?? "";
        QuestionBox.Focus();
        QuestionBox.SelectAll();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(QuestionText))
        {
            MessageBox.Show("Введите текст вопроса.");
            return;
        }
        if (string.IsNullOrWhiteSpace(Explanation))
        {
            MessageBox.Show("Введите пояснение (Explanation).");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}