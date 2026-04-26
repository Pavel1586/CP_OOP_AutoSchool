using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
using AutoSchool.Views;
using AutoSchool.Views.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels.Admin;

public class AdminResultsViewModel : BaseViewModel
{
    private const int AllowedMistakes = 1;

    public class TicketItem
    {
        public int? Id { get; set; }           // null = Все
        public string Title { get; set; } = "";
    }

    public class ResultRow
    {
        public int Id { get; set; }
        public DateTime PassedAt { get; set; }
        public string UserEmail { get; set; } = "";
        public string UserName { get; set; } = "";
        public string TicketTitle { get; set; } = "";
        public int Correct { get; set; }
        public int Wrong { get; set; }
        public string Status { get; set; } = "";
    }

    private readonly IPageNavigationService _nav;

    public ObservableCollection<ResultRow> Results { get; } = new();
    public ObservableCollection<TicketItem> Tickets { get; } = new();
    public ObservableCollection<string> Statuses { get; } = new() { "Все", "СДАН", "НЕ СДАН" };

    private ResultRow? _selectedResult;
    public ResultRow? SelectedResult
    {
        get => _selectedResult;
        set { _selectedResult = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private TicketItem? _selectedTicket;
    public TicketItem? SelectedTicket
    {
        get => _selectedTicket;
        set { _selectedTicket = value; OnPropertyChanged(); }
    }

    private string _selectedStatus = "Все";
    public string SelectedStatus
    {
        get => _selectedStatus;
        set { _selectedStatus = value; OnPropertyChanged(); }
    }

    private DateTime? _dateFrom;
    public DateTime? DateFrom
    {
        get => _dateFrom;
        set { _dateFrom = value; OnPropertyChanged(); }
    }

    private DateTime? _dateTo;
    public DateTime? DateTo
    {
        get => _dateTo;
        set { _dateTo = value; OnPropertyChanged(); }
    }

    public ICommand RefreshCommand { get; }
    public ICommand OpenReviewCommand { get; }
    public ICommand OpenResultCommand { get; }
    public ICommand ExportCsvCommand { get; }
    public ICommand BackCommand { get; }

    public AdminResultsViewModel(IPageNavigationService nav)
    {
        _nav = nav;

        RefreshCommand = new RelayCommand(_ => Load());
        OpenReviewCommand = new RelayCommand(_ => OpenReview());
        OpenResultCommand = new RelayCommand(_ => OpenResult());
        ExportCsvCommand = new RelayCommand(_ => ExportCsv());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketsPage(_nav)));

        LoadTickets();
        Load();
    }

    private void LoadTickets()
    {
        using var context = new ApplicationDbContext();

        Tickets.Clear();
        Tickets.Add(new TicketItem { Id = null, Title = "Все" });

        foreach (var t in context.Tickets.OrderBy(x => x.Id).ToList())
            Tickets.Add(new TicketItem { Id = t.Id, Title = t.Title });

        SelectedTicket ??= Tickets.FirstOrDefault();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var q = context.TestResults
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Ticket)
            .AsQueryable();

        // Ticket filter
        if (SelectedTicket?.Id != null)
        {
            var tid = SelectedTicket.Id.Value;
            q = q.Where(r => r.TicketId == tid);
        }

        // Status filter
        if (SelectedStatus == "СДАН")
            q = q.Where(r => r.WrongAnswers <= AllowedMistakes);
        else if (SelectedStatus == "НЕ СДАН")
            q = q.Where(r => r.WrongAnswers > AllowedMistakes);

        // Date filter (по дате без времени)
        if (DateFrom.HasValue)
        {
            var from = DateFrom.Value.Date;
            q = q.Where(r => r.PassedAt >= from);
        }
        if (DateTo.HasValue)
        {
            var toExclusive = DateTo.Value.Date.AddDays(1);
            q = q.Where(r => r.PassedAt < toExclusive);
        }

        // Search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim().ToLower();
            q = q.Where(r =>
                r.User!.Email.ToLower().Contains(s) ||
                r.User.FirstName.ToLower().Contains(s) ||
                r.User.LastName.ToLower().Contains(s) ||
                r.Ticket!.Title.ToLower().Contains(s));
        }

        var list = q.OrderByDescending(r => r.PassedAt).ToList();

        Results.Clear();
        foreach (var r in list)
        {
            Results.Add(new ResultRow
            {
                Id = r.Id,
                PassedAt = r.PassedAt,
                UserEmail = r.User?.Email ?? "",
                UserName = $"{r.User?.FirstName} {r.User?.LastName}".Trim(),
                TicketTitle = r.Ticket?.Title ?? "",
                Correct = r.CorrectAnswers,
                Wrong = r.WrongAnswers,
                Status = r.WrongAnswers <= AllowedMistakes ? "СДАН" : "НЕ СДАН"
            });
        }
    }

    private void OpenReview()
    {
        if (SelectedResult == null)
        {
            MessageBox.Show("Выберите результат.");
            return;
        }

        // Открываем существующее окно разбора
        var w = new TestReviewWindow(SelectedResult.Id);
        w.Show();
    }

    private void OpenResult()
    {
        if (SelectedResult == null)
        {
            MessageBox.Show("Выберите результат.");
            return;
        }

        var w = new TestResultWindow(SelectedResult.Id);
        w.Show();
    }

    private void ExportCsv()
    {
        if (Results.Count == 0)
        {
            MessageBox.Show("Нет данных для экспорта.");
            return;
        }

        var dlg = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"results_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };

        if (dlg.ShowDialog() != true) return;

        // Excel/Windows лучше воспринимают UTF-8 с BOM для кириллицы
        var enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

        using var sw = new StreamWriter(dlg.FileName, false, enc);

        // разделитель ; (часто удобнее для RU Excel)
        sw.WriteLine("Id;Дата;Email;Пользователь;Билет;Верно;Ошибок;Статус");

        foreach (var r in Results)
        {
            sw.WriteLine(string.Join(";",
                r.Id,
                Escape(r.PassedAt.ToString("dd.MM.yyyy HH:mm")),
                Escape(r.UserEmail),
                Escape(r.UserName),
                Escape(r.TicketTitle),
                r.Correct,
                r.Wrong,
                Escape(r.Status)
            ));
        }

        MessageBox.Show($"Экспортировано: {Results.Count} строк.");
    }

    private static string Escape(string s)
    {
        // CSV-экранирование для ;
        if (s.Contains(';') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }
}