using System;

namespace AutoSchool.Models;

public class TheoryCredit
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int? TopicId { get; set; }   // можно привязать к теме
    public Topic? Topic { get; set; }

    public DateTime PlannedAt { get; set; }
    public int DurationMinutes { get; set; } = 60;

    public string Room { get; set; } = "Каб. 1";
    public TheoryCreditStatus Status { get; set; } = TheoryCreditStatus.Planned;

    public string? Notes { get; set; }
}