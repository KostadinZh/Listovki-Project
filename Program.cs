using ListovkiQuiz.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages = the website. QuestionRepository = loads the JSON once and keeps it in memory.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<QuestionRepository>();

var app = builder.Build();

app.UseStaticFiles();   // serves wwwroot (css and the question pictures)
app.MapRazorPages();    // serves the pages in the Pages folder

app.Run();
