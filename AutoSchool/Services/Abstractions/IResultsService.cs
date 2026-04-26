namespace AutoSchool.Services.Abstractions;

public record UserResultItem(int Id, DateTime PassedAt, string Title, int Correct, int Wrong);

public interface IResultsService
{
    IReadOnlyList<UserResultItem> GetUserHistory(int userId);
}