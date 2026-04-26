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

public class AdminTicketSelectForQuestionsViewModel : BaseViewModel
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

    public ICommand OpenQuestionsCommand { get; }
    public ICommand BackCommand { get; }

    public AdminTicketSelectForQuestionsViewModel(IPageNavigationService nav)
    {
        _nav = nav;
        OpenQuestionsCommand = new RelayCommand(_ => OpenQuestions());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketsPage(_nav)));

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();
        var list = context.Tickets.Include(t => t.Questions).OrderBy(t => t.Id).ToList();

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

    private void OpenQuestions()
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Выберите билет.");
            return;
        }

        _nav.Navigate(new AdminQuestionsPage(_nav, SelectedTicket.Id));
    }
}