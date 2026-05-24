using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Views;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels;

public class TestResultViewModel : BaseViewModel
{
    private const int AllowedMistakes = 1;

    public int TestResultId { get; }
    public string StatusText { get; private set; }
    public string SummaryText { get; private set; }
    public string MetaText { get; private set; }
    public string AnsweredText { get; private set; }

    public ICommand OpenReviewCommand { get; }
    public ICommand ToMenuCommand { get; }

    public TestResultViewModel(int testResultId)
    {
        TestResultId = testResultId;

        OpenReviewCommand = new RelayCommand(OpenReview);
        ToMenuCommand = new RelayCommand(ToMenu);

        LocalizationManager.LanguageChanged += (_, __) => Reload();
        Reload();
    }

    private void Reload()
    {
        using var context = new ApplicationDbContext();
        var result = context.TestResults
            .Include(r => r.Ticket)
            .Include(r => r.Answers)
            .First(r => r.Id == TestResultId);

        bool passed = result.WrongAnswers <= AllowedMistakes;
        StatusText = passed ? Loc.T("Str_TestPassedUpper") : Loc.T("Str_TestFailedUpper");

        int total = result.Answers.Count;
        int answered = result.Answers.Count(a => a.SelectedOptionId != null);

        AnsweredText = Loc.F("Str_AnsweredFormat", answered, total);
        SummaryText = Loc.F("Str_SummaryFormat", result.CorrectAnswers, result.WrongAnswers);
        MetaText = $"{result.Ticket?.Title} • {result.PassedAt:dd.MM.yyyy HH:mm}";

        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(AnsweredText));
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(MetaText));
    }

    private void OpenReview(object? parameter)
    {
        var w = new TestReviewWindow(TestResultId);
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }

    private void ToMenu(object? parameter)
    {
        var w = new MainMenuWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }
}