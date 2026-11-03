using ListovkiQuiz.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<QuestionRepository>();   // the questions
builder.Services.AddSingleton<UserStore>();            // users and their attempts

// Login with a cookie. Pages marked [Authorize] send visitors who are not logged in to /Login.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   // reads the login cookie: who is this?
app.UseAuthorization();    // checks [Authorize]: is this person allowed here?
app.MapRazorPages();

app.Run();
