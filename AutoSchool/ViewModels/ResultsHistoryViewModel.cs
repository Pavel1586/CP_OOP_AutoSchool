using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Services.Abstractions;
using AutoSchool.Views;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.ViewModels;

public class ResultsHistoryViewModel : BaseViewModel
{
    private const int AllowedMistakes = 1;

    public class ResultRow
    {
        public int Id { get; set; }
        public DateTime PassedAt { get; set; }
        public string TicketTitle { get; set; } = "";
        public int Correct { get; set; }
        public int Wrong { get; set; }
        public string Status { get; set; } = "";
    }

    public ObservableCollection<ResultRow> Results { get; } = new();

    private ResultRow? _selectedResult;
    public ResultRow? SelectedResult
    {
        get => _selectedResult;
        set { _selectedResult = value; OnPropertyChanged(); }
    }

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        private set { _isEmpty = value; OnPropertyChanged(); OnPropertyChanged(nameof(EmptyVisibility)); }
    }

    public Visibility EmptyVisibility => IsEmpty ? Visibility.Visible : Visibility.Collapsed;

    public ICommand OpenSelectedCommand { get; }
    public ICommand BackCommand { get; }

    private readonly IResultsService _resultsService;

    public ResultsHistoryViewModel() : this(new ResultsService()) { }
    public ResultsHistoryViewModel(IResultsService resultsService)
    {
        _resultsService = resultsService;
        OpenSelectedCommand = new RelayCommand(OpenSelected);
        BackCommand = new RelayCommand(Back);
        Load();
    }

    private void Load()
    {
        Results.Clear();

        if (UserSession.CurrentUser == null)
        {
            IsEmpty = true;
            return;
        }

        var list = _resultsService.GetUserHistory(UserSession.CurrentUser.Id);

        foreach (var r in list)
        {
            Results.Add(new ResultRow
            {
                Id = r.Id,
                PassedAt = r.PassedAt,
                TicketTitle = r.Title,
                Correct = r.Correct,
                Wrong = r.Wrong,
                Status = r.Wrong <= AllowedMistakes ? "СДАН" : "НЕ СДАН"
            });
        }

        IsEmpty = Results.Count == 0;
    }

    private void OpenSelected(object? parameter)
    {
        if (SelectedResult == null)
        {
            MessageBox.Show("Выберите результат.");
            return;
        }

        var w = new TestResultWindow(SelectedResult.Id);
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }

    private void Back(object? parameter)
    {
        var w = new MainMenuWindow();
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }
}