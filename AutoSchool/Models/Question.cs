using System.Collections.Generic;

namespace AutoSchool.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;

        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        // NEW
        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}