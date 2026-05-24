using AutoSchool.Data.Repositories.Abstractions;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Data.Repositories;

public sealed class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context) { }

    public Ticket GetTicketWithQuestionsAndOptions(int ticketId)
        => Context.Tickets
            .AsNoTracking()
            .Include(t => t.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .First(t => t.Id == ticketId);
}