using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Services.Abstractions;
using AutoSchool.Views;

namespace AutoSchool.ViewModels;

public class TopicSelectionViewModel : BaseViewModel
{
    public class TopicRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int QuestionsCount { get; set; }
        public int TicketsCount { get; set; }
    }

    public ObservableCollection<TopicRow> Topics { get; } = new();

    private TopicRow? _selectedTopic;
    public TopicRow? SelectedTopic
    {
        get => _selectedTopic;
        set { _selectedTopic = value; OnPropertyChanged(); }
    }

    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }

    private readonly ITopicService _topicService;

    public TopicSelectionViewModel() : this(new TopicService()) { }

    public TopicSelectionViewModel(ITopicService topicService)
    {
        _topicService = topicService;
        NextCommand = new RelayCommand(Next);
        BackCommand = new RelayCommand(Back);
        Load();
    }

    private void Load()
    {
        Topics.Clear();
        foreach (var t in _topicService.GetTopicsForSelection())
        {
            Topics.Add(new TopicRow
            {
                Id = t.Id,
                Name = t.Name,
                QuestionsCount = t.QuestionsCount,
                TicketsCount = t.TicketsCount
            });
        }
    }

    private void Next(object? parameter)
    {
        if (SelectedTopic == null)
        {
            MessageBox.Show(Loc.T("Msg_SelectTopic"));
            return;
        }

        if (SelectedTopic.TicketsCount == 0)
        {
            MessageBox.Show(Loc.T("Msg_NoTicketsInTopic"));
            return;
        }

        var w = new TicketSelectionByTopicWindow(SelectedTopic.Id, SelectedTopic.Name);
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }

    private void Back(object? parameter)
    {
        var w = new MainMenuWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }
}