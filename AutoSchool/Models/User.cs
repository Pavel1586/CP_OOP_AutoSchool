using System.Collections.Generic;

namespace AutoSchool.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        public DateTime? TrainingStartDate { get; set; }
        public DateTime? TrainingPlannedEndDate { get; set; }

        public ICollection<TheoryCredit> TheoryCredits { get; set; } = new List<TheoryCredit>();

        public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
}