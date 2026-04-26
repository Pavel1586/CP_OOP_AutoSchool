using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views;

public partial class TicketSelectionByTopicWindow : Window
{
    public TicketSelectionByTopicWindow(int topicId, string topicName)
    {
        InitializeComponent();
        DataContext = new TicketSelectionByTopicViewModel(topicId, topicName);
    }
}