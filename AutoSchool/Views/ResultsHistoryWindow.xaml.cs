using System.Windows;
using AutoSchool.ViewModels;

namespace AutoSchool.Views
{
    public partial class ResultsHistoryWindow : Window
    {
        public ResultsHistoryWindow()
        {
            InitializeComponent();
            DataContext = new ResultsHistoryViewModel();
        }
    }
}