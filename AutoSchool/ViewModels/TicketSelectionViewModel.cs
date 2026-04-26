using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Models;
using AutoSchool.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels
{
    public class TicketSelectionViewModel : BaseViewModel
    {
        private Ticket? _selectedTicket;

        public ObservableCollection<Ticket> Tickets { get; set; } = new();

        public Ticket? SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenTicketCommand { get; }
        public ICommand BackCommand { get; }

        public TicketSelectionViewModel()
        {
            OpenTicketCommand = new RelayCommand(OpenTicket);
            BackCommand = new RelayCommand(Back);
            LoadTickets();
        }

        private void LoadTickets()
        {
            using var context = new ApplicationDbContext();
            var tickets = context.Tickets
                .Include(t => t.Questions)
                .ToList();

            Tickets.Clear();

            foreach (var ticket in tickets)
                Tickets.Add(ticket);
        }

        private void OpenTicket(object? parameter)
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Выберите билет.");
                return;
            }

            if (SelectedTicket.Questions.Count < 10)
            {
                MessageBox.Show($"В билете недостаточно вопросов: {SelectedTicket.Questions.Count}. Нужно 10.");
                return;
            }

            var testWindow = new TestPassingWindow(SelectedTicket.Id);
            testWindow.Show();

            if (parameter is Window currentWindow)
                currentWindow.Close();
        }

        private void Back(object? parameter)
        {
            MainMenuWindow mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();

            if (parameter is Window currentWindow)
                currentWindow.Close();
        }
    }
}