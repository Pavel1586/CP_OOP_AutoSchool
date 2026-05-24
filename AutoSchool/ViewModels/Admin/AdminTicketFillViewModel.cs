using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels.Admin;

public class AdminTicketFillViewModel : BaseViewModel
{
    private const int QuestionsPerTicket = 10;
    private readonly int _ticketId;

    public string HeaderText { get; private set; } = "Заполнение билета";
    public string CounterText => $"Вопросов: {Questions.Count}/{QuestionsPerTicket}";

    public ObservableCollection<QuestionRow> Questions { get; } = new();

    private QuestionRow? _selectedQuestion;
    public QuestionRow? SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            _selectedQuestion = value;
            OnPropertyChanged();
            LoadSelected();
        }
    }

    private bool _isNewMode;
    public string EditorTitle =>
        _isNewMode ? "Новый вопрос" :
        SelectedQuestion == null ? "Выберите вопрос слева" :
        "Редактирование вопроса";

    private string _editQuestionText = "";
    public string EditQuestionText
    {
        get => _editQuestionText;
        set { _editQuestionText = value; OnPropertyChanged(); }
    }

    private string _editExplanation = "";
    public string EditExplanation
    {
        get => _editExplanation;
        set { _editExplanation = value; OnPropertyChanged(); }
    }

    // ===== NEW: картинка =====
    private byte[]? _editImageBytes;
    public byte[]? EditImageBytes
    {
        get => _editImageBytes;
        set
        {
            _editImageBytes = (value != null && value.Length == 0) ? null : value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasImage));
        }
    }

    public bool HasImage => EditImageBytes != null && EditImageBytes.Length > 0;

    public ObservableCollection<AnswerEditRow> EditAnswers { get; } = new();

    private AnswerEditRow? _selectedEditAnswer;
    public AnswerEditRow? SelectedEditAnswer
    {
        get => _selectedEditAnswer;
        set
        {
            _selectedEditAnswer = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private string _validationText = "";
    public string ValidationText
    {
        get => _validationText;
        set { _validationText = value; OnPropertyChanged(); }
    }

    public ICommand NewQuestionCommand { get; }
    public ICommand DeleteQuestionCommand { get; }
    public ICommand SaveQuestionCommand { get; }
    public ICommand AddAnswerCommand { get; }
    public ICommand RemoveAnswerCommand { get; }
    public ICommand SetCorrectAnswerCommand { get; }
    public ICommand CloseCommand { get; }

    // NEW
    public ICommand LoadImageCommand { get; }
    public ICommand ClearImageCommand { get; }

    public AdminTicketFillViewModel(int ticketId)
    {
        _ticketId = ticketId;

        NewQuestionCommand = new RelayCommand(_ => StartNew());
        DeleteQuestionCommand = new RelayCommand(_ => DeleteQuestion());
        SaveQuestionCommand = new RelayCommand(_ => SaveQuestion());
        AddAnswerCommand = new RelayCommand(_ => AddAnswer());
        RemoveAnswerCommand = new RelayCommand(_ => RemoveAnswer(), _ => SelectedEditAnswer != null);
        SetCorrectAnswerCommand = new RelayCommand(a => SetCorrect(a as AnswerEditRow));
        CloseCommand = new RelayCommand(w => (w as Window)?.Close());

        LoadImageCommand = new RelayCommand(_ => LoadImage());
        ClearImageCommand = new RelayCommand(_ => EditImageBytes = null, _ => HasImage);

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();
        var ticket = context.Tickets.AsNoTracking().First(t => t.Id == _ticketId);

        HeaderText = $"Заполнение — {ticket.Title}";
        OnPropertyChanged(nameof(HeaderText));

        var list = context.Questions
            .Where(q => q.TicketId == _ticketId)
            .Include(q => q.AnswerOptions)
            .OrderBy(q => q.Id)
            .ToList();

        Questions.Clear();
        foreach (var q in list)
        {
            Questions.Add(new QuestionRow
            {
                Id = q.Id,
                Text = q.Text,
                AnswersCount = q.AnswerOptions.Count,
                HasCorrectText = q.AnswerOptions.Any(a => a.IsCorrect) ? "Да" : "Нет",
                HasImageText = (q.Image != null && q.Image.Length > 0) ? "Да" : "Нет"
            });
        }

        OnPropertyChanged(nameof(CounterText));
        OnPropertyChanged(nameof(EditorTitle));

        SelectedQuestion = Questions.FirstOrDefault();
    }

    private void LoadSelected()
    {
        ValidationText = "";
        _isNewMode = false;

        if (SelectedQuestion == null)
        {
            EditQuestionText = "";
            EditExplanation = "";
            EditImageBytes = null;
            EditAnswers.Clear();
            OnPropertyChanged(nameof(EditorTitle));
            return;
        }

        using var context = new ApplicationDbContext();
        var q = context.Questions
            .Include(x => x.AnswerOptions)
            .First(x => x.Id == SelectedQuestion.Id);

        EditQuestionText = q.Text;
        EditExplanation = q.Explanation;
        EditImageBytes = q.Image; // NEW

        EditAnswers.Clear();
        foreach (var a in q.AnswerOptions.OrderBy(x => x.Id))
        {
            EditAnswers.Add(new AnswerEditRow { Id = a.Id, Text = a.Text, IsCorrect = a.IsCorrect });
        }

        if (EditAnswers.Count == 0)
        {
            EditAnswers.Add(new AnswerEditRow { Text = "", IsCorrect = true });
            EditAnswers.Add(new AnswerEditRow { Text = "", IsCorrect = false });
        }

        OnPropertyChanged(nameof(EditorTitle));
        CommandManager.InvalidateRequerySuggested();
    }

    private void StartNew()
    {
        ValidationText = "";

        if (Questions.Count >= QuestionsPerTicket)
        {
            MessageBox.Show($"Нельзя добавить больше {QuestionsPerTicket} вопросов в билет.");
            return;
        }

        _isNewMode = true;
        SelectedQuestion = null;

        EditQuestionText = "";
        EditExplanation = "";
        EditImageBytes = null; // NEW

        EditAnswers.Clear();
        EditAnswers.Add(new AnswerEditRow { Text = "", IsCorrect = true });
        EditAnswers.Add(new AnswerEditRow { Text = "", IsCorrect = false });

        OnPropertyChanged(nameof(EditorTitle));
        CommandManager.InvalidateRequerySuggested();
    }

    private void LoadImage()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
        };
        if (dlg.ShowDialog() != true) return;

        EditImageBytes = File.ReadAllBytes(dlg.FileName);
    }

    private void AddAnswer()
    {
        EditAnswers.Add(new AnswerEditRow { Text = "", IsCorrect = !EditAnswers.Any(x => x.IsCorrect) });
        SelectedEditAnswer = EditAnswers.LastOrDefault();
    }

    private void RemoveAnswer()
    {
        if (SelectedEditAnswer == null) return;

        var wasCorrect = SelectedEditAnswer.IsCorrect;
        EditAnswers.Remove(SelectedEditAnswer);
        SelectedEditAnswer = null;

        if (wasCorrect && EditAnswers.Count > 0)
            EditAnswers[0].IsCorrect = true;
    }

    private void SetCorrect(AnswerEditRow? row)
    {
        if (row == null) return;
        foreach (var a in EditAnswers)
            a.IsCorrect = ReferenceEquals(a, row);
    }

    private void SaveQuestion()
    {
        ValidationText = "";

        var text = (EditQuestionText ?? "").Trim();
        var expl = (EditExplanation ?? "").Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            ValidationText = "Введите условие вопроса.";
            return;
        }

        if (string.IsNullOrWhiteSpace(expl))
        {
            ValidationText = "Введите пояснение.";
            return;
        }

        var answers = EditAnswers
            .Select(a => new { a.Id, Text = (a.Text ?? "").Trim(), a.IsCorrect })
            .Where(a => !string.IsNullOrWhiteSpace(a.Text))
            .ToList();

        if (answers.Count < 2)
        {
            ValidationText = "Добавьте минимум 2 варианта ответа (с текстом).";
            return;
        }

        if (!answers.Any(a => a.IsCorrect))
        {
            ValidationText = "Отметьте правильный вариант.";
            return;
        }

        // один правильный
        if (answers.Count(a => a.IsCorrect) > 1)
        {
            var firstCorrect = answers.First(a => a.IsCorrect).Id;
            for (int i = 0; i < EditAnswers.Count; i++)
                EditAnswers[i].IsCorrect = (EditAnswers[i].Id == firstCorrect);
        }

        using var context = new ApplicationDbContext();

        if (_isNewMode || SelectedQuestion == null)
        {
            var count = context.Questions.Count(q => q.TicketId == _ticketId);
            if (count >= QuestionsPerTicket)
            {
                MessageBox.Show($"Нельзя добавить больше {QuestionsPerTicket} вопросов в билет.");
                return;
            }

            var q = new Question
            {
                TicketId = _ticketId,
                Text = text,
                Explanation = expl,
                Image = EditImageBytes // NEW
            };

            context.Questions.Add(q);
            context.SaveChanges();

            foreach (var a in answers)
            {
                context.AnswerOptions.Add(new AnswerOption
                {
                    QuestionId = q.Id,
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                });
            }

            context.SaveChanges();
        }
        else
        {
            var q = context.Questions.First(x => x.Id == SelectedQuestion.Id);

            q.Text = text;
            q.Explanation = expl;
            q.Image = EditImageBytes; // NEW

            var dbOptions = context.AnswerOptions.Where(o => o.QuestionId == q.Id).ToList();

            foreach (var a in answers)
            {
                if (a.Id.HasValue)
                {
                    var opt = dbOptions.FirstOrDefault(x => x.Id == a.Id.Value);
                    if (opt != null)
                    {
                        opt.Text = a.Text;
                        opt.IsCorrect = a.IsCorrect;
                    }
                }
                else
                {
                    context.AnswerOptions.Add(new AnswerOption
                    {
                        QuestionId = q.Id,
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    });
                }
            }

            var answerIdsLeft = answers.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
            var toDelete = dbOptions.Where(o => !answerIdsLeft.Contains(o.Id)).ToList();

            foreach (var del in toDelete)
            {
                bool used = context.TestAnswers.Any(a => a.SelectedOptionId == del.Id);
                if (used)
                {
                    MessageBox.Show("Нельзя удалить вариант ответа: он уже используется в результатах тестов.");
                    continue;
                }
                context.AnswerOptions.Remove(del);
            }

            context.SaveChanges();
        }

        ValidationText = "Сохранено.";
        Load();
    }

    private void DeleteQuestion()
    {
        ValidationText = "";

        if (SelectedQuestion == null)
        {
            MessageBox.Show("Выберите вопрос.");
            return;
        }

        using var context = new ApplicationDbContext();

        bool used = context.TestAnswers.Any(a => a.QuestionId == SelectedQuestion.Id);
        if (used)
        {
            MessageBox.Show("Нельзя удалить вопрос: он уже используется в результатах тестов.");
            return;
        }

        if (MessageBox.Show("Удалить вопрос? Варианты ответов будут удалены.",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        var q = context.Questions
            .Include(x => x.AnswerOptions)
            .First(x => x.Id == SelectedQuestion.Id);

        context.Questions.Remove(q);
        context.SaveChanges();

        Load();
    }

    // ==== внутренние классы ====
    public class QuestionRow
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public int AnswersCount { get; set; }
        public string HasCorrectText { get; set; } = "";
        public string HasImageText { get; set; } = ""; // NEW (инфо в списке)
    }

    public class AnswerEditRow : BaseViewModel
    {
        public int? Id { get; set; }

        private string _text = "";
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        private bool _isCorrect;
        public bool IsCorrect
        {
            get => _isCorrect;
            set { _isCorrect = value; OnPropertyChanged(); }
        }
    }
}