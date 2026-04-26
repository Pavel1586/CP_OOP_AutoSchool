using AutoSchool.Data;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoSchool.Views.Admin;

public partial class TheoryCreditEditWindow : Window
{
    public int? TopicId { get; private set; }
    public DateTime PlannedAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public string Room { get; private set; } = "";
    public TheoryCreditStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public TheoryCreditEditWindow(TheoryCredit? existing = null)
    {
        InitializeComponent();

        using var context = new ApplicationDbContext();
        var topics = context.Topics.AsNoTracking().OrderBy(t => t.Id).ToList();
        TopicBox.ItemsSource = topics;

        StatusBox.SelectedIndex = 0;

        if (existing != null)
        {
            DatePicker.SelectedDate = existing.PlannedAt.Date;
            TimeBox.Text = existing.PlannedAt.ToString("HH:mm");
            TopicBox.SelectedValue = existing.TopicId;
            DurationBox.Text = existing.DurationMinutes.ToString();
            RoomBox.Text = existing.Room;
            NotesBox.Text = existing.Notes ?? "";

            StatusBox.SelectedIndex = existing.Status switch
            {
                TheoryCreditStatus.Planned => 0,
                TheoryCreditStatus.Passed => 1,
                TheoryCreditStatus.Failed => 2,
                TheoryCreditStatus.Missed => 3,
                _ => 0
            };
        }
        else
        {
            DatePicker.SelectedDate = DateTime.Today;
        }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (DatePicker.SelectedDate == null)
        {
            MessageBox.Show("Выберите дату.");
            return;
        }

        if (!TimeSpan.TryParseExact(TimeBox.Text.Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out var time))
        {
            MessageBox.Show("Введите время в формате HH:mm, например 18:00.");
            return;
        }

        if (!int.TryParse(DurationBox.Text.Trim(), out var dur) || dur <= 0 || dur > 600)
        {
            MessageBox.Show("Длительность должна быть числом от 1 до 600 минут.");
            return;
        }

        var statusTag = (StatusBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Planned";
        Status = Enum.Parse<TheoryCreditStatus>(statusTag);

        TopicId = TopicBox.SelectedValue as int?;
        PlannedAt = DatePicker.SelectedDate.Value.Date.Add(time);
        DurationMinutes = dur;
        Room = RoomBox.Text.Trim();
        Notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(Room))
        {
            MessageBox.Show("Укажите кабинет.");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}