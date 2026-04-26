using System;
using System.Collections.Generic;

namespace AutoSchool.Models;

public class TestResult
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int? TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public int? TopicId { get; set; }
    public Topic? Topic { get; set; }

    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public DateTime PassedAt { get; set; } = DateTime.Now;

    public ICollection<TestAnswer> Answers { get; set; } = new List<TestAnswer>();
}