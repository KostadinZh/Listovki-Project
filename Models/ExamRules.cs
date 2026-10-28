namespace ListovkiQuiz.Models;

/// <summary>The fixed rules of the exam. Change them here, in one place.</summary>
public static class ExamRules
{
    public const int QuestionCount = 45;
    public const int TotalPoints = 97;
    public const int PassPoints = 87;

    /// <summary>
    /// How many questions of each (section, point value) every exam contains.
    /// The counts must add up to exactly 45 questions worth exactly 97 points
    /// (checked when the app starts).
    /// </summary>
    public static readonly (int Section, int Points, int Count)[] Blueprint =
    {
        (1, 1, 3),   //  3 q ×1 =  3
        (2, 2, 5),   //  5 q ×2 = 10
        (3, 2, 2),   //  2 q ×2 =  4
        (3, 3, 2),   //  2 q ×3 =  6
        (4, 1, 4),   //  4 q ×1 =  4
        (4, 2, 1),   //  1 q ×2 =  2
        (4, 3, 20),  // 20 q ×3 = 60
        (5, 1, 4),   //  4 q ×1 =  4
        (6, 1, 4),   //  4 q ×1 =  4
    };                // total: 45 questions, 97 points
}
