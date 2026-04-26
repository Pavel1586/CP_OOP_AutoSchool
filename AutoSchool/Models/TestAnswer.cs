namespace AutoSchool.Models;

public class TestAnswer
{
    public int Id { get; set; }

    public int TestResultId { get; set; }
    public TestResult? TestResult { get; set; }

    public int QuestionId { get; set; }
    public Question? Question { get; set; }

    public int? SelectedOptionId { get; set; }
    public AnswerOption? SelectedOption { get; set; }

    public bool IsCorrect { get; set; }
}