using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Views;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels;

public class AdminViewModel : BaseViewModel
{
    private string _sectionTitle = "Выберите раздел";
    public string SectionTitle
    {
        get => _sectionTitle;
        set { _sectionTitle = value; OnPropertyChanged(); }
    }

    private string _sectionHint =
        "Здесь будет управление билетами, вопросами, пользователями и просмотр результатов.\n" +
        "Следующим шагом реализуем CRUD по билетам/вопросам/ответам и просмотр результатов всех пользователей.";
    public string SectionHint
    {
        get => _sectionHint;
        set { _sectionHint = value; OnPropertyChanged(); }
    }

    public ICommand OpenTicketsCommand { get; }
    public ICommand OpenQuestionsCommand { get; }
    public ICommand OpenUsersCommand { get; }
    public ICommand OpenResultsCommand { get; }
    public ICommand LogoutCommand { get; }

    public AdminViewModel()
    {
        OpenTicketsCommand = new RelayCommand(_ => Select("Билеты", "CRUD билетов (создать/редактировать/удалить)."));
        OpenQuestionsCommand = new RelayCommand(_ => Select("Вопросы/ответы", "CRUD вопросов и вариантов ответов, назначение правильного ответа."));
        OpenUsersCommand = new RelayCommand(_ => Select("Пользователи", "Просмотр/редактирование пользователей, блокировка/удаление и т.д."));
        OpenResultsCommand = new RelayCommand(_ => Select("Результаты", "Просмотр результатов тестов всех пользователей, фильтры/поиск."));

        LogoutCommand = new RelayCommand(Logout);
    }

    private void Select(string title, string hint)
    {
        SectionTitle = title;
        SectionHint = hint;
    }

    private void Logout(object? parameter)
    {
        UserSession.CurrentUser = null;

        var w = new LoginWindow();
        w.Show();

        if (parameter is Window currentWindow)
            currentWindow.Close();
    }
}