using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListovkiQuiz.Pages;

public class RegisterModel : PageModel
{
    private readonly UserStore _store;

    public RegisterModel(UserStore store)
    {
        _store = store;
    }

    // What the person typed. "string?" because an empty box arrives as null.
    [BindProperty] public string? Username { get; set; }
    [BindProperty] public string? Password { get; set; }
    [BindProperty] public string? ConfirmPassword { get; set; }

    public string? Error { get; set; }

    public IActionResult OnGet()
    {
        // Already logged in? Nothing to do here.
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string username = (Username ?? "").Trim();
        string password = Password ?? "";

        if (username.Length < 3 || username.Length > 20)
        {
            Error = "The username must be 3 to 20 characters long.";
            return Page();
        }

        foreach (char c in username)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
            {
                Error = "The username can only contain letters, digits and _";
                return Page();
            }
        }

        if (password.Length < 6 || password.Length > 100)
        {
            Error = "The password must be 6 to 100 characters long.";
            return Page();
        }

        if (password != ConfirmPassword)
        {
            Error = "The two passwords are not the same.";
            return Page();
        }

        var user = _store.CreateUser(username, password);
        if (user == null)
        {
            Error = "That username is already taken.";
            return Page();
        }

        // The account is created - log the person in straight away
        await AuthHelper.SignInAsync(HttpContext, user, true);
        return RedirectToPage("/Index");
    }
}
