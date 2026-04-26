using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Services;
using AutoSchool.Services.Navigation;
using AutoSchool.Views.Admin;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AutoSchool.ViewModels.Admin;

public class AdminUsersViewModel : BaseViewModel
{
    public class UserRow
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public int ResultsCount { get; set; }
    }

    private readonly IPageNavigationService _nav;

    public ObservableCollection<UserRow> Users { get; } = new();

    private UserRow? _selectedUser;
    public UserRow? SelectedUser
    {
        get => _selectedUser;
        set { _selectedUser = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    public ICommand SearchCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand OpenScheduleCommand { get; }

    public AdminUsersViewModel(IPageNavigationService nav)
    {
        _nav = nav;

        SearchCommand = new RelayCommand(_ => Load(SearchText));
        ResetCommand = new RelayCommand(_ => { SearchText = ""; Load(null); });
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketsPage(_nav)));
        OpenScheduleCommand = new RelayCommand(_ => OpenSchedule());

        Load(null);
    }

    private void Load(string? search)
    {
        using var context = new ApplicationDbContext();

        var q = context.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(u =>
                u.Email.ToLower().Contains(s) ||
                u.FirstName.ToLower().Contains(s) ||
                u.LastName.ToLower().Contains(s));
        }

        var users = q.OrderBy(u => u.Id).ToList();

        // количество тестов (можно оптимизировать, но для курсового ок)
        var resultsCountByUser = context.TestResults
            .GroupBy(r => r.UserId)
            .Select(g => new { UserId = g.Key, Cnt = g.Count() })
            .ToDictionary(x => x.UserId, x => x.Cnt);

        Users.Clear();
        foreach (var u in users)
        {
            Users.Add(new UserRow
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? "",
                ResultsCount = resultsCountByUser.TryGetValue(u.Id, out var cnt) ? cnt : 0
            });
        }
    }
    private void OpenSchedule()
    {
        if (SelectedUser == null)
        {
            MessageBox.Show("Выберите пользователя.");
            return;
        }

        _nav.Navigate(new AdminUserSchedulePage(_nav, SelectedUser.Id));
    }

    private void Edit()
    {
        if (SelectedUser == null)
        {
            MessageBox.Show("Выберите пользователя.");
            return;
        }

        var dlg = new UserEditWindow(
            SelectedUser.FirstName,
            SelectedUser.LastName,
            SelectedUser.Email,
            SelectedUser.RoleId);

        if (dlg.ShowDialog() != true) return;

        if (!Regex.IsMatch(dlg.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MessageBox.Show("Введите корректный email.");
            return;
        }

        using var context = new ApplicationDbContext();

        if (context.Users.Any(u => u.Email == dlg.Email && u.Id != SelectedUser.Id))
        {
            MessageBox.Show("Пользователь с таким email уже существует.");
            return;
        }

        var user = context.Users.First(u => u.Id == SelectedUser.Id);
        user.FirstName = dlg.FirstName;
        user.LastName = dlg.LastName;
        user.Email = dlg.Email;
        user.RoleId = dlg.RoleId; // только Admin/Client

        context.SaveChanges();
        Load(SearchText);
    }

    private void Delete()
    {
        if (SelectedUser == null)
        {
            MessageBox.Show("Выберите пользователя.");
            return;
        }

        // не удаляем текущего пользователя (админа)
        if (UserSession.CurrentUser != null && SelectedUser.Id == UserSession.CurrentUser.Id)
        {
            MessageBox.Show("Нельзя удалить текущего пользователя.");
            return;
        }

        if (SelectedUser.Id == 1000)
        {
            MessageBox.Show("Нельзя удалить системного администратора (Id=1000).");
            return;
        }

        if (MessageBox.Show("Удалить пользователя? Его результаты также будут удалены.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        using var context = new ApplicationDbContext();
        var user = context.Users.First(u => u.Id == SelectedUser.Id);
        context.Users.Remove(user);
        context.SaveChanges();

        Load(SearchText);
    }
}