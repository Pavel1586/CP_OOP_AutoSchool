using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views;

public partial class TestPassingWindow : Window
{
    public TestPassingWindow(int ticketId)
    {
        InitializeComponent();
        DataContext = new TestPassingViewModel(ticketId);
    }
}