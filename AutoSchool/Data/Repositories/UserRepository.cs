using AutoSchool.Data.Repositories.Abstractions;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Data.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public bool ExistsByEmail(string email)
        => Context.Users.Any(u => u.Email == email);

    public User? FindByCredentials(string email, string passwordHash)
        => Context.Users
            .Include(u => u.Role)
            .Include(u => u.Instructor)
            .FirstOrDefault(u => u.Email == email && u.PasswordHash == passwordHash);

    public User GetRequired(int userId)
        => Context.Users.First(u => u.Id == userId);
}