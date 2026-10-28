# Downloads the question pictures into wwwroot\assets.
# Run from the project folder:   powershell -ExecutionPolicy Bypass -File .\tools\download-assets.ps1
param(
    [string]$BaseUrl = "https://public-eis.rta.government.bg/exam-questions/7_B_%D0%B1%D0%B5%D0%B7_%D0%BF%D1%80%D0%B0%D0%B2%D0%BE%D1%81%D0%BF%D0%BE%D1%81%D0%BE%D0%B1%D0%BD%D0%BE%D1%81%D1%82_%D0%B8%D0%BB%D0%B8_%D1%81_%D0%BF%D1%80%D0%B0%D0%B2%D0%BE%D1%81%D0%BF%D0%BE%D1%81%D0%BE%D0%B1%D0%BD%D0%BE%D1%81%D1%82_%D0%B7%D0%B0_%D0%BA%D0%B0%D1%82%D0%B5%D0%B3%D0%BE%D1%80%D0%B8%D1%8F_%D0%90%D0%9C_01.10.2026"
)

$questions = Get-Content "Data\questions.json" -Raw -Encoding UTF8 | ConvertFrom-Json
# Video questions are not used, so skip their pictures
$names = $questions | Where-Object { $_.videoRefs.Count -eq 0 } |
         ForEach-Object { $_.imageRefs } |
         ForEach-Object { $_ -replace '^\./assets/', '' } |
         Sort-Object -Unique

New-Item -ItemType Directory -Force "wwwroot\assets" | Out-Null
$ok = 0; $failed = 0; $i = 0
foreach ($name in $names) {
    $i++
    $target = "wwwroot\assets\$name"
    if (Test-Path $target) { $ok++; continue }
    try {
        Invoke-WebRequest -Uri "$BaseUrl/assets/$name" -OutFile $target -UseBasicParsing
        $ok++
    } catch {
        $failed++
        Write-Warning "Could not download $name"
    }
    Write-Progress -Activity "Downloading pictures" -Status "$i / $($names.Count)" -PercentComplete (100 * $i / $names.Count)
}
Write-Host "Done. $ok present, $failed failed (of $($names.Count))."
