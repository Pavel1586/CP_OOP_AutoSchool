using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views;

public partial class TestReviewWindow : Window
{
    public TestReviewWindow(int testResultId)
    {
        InitializeComponent();
        DataContext = new TestReviewViewModel(testResultId);
    }
}