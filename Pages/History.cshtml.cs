using System.Security.Claims;
using ListovkiQuiz.Models;
using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListovkiQuiz.Pages;

[Authorize]   // only for logged-in people
public class HistoryModel : PageModel
{
    private readonly UserStore _store;

    public HistoryModel(UserStore store)
    {
        _store = store;
    }

    public List<Attempt> Exams { get; set; } = new List<Attempt>();        // full exams, newest first
    public List<Attempt> ShortTests { get; set; } = new List<Attempt>();   // short tests, newest first

    // Statistics - only for the full exams, so short tests do not change them
    public int BestPoints { get; set; }
    public int AveragePoints { get; set; }
    public int PassedCount { get; set; }

    public void OnGet()
    {
        // The login cookie holds the user's id
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);

        var all = _store.GetAttempts(userId);
        Exams = all.Where(a => !a.IsShortTest).ToList();
        ShortTests = all.Where(a => a.IsShortTest).ToList();

        if (Exams.Count > 0)
        {
            BestPoints = Exams.Max(a => a.Points);
            AveragePoints = (int)Math.Round(Exams.Average(a => a.Points));
            PassedCount = Exams.Count(a => a.Passed);
        }
    }
}
