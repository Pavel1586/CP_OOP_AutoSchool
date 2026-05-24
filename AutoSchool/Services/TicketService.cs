using AutoSchool.Data;
using AutoSchool.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services;

public class TicketService : ITicketService
{
    public IReadOnlyList<TicketByTopicItem> GetTicketsByTopic(int topicId)
    {
        using var context = new ApplicationDbContext();

        return context.Tickets
            .AsNoTracking()
            .Where(t => t.TopicId == topicId)     // билет принадлежит теме
            .OrderBy(t => t.Id)                   // сортируем по простому
            .Select(t => new TicketByTopicItem(
                t.Id,
                t.Title,
                // если тема у билета — то "вопросов по теме" = все вопросы билета
                t.Questions.Count(),
                t.Questions.Count()
            ))
            .ToList();
    }
}