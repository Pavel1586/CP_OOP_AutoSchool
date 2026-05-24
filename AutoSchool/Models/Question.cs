using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoSchool.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        // NEW: изображение к вопросу (может быть null => без изображения)
        [Column(TypeName = "varbinary(max)")]
        public byte[]? Image { get; set; }

        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}