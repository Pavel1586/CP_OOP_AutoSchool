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
    }

    private readonly IPageNavigationService _nav;

    public ObservableCollection<TicketRow> Tickets { get; } = new();

    private TicketRow? _selectedTicket;
    public TicketRow? SelectedTicket
    {
        get => _selectedTicket;
        set { _selectedTicket = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenQuestionsCommand { get; }

    public AdminTicketsViewModel(IPageNavigationService nav)
    {
        _nav = nav;

        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        OpenQuestionsCommand = new RelayCommand(_ => OpenQuestions());

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var list = context.Tickets
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
                QuestionsCount = t.Questions.Count
            });
        }
    }

    private void Add()
    {
        var dlg = new TicketEditWindow();
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        context.Tickets.Add(new Models.Ticket { Title = dlg.TicketTitle });
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

        var dlg = new TicketEditWindow(SelectedTicket.Title);
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        var ticket = context.Tickets.First(t => t.Id == SelectedTicket.Id);
        ticket.Title = dlg.TicketTitle;
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

        if (MessageBox.Show("Удалить билет? Будут удалены связанные вопросы/ответы.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        using var context = new ApplicationDbContext();
        var ticket = context.Tickets.First(t => t.Id == SelectedTicket.Id);
        context.Tickets.Remove(ticket);
        context.SaveChanges();

        Load();
    }

    private void OpenQuestions()
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        // следующий шаг: AdminQuestionsPage(ticketId)
        _nav.Navigate(new AdminQuestionsPage(_nav, SelectedTicket.Id));
    }
}