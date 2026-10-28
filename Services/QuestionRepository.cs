using System.Text.Json;
using ListovkiQuiz.Models;

namespace ListovkiQuiz.Services;

public class QuestionRepository
{
    private readonly List<Question> _questions;   // everything in the JSON
    private readonly List<Question> _pool;        // questions the exam may use (no videos)
    private readonly Dictionary<int, Question> _byId;

    public QuestionRepository(IWebHostEnvironment env, ILogger<QuestionRepository> logger)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "questions.json");
        using var stream = File.OpenRead(path);
        _questions = JsonSerializer.Deserialize<List<Question>>(
            stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        _byId = _questions.ToDictionary(q => q.Id);

        // Video questions are not used
        _pool = _questions.Where(q => q.VideoRefs.Count == 0).ToList();

        ValidateBlueprint();
        logger.LogInformation("Loaded {Total} questions ({Pool} usable, videos excluded) from {Path}",
            _questions.Count, _pool.Count, path);
    }

    public int TotalCount => _questions.Count;
    public int PoolSize => _pool.Count;

    public Question? GetById(int id) => _byId.GetValueOrDefault(id);

    /// <summary>
    /// Builds one exam: for every blueprint cell, random questions of that section and point value.
    /// Always 45 questions worth exactly 97 points, ordered by section.
    /// </summary>
    public List<Question> BuildExam()
    {
        var exam = new List<Question>();
        foreach (var (section, points, count) in ExamRules.Blueprint)
        {
            exam.AddRange(_pool
                .Where(q => q.SectionNo == section && q.Points == points)
                .OrderBy(_ => System.Random.Shared.Next())
                .Take(count));
        }
        return exam.OrderBy(q => q.SectionNo)
                   .ThenBy(_ => System.Random.Shared.Next())
                   .ToList();
    }

    /// <summary>Fails fast at startup if the blueprint or the data can't produce a valid exam.</summary>
    private void ValidateBlueprint()
    {
        var count = ExamRules.Blueprint.Sum(b => b.Count);
        var points = ExamRules.Blueprint.Sum(b => b.Points * b.Count);
        if (count != ExamRules.QuestionCount || points != ExamRules.TotalPoints)
            throw new InvalidOperationException(
                $"Exam blueprint gives {count} questions / {points} points, expected " +
                $"{ExamRules.QuestionCount} / {ExamRules.TotalPoints}.");

        foreach (var (section, pts, need) in ExamRules.Blueprint)
        {
            var have = _pool.Count(q => q.SectionNo == section && q.Points == pts);
            if (have < need)
                throw new InvalidOperationException(
                    $"Section {section} has only {have} questions worth {pts} point(s); the exam needs {need}.");
        }
    }

    /// <summary>For the development-only API.</summary>
    public IEnumerable<Question> Search(int? section) =>
        section is > 0 ? _questions.Where(q => q.SectionNo == section) : _questions;
}
