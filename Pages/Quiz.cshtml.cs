using System.Security.Claims;
using ListovkiQuiz.Models;
using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListovkiQuiz.Pages;

[Authorize]   // only logged-in people can take the exam
public class QuizModel : PageModel
{
    private readonly QuestionRepository _repo;
    private readonly UserStore _store;

    public QuizModel(QuestionRepository repo, UserStore store)
    {
        _repo = repo;
        _store = store;
    }

    // The ids of the questions in this exam. The page sends them back (as hidden fields)
    // when the form is submitted, so we know which questions to grade.
    [BindProperty]
    public List<int> Ids { get; set; } = new List<int>();

    // A unique code for this exam (see Attempt.ExamCode). Sent back with the form too.
    [BindProperty]
    public string? ExamCode { get; set; }

    public List<Question> Questions { get; set; } = new List<Question>();

    // Everything below is filled in only after the exam is submitted
    public bool Submitted { get; set; }
    public Dictionary<int, List<int>> Picked { get; } = new Dictionary<int, List<int>>();  // question id -> ticked answers
    public Dictionary<int, bool> IsRight { get; } = new Dictionary<int, bool>();           // question id -> correct?
    public List<int> WrongNumbers { get; } = new List<int>();                              // positions of wrong questions
    public int CorrectCount { get; set; }
    public int PointsEarned { get; set; }

    public bool Passed => PointsEarned >= ExamRules.PassPoints;
    public int ScorePercent => PointsEarned * 100 / ExamRules.TotalPoints;
    public int PassPercent => ExamRules.PassPoints * 100 / ExamRules.TotalPoints;

    // Opening the page: build a new exam
    public void OnGet()
    {
        Questions = _repo.BuildExam();
        ExamCode = Guid.NewGuid().ToString();
    }

    // Pressing the "Finish" button: grade the exam
    public IActionResult OnPost()
    {
        Questions = _repo.GetByIds(Ids.Distinct().ToList());

        // If the form is incomplete or was changed, start a new exam instead
        if (Questions.Count != ExamRules.QuestionCount || Questions.Sum(q => q.Points) != ExamRules.TotalPoints)
            return RedirectToPage();

        Submitted = true;

        for (int i = 0; i < Questions.Count; i++)
        {
            Question question = Questions[i];

            // The ticked answers of a question are sent under the name q<question id>
            var picked = new List<int>();
            foreach (var value in Request.Form["q" + question.Id])
            {
                if (int.TryParse(value, out int answerNumber))
                    picked.Add(answerNumber);
            }

            bool right = question.IsAnsweredCorrectly(picked);
            Picked[question.Id] = picked;
            IsRight[question.Id] = right;

            if (right)
            {
                CorrectCount++;
                PointsEarned += question.Points;
            }
            else
            {
                WrongNumbers.Add(i + 1);
            }
        }

        SaveAttempt();
        return Page();
    }

    // Remembers this result in the logged-in user's history
    private void SaveAttempt()
    {
        // The login cookie holds the user's id
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);

        _store.AddAttempt(new Attempt
        {
            UserId = userId,
            TakenAt = DateTime.UtcNow,
            Points = PointsEarned,
            CorrectCount = CorrectCount,
            Passed = Passed,
            ExamCode = ExamCode ?? ""
        });
    }
}
