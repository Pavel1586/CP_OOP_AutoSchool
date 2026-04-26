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

public class AdminInstructorsViewModel : BaseViewModel
{
    public class InstructorRow
    {
        public int Id { get; set; }
        public byte[]? Photo { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Vehicle { get; set; } = "";
        public string TransmissionText { get; set; } = "";
        public string VehicleCategory { get; set; } = "";
        public int StudentsCount { get; set; }
    }

    private readonly IPageNavigationService _nav;

    public ObservableCollection<InstructorRow> Instructors { get; } = new();

    private InstructorRow? _selectedInstructor;
    public InstructorRow? SelectedInstructor
    {
        get => _selectedInstructor;
        set { _selectedInstructor = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BackCommand { get; }

    public AdminInstructorsViewModel(IPageNavigationService nav)
    {
        _nav = nav;
        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit());
        DeleteCommand = new RelayCommand(_ => Delete());
        BackCommand = new RelayCommand(_ => _nav.Navigate(new AdminTicketsPage(_nav)));

        Load();
    }

    private void Load()
    {
        using var context = new ApplicationDbContext();

        var list = context.Instructors
            .Include(i => i.Users)
            .OrderBy(i => i.LastName).ThenBy(i => i.FirstName)
            .ToList();

        Instructors.Clear();
        foreach (var i in list)
        {
            Instructors.Add(new InstructorRow
            {
                Id = i.Id,
                Photo = i.Photo,
                FullName = $"{i.FirstName} {i.LastName}",
                Phone = i.Phone,
                Email = i.Email,
                Vehicle = $"{i.VehicleBrand} {i.VehicleModel}".Trim(),
                TransmissionText = i.Transmission == Models.TransmissionType.Automatic ? "Автомат" : "Механика",
                VehicleCategory = i.VehicleCategory,
                StudentsCount = i.Users.Count
            });
        }
    }

    private void Add()
    {
        var dlg = new InstructorEditWindow();
        if (dlg.ShowDialog() != true) return;

        using var context = new ApplicationDbContext();
        context.Instructors.Add(new Models.Instructor
        {
            FirstName = dlg.FirstName,
            LastName = dlg.LastName,
            Phone = dlg.Phone,
            Email = dlg.Email,
            Photo = dlg.PhotoBytes,
            VehicleBrand = dlg.VehicleBrand,
            VehicleModel = dlg.VehicleModel,
            Transmission = dlg.Transmission,
            VehicleCategory = dlg.VehicleCategory
        });

        context.SaveChanges();
        Load();
    }

    private void Edit()
    {
        if (SelectedInstructor == null)
        {
            MessageBox.Show("Выберите инструктора.");
            return;
        }

        using var context = new ApplicationDbContext();
        var inst = context.Instructors.First(x => x.Id == SelectedInstructor.Id);

        var dlg = new InstructorEditWindow(inst);
        if (dlg.ShowDialog() != true) return;

        inst.FirstName = dlg.FirstName;
        inst.LastName = dlg.LastName;
        inst.Phone = dlg.Phone;
        inst.Email = dlg.Email;
        inst.Photo = dlg.PhotoBytes;
        inst.VehicleBrand = dlg.VehicleBrand;
        inst.VehicleModel = dlg.VehicleModel;
        inst.Transmission = dlg.Transmission;
        inst.VehicleCategory = dlg.VehicleCategory;

        context.SaveChanges();
        Load();
    }

    private void Delete()
    {
        if (SelectedInstructor == null)
        {
            MessageBox.Show("Выберите инструктора.");
            return;
        }

        if (MessageBox.Show("Удалить инструктора? У пользователей будет снята привязка.",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        using var context = new ApplicationDbContext();
        var inst = context.Instructors.First(x => x.Id == SelectedInstructor.Id);
        context.Instructors.Remove(inst);
        context.SaveChanges();

        Load();
    }
}