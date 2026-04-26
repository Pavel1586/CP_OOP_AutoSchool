namespace AutoSchool.Services.Abstractions;

public record TicketByTopicItem(int Id, string Title, int TopicQuestionsCount, int TotalQuestionsCount);

public interface ITicketService
{
    IReadOnlyList<TicketByTopicItem> GetTicketsByTopic(int topicId);
}