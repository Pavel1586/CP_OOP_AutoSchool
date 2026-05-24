using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutoSchool.Infrastructure;

namespace AutoSchool.ViewModels.Admin
{
    public class AdminTicketsEditorViewModel : BaseViewModel
    {
        private readonly object? _nav;

        public AdminTicketsEditorViewModel(object? nav)
        {
            _nav = nav;

            // демо-данные (замени на загрузку из БД/сервиса)
            Tickets.Add(new TicketRow
            {
                Id = 1,
                Title = "Билет 1",
                Questions = new ObservableCollection<QuestionRow>
                {
                    new QuestionRow
                    {
                        Id = 1,
                        Text = "Пример вопроса",
                        Explanation = "",
                        Answers = new ObservableCollection<AnswerRow>
                        {
                            new AnswerRow{ Text="Вариант 1", IsCorrect=true },
                            new AnswerRow{ Text="Вариант 2", IsCorrect=false },
                        }
                    }
                }
            });

            SelectedTicket = Tickets.FirstOrDefault();
            SelectedQuestion = CurrentQuestions.FirstOrDefault();

            NewTicketCommand = new RelayCommand(_ => NewTicket());
            DeleteTicketCommand = new RelayCommand(_ => DeleteTicket(), _ => SelectedTicket != null);

            NewQuestionCommand = new RelayCommand(_ => NewQuestion(), _ => SelectedTicket != null);
            DeleteQuestionCommand = new RelayCommand(_ => DeleteQuestion(), _ => SelectedQuestion != null);

            SaveQuestionCommand = new RelayCommand(_ => SaveQuestion(), _ => SelectedTicket != null);
            CancelEditCommand = new RelayCommand(_ => CancelEdit());

            AddAnswerCommand = new RelayCommand(_ => AddAnswer(), _ => SelectedTicket != null);
            RemoveAnswerCommand = new RelayCommand(_ => RemoveAnswer(), _ => SelectedEditAnswer != null);

            SetCorrectAnswerCommand = new RelayCommand(a => SetCorrect(a as AnswerRow));
        }

        public ObservableCollection<TicketRow> Tickets { get; } = new();

        private TicketRow? _selectedTicket;
        public TicketRow? SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuestionsHeaderText));

                CurrentQuestions = _selectedTicket?.Questions ?? new ObservableCollection<QuestionRow>();
                OnPropertyChanged(nameof(CurrentQuestions));

                SelectedQuestion = CurrentQuestions.FirstOrDefault();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private ObservableCollection<QuestionRow> _currentQuestions = new();
        public ObservableCollection<QuestionRow> CurrentQuestions
        {
            get => _currentQuestions;
            private set { _currentQuestions = value; OnPropertyChanged(); }
        }

        private QuestionRow? _selectedQuestion;
        public QuestionRow? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                OnPropertyChanged();
                LoadFromSelectedQuestion();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string QuestionsHeaderText =>
            SelectedTicket == null ? "Вопросы (выбери билет слева)" : $"Вопросы билета: {SelectedTicket.Title}";

        public string StatusText => $"Билетов: {Tickets.Count}";

        private bool _isNewQuestionMode;

        public string EditorTitle =>
            SelectedTicket == null ? "Выбери билет" :
            _isNewQuestionMode ? "Новый вопрос" :
            SelectedQuestion == null ? "Выбери вопрос" : "Редактирование вопроса";

        private string _editQuestionText = "";
        public string EditQuestionText
        {
            get => _editQuestionText;
            set { _editQuestionText = value; OnPropertyChanged(); }
        }

        private string _editQuestionExplanation = "";
        public string EditQuestionExplanation
        {
            get => _editQuestionExplanation;
            set { _editQuestionExplanation = value; OnPropertyChanged(); }
        }

        public ObservableCollection<AnswerRow> EditAnswers { get; } = new();

        private AnswerRow? _selectedEditAnswer;
        public AnswerRow? SelectedEditAnswer
        {
            get => _selectedEditAnswer;
            set { _selectedEditAnswer = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        private string _validationText = "";
        public string ValidationText
        {
            get => _validationText;
            set { _validationText = value; OnPropertyChanged(); }
        }

        public ICommand NewTicketCommand { get; }
        public ICommand DeleteTicketCommand { get; }

        public ICommand NewQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand SaveQuestionCommand { get; }
        public ICommand CancelEditCommand { get; }

        public ICommand AddAnswerCommand { get; }
        public ICommand RemoveAnswerCommand { get; }
        public ICommand SetCorrectAnswerCommand { get; }

        private void NewTicket()
        {
            int id = Tickets.Count == 0 ? 1 : Tickets.Max(t => t.Id) + 1;
            var t = new TicketRow { Id = id, Title = $"Билет {id}" };
            Tickets.Add(t);
            SelectedTicket = t;
            ValidationText = "";
        }

        private void DeleteTicket()
        {
            if (SelectedTicket == null) return;
            var t = SelectedTicket;
            int idx = Tickets.IndexOf(t);
            Tickets.Remove(t);
            SelectedTicket = Tickets.ElementAtOrDefault(Math.Max(0, idx - 1));
            ValidationText = "";
        }

        private void NewQuestion()
        {
            if (SelectedTicket == null) return;

            _isNewQuestionMode = true;
            ValidationText = "";

            EditQuestionText = "";
            EditQuestionExplanation = "";

            EditAnswers.Clear();
            EditAnswers.Add(new AnswerRow { Text = "", IsCorrect = true });
            EditAnswers.Add(new AnswerRow { Text = "", IsCorrect = false });

            OnPropertyChanged(nameof(EditorTitle));
        }

        private void DeleteQuestion()
        {
            if (SelectedTicket == null || SelectedQuestion == null) return;

            var q = SelectedQuestion;
            int idx = CurrentQuestions.IndexOf(q);

            CurrentQuestions.Remove(q);
            SelectedQuestion = CurrentQuestions.ElementAtOrDefault(Math.Max(0, idx - 1));

            SelectedTicket.RefreshComputed();
            ValidationText = "";
        }

        private void LoadFromSelectedQuestion()
        {
            ValidationText = "";

            if (SelectedQuestion == null)
            {
                if (!_isNewQuestionMode)
                {
                    EditQuestionText = "";
                    EditQuestionExplanation = "";
                    EditAnswers.Clear();
                }

                OnPropertyChanged(nameof(EditorTitle));
                return;
            }

            _isNewQuestionMode = false;

            EditQuestionText = SelectedQuestion.Text ?? "";
            EditQuestionExplanation = SelectedQuestion.Explanation ?? "";

            EditAnswers.Clear();
            foreach (var a in SelectedQuestion.Answers)
                EditAnswers.Add(new AnswerRow { Text = a.Text, IsCorrect = a.IsCorrect });

            if (!EditAnswers.Any())
                EditAnswers.Add(new AnswerRow { Text = "", IsCorrect = true });

            OnPropertyChanged(nameof(EditorTitle));
        }

        private void AddAnswer()
        {
            EditAnswers.Add(new AnswerRow { Text = "", IsCorrect = !EditAnswers.Any(x => x.IsCorrect) });
            SelectedEditAnswer = EditAnswers.LastOrDefault();
        }

        private void RemoveAnswer()
        {
            if (SelectedEditAnswer == null) return;

            bool wasCorrect = SelectedEditAnswer.IsCorrect;
            int idx = EditAnswers.IndexOf(SelectedEditAnswer);

            EditAnswers.Remove(SelectedEditAnswer);
            SelectedEditAnswer = EditAnswers.ElementAtOrDefault(Math.Max(0, idx - 1));

            if (wasCorrect && EditAnswers.Count > 0)
                EditAnswers[0].IsCorrect = true;
        }

        private void SetCorrect(AnswerRow? answer)
        {
            if (answer == null) return;
            foreach (var a in EditAnswers)
                a.IsCorrect = ReferenceEquals(a, answer);
        }

        private void SaveQuestion()
        {
            if (SelectedTicket == null)
            {
                ValidationText = "Сначала выбери билет.";
                return;
            }

            ValidationText = "";

            var text = (EditQuestionText ?? "").Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                ValidationText = "Введите текст вопроса.";
                return;
            }

            var answers = EditAnswers
                .Select(a => new AnswerRow { Text = (a.Text ?? "").Trim(), IsCorrect = a.IsCorrect })
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .ToList();

            if (answers.Count < 2)
            {
                ValidationText = "Нужно минимум 2 варианта ответа (с текстом).";
                return;
            }

            if (!answers.Any(a => a.IsCorrect))
            {
                ValidationText = "Отметь правильный вариант.";
                return;
            }

            // защита: если вдруг несколько правильных — оставим первый
            if (answers.Count(a => a.IsCorrect) > 1)
            {
                bool first = true;
                foreach (var a in answers.Where(x => x.IsCorrect))
                {
                    if (first) { first = false; continue; }
                    a.IsCorrect = false;
                }
            }

            if (_isNewQuestionMode || SelectedQuestion == null)
            {
                int newId = CurrentQuestions.Count == 0 ? 1 : CurrentQuestions.Max(q => q.Id) + 1;

                var q = new QuestionRow
                {
                    Id = newId,
                    Text = text,
                    Explanation = (EditQuestionExplanation ?? "").Trim(),
                    Answers = new ObservableCollection<AnswerRow>(answers)
                };
                q.RefreshComputed();

                CurrentQuestions.Add(q);
                SelectedQuestion = q;
                _isNewQuestionMode = false;
            }
            else
            {
                SelectedQuestion.Text = text;
                SelectedQuestion.Explanation = (EditQuestionExplanation ?? "").Trim();

                SelectedQuestion.Answers.Clear();
                foreach (var a in answers)
                    SelectedQuestion.Answers.Add(a);

                SelectedQuestion.RefreshComputed();
            }

            SelectedTicket.RefreshComputed();
            ValidationText = "Сохранено.";
            OnPropertyChanged(nameof(EditorTitle));
        }

        private void CancelEdit()
        {
            ValidationText = "";
            _isNewQuestionMode = false;
            LoadFromSelectedQuestion();
            OnPropertyChanged(nameof(EditorTitle));
        }

        // ====== внутренние классы ======
        public class TicketRow : BaseViewModel
        {
            public int Id { get; set; }

            private string _title = "";
            public string Title
            {
                get => _title;
                set { _title = value; OnPropertyChanged(); }
            }

            public ObservableCollection<QuestionRow> Questions { get; set; } = new();

            public int QuestionsCount => Questions?.Count ?? 0;

            public void RefreshComputed() => OnPropertyChanged(nameof(QuestionsCount));
        }

        public class QuestionRow : BaseViewModel
        {
            public int Id { get; set; }

            private string? _text;
            public string? Text
            {
                get => _text;
                set { _text = value; OnPropertyChanged(); }
            }

            private string? _explanation;
            public string? Explanation
            {
                get => _explanation;
                set { _explanation = value; OnPropertyChanged(); }
            }

            public ObservableCollection<AnswerRow> Answers { get; set; } = new();

            public int AnswersCount => Answers?.Count ?? 0;
            public bool HasCorrect => Answers != null && Answers.Any(a => a.IsCorrect);

            public void RefreshComputed()
            {
                OnPropertyChanged(nameof(AnswersCount));
                OnPropertyChanged(nameof(HasCorrect));
            }
        }

        public class AnswerRow : BaseViewModel
        {
            private string? _text;
            public string? Text
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
}