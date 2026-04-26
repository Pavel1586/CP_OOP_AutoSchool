using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
using AutoSchool.Views.Admin;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels.Admin;

public class AdminQuestionsViewModel : BaseViewModel
{
    private const int QuestionsPerTicket = 10;

    public class QuestionRow
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public string Explanation { get; set; } = "";
        public int AnswersCount { get; set; }
        public string HasCorrect { get; set; } = ""; // "Да"/"Нет"
    }

    private readonly IPageNavigationService _nav;
    private readonly int _ticketId;

    private string _ticketTitle = "";
    public string HeaderText => $"Вопросы — {_ticketTitle}";
    public string CounterText => $"Вопросов: {Questions.Count}/{QuestionsPerTicket}";

    public ObservableCollection<QuestionRow> Questions { get; } = new();

    private QuestionRow? _selectedQuestion;
    public QuestionRow? SelectedQuestion
    {
        get => _selectedQuestion;
        set { _selectedQuestion = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenAnswersCommand { get; }
    public ICommand BackCommand { get; }

    public AdminQuestionsViewModel(IPageNavigationService nav, int ticketId)
    {
        _nav = nav;
        _ticketId = ticketId;

        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        OpenAnswersCommand = new RelayCommand(_ => OpenAnswers());
        BackCommand = new RelayCommand(_ => Back());

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var ticket = context.Tickets.First(t => t.Id == _ticketId);
        _ticketTitle = ticket.Title;

        var list = context.Questions
            .Where(q => q.TicketId == _ticketId)
            .Include(q => q.AnswerOptions)
            .OrderBy(q => q.Id)
            .ToList();

        Questions.Clear();
        foreach (var q in list)
        {
            bool hasCorrect = q.AnswerOptions.Any(a => a.IsCorrect);

            Questions.Add(new QuestionRow
            {
                Id = q.Id,
                Text = q.Text,
                Explanation = q.Explanation,
                AnswersCount = q.AnswerOptions.Count,
                HasCorrect = hasCorrect ? "Да" : "Нет"
            });
        }

        OnPropertyChanged(nameof(HeaderText));
        OnPropertyChanged(nameof(CounterText));
    }

    private void Add()
    {
        if (Questions.Count >= QuestionsPerTicket)
        {
            MessageBox.Show($"Нельзя добавить больше {QuestionsPerTicket} вопросов в билет.");
            return;
        }

        var dlg = new QuestionEditWindow();
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        context.Questions.Add(new Models.Question
        {
            TicketId = _ticketId,
            Text = dlg.QuestionText,
            Explanation = dlg.Explanation
        });
        context.SaveChanges();

        Load();
    }

    private void Edit()
    {
        if (SelectedQuestion == null)
        {
            MessageBox.Show("Выберите вопрос.");
            return;
        }

        var dlg = new QuestionEditWindow(SelectedQuestion.Text, SelectedQuestion.Explanation);
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        var q = context.Questions.First(x => x.Id == SelectedQuestion.Id);
        q.Text = dlg.QuestionText;
        q.Explanation = dlg.Explanation;
        context.SaveChanges();

        Load();
    }

    private void Delete()
    {
        if (SelectedQuestion == null)
        {
            MessageBox.Show("Выберите вопрос.");
            return;
        }

        if (MessageBox.Show("Удалить вопрос? Варианты ответов будут удалены.",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        using var context = new ApplicationDbContext();
        var q = context.Questions.First(x => x.Id == SelectedQuestion.Id);
        context.Questions.Remove(q);
        context.SaveChanges();

        Load();
    }

    private void OpenAnswers()
    {
        if (SelectedQuestion == null)
        {
            MessageBox.Show("Выберите вопрос.");
            return;
        }

        _nav.Navigate(new AdminAnswersPage(_nav, _ticketId, SelectedQuestion.Id));
    }

    private void Back()
    {
        _nav.Navigate(new AdminTicketsPage(_nav));
    }
}