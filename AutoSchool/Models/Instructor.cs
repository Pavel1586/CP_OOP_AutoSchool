using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoSchool.Models;

public class Instructor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";

    // Фото храним в БД
    [Column(TypeName = "varbinary(max)")]
    public byte[]? Photo { get; set; }

    // Машина
    public string VehicleBrand { get; set; } = "";
    public string VehicleModel { get; set; } = "";

    public TransmissionType Transmission { get; set; }

    // Категория обучения
    public string VehicleCategory { get; set; } = "B";

    public ICollection<User> Users { get; set; } = new List<User>();
}