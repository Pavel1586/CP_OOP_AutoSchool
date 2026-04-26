using AutoSchool.Models;

namespace AutoSchool.Services.Abstractions;

public interface IInstructorService
{
    IReadOnlyList<Instructor> GetAll();
    Instructor? GetById(int id);
    void SetForUser(int userId, int instructorId);
    void ClearForUser(int userId);
}