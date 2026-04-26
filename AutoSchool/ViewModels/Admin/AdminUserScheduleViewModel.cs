using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
using AutoSchool.Views.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels.Admin;

public class AdminUserScheduleViewModel : BaseViewModel
{
    public class CreditRow
    {
        public int Id { get; set; }
        public DateTime PlannedAt { get; set; }
        public string TopicName { get; set; } = "";
        public int DurationMinutes { get; set; }
        public string Room { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Notes { get; set; }
    }

    private readonly IPageNavigationService _nav;
    private readonly int _userId;

    public string HeaderText { get; private set; } = "Расписание";

    public DateTime? TrainingStartDate { get; set; }
    public DateTime? TrainingPlannedEndDate { get; set; }

    public ObservableCollection<CreditRow> Credits { get; } = new();

    private CreditRow? _selectedCredit;
    public CreditRow? SelectedCredit
    {
        get => _selectedCredit;
        set { _selectedCredit = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SavePeriodCommand { get; }
    public ICommand BackCommand { get; }

    public AdminUserScheduleViewModel(IPageNavigationService nav, int userId)
    {
        _nav = nav;
        _userId = userId;

        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        SavePeriodCommand = new RelayCommand(_ => SavePeriod());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminUsersPage(_nav)));

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var user = context.Users.First(u => u.Id == _userId);
        HeaderText = $"Расписание — {user.FirstName} {user.LastName} ({user.Email})";
        OnPropertyChanged(nameof(HeaderText));

        TrainingStartDate = user.TrainingStartDate?.Date;
        TrainingPlannedEndDate = user.TrainingPlannedEndDate?.Date;
        OnPropertyChanged(nameof(TrainingStartDate));
        OnPropertyChanged(nameof(TrainingPlannedEndDate));

        var list = context.TheoryCredits
            .Include(c => c.Topic)
            .Where(c => c.UserId == _userId)
            .OrderBy(c => c.PlannedAt)
            .ToList();

        Credits.Clear();
        foreach (var c in list)
        {
            Credits.Add(new CreditRow
            {
                Id = c.Id,
                PlannedAt = c.PlannedAt,
                TopicName = c.Topic?.Name ?? "—",
                DurationMinutes = c.DurationMinutes,
                Room = c.Room,
                Status = c.Status.ToString(),
                Notes = c.Notes
            });
        }
    }

    private void SavePeriod()
    {
        if (TrainingStartDate.HasValue && TrainingPlannedEndDate.HasValue &&
            TrainingPlannedEndDate.Value.Date < TrainingStartDate.Value.Date)
        {
            MessageBox.Show("Дата окончания не может быть раньше даты начала.");
            return;
        }

        using var context = new ApplicationDbContext();
        var user = context.Users.First(u => u.Id == _userId);

        user.TrainingStartDate = TrainingStartDate?.Date;
        user.TrainingPlannedEndDate = TrainingPlannedEndDate?.Date;

        context.SaveChanges();
        MessageBox.Show("Период обучения сохранён.");
    }

    private void Add()
    {
        var dlg = new TheoryCreditEditWindow();
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();

        context.TheoryCredits.Add(new Models.TheoryCredit
        {
            UserId = _userId,
            TopicId = dlg.TopicId,
            PlannedAt = dlg.PlannedAt,
            DurationMinutes = dlg.DurationMinutes,
            Room = dlg.Room,
            Status = dlg.Status,
            Notes = dlg.Notes
        });

        context.SaveChanges();
        Load();
    }

    private void Edit()
    {
        if (SelectedCredit == null)
        {
            MessageBox.Show("Выберите запись расписания.");
            return;
        }

        using var context = new ApplicationDbContext();
        var credit = context.TheoryCredits.First(c => c.Id == SelectedCredit.Id);

        var dlg = new TheoryCreditEditWindow(credit);
        if (dlg.ShowDialog() != true) return;

        credit.TopicId = dlg.TopicId;
        credit.PlannedAt = dlg.PlannedAt;
        credit.DurationMinutes = dlg.DurationMinutes;
        credit.Room = dlg.Room;
        credit.Status = dlg.Status;
        credit.Notes = dlg.Notes;

        context.SaveChanges();
        Load();
    }

    private void Delete()
    {
        if (SelectedCredit == null)
        {
            MessageBox.Show("Выберите запись расписания.");
            return;
        }

        if (MessageBox.Show("Удалить запись из расписания?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        using var context = new ApplicationDbContext();
        var credit = context.TheoryCredits.First(c => c.Id == SelectedCredit.Id);
        context.TheoryCredits.Remove(credit);
        context.SaveChanges();
        Load();
    }
}