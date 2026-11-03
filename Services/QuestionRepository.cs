using System.Text.Json;
using ListovkiQuiz.Models;

namespace ListovkiQuiz.Services;

/// <summary>Reads Data/questions.json once, and builds exams from it.</summary>
public class QuestionRepository
{
    // All questions that can be used (questions with a video are left out)
    private readonly List<Question> _questions;

    public QuestionRepository(IWebHostEnvironment env)
    {
        string path = Path.Combine(env.ContentRootPath, "Data", "questions.json");
        string json = File.ReadAllText(path);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var all = JsonSerializer.Deserialize<List<Question>>(json, options) ?? new List<Question>();

        _questions = all.Where(q => q.VideoRefs.Count == 0).ToList();

        CheckRecipe();
    }

    /// <summary>How many questions an exam can be drawn from.</summary>
    public int PoolSize => _questions.Count;

    /// <summary>
    /// Builds one new exam by following the recipe in ExamRules.Parts:
    /// for every line, pick random questions of that section and point value.
    /// The result is always 45 questions worth 97 points.
    /// </summary>
    public List<Question> BuildExam()
    {
        var exam = new List<Question>();

        foreach (var part in ExamRules.Parts)
        {
            // every question of this section with this number of points
            Question[] candidates = _questions
                .Where(q => q.SectionNo == part.Section && q.Points == part.Points)
                .ToArray();

            Random.Shared.Shuffle(candidates);               // mix them like a deck of cards
            exam.AddRange(candidates.Take(part.Count));      // take the first ones
        }

        // Mix the whole exam so the sections are not in a fixed order
        Question[] mixed = exam.ToArray();
        Random.Shared.Shuffle(mixed);
        return mixed.ToList();
    }

    /// <summary>Finds the questions with the given ids, in the same order. Unknown ids are skipped.</summary>
    public List<Question> GetByIds(List<int> ids)
    {
        var result = new List<Question>();
        foreach (int id in ids)
        {
            Question? question = _questions.FirstOrDefault(q => q.Id == id);
            if (question != null)
                result.Add(question);
        }
        return result;
    }

    /// <summary>Stops the program at startup if the recipe cannot make a correct exam.</summary>
    private void CheckRecipe()
    {
        int questions = 0;
        int points = 0;
        foreach (var part in ExamRules.Parts)
        {
            questions += part.Count;
            points += part.Count * part.Points;
        }

        if (questions != ExamRules.QuestionCount || points != ExamRules.TotalPoints)
            throw new InvalidOperationException(
                $"The exam recipe gives {questions} questions and {points} points, " +
                $"but it must give {ExamRules.QuestionCount} and {ExamRules.TotalPoints}.");

        foreach (var part in ExamRules.Parts)
        {
            int available = _questions.Count(q => q.SectionNo == part.Section && q.Points == part.Points);
            if (available < part.Count)
                throw new InvalidOperationException(
                    $"Section {part.Section} has only {available} questions worth {part.Points} point(s), " +
                    $"but the exam needs {part.Count}.");
        }
    }
}
