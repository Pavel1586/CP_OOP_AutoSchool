using AutoSchool.Data;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class TopicService : ITopicService
{
    public IReadOnlyList<TopicListItem> GetTopicsForSelection()
    {
        using var context = new ApplicationDbContext();

        // 1) агрегируем вопросы по темам (в SQL)
        var stats = context.Questions
            .AsNoTracking()
            .GroupBy(q => q.TopicId)
            .Select(g => new
            {
                TopicId = g.Key,
                QuestionsCount = g.Count(),
                TicketsCount = g.Select(x => x.TicketId).Distinct().Count()
            })
            .ToDictionary(x => x.TopicId, x => (x.QuestionsCount, x.TicketsCount));

        // 2) берём темы (в SQL) и "приклеиваем" статистику
        var topics = context.Topics
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToList();

        return topics
            .Select(t =>
            {
                stats.TryGetValue(t.Id, out var s);
                return new TopicListItem(
                    t.Id,
                    t.Name,
                    s.QuestionsCount,
                    s.TicketsCount
                );
            })
            .ToList();
    }
}