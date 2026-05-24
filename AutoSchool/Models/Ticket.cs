namespace AutoSchool.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // NEW: тема билета
        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}