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

    public string Title { get; private set; } = "";
    public ObservableCollection<ReviewItem> Items { get; } = new();
    public ICommand BackCommand { get; }

    private readonly int _testResultId;

    public TestReviewViewModel(int testResultId)
    {
        _testResultId = testResultId;
        BackCommand = new RelayCommand(Back);

        LocalizationManager.LanguageChanged += (_, __) => Load();
        Load();
    }

    private void Load()
    {
        Items.Clear();

        using var context = new ApplicationDbContext();
        var result = context.TestResults
            .Include(r => r.Ticket)
            .Include(r => r.Answers).ThenInclude(a => a.Question).ThenInclude(q => q.AnswerOptions)
            .Include(r => r.Answers).ThenInclude(a => a.SelectedOption)
            .First(r => r.Id == _testResultId);

        Title = Loc.F("Str_ReviewTitleFormat", result.Ticket?.Title ?? "");
        OnPropertyChanged(nameof(Title));

        foreach (var a in result.Answers.OrderBy(x => x.QuestionId))
        {
            var q = a.Question!;
            var correct = q.AnswerOptions.FirstOrDefault(o => o.IsCorrect)?.Text ?? Loc.T("Str_NoCorrectAnswer");
            var your = a.SelectedOption?.Text ?? Loc.T("Str_NoAnswer");

            Items.Add(new ReviewItem
            {
                QuestionText = q.Text,
                YourAnswer = your,
                CorrectAnswer = correct,
                Explanation = q.Explanation
            });
        }
    }

    private void Back(object? parameter)
    {
        var w = new TestResultWindow(_testResultId);
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }
}