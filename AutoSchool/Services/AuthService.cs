using AutoSchool.Data;
using AutoSchool.Infrastructure;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services
{
    public class AuthService
    {
        public bool Register(string firstName, string lastName, string email, string password)
        {
            using var context = new ApplicationDbContext();
            if (context.Users.Any(u => u.Email == email))
                return false;

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                RoleId = 2
            };

            context.Users.Add(user);
            context.SaveChanges(); // получаем user.Id

            EnsureTrainingPlan(context, user.Id); // NEW
            context.SaveChanges();

            return true;
        }

        public User? Login(string email, string password)
        {
            using var context = new ApplicationDbContext();
            string hash = PasswordHasher.Hash(password);

            return context.Users
                .Include(u => u.Role)
                .Include(u => u.Instructor)
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
        }

        private static void EnsureTrainingPlan(ApplicationDbContext context, int userId)
        {
            var user = context.Users.First(u => u.Id == userId);

            // если уже есть план — ничего не делаем
            bool hasCredits = context.TheoryCredits.Any(c => c.UserId == userId);
            if (user.TrainingStartDate != null && user.TrainingPlannedEndDate != null && hasCredits)
                return;

            var start = DateTime.Today;
            var end = start.AddDays(56); // 8 недель (можешь поменять)

            user.TrainingStartDate = start;
            user.TrainingPlannedEndDate = end;

            // берём темы, создаём зачёты раз в неделю
            var topics = context.Topics.OrderBy(t => t.Id).ToList();
            int week = 1;

            foreach (var t in topics)
            {
                // пример: занятия по будням в 18:00
                var date = start.AddDays(7 * week).AddHours(18);

                context.TheoryCredits.Add(new Models.TheoryCredit
                {
                    UserId = userId,
                    TopicId = t.Id,
                    PlannedAt = date,
                    DurationMinutes = 60,
                    Room = "Каб. 1",
                    Status = Models.TheoryCreditStatus.Planned,
                    Notes = "Зачёт по теме"
                });

                week++;
                if (week > 8) break; // максимум 8 зачётов
            }
        }
    }
}