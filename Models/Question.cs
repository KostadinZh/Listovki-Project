using System.Text.Json.Serialization;

namespace ListovkiQuiz.Models;

public class Answer
{
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
}

/// <summary>One question, with the same fields as in Data/questions.json.</summary>
public class Question
{
    public int Id { get; set; }
    public string Section { get; set; } = "";   // for example "Раздел 4. ПРАВИЛА ЗА ДВИЖЕНИЕТО ПО ПЪТИЩАТА"
    public int Points { get; set; }

    [JsonPropertyName("question")]              // in the JSON file this field is called "question"
    public string Text { get; set; } = "";

    public List<Answer> Answers { get; set; } = new List<Answer>();
    public List<string> ImageRefs { get; set; } = new List<string>();   // for example "./assets/30115.png"
    public List<string> VideoRefs { get; set; } = new List<string>();

    // ----- Small helpers used by the pages -----

    /// <summary>The section number. The text always starts with "Раздел " (7 characters), then the digit.</summary>
    public int SectionNo
    {
        get
        {
            if (Section.Length < 8)
                return 0;
            string digit = Section.Substring(7, 1);
            return int.TryParse(digit, out int number) ? number : 0;
        }
    }

    /// <summary>The short section name shown on the page: "Раздел 4".</summary>
    public string SectionLabel => Section.Split('.')[0];

    /// <summary>
    /// True when the answers are pictures: no answer has text, and there is
    /// exactly one picture for every answer (first picture = first answer, and so on).
    /// </summary>
    public bool HasImageAnswers
    {
        get
        {
            if (Answers.Count < 2 || ImageRefs.Count != Answers.Count)
                return false;
            foreach (var answer in Answers)
            {
                if (!string.IsNullOrWhiteSpace(answer.Text))
                    return false;
            }
            return true;
        }
    }

    /// <summary>The pictures shown under the question text. Empty when the answers are the pictures.</summary>
    public List<string> QuestionImages
    {
        get
        {
            if (HasImageAnswers)
                return new List<string>();
            return ImageRefs;
        }
    }

    /// <summary>The picture of answer number i, or null when that answer is only text.</summary>
    public string? AnswerImageUrl(int i)
    {
        if (!HasImageAnswers)
            return null;
        return AssetUrl(ImageRefs[i]);
    }

    /// <summary>Turns "./assets/30115.png" into "/assets/30115.png" (the file is in wwwroot/assets).</summary>
    public static string AssetUrl(string reference)
    {
        if (reference.StartsWith("./"))
            return "/" + reference.Substring(2);
        return reference;
    }

    /// <summary>
    /// A question is correct only when every correct answer is ticked and no wrong answer is ticked.
    /// "picked" holds the numbers of the answers the person ticked.
    /// </summary>
    public bool IsAnsweredCorrectly(List<int> picked)
    {
        for (int i = 0; i < Answers.Count; i++)
        {
            bool wasTicked = picked.Contains(i);
            if (Answers[i].IsCorrect != wasTicked)
                return false;
        }
        return true;
    }
}
