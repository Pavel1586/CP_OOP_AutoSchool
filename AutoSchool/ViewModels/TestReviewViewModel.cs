using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels;

public class TestReviewViewModel : BaseViewModel
{
    public class ReviewItem
    {
        public string QuestionText { get; set; } = "";
        public string YourAnswer { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
        public string Explanation { get; set; } = "";
    }

    public string Title { get; }
    public ObservableCollection<ReviewItem> Items { get; } = new();

    public ICommand BackCommand { get; }

    private readonly int _testResultId;

    public TestReviewViewModel(int testResultId)
    {
        _testResultId = testResultId;

        using var context = new ApplicationDbContext();
        var result = context.TestResults
            .Include(r => r.Ticket)
            .Include(r => r.Answers).ThenInclude(a => a.Question).ThenInclude(q => q.AnswerOptions)
            .Include(r => r.Answers).ThenInclude(a => a.SelectedOption)
            .First(r => r.Id == testResultId);

        Title = $"Разбор: {result.Ticket?.Title}";

        foreach (var a in result.Answers.OrderBy(x => x.QuestionId))
        {
            var q = a.Question!;
            var correct = q.AnswerOptions.FirstOrDefault(o => o.IsCorrect)?.Text ?? "(не задан)";
            var your = a.SelectedOption?.Text ?? "(нет ответа)";

            Items.Add(new ReviewItem
            {
                QuestionText = q.Text,
                YourAnswer = your,
                CorrectAnswer = correct,
                Explanation = q.Explanation
            });
        }

        BackCommand = new RelayCommand(Back);
    }

    private void Back(object? parameter)
    {
        var w = new TestResultWindow(_testResultId);
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }
}