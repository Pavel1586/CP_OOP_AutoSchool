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

    public string StatusText { get; }
    public string SummaryText { get; }
    public string MetaText { get; }
    public string AnsweredText { get; }

    public ICommand OpenReviewCommand { get; }
    public ICommand ToMenuCommand { get; }

    public TestResultViewModel(int testResultId)
    {
        TestResultId = testResultId;

        using var context = new ApplicationDbContext();
        var result = context.TestResults
            .Include(r => r.Ticket)
            .Include(r => r.Answers) // важно
            .First(r => r.Id == testResultId);

        bool passed = result.WrongAnswers <= AllowedMistakes;
        StatusText = passed ? "ТЕСТ СДАН" : "ТЕСТ НЕ СДАН";

        int total = result.Answers.Count;
        int answered = result.Answers.Count(a => a.SelectedOptionId != null);

        AnsweredText = $"Отвечено: {answered}/{total}";
        SummaryText = $"Правильных: {result.CorrectAnswers} Ошибок: {result.WrongAnswers}";
        MetaText = $"{result.Ticket?.Title} • {result.PassedAt:dd.MM.yyyy HH:mm}";

        OpenReviewCommand = new RelayCommand(OpenReview);
        ToMenuCommand = new RelayCommand(ToMenu);
    }

    private void OpenReview(object? parameter)
    {
        var w = new TestReviewWindow(TestResultId);
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }

    private void ToMenu(object? parameter)
    {
        var w = new MainMenuWindow();
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }
}