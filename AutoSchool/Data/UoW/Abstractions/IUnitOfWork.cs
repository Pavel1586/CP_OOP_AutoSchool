using AutoSchool.Data.Repositories.Abstractions;

namespace AutoSchool.Data.UoW.Abstractions;

public interface IUnitOfWork : IDisposable
{
    ApplicationDbContext Context { get; }

    IUserRepository Users { get; }
    ITicketRepository Tickets { get; }
    IQuestionRepository Questions { get; }

    int SaveChanges();
}