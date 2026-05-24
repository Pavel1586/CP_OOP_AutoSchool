using AutoSchool.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace AutoSchool.Views.Admin;

public partial class TicketEditWindow : Window
{
    public string TicketTitle => TitleBox.Text.Trim();

    public int TopicId
    {
        get
        {
            if (TopicBox.SelectedValue is int id) return id;
            return 1;
        }
    }

    public TicketEditWindow(string? title = null, int? topicId = null)
    {
        InitializeComponent();

        using var context = new ApplicationDbContext();
        var topics = context.Topics.AsNoTracking().OrderBy(t => t.Id).ToList();
        TopicBox.ItemsSource = topics;

        TitleBox.Text = title ?? "";

        // выбрать тему по умолчанию
        TopicBox.SelectedValue = topicId ?? topics.FirstOrDefault()?.Id ?? 1;

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

        if (TopicBox.SelectedItem == null)
        {
            MessageBox.Show("Выберите тему.");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}