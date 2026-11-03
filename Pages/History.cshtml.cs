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

    public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    public int BestPoints { get; set; }
    public int AveragePoints { get; set; }
    public int PassedCount { get; set; }

    public void OnGet()
    {
        // The login cookie holds the user's id
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);

        Attempts = _store.GetAttempts(userId);

        if (Attempts.Count > 0)
        {
            BestPoints = Attempts.Max(a => a.Points);
            AveragePoints = (int)Math.Round(Attempts.Average(a => a.Points));
            PassedCount = Attempts.Count(a => a.Passed);
        }
    }
}
