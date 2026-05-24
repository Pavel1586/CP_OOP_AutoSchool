using AutoSchool.Data;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class TopicService : ITopicService
{
    public IReadOnlyList<TopicListItem> GetTopicsForSelection()
    {
        using var context = new ApplicationDbContext();

        // 1) Кол-во билетов по темам (простая агрегация)
        var ticketsCountByTopic = context.Tickets
            .AsNoTracking()
            .GroupBy(t => t.TopicId)
            .Select(g => new { TopicId = g.Key, Count = g.Count() })
            .ToDictionary(x => x.TopicId, x => x.Count);

        // 2) Кол-во вопросов по темам (считаем через Tickets.TopicId)
        var questionsCountByTopic = context.Questions
            .AsNoTracking()
            .Join(context.Tickets.AsNoTracking(),
                  q => q.TicketId,
                  t => t.Id,
                  (q, t) => new { t.TopicId })
            .GroupBy(x => x.TopicId)
            .Select(g => new { TopicId = g.Key, Count = g.Count() })
            .ToDictionary(x => x.TopicId, x => x.Count);

        // 3) Список тем
        var topics = context.Topics
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToList();

        return topics.Select(t => new TopicListItem(
                t.Id,
                t.Name,
                questionsCountByTopic.TryGetValue(t.Id, out var qc) ? qc : 0,
                ticketsCountByTopic.TryGetValue(t.Id, out var tc) ? tc : 0
            ))
            .ToList();
    }
}