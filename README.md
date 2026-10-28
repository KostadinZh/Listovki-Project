# Listovki Quiz (ASP.NET Core, Razor Pages)

A fixed practice exam built from `Data/questions.json`.

## Run
```
dotnet run
```
Then open http://localhost:5000

## The exam
- 45 questions, worth exactly 97 points, pass mark 87 (see `Models/ExamRules.cs`)
- Drawn at random from all sections; the user cannot choose the number of questions or the topics
- Video questions (58) are never used
- A question only scores if exactly the correct options are ticked

The mix of questions per section and point value is the `Blueprint` table in `Models/ExamRules.cs`.
The app checks at startup that it adds up to 45 questions / 97 points.

## Pictures
The JSON only references pictures (`./assets/30115.png`). Put them in `wwwroot/assets/`.
`tools/download-assets.ps1` downloads the 863 that are needed.

## Checking the data (Development only)
With `ASPNETCORE_ENVIRONMENT=Development`: `/api/questions?take=5` and `/api/questions/5300` return raw JSON.
