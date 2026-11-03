namespace ListovkiQuiz.Models;

/// <summary>The result of one finished exam.</summary>
public class Attempt
{
    public int Id { get; set; }
    public int UserId { get; set; }          // who took the exam
    public DateTime TakenAt { get; set; }    // stored in UTC
    public int Points { get; set; }
    public int CorrectCount { get; set; }
    public bool Passed { get; set; }

    // A unique code given to each exam when it is opened. If the same exam is sent twice
    // (for example the page is refreshed after finishing), it is only saved once.
    public string ExamCode { get; set; } = "";
}
