using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListovkiQuiz.Pages;

public class IndexModel : PageModel
{
    private readonly QuestionRepository _repo;

    public IndexModel(QuestionRepository repo)
    {
        _repo = repo;
    }

    public int PoolSize => _repo.PoolSize;

    public void OnGet()
    {
    }
}
