using System.Security.Claims;
using ListovkiQuiz.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ListovkiQuiz.Services;

public static class AuthHelper
{
    /// <summary>
    /// Logs the user in by giving the browser a login cookie.
    /// If "remember" is true the cookie lasts 14 days, otherwise only until the browser is closed.
    /// </summary>
    public static async Task SignInAsync(HttpContext http, AppUser user, bool remember)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   // the user's id
            new Claim(ClaimTypes.Name, user.Username)                   // shown in the header
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties { IsPersistent = remember };
        if (remember)
            properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }
}
