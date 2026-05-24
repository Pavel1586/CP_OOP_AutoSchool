using AutoSchool.Data.Repositories.Abstractions;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Data.Repositories;

public sealed class QuestionRepository : Repository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context) : base(context) { }

    public List<Question> GetWithOptionsByIds(IReadOnlyList<int> questionIds)
        => Context.Questions
            .Include(q => q.AnswerOptions)
            .Where(q => questionIds.Contains(q.Id))
            .ToList();
}