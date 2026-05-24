using System.Windows;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin;

public partial class TicketFillWindow : Window
{
    public TicketFillWindow(int ticketId)
    {
        InitializeComponent();
        DataContext = new AdminTicketFillViewModel(ticketId);
    }
}