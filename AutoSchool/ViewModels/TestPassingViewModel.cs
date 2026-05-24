using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using AutoSchool.Infrastructure;
using AutoSchool.Models;
using AutoSchool.Services;
using AutoSchool.Services.Abstractions;
using AutoSchool.Views;

namespace AutoSchool.ViewModels;

public enum QuestionAnswerState { None = 0, Correct = 1, Wrong = 2 }

public sealed class QuestionNavItem : BaseViewModel
{
    public int Index { get; init; }
    public int QuestionId { get; init; }
    public int Number { get; init; }

    private bool _isCurrent;
    public bool IsCurrent { get => _isCurrent; set { _isCurrent = value; OnPropertyChanged(); } }

    private bool _hasSelection;
    public bool HasSelection { get => _hasSelection; set { _hasSelection = value; OnPropertyChanged(); } }

    private bool _isCommitted;
    public bool IsCommitted { get => _isCommitted; set { _isCommitted = value; OnPropertyChanged(); } }

    private QuestionAnswerState _state = QuestionAnswerState.None;
    public QuestionAnswerState State
    {
        get => _state;
        set
        {
            _state = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCorrect));
            OnPropertyChanged(nameof(IsWrong));
        }
    }

    public bool IsCorrect => State == QuestionAnswerState.Correct;
    public bool IsWrong => State == QuestionAnswerState.Wrong;
}

public class TestPassingViewModel : BaseViewModel
{
    private const int QuestionsPerTicket = 10;
    private const int AllowedMistakes = 1;
    private static readonly TimeSpan TimeLimit = TimeSpan.FromMinutes(15);

    private readonly ITestService _testService;

    private int _mistakes;
    private readonly HashSet<int> _committedQuestions = new();
    private readonly Dictionary<int, int?> _selectedOptionByQuestionId = new();

    private List<Question> _questions;
    private int _currentIndex;
    private Question _currentQuestion = null!;
    private AnswerOption? _selectedOption;

    private readonly DispatcherTimer _timer;
    private TimeSpan _timeLeft;

    private bool _isRestoringSelection;
    private bool _confirmNextPending;

    // ===== FEEDBACK =====
    private QuestionAnswerState _feedbackState = QuestionAnswerState.None;

    public bool IsFeedbackVisible => _feedbackState != QuestionAnswerState.None;
    public bool IsFeedbackCorrect => _feedbackState == QuestionAnswerState.Correct;
    public bool IsFeedbackWrong => _feedbackState == QuestionAnswerState.Wrong;

    public string FeedbackText => _feedbackState switch
    {
        QuestionAnswerState.Correct => Loc.T("Str_CorrectUpper"),
        QuestionAnswerState.Wrong => Loc.T("Str_WrongUpper"),
        _ => ""
    };

    private void SetFeedback(QuestionAnswerState state)
    {
        _feedbackState = state;
        OnPropertyChanged(nameof(IsFeedbackVisible));
        OnPropertyChanged(nameof(IsFeedbackCorrect));
        OnPropertyChanged(nameof(IsFeedbackWrong));
        OnPropertyChanged(nameof(FeedbackText));
    }

    private void ClearFeedback() => SetFeedback(QuestionAnswerState.None);

    // ===== RESULT OVERLAY =====
    private bool _isResultVisible;
    public bool IsResultVisible
    {
        get => _isResultVisible;
        private set
        {
            _isResultVisible = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private object? _resultContent;
    public object? ResultContent
    {
        get => _resultContent;
        private set { _resultContent = value; OnPropertyChanged(); }
    }

    public string TicketTitle { get; }

    public ObservableCollection<QuestionNavItem> QuestionNav { get; } = new();

    public Question CurrentQuestion
    {
        get => _currentQuestion;
        private set
        {
            _currentQuestion = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsOptionsEnabled));
        }
    }

    public AnswerOption? SelectedOption
    {
        get => _selectedOption;
        set
        {
            if (!_isRestoringSelection && CurrentQuestion != null && _committedQuestions.Contains(CurrentQuestion.Id))
                return;

            _selectedOption = value;
            OnPropertyChanged();

            if (CurrentQuestion != null)
            {
                _selectedOptionByQuestionId[CurrentQuestion.Id] = _selectedOption?.Id;
                UpdateNavFlags(CurrentQuestion.Id);
            }

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public bool IsOptionsEnabled => CurrentQuestion != null && !_committedQuestions.Contains(CurrentQuestion.Id);

    public string ProgressText => _questions.Count == 0 ? "" : Loc.F("Str_QuestionXOfY", _currentIndex + 1, _questions.Count);

    public string TimeLeftText => _timeLeft.ToString(@"mm\:ss");

    public string ConfirmButtonText =>
        _confirmNextPending
            ? (_currentIndex == _questions.Count - 1 ? Loc.T("Str_Result") : Loc.T("Str_NextQuestion"))
            : Loc.T("Str_Confirm");

    public ICommand NextCommand { get; }
    public ICommand PrevCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand NavigateToQuestionCommand { get; }
    public ICommand ExitCommand { get; }

    public TestPassingViewModel(int ticketId) : this(ticketId, new TestService()) { }

    public TestPassingViewModel(int ticketId, ITestService testService)
    {
        _testService = testService;

        LocalizationManager.LanguageChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(ConfirmButtonText));
        };

        var ticket = _testService.GetTicketForTest(ticketId);
        TicketTitle = ticket.Title;

        _questions = ticket.Questions
            .OrderBy(q => q.Id)
            .Take(QuestionsPerTicket)
            .ToList();

        NextCommand = new RelayCommand(_ => Next(), _ => !IsResultVisible && _questions.Count > 0 && _currentIndex < _questions.Count - 1);
        PrevCommand = new RelayCommand(_ => Prev(), _ => !IsResultVisible && _questions.Count > 0 && _currentIndex > 0);
        ConfirmCommand = new RelayCommand(_ => Confirm(), _ => !IsResultVisible && _questions.Count > 0);
        NavigateToQuestionCommand = new RelayCommand(p => NavigateTo(p as QuestionNavItem), _ => !IsResultVisible);
        ExitCommand = new RelayCommand(Exit, _ => !IsResultVisible);

        if (_questions.Count < QuestionsPerTicket)
        {
            MessageBox.Show(Loc.F("Msg_NotEnoughQuestionsFormat", QuestionsPerTicket, _questions.Count));
            _questions = new List<Question>();
            _currentQuestion = new Question { Text = "—", AnswerOptions = new List<AnswerOption>() };
            _timeLeft = TimeLimit;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            return;
        }

        BuildNav();
        _currentIndex = 0;
        CurrentQuestion = _questions[_currentIndex];
        SyncCurrentInNav();
        RestoreSelection();
        ClearFeedback();

        _timeLeft = TimeLimit;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, __) =>
        {
            _timeLeft = _timeLeft.Add(TimeSpan.FromSeconds(-1));
            OnPropertyChanged(nameof(TimeLeftText));

            if (_timeLeft <= TimeSpan.Zero)
            {
                _timer.Stop();
                SaveAndShowResultInsideCurrentWindow();
            }
        };
        _timer.Start();
    }

    private void BuildNav()
    {
        QuestionNav.Clear();
        for (int i = 0; i < _questions.Count; i++)
        {
            var q = _questions[i];
            QuestionNav.Add(new QuestionNavItem
            {
                Index = i,
                QuestionId = q.Id,
                Number = i + 1,
                IsCurrent = false,
                HasSelection = false,
                IsCommitted = false,
                State = QuestionAnswerState.None
            });
        }
    }

    private void NavigateTo(QuestionNavItem? item)
    {
        if (item == null) return;
        GoToIndex(item.Index);
    }

    private void GoToIndex(int index)
    {
        if (index < 0 || index >= _questions.Count) return;

        _currentIndex = index;
        CurrentQuestion = _questions[_currentIndex];

        _confirmNextPending = false;
        OnPropertyChanged(nameof(ConfirmButtonText));

        ClearFeedback();

        OnPropertyChanged(nameof(ProgressText));
        SyncCurrentInNav();
        RestoreSelection();
    }

    private void SyncCurrentInNav()
    {
        for (int i = 0; i < QuestionNav.Count; i++)
            QuestionNav[i].IsCurrent = (i == _currentIndex);
    }

    private void Next()
    {
        if (_currentIndex >= _questions.Count - 1) return;
        GoToIndex(_currentIndex + 1);
    }

    private void Prev()
    {
        if (_currentIndex <= 0) return;
        GoToIndex(_currentIndex - 1);
    }

    private void RestoreSelection()
    {
        _isRestoringSelection = true;
        try
        {
            if (_selectedOptionByQuestionId.TryGetValue(CurrentQuestion.Id, out var optionId) && optionId.HasValue)
                SelectedOption = CurrentQuestion.AnswerOptions.FirstOrDefault(o => o.Id == optionId.Value);
            else
                SelectedOption = null;
        }
        finally
        {
            _isRestoringSelection = false;
        }
    }

    private void UpdateNavFlags(int questionId)
    {
        var nav = QuestionNav.FirstOrDefault(x => x.QuestionId == questionId);
        if (nav == null) return;

        nav.HasSelection = _selectedOptionByQuestionId.TryGetValue(questionId, out var optId) && optId.HasValue;
        nav.IsCommitted = _committedQuestions.Contains(questionId);
    }

    private void Confirm()
    {
        if (IsResultVisible) return;

        if (_confirmNextPending)
        {
            _confirmNextPending = false;
            OnPropertyChanged(nameof(ConfirmButtonText));
            ClearFeedback();

            if (_currentIndex < _questions.Count - 1)
            {
                GoToIndex(_currentIndex + 1);
                return;
            }

            _timer.Stop();
            SaveAndShowResultInsideCurrentWindow();
            return;
        }

        if (SelectedOption == null)
        {
            MessageBox.Show(Loc.T("Msg_SelectOption"));
            return;
        }

        int qId = CurrentQuestion.Id;

        if (_committedQuestions.Contains(qId))
        {
            _confirmNextPending = true;
            OnPropertyChanged(nameof(ConfirmButtonText));
            Confirm();
            return;
        }

        bool isCorrect = SelectedOption.IsCorrect;
        _committedQuestions.Add(qId);

        var nav = QuestionNav.First(x => x.QuestionId == qId);
        nav.IsCommitted = true;
        nav.State = isCorrect ? QuestionAnswerState.Correct : QuestionAnswerState.Wrong;

        OnPropertyChanged(nameof(IsOptionsEnabled));
        SetFeedback(isCorrect ? QuestionAnswerState.Correct : QuestionAnswerState.Wrong);

        if (!isCorrect)
        {
            _mistakes++;
            if (_mistakes > AllowedMistakes)
            {
                _timer.Stop();
                SaveAndShowResultInsideCurrentWindow();
                return;
            }
        }

        _confirmNextPending = true;
        OnPropertyChanged(nameof(ConfirmButtonText));
    }

    private void SaveAndShowResultInsideCurrentWindow()
    {
        if (IsResultVisible) return;
        if (UserSession.CurrentUser == null || _questions.Count == 0) return;

        var map = _questions.ToDictionary(
            q => q.Id,
            q => _selectedOptionByQuestionId.TryGetValue(q.Id, out var optId) ? optId : (int?)null
        );

        int testResultId = _testService.SaveTicketTestResult(
            UserSession.CurrentUser.Id,
            _questions.First().TicketId,
            map
        );

        var resultWindow = new TestResultWindow(testResultId);
        try
        {
            if (resultWindow.Content is FrameworkElement root)
            {
                root.DataContext = resultWindow.DataContext;

                foreach (var key in resultWindow.Resources.Keys)
                    root.Resources[key] = resultWindow.Resources[key];

                foreach (var md in resultWindow.Resources.MergedDictionaries)
                    root.Resources.MergedDictionaries.Add(md);

                resultWindow.Content = null;

                ResultContent = root;
                IsResultVisible = true;
            }
            else
            {
                ResultContent = new Border
                {
                    Padding = new Thickness(20),
                    Child = new System.Windows.Controls.TextBlock
                    {
                        Text = Loc.T("Msg_ShowResultFailed"),
                        FontSize = 16,
                        TextWrapping = TextWrapping.Wrap
                    }
                };
                IsResultVisible = true;
            }
        }
        finally
        {
            resultWindow.Close();
        }
    }

    private Window? GetCurrentWindow()
    {
        return Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this);
    }

    private void Exit(object? parameter)
    {
        if (IsResultVisible) return;

        var currentWindow = parameter as Window ?? GetCurrentWindow();

        var res = MessageBox.Show(
            Loc.T("Msg_ExitTestText"),
            Loc.T("Msg_ExitTestTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (res != MessageBoxResult.Yes) return;

        try { _timer?.Stop(); } catch { }

        var w = new MainMenuWindow();
        w.Show();
        currentWindow?.Close();
    }
}