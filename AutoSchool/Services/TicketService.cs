using AutoSchool.Data;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class TicketService : ITicketService
{
    public IReadOnlyList<TicketByTopicItem> GetTicketsByTopic(int topicId)
    {
        using var context = new ApplicationDbContext();

        // Агрегируем вопросы по билетам в SQL
        var qStats = context.Questions
            .AsNoTracking()
            .GroupBy(q => q.TicketId)
            .Select(g => new
            {
                TicketId = g.Key,
                TotalQuestions = g.Count(),
                TopicQuestions = g.Count(x => x.TopicId == topicId)
            })
            .Where(x => x.TopicQuestions > 0);

        // Джойним со списком билетов
        var result = (from t in context.Tickets.AsNoTracking()
                      join s in qStats on t.Id equals s.TicketId
                      orderby t.Id
                      select new TicketByTopicItem(
                          t.Id,
                          t.Title,
                          s.TopicQuestions,
                          s.TotalQuestions
                      ))
                     .ToList();

        return result;
    }
}