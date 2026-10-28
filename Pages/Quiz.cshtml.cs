using ListovkiQuiz.Models;
using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListovkiQuiz.Pages;

public class QuizModel : PageModel
{
    private readonly QuestionRepository _repo;
    public QuizModel(QuestionRepository repo) => _repo = repo;

    // Posted back from the form: which questions were shown, and which options were ticked ("questionId:answerIndex")
    [BindProperty] public List<int> QuestionIds { get; set; } = new();
    [BindProperty] public List<string> Picked { get; set; } = new();

    public List<Question> Questions { get; private set; } = new();
    public Dictionary<int, HashSet<int>> PickedMap { get; } = new();
    public Dictionary<int, bool> Results { get; } = new();
    public bool Submitted { get; private set; }
    public int CorrectCount { get; private set; }
    public int PointsEarned { get; private set; }

    public int QuestionCount => ExamRules.QuestionCount;
    public int TotalPoints => ExamRules.TotalPoints;
    public int PassPoints => ExamRules.PassPoints;
    public bool Passed => PointsEarned >= ExamRules.PassPoints;

    /// <summary>Every visit starts a brand-new exam.</summary>
    public void OnGet()
    {
        Questions = _repo.BuildExam();
    }

    public IActionResult OnPost()
    {
        // Only accept a complete, valid exam; anything else starts a fresh one
        var ids = QuestionIds.Distinct().ToList();
        if (ids.Count != ExamRules.QuestionCount) return RedirectToPage();

        Questions = ids.Select(id => _repo.GetById(id))
                       .Where(q => q != null)
                       .Select(q => q!)
                       .ToList();
        if (Questions.Count != ExamRules.QuestionCount
            || Questions.Sum(q => q.Points) != ExamRules.TotalPoints)
            return RedirectToPage();

        Submitted = true;

        foreach (var p in Picked)
        {
            var parts = p.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out var qid) && int.TryParse(parts[1], out var idx))
            {
                if (!PickedMap.ContainsKey(qid)) PickedMap[qid] = new HashSet<int>();
                PickedMap[qid].Add(idx);
            }
        }

        foreach (var q in Questions)
        {
            var chosen = PickedMap.TryGetValue(q.Id, out var set) ? set : new HashSet<int>();
            var correct = q.Answers
                .Select((a, i) => (a, i))
                .Where(x => x.a.IsCorrect)
                .Select(x => x.i)
                .ToHashSet();

            // All-or-nothing: you must tick exactly the correct options
            var ok = chosen.SetEquals(correct);
            Results[q.Id] = ok;
            if (ok)
            {
                CorrectCount++;
                PointsEarned += q.Points;
            }
        }
        return Page();
    }
}
