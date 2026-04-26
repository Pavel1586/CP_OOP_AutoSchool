using AutoSchool.Models;

namespace AutoSchool.Services.Abstractions;

public interface ITestService
{
    Ticket GetTicketForTest(int ticketId);
    int SaveTicketTestResult(int userId, int ticketId, IReadOnlyDictionary<int, int?> selectedOptionByQuestionId);
}