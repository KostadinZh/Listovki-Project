# Listovki Quiz (ASP.NET Core, Razor Pages)

A practice driving-theory exam built from `Data/questions.json`.

## Run
    dotnet run
Then open http://localhost:5000

## The rules
- 45 questions, 97 points in total, 87 points to pass (`Models/ExamRules.cs`)
- Questions come from all sections - the user cannot choose the number or the topics
- Questions with a video are never used
- A question gives its points only if exactly the correct answers are ticked

## Short tests
- `/Quiz?size=5` and `/Quiz?size=10` give 5 or 10 random questions from all topics (sizes are in `ExamRules.ShortTestSizes`)
- No pass mark; they are saved in a separate "Short tests" table in My attempts and do not change the exam statistics

## Accounts
- People register with a username and password, then log in (`/Register`, `/Login`)
- The exam needs a login. Every finished exam is saved and listed on `/History` ("My attempts")
- Passwords are never stored - only a salted PBKDF2 hash (`Services/Passwords.cs`)
- Users and attempts are saved in `App_Data/users.json` and `App_Data/attempts.json`.
  Keep `App_Data/` out of git (add the line `App_Data/` to `.gitignore`).

## What each file does
| File | Job |
|------|-----|
| `Program.cs` | Starts the website |
| `Data/questions.json` | The questions |
| `Models/Question.cs` | One question + small helpers (grading, picture paths) |
| `Models/ExamRules.cs` | 45 / 97 / 87 and the recipe: how many questions from each section |
| `Services/QuestionRepository.cs` | Reads the JSON, builds an exam, checks the recipe at startup |
| `Pages/Index.cshtml` | Home page |
| `Pages/Quiz.cshtml` | The exam page (questions, then the result) |
| `Pages/Quiz.cshtml.cs` | Builds the exam and grades the answers |
| `Models/AppUser.cs`, `Models/Attempt.cs` | A user, and the result of one exam |
| `Services/Passwords.cs` | Hashes and checks passwords |
| `Services/UserStore.cs` | Saves/loads users and attempts (JSON files) |
| `Services/AuthHelper.cs` | Gives the browser the login cookie |
| `Pages/Register`, `Login`, `Logout` | Account pages |
| `Pages/History.cshtml` | The list of past attempts and statistics |
| `Pages/Shared/_Layout.cshtml` | Header and footer around every page |
| `wwwroot/css/site.css` | All colours and styling |

## Pictures
Put the question pictures in `wwwroot/assets/` (for example `wwwroot/assets/30115.png`).
`tools/download-assets.ps1` can download them.
