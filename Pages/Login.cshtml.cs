using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListovkiQuiz.Pages;

public class LoginModel : PageModel
{
    private readonly UserStore _store;

    public LoginModel(UserStore store)
    {
        _store = store;
    }

    [BindProperty] public string? Username { get; set; }
    [BindProperty] public string? Password { get; set; }
    [BindProperty] public bool RememberMe { get; set; }

    // The page the person wanted before being sent to the login (for example /Quiz)
    [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

    public string? Error { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string username = (Username ?? "").Trim();
        string password = Password ?? "";

        var user = _store.CheckLogin(username, password);
        if (user == null)
        {
            // The same message for a wrong username and a wrong password, on purpose
            Error = "Wrong username or password.";
            return Page();
        }

        await AuthHelper.SignInAsync(HttpContext, user, RememberMe);

        // Go back to the page the person wanted - but only if it is a page on this site
        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Index");
    }
}
