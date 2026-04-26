namespace AutoSchool.Services.Abstractions;

public record TopicListItem(int Id, string Name, int QuestionsCount, int TicketsCount);

public interface ITopicService
{
    IReadOnlyList<TopicListItem> GetTopicsForSelection();
}