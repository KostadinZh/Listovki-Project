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

    // Short tests are saved too, but listed separately. Old attempts saved before
    // short tests existed have IsShortTest = false, which is right: they were full exams.
    public bool IsShortTest { get; set; }
    public int QuestionCount { get; set; }   // how many questions this test had
    public int TotalPoints { get; set; }     // how many points were possible

    // A unique code given to each exam when it is opened. If the same exam is sent twice
    // (for example the page is refreshed after finishing), it is only saved once.
    public string ExamCode { get; set; } = "";
}
