using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ListovkiQuiz.Models;

public class Answer
{
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
}

/// <summary>
/// Mirrors one entry of Data/questions.json:
/// id, section, points, question, answers[{text,isCorrect}], imageRefs[], videoRefs[]
/// </summary>
public class Question
{
    public int Id { get; set; }
    public string Section { get; set; } = "";
    public int Points { get; set; }

    // The JSON property is called "question"; a member can't share its class's name in C#.
    [JsonPropertyName("question")]
    public string Text { get; set; } = "";

    public List<Answer> Answers { get; set; } = new();
    public List<string> ImageRefs { get; set; } = new();
    public List<string> VideoRefs { get; set; } = new();

    // ---- Derived helpers (not part of the JSON) ----

    /// <summary>Section number parsed from "Раздел 3. ...".</summary>
    [JsonIgnore]
    public int SectionNo
    {
        get
        {
            var m = Regex.Match(Section, @"^\s*Раздел\s+(\d+)");
            return m.Success ? int.Parse(m.Groups[1].Value) : 0;
        }
    }

    /// <summary>
    /// True when the answer options are pictures: every answer has empty text and
    /// there is exactly one image per answer (imageRefs[i] belongs to answers[i]).
    /// </summary>
    [JsonIgnore]
    public bool HasImageAnswers =>
        Answers.Count > 1
        && ImageRefs.Count == Answers.Count
        && Answers.All(a => string.IsNullOrWhiteSpace(a.Text));

    /// <summary>Pictures shown with the question itself (empty for picture-answer questions).</summary>
    [JsonIgnore]
    public IReadOnlyList<string> QuestionImages =>
        HasImageAnswers ? Array.Empty<string>() : ImageRefs;

    /// <summary>URL of the picture for answer i, or null if this answer is text only.</summary>
    public string? AnswerImageUrl(int i) =>
        HasImageAnswers && i < ImageRefs.Count ? AssetUrl(ImageRefs[i]) : null;

    /// <summary>"./assets/123.png" -> "/assets/123.png" (served from wwwroot/assets).</summary>
    public static string AssetUrl(string reference) =>
        reference.StartsWith("./") ? "/" + reference[2..] : reference;
}
