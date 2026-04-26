using AutoSchool.Data;
using AutoSchool.Models;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class TestService : ITestService
{
    private const int QuestionsPerTicket = 10;

    public Ticket GetTicketForTest(int ticketId)
    {
        using var context = new ApplicationDbContext();

        var ticket = context.Tickets
            .AsNoTracking()
            .Include(t => t.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .First(t => t.Id == ticketId);

        // тест всегда на 10 вопросов (как у тебя в VM)
        ticket.Questions = ticket.Questions
            .OrderBy(q => q.Id)
            .Take(QuestionsPerTicket)
            .ToList();

        return ticket;
    }

    public int SaveTicketTestResult(int userId, int ticketId, IReadOnlyDictionary<int, int?> selectedOptionByQuestionId)
    {
        using var context = new ApplicationDbContext();

        var questionIds = selectedOptionByQuestionId.Keys.ToList();

        var questions = context.Questions
            .Include(q => q.AnswerOptions)
            .Where(q => questionIds.Contains(q.Id))
            .ToList();

        int correct = 0;
        int wrong = 0;

        var result = new TestResult
        {
            UserId = userId,
            TicketId = ticketId,
            TopicId = null,
            PassedAt = DateTime.Now
        };

        foreach (var q in questions)
        {
            selectedOptionByQuestionId.TryGetValue(q.Id, out var selectedOptionId);

            var selected = selectedOptionId.HasValue
                ? q.AnswerOptions.FirstOrDefault(o => o.Id == selectedOptionId.Value)
                : null;

            bool isCorrect = selected != null && selected.IsCorrect;

            if (isCorrect) correct++; else wrong++;

            result.Answers.Add(new TestAnswer
            {
                QuestionId = q.Id,
                SelectedOptionId = selected?.Id,
                IsCorrect = isCorrect
            });
        }

        result.CorrectAnswers = correct;
        result.WrongAnswers = wrong;

        context.TestResults.Add(result);
        context.SaveChanges();

        return result.Id;
    }
}