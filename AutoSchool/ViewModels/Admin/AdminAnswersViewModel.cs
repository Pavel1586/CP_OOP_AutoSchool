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

public class AdminAnswersViewModel : BaseViewModel
{
    public class OptionRow
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }
        public string IsCorrectText => IsCorrect ? "Да" : "Нет";
    }

    private readonly IPageNavigationService _nav;
    private readonly int _ticketId;
    private readonly int _questionId;

    public string HeaderText { get; private set; } = "Ответы";
    public string QuestionText { get; private set; } = "";

    public ObservableCollection<OptionRow> Options { get; } = new();

    private OptionRow? _selectedOption;
    public OptionRow? SelectedOption
    {
        get => _selectedOption;
        set { _selectedOption = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand MarkCorrectCommand { get; }
    public ICommand BackCommand { get; }

    public AdminAnswersViewModel(IPageNavigationService nav, int ticketId, int questionId)
    {
        _nav = nav;
        _ticketId = ticketId;
        _questionId = questionId;

        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        MarkCorrectCommand = new RelayCommand(_ => MarkCorrect());
        BackCommand = new RelayCommand(_ => Back());

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();
        var q = context.Questions
            .Include(x => x.AnswerOptions)
            .First(x => x.Id == _questionId);

        HeaderText = "Ответы";
        QuestionText = q.Text;

        OnPropertyChanged(nameof(HeaderText));
        OnPropertyChanged(nameof(QuestionText));

        Options.Clear();
        foreach (var o in q.AnswerOptions.OrderBy(x => x.Id))
        {
            Options.Add(new OptionRow
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect
            });
        }
    }

    private void Add()
    {
        var dlg = new AnswerOptionEditWindow();
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        context.AnswerOptions.Add(new Models.AnswerOption
        {
            QuestionId = _questionId,
            Text = dlg.OptionText,
            IsCorrect = false
        });
        context.SaveChanges();
        Load();
    }

    private void Edit()
    {
        if (SelectedOption == null)
        {
            MessageBox.Show("Выберите вариант ответа.");
            return;
        }

        var dlg = new AnswerOptionEditWindow(SelectedOption.Text);
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        var opt = context.AnswerOptions.First(x => x.Id == SelectedOption.Id);
        opt.Text = dlg.OptionText;
        context.SaveChanges();
        Load();
    }

    private void Delete()
    {
        if (SelectedOption == null)
        {
            MessageBox.Show("Выберите вариант ответа.");
            return;
        }

        if (MessageBox.Show("Удалить вариант ответа?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        using var context = new ApplicationDbContext();
        var opt = context.AnswerOptions.First(x => x.Id == SelectedOption.Id);
        context.AnswerOptions.Remove(opt);
        context.SaveChanges();
        Load();
    }

    private void MarkCorrect()
    {
        if (SelectedOption == null)
        {
            MessageBox.Show("Выберите вариант ответа.");
            return;
        }

        using var context = new ApplicationDbContext();

        var options = context.AnswerOptions
            .Where(x => x.QuestionId == _questionId)
            .ToList();

        foreach (var o in options)
            o.IsCorrect = (o.Id == SelectedOption.Id);

        context.SaveChanges();
        Load();
    }

    private void Back()
    {
        _nav.Navigate(new AdminQuestionsPage(_nav, _ticketId));
    }
}