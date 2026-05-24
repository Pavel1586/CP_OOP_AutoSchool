using AutoSchool.Models;

namespace AutoSchool.Data.Repositories.Abstractions;

public interface IUserRepository : IRepository<User>
{
    bool ExistsByEmail(string email);
    User? FindByCredentials(string email, string passwordHash);
    User GetRequired(int userId);
}