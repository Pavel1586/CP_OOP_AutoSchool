using AutoSchool.Data;
using AutoSchool.Models;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class InstructorService : IInstructorService
{
    public IReadOnlyList<Instructor> GetAll()
    {
        using var context = new ApplicationDbContext();
        return context.Instructors.AsNoTracking()
            .OrderBy(i => i.LastName).ThenBy(i => i.FirstName)
            .ToList();
    }

    public Instructor? GetById(int id)
    {
        using var context = new ApplicationDbContext();
        return context.Instructors.AsNoTracking().FirstOrDefault(i => i.Id == id);
    }

    public void SetForUser(int userId, int instructorId)
    {
        using var context = new ApplicationDbContext();
        var user = context.Users.First(u => u.Id == userId);
        user.InstructorId = instructorId;
        context.SaveChanges();
    }

    public void ClearForUser(int userId)
    {
        using var context = new ApplicationDbContext();
        var user = context.Users.First(u => u.Id == userId);
        user.InstructorId = null;
        context.SaveChanges();
    }
}