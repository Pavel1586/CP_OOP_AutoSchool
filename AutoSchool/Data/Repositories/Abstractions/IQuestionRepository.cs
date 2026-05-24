using AutoSchool.Models;

namespace AutoSchool.Data.Repositories.Abstractions;

public interface IQuestionRepository : IRepository<Question>
{
    List<Question> GetWithOptionsByIds(IReadOnlyList<int> questionIds);
}