using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views;

public partial class TestResultWindow : Window
{
    public TestResultWindow(int testResultId)
    {
        InitializeComponent();
        DataContext = new TestResultViewModel(testResultId);
    }
}