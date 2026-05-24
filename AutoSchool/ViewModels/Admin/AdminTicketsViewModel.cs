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

public class AdminTicketsViewModel : BaseViewModel
{
    public class TicketRow
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int QuestionsCount { get; set; }
        public string TopicName { get; set; } = "";
    }

    private readonly IPageNavigationService _nav;

    public ObservableCollection<TicketRow> Tickets { get; } = new();

    private TicketRow? _selectedTicket;
    public TicketRow? SelectedTicket
    {
        get => _selectedTicket;
        set { _selectedTicket = value; OnPropertyChanged(); }
    }

    public string StatusText => $"Билетов: {Tickets.Count}";

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenQuestionsCommand { get; }
    public ICommand FillTicketCommand { get; }

    public AdminTicketsViewModel(IPageNavigationService nav)
    {
        _nav = nav;

        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        OpenQuestionsCommand = new RelayCommand(_ => OpenQuestions());
        FillTicketCommand = new RelayCommand(w => FillTicket(w as Window));

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var list = context.Tickets
            .AsNoTracking()
            .Include(t => t.Topic)
            .Include(t => t.Questions)
            .OrderBy(t => t.Id)
            .ToList();

        Tickets.Clear();

        foreach (var t in list)
        {
            Tickets.Add(new TicketRow
            {
                Id = t.Id,
                Title = t.Title,
                QuestionsCount = t.Questions.Count,
                TopicName = t.Topic?.Name ?? "—"
            });
        }

        OnPropertyChanged(nameof(StatusText));
    }

    private void Add()
    {
        var dlg = new TicketEditWindow();
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        context.Tickets.Add(new Models.Ticket
        {
            Title = dlg.TicketTitle,
            TopicId = dlg.TopicId
        });

        context.SaveChanges();
        Load();
    }

    private void Edit()
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        using var context = new ApplicationDbContext();
        var ticket = context.Tickets.First(t => t.Id == SelectedTicket.Id);

        var dlg = new TicketEditWindow(ticket.Title, ticket.TopicId);
        if (dlg.ShowDialog() != true) return;

        ticket.Title = dlg.TicketTitle;
        ticket.TopicId = dlg.TopicId;

        context.SaveChanges();
        Load();
    }

    private void Delete()
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        int ticketId = SelectedTicket.Id;

        using var context = new ApplicationDbContext();

        int resultsCount = context.TestResults.Count(r => r.TicketId == ticketId);

        string msg = resultsCount > 0
            ? $"Удалить билет?\n\nВНИМАНИЕ: по этому билету есть результатов тестов: {resultsCount}.\n" +
              $"Они будут удалены вместе с ответами (TestAnswers)."
            : "Удалить билет? Будут удалены связанные вопросы/ответы.";

        if (MessageBox.Show(msg, "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        var results = context.TestResults.Where(r => r.TicketId == ticketId).ToList();
        if (results.Count > 0)
            context.TestResults.RemoveRange(results);

        var ticket = context.Tickets.First(t => t.Id == ticketId);
        context.Tickets.Remove(ticket);

        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show(
                "Не удалось удалить билет.\n\n" + (ex.InnerException?.Message ?? ex.Message),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        Load();
    }

    private void OpenQuestions()
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        _nav.Navigate(new AdminQuestionsPage(_nav, SelectedTicket.Id));
    }

    private void FillTicket(Window? owner)
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        var win = new TicketFillWindow(SelectedTicket.Id);
        if (owner != null) win.Owner = owner;

        win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        win.ShowDialog();

        Load();
    }
}