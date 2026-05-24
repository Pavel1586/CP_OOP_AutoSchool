using AutoSchool.Data.Repositories;
using AutoSchool.Data.Repositories.Abstractions;
using AutoSchool.Data.UoW.Abstractions;

namespace AutoSchool.Data.UoW;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork() : this(new ApplicationDbContext()) { }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Users = new UserRepository(_context);
        Tickets = new TicketRepository(_context);
        Questions = new QuestionRepository(_context);
    }

    public ApplicationDbContext Context => _context;

    public IUserRepository Users { get; }
    public ITicketRepository Tickets { get; }
    public IQuestionRepository Questions { get; }

    public int SaveChanges() => _context.SaveChanges();

    public void Dispose() => _context.Dispose();
}