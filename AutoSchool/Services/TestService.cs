using AutoSchool.Data.UoW;
using AutoSchool.Models;
using AutoSchool.Services.Abstractions;

namespace AutoSchool.Services;

public class TestService : ITestService
{
    private const int QuestionsPerTicket = 10;

    public Ticket GetTicketForTest(int ticketId)
    {
        using var uow = new UnitOfWork();

        var ticket = uow.Tickets.GetTicketWithQuestionsAndOptions(ticketId);

        ticket.Questions = ticket.Questions
            .OrderBy(q => q.Id)
            .Take(QuestionsPerTicket)
            .ToList();

        return ticket;
    }

    public int SaveTicketTestResult(
        int userId,
        int ticketId,
        IReadOnlyDictionary<int, int?> selectedOptionByQuestionId)
    {
        using var uow = new UnitOfWork();

        var questionIds = selectedOptionByQuestionId.Keys.ToList();
        var questions = uow.Questions.GetWithOptionsByIds(questionIds);

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

        uow.Context.TestResults.Add(result);
        uow.SaveChanges();

        return result.Id;
    }
}