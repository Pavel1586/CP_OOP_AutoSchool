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

public class TicketSelectionByTopicViewModel : BaseViewModel
{
    public class TicketRow
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int TopicQuestionsCount { get; set; }
        public int TotalQuestionsCount { get; set; }
    }

    private readonly int _topicId;

    public string HeaderText { get; }

    public ObservableCollection<TicketRow> Tickets { get; } = new();

    private TicketRow? _selectedTicket;
    public TicketRow? SelectedTicket
    {
        get => _selectedTicket;
        set { _selectedTicket = value; OnPropertyChanged(); }
    }

    public ICommand OpenTicketCommand { get; }
    public ICommand BackCommand { get; }

    private readonly ITicketService _ticketService;

    public TicketSelectionByTopicViewModel(int topicId, string topicName)
        : this(topicId, topicName, new TicketService()) { }

    public TicketSelectionByTopicViewModel(int topicId, string topicName, ITicketService ticketService)
    {
        _topicId = topicId;
        _ticketService = ticketService;
        HeaderText = $"Тема: {topicName} — выберите билет";
        OpenTicketCommand = new RelayCommand(OpenTicket);
        BackCommand = new RelayCommand(Back);
        Load();
    }

    private void Load()
    {
        Tickets.Clear();
        foreach (var t in _ticketService.GetTicketsByTopic(_topicId))
        {
            Tickets.Add(new TicketRow
            {
                Id = t.Id,
                Title = t.Title,
                TopicQuestionsCount = t.TopicQuestionsCount,
                TotalQuestionsCount = t.TotalQuestionsCount
            });
        }
    }

    private void OpenTicket(object? parameter)
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        // Тест запускаем по билету как обычно
        var w = new TestPassingWindow(SelectedTicket.Id);
        w.Show();

        if (parameter is Window currentWindow) currentWindow.Close();
    }

    private void Back(object? parameter)
    {
        var w = new TopicSelectionWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }
}