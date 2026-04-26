using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Models;
using AutoSchool.Services;
using AutoSchool.Services.Abstractions;
using AutoSchool.Views;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.ViewModels;

public class InstructorSelectionViewModel : BaseViewModel
{
    public class InstructorRow
    {
        public int Id { get; set; }
        public byte[]? Photo { get; set; }

        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public string VehicleBrand { get; set; } = "";
        public string VehicleModel { get; set; } = "";
        public string Vehicle => $"{VehicleBrand} {VehicleModel}".Trim();

        public TransmissionType Transmission { get; set; }
        public string TransmissionText => Transmission == TransmissionType.Automatic ? "Автомат" : "Механика";

        public string VehicleCategory { get; set; } = "";
    }

    public ObservableCollection<InstructorRow> Instructors { get; } = new();

    private InstructorRow? _selectedInstructor;
    public InstructorRow? SelectedInstructor
    {
        get => _selectedInstructor;
        set { _selectedInstructor = value; OnPropertyChanged(); }
    }

    public ICommand SelectCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand BackCommand { get; }

    private readonly IInstructorService _instructorService;

    public InstructorSelectionViewModel() : this(new InstructorService()) { }
    public InstructorSelectionViewModel(IInstructorService instructorService)
    {
        _instructorService = instructorService;
        SelectCommand = new RelayCommand(Select);
        ClearCommand = new RelayCommand(Clear);
        BackCommand = new RelayCommand(Back);
        Load();
    }

    private void Load()
    {
        Instructors.Clear();

        foreach (var i in _instructorService.GetAll())
        {
            Instructors.Add(new InstructorRow
            {
                Id = i.Id,
                Photo = i.Photo,
                FullName = i.FirstName + " " + i.LastName,
                Phone = i.Phone,
                Email = i.Email,
                VehicleBrand = i.VehicleBrand,
                VehicleModel = i.VehicleModel,
                Transmission = i.Transmission,
                VehicleCategory = i.VehicleCategory
            });
        }

        if (UserSession.CurrentUser?.InstructorId != null)
            SelectedInstructor = Instructors.FirstOrDefault(x => x.Id == UserSession.CurrentUser.InstructorId.Value);
    }

    private void Select(object? parameter)
    {
        if (UserSession.CurrentUser == null)
            return;

        if (SelectedInstructor == null)
            return;

        _instructorService.SetForUser(UserSession.CurrentUser.Id, SelectedInstructor.Id);

        UserSession.CurrentUser.InstructorId = SelectedInstructor.Id;
        UserSession.CurrentUser.Instructor = _instructorService.GetById(SelectedInstructor.Id);

        var w = new MainMenuWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }

    private void Clear(object? parameter)
    {
        if (UserSession.CurrentUser == null)
            return;

        _instructorService.ClearForUser(UserSession.CurrentUser.Id);

        UserSession.CurrentUser.InstructorId = null;
        UserSession.CurrentUser.Instructor = null;

        var w = new MainMenuWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }

    private void Back(object? parameter)
    {
        var w = new MainMenuWindow();
        w.Show();
        if (parameter is Window currentWindow) currentWindow.Close();
    }
}