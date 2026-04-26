using AutoSchool.Data;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class ResultsService : IResultsService
{
    public IReadOnlyList<UserResultItem> GetUserHistory(int userId)
    {
        using var context = new ApplicationDbContext();

        return context.TestResults
            .AsNoTracking()
            .Include(r => r.Ticket)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.PassedAt)
            .Select(r => new UserResultItem(
                r.Id,
                r.PassedAt,
                r.Ticket != null ? r.Ticket.Title : "(без билета)",
                r.CorrectAnswers,
                r.WrongAnswers
            ))
            .ToList();
    }
}