using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
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

public class AdminAnalyticsViewModel : BaseViewModel
{
    private const int AllowedMistakes = 1;

    public class TopUserRow
    {
        public string Email { get; set; } = "";
        public int Tests { get; set; }
        public int Passed { get; set; }
        public string PassRateText => Tests == 0 ? "0%" : $"{(Passed * 100.0 / Tests):0.#}%";
    }

    public class TicketStatRow
    {
        public string TicketTitle { get; set; } = "";
        public int Tests { get; set; }
        public int Passed { get; set; }
        public string PassRateText => Tests == 0 ? "0%" : $"{(Passed * 100.0 / Tests):0.#}%";
    }

    public class MistakeQuestionRow
    {
        public string TicketTitle { get; set; } = "";
        public string QuestionText { get; set; } = "";
        public int WrongCount { get; set; }
        public int TotalCount { get; set; }
        public string WrongRateText => TotalCount == 0 ? "0%" : $"{(WrongCount * 100.0 / TotalCount):0.#}%";
    }

    private readonly IPageNavigationService _nav;

    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    private int _totalTests;
    public int TotalTests { get => _totalTests; private set { _totalTests = value; OnPropertyChanged(); } }

    private int _uniqueUsers;
    public int UniqueUsers { get => _uniqueUsers; private set { _uniqueUsers = value; OnPropertyChanged(); } }

    private int _passedTests;
    public int PassedTests { get => _passedTests; private set { _passedTests = value; OnPropertyChanged(); OnPropertyChanged(nameof(PassRateText)); } }

    public string PassRateText => TotalTests == 0 ? "0%" : $"{(PassedTests * 100.0 / TotalTests):0.#}%";

    private double _avgCorrect;
    private double _avgWrong;
    public string AvgScoreText => TotalTests == 0 ? "—" : $"верно {_avgCorrect:0.#} / ошибок {_avgWrong:0.#}";

    public ObservableCollection<TopUserRow> TopUsers { get; } = new();
    public ObservableCollection<TicketStatRow> TicketStats { get; } = new();
    public ObservableCollection<MistakeQuestionRow> TopMistakeQuestions { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand ExportCsvCommand { get; }
    public ICommand BackCommand { get; }

    public AdminAnalyticsViewModel(IPageNavigationService nav)
    {
        _nav = nav;

        RefreshCommand = new RelayCommand(_ => Load());
        ExportCsvCommand = new RelayCommand(_ => ExportCsv());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminResultsPage(_nav)));

        // по умолчанию: текущий месяц (можешь поменять)
        var now = DateTime.Now;
        DateFrom = new DateTime(now.Year, now.Month, 1);
        DateTo = now.Date;

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var resultsQ = context.TestResults
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Ticket)
            .AsQueryable();

        if (DateFrom.HasValue)
        {
            var from = DateFrom.Value.Date;
            resultsQ = resultsQ.Where(r => r.PassedAt >= from);
        }
        if (DateTo.HasValue)
        {
            var toExclusive = DateTo.Value.Date.AddDays(1);
            resultsQ = resultsQ.Where(r => r.PassedAt < toExclusive);
        }

        var results = resultsQ.ToList();

        TotalTests = results.Count;
        UniqueUsers = results.Select(r => r.UserId).Distinct().Count();
        PassedTests = results.Count(r => r.WrongAnswers <= AllowedMistakes);

        _avgCorrect = results.Count == 0 ? 0 : results.Average(r => r.CorrectAnswers);
        _avgWrong = results.Count == 0 ? 0 : results.Average(r => r.WrongAnswers);
        OnPropertyChanged(nameof(AvgScoreText));

        // Топ пользователей
        TopUsers.Clear();
        foreach (var row in results
            .GroupBy(r => r.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                Email = g.First().User?.Email ?? "",
                Tests = g.Count(),
                Passed = g.Count(x => x.WrongAnswers <= AllowedMistakes)
            })
            .OrderByDescending(x => x.Passed)
            .ThenByDescending(x => x.Tests)
            .Take(15))
        {
            TopUsers.Add(new TopUserRow { Email = row.Email, Tests = row.Tests, Passed = row.Passed });
        }

        // Статистика по билетам
        TicketStats.Clear();
        foreach (var row in results
            .GroupBy(r => r.TicketId)
            .Select(g => new
            {
                TicketId = g.Key,
                TicketTitle = g.First().Ticket?.Title ?? "",
                Tests = g.Count(),
                Passed = g.Count(x => x.WrongAnswers <= AllowedMistakes)
            })
            .OrderByDescending(x => x.Tests))
        {
            TicketStats.Add(new TicketStatRow { TicketTitle = row.TicketTitle, Tests = row.Tests, Passed = row.Passed });
        }

        // Топ ошибок по вопросам (берём ответы TestAnswers за этот период)
        // Важно: группировку делаем в памяти — так надежнее для EF.
        var answersQ = context.TestAnswers
            .AsNoTracking()
            .Include(a => a.TestResult)
            .Include(a => a.Question).ThenInclude(q => q.Ticket)
            .AsQueryable();

        if (DateFrom.HasValue)
        {
            var from = DateFrom.Value.Date;
            answersQ = answersQ.Where(a => a.TestResult!.PassedAt >= from);
        }
        if (DateTo.HasValue)
        {
            var toExclusive = DateTo.Value.Date.AddDays(1);
            answersQ = answersQ.Where(a => a.TestResult!.PassedAt < toExclusive);
        }

        var answers = answersQ
            .Select(a => new
            {
                a.QuestionId,
                a.IsCorrect,
                QuestionText = a.Question!.Text,
                TicketTitle = a.Question!.Ticket!.Title
            })
            .ToList();

        TopMistakeQuestions.Clear();
        foreach (var row in answers
            .GroupBy(a => a.QuestionId)
            .Select(g => new
            {
                QuestionId = g.Key,
                TicketTitle = g.First().TicketTitle,
                QuestionText = g.First().QuestionText,
                Total = g.Count(),
                Wrong = g.Count(x => !x.IsCorrect)
            })
            .OrderByDescending(x => x.Wrong)
            .ThenByDescending(x => x.Total)
            .Take(20))
        {
            TopMistakeQuestions.Add(new MistakeQuestionRow
            {
                TicketTitle = row.TicketTitle,
                QuestionText = row.QuestionText,
                TotalCount = row.Total,
                WrongCount = row.Wrong
            });
        }
    }

    private void ExportCsv()
    {
        var dlg = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"analytics_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (dlg.ShowDialog() != true) return;

        var enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        using var sw = new StreamWriter(dlg.FileName, false, enc);

        sw.WriteLine("Раздел;Параметр;Значение");
        sw.WriteLine($"Сводка;Тестов;{TotalTests}");
        sw.WriteLine($"Сводка;Пользователей;{UniqueUsers}");
        sw.WriteLine($"Сводка;Сдано;{PassedTests}");
        sw.WriteLine($"Сводка;Процент сдачи;{PassRateText}");
        sw.WriteLine($"Сводка;Среднее верно;{_avgCorrect:0.#}");
        sw.WriteLine($"Сводка;Среднее ошибок;{_avgWrong:0.#}");

        sw.WriteLine();
        sw.WriteLine("Топ пользователей;Email;Тестов;Сдано;Процент сдачи");
        foreach (var u in TopUsers)
            sw.WriteLine($"Топ пользователей;{Esc(u.Email)};{u.Tests};{u.Passed};{u.PassRateText}");

        sw.WriteLine();
        sw.WriteLine("Билеты;Билет;Тестов;Сдано;Процент сдачи");
        foreach (var t in TicketStats)
            sw.WriteLine($"Билеты;{Esc(t.TicketTitle)};{t.Tests};{t.Passed};{t.PassRateText}");

        sw.WriteLine();
        sw.WriteLine("Ошибки;Билет;Вопрос;Ошибок;Всего;Процент");
        foreach (var q in TopMistakeQuestions)
            sw.WriteLine($"Ошибки;{Esc(q.TicketTitle)};{Esc(q.QuestionText)};{q.WrongCount};{q.TotalCount};{q.WrongRateText}");

        MessageBox.Show("Экспорт аналитики выполнен.");
    }

    private static string Esc(string s)
    {
        if (s.Contains(';') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }
}