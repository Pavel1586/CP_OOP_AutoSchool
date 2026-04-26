using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels;

public class TheoryScheduleViewModel : BaseViewModel
{
    public class CreditRow
    {
        public DateTime PlannedAt { get; set; }
        public string TopicName { get; set; } = "";
        public int DurationMinutes { get; set; }
        public string Room { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Notes { get; set; }
    }

    public ObservableCollection<CreditRow> Credits { get; } = new();

    public string TrainingInfoText { get; private set; } = "";

    public ICommand BackCommand { get; }

    public TheoryScheduleViewModel()
    {
        BackCommand = new RelayCommand(Back);
        Load();
    }

    private void Load()
    {
        if (UserSession.CurrentUser == null) return;

        using var context = new ApplicationDbContext();

        var user = context.Users.First(u => u.Id == UserSession.CurrentUser.Id);

        // длительность обучения
        if (user.TrainingStartDate.HasValue && user.TrainingPlannedEndDate.HasValue)
        {
            var start = user.TrainingStartDate.Value.Date;
            var end = user.TrainingPlannedEndDate.Value.Date;

            var totalDays = (end - start).Days;
            var passedDays = (DateTime.Today - start).Days;
            var leftDays = (end - DateTime.Today).Days;

            if (passedDays < 0) passedDays = 0;
            if (leftDays < 0) leftDays = 0;

            TrainingInfoText = $"План обучения: {start:dd.MM.yyyy} — {end:dd.MM.yyyy} " +
                               $"(длительность {totalDays} дней). Пройдено: {passedDays} дней, осталось: {leftDays} дней.";
        }
        else
        {
            TrainingInfoText = "План обучения не задан.";
        }
        OnPropertyChanged(nameof(TrainingInfoText));

        var list = context.TheoryCredits
            .Include(c => c.Topic)
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.PlannedAt)
            .ToList();

        Credits.Clear();
        foreach (var c in list)
        {
            Credits.Add(new CreditRow
            {
                PlannedAt = c.PlannedAt,
                TopicName = c.Topic?.Name ?? "—",
                DurationMinutes = c.DurationMinutes,
                Room = c.Room,
                Status = c.Status.ToString(),
                Notes = c.Notes
            });
        }
    }

    private void Back(object? parameter)
    {
        var w = new MainMenuWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }
}