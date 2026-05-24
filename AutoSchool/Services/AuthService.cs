using AutoSchool.Data.UoW;
using AutoSchool.Infrastructure;
using AutoSchool.Models;

namespace AutoSchool.Services;

public class AuthService
{
    public bool Register(string firstName, string lastName, string email, string password)
    {
        using var uow = new UnitOfWork();

        if (uow.Users.ExistsByEmail(email))
            return false;

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            RoleId = 2
        };

        uow.Users.Add(user);
        uow.SaveChanges();

        EnsureTrainingPlan(uow, user.Id);
        uow.SaveChanges();

        return true;
    }

    public User? Login(string email, string password)
    {
        using var uow = new UnitOfWork();
        string hash = PasswordHasher.Hash(password);
        return uow.Users.FindByCredentials(email, hash);
    }

    private static void EnsureTrainingPlan(UnitOfWork uow, int userId)
    {
        var user = uow.Users.GetRequired(userId);

        bool hasCredits = uow.Context.TheoryCredits.Any(c => c.UserId == userId);
        if (user.TrainingStartDate != null && user.TrainingPlannedEndDate != null && hasCredits)
            return;

        var start = DateTime.Today;
        var end = start.AddDays(56);

        user.TrainingStartDate = start;
        user.TrainingPlannedEndDate = end;

        var topics = uow.Context.Topics.OrderBy(t => t.Id).ToList();
        int week = 1;

        foreach (var t in topics)
        {
            var date = start.AddDays(7 * week).AddHours(18);

            uow.Context.TheoryCredits.Add(new TheoryCredit
            {
                UserId = userId,
                TopicId = t.Id,
                PlannedAt = date,
                DurationMinutes = 60,
                Room = "Каб. 1",
                Status = TheoryCreditStatus.Planned,
                Notes = "Зачёт по теме"
            });

            week++;
            if (week > 8) break;
        }
    }
}