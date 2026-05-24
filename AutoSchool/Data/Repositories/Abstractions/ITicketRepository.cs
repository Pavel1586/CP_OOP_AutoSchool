using AutoSchool.Models;

namespace AutoSchool.Data.Repositories.Abstractions;

public interface ITicketRepository : IRepository<Ticket>
{
    Ticket GetTicketWithQuestionsAndOptions(int ticketId);
}