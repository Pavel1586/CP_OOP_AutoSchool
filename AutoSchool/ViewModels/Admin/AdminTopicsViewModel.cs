using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
using AutoSchool.Views.Admin;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.ViewModels.Admin;

public class AdminTopicsViewModel : BaseViewModel
{
    public class TopicRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    private readonly IPageNavigationService _nav;

    public ObservableCollection<TopicRow> Topics { get; } = new();

    private TopicRow? _selectedTopic;
    public TopicRow? SelectedTopic
    {
        get => _selectedTopic;
        set { _selectedTopic = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BackCommand { get; }

    public AdminTopicsViewModel(IPageNavigationService nav)
    {
        _nav = nav;
        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketsPage(_nav)));
        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();
        Topics.Clear();
        foreach (var t in context.Topics.OrderBy(t => t.Id).ToList())
            Topics.Add(new TopicRow { Id = t.Id, Name = t.Name });
    }

    private void Add()
    {
        var dlg = new TopicEditWindow();
        if (dlg.ShowDialog() != true) return;

        var name = (dlg.TopicName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Введите название темы.");
            return;
        }

        using var context = new ApplicationDbContext();

        bool exists = context.Topics.Any(t => t.Name.ToLower() == name.ToLower());
        if (exists)
        {
            MessageBox.Show("Такая тема уже существует.");
            return;
        }

        context.Topics.Add(new Models.Topic { Name = name });
        context.SaveChanges();
        Load();
    }

    private void Edit()
    {
        if (SelectedTopic == null)
        {
            MessageBox.Show("Выберите тему.");
            return;
        }

        var dlg = new TopicEditWindow(SelectedTopic.Name);
        if (dlg.ShowDialog() != true) return;

        var name = (dlg.TopicName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Введите название темы.");
            return;
        }

        using var context = new ApplicationDbContext();

        bool exists = context.Topics.Any(t => t.Id != SelectedTopic.Id && t.Name.ToLower() == name.ToLower());
        if (exists)
        {
            MessageBox.Show("Такая тема уже существует.");
            return;
        }

        var topic = context.Topics.First(t => t.Id == SelectedTopic.Id);
        topic.Name = name;

        context.SaveChanges();
        Load();
    }

    private void Delete()
    {
        if (SelectedTopic == null)
        {
            MessageBox.Show("Выберите тему.");
            return;
        }

        using var context = new ApplicationDbContext();

        bool usedByQuestions = context.Questions.Any(q => q.TopicId == SelectedTopic.Id);
        bool usedByCredits = context.TheoryCredits.Any(c => c.TopicId == SelectedTopic.Id);
        bool usedByTickets = context.Tickets.Any(t => EF.Property<int>(t, "TopicId") == SelectedTopic.Id);

        if (usedByQuestions || usedByCredits || usedByTickets)
        {
            MessageBox.Show("Нельзя удалить тему: она используется (вопросы/билеты/зачёты).");
            return;
        }

        if (MessageBox.Show("Удалить тему?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        var topic = context.Topics.First(t => t.Id == SelectedTopic.Id);
        context.Topics.Remove(topic);
        context.SaveChanges();
        Load();
    }
}