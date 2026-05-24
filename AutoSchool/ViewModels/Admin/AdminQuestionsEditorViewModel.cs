using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutoSchool.Infrastructure;

namespace AutoSchool.ViewModels.Admin
{
    public class AdminQuestionsEditorViewModel : BaseViewModel
    {
        private readonly object? _nav;
        private readonly int? _ticketId;

        public AdminQuestionsEditorViewModel() : this(null, null) { }

        public AdminQuestionsEditorViewModel(object? nav, int? ticketId)
        {
            _nav = nav;
            _ticketId = ticketId;

            // демо-данные (убери, если грузишь из БД)
            if (Questions.Count == 0)
            {
                Questions.Add(new QuestionRow
                {
                    Id = 1,
                    Text = "Пример вопроса",
                    Explanation = "Пояснение (необязательно)",
                    Answers = new ObservableCollection<AnswerRow>
                    {
                        new() { Text = "Вариант 1", IsCorrect = true },
                        new() { Text = "Вариант 2", IsCorrect = false }
                    }
                });
                Questions[0].RefreshComputed();
            }

            NewQuestionCommand = new RelayCommand(_ => StartNew());
            SaveQuestionCommand = new RelayCommand(_ => Save());
            CancelEditCommand = new RelayCommand(_ => Cancel());

            DeleteQuestionCommand = new RelayCommand(_ => DeleteSelected(), _ => SelectedQuestion != null);

            AddAnswerCommand = new RelayCommand(_ => AddAnswer());
            RemoveAnswerCommand = new RelayCommand(_ => RemoveAnswer(), _ => SelectedEditAnswer != null);

            SetCorrectAnswerCommand = new RelayCommand(a => SetCorrect(a as AnswerRow));

            BackCommand = new RelayCommand(_ => GoBack());

            SelectedQuestion = Questions.FirstOrDefault();
            OnPropertyChanged(nameof(CounterText));
        }

        public string HeaderText => _ticketId.HasValue ? $"Вопросы (билет {_ticketId.Value})" : "Вопросы";
        public string CounterText => $"Всего: {Questions.Count}";

        public ObservableCollection<QuestionRow> Questions { get; } = new();

        private QuestionRow? _selectedQuestion;
        public QuestionRow? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                OnPropertyChanged();
                LoadFromSelected();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool _isNewMode;
        public string EditorTitle =>
            _isNewMode ? "Новый вопрос" :
            SelectedQuestion == null ? "Выбери вопрос слева" : "Редактирование вопроса";

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

        public ICommand NewQuestionCommand { get; }
        public ICommand SaveQuestionCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteQuestionCommand { get; }

        public ICommand AddAnswerCommand { get; }
        public ICommand RemoveAnswerCommand { get; }
        public ICommand SetCorrectAnswerCommand { get; }

        public ICommand BackCommand { get; }

        private void StartNew()
        {
            _isNewMode = true;
            ValidationText = "";

            SelectedQuestion = null;
            EditQuestionText = "";
            EditQuestionExplanation = "";

            EditAnswers.Clear();
            EditAnswers.Add(new AnswerRow { Text = "", IsCorrect = true });
            EditAnswers.Add(new AnswerRow { Text = "", IsCorrect = false });

            OnPropertyChanged(nameof(EditorTitle));
            CommandManager.InvalidateRequerySuggested();
        }

        private void LoadFromSelected()
        {
            if (SelectedQuestion == null)
            {
                if (!_isNewMode)
                {
                    EditQuestionText = "";
                    EditQuestionExplanation = "";
                    EditAnswers.Clear();
                    ValidationText = "";
                }

                OnPropertyChanged(nameof(EditorTitle));
                return;
            }

            _isNewMode = false;
            ValidationText = "";

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

        private void Save()
        {
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
                ValidationText = "Добавьте минимум 2 варианта ответа (с текстом).";
                return;
            }

            if (!answers.Any(a => a.IsCorrect))
            {
                ValidationText = "Отметьте правильный вариант.";
                return;
            }

            // защита: если отметили несколько — оставим первый
            if (answers.Count(a => a.IsCorrect) > 1)
            {
                bool first = true;
                foreach (var a in answers.Where(x => x.IsCorrect))
                {
                    if (first) { first = false; continue; }
                    a.IsCorrect = false;
                }
            }

            if (_isNewMode || SelectedQuestion == null)
            {
                int newId = Questions.Count == 0 ? 1 : Questions.Max(q => q.Id) + 1;

                var q = new QuestionRow
                {
                    Id = newId,
                    Text = text,
                    Explanation = (EditQuestionExplanation ?? "").Trim(),
                    Answers = new ObservableCollection<AnswerRow>(answers)
                };
                q.RefreshComputed();

                Questions.Add(q);
                SelectedQuestion = q;
                _isNewMode = false;
                OnPropertyChanged(nameof(CounterText));
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

            ValidationText = "Сохранено.";
            OnPropertyChanged(nameof(EditorTitle));
            CommandManager.InvalidateRequerySuggested();
        }

        private void Cancel()
        {
            ValidationText = "";
            _isNewMode = false;
            LoadFromSelected();
            OnPropertyChanged(nameof(EditorTitle));
        }

        private void DeleteSelected()
        {
            if (SelectedQuestion == null) return;

            var toRemove = SelectedQuestion;
            int index = Questions.IndexOf(toRemove);

            Questions.Remove(toRemove);
            OnPropertyChanged(nameof(CounterText));

            SelectedQuestion = Questions.ElementAtOrDefault(Math.Max(0, index - 1));
            ValidationText = "";
            CommandManager.InvalidateRequerySuggested();
        }

        private void GoBack()
        {
            // если у твоего navigation service есть метод GoBack() — вызовем его через reflection
            try
            {
                var m = _nav?.GetType().GetMethod("GoBack");
                m?.Invoke(_nav, null);
            }
            catch { }
        }

        // ======= модели для UI =======

        public class QuestionRow : BaseViewModel
        {
            public int Id { get; set; }

            private string? _text;
            public string? Text
            {
                get => _text;
                set { _text = value; OnPropertyChanged(); OnPropertyChanged(nameof(AnswersCount)); }
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