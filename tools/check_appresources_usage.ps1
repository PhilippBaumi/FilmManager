# Prüft, welche AppResources-Einträge im Projekt verwendet werden.
$designerPath = "Resources\Strings\Sprachen\AppResources.Designer.cs"
if (-not (Test-Path $designerPath)) {
	Write-Error "Designer-Datei nicht gefunden: $designerPath"
	exit 1
}
$designerText = Get-Content $designerPath -Raw
# Regex: internal static string <name>
$names = [System.Text.RegularExpressions.Regex]::Matches($designerText, 'internal static string ([A-Za-z0-9_]+)') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
if ($names.Count -eq 0) {
	Write-Error "Keine Ressourcen gefunden in Designer-Datei."
	exit 1
}
# Sammle zu durchsuchende Dateien (Code und XAML), schließe Designer und ResX-Dateien aus
$searchFiles = Get-ChildItem -Recurse -File -Include *.cs,*.xaml | Where-Object { $_.FullName -notmatch 'AppResources.Designer.cs' -and $_.FullName -notmatch '\.resx$' }

$result = @()
foreach ($name in $names) {
	$pattern = "AppResources\.$name"
	$count = 0
	foreach ($f in $searchFiles) {
		if (Select-String -Path $f.FullName -Pattern $pattern -SimpleMatch -Quiet) {
			$count++
		}
	}
	$result += [PSCustomObject]@{ Key = $name; Uses = $count }
}

$unused = $result | Where-Object { $_.Uses -eq 0 }

Write-Host "Gefundene Ressourcen: $($result.Count)" -ForegroundColor Cyan
Write-Host "Verwendete Ressourcen: $($result.Count - $unused.Count)" -ForegroundColor Green
Write-Host "Unbenutzte Ressourcen: $($unused.Count)" -ForegroundColor Yellow
Write-Host ""
if ($unused.Count -gt 0) {
	Write-Host "Unbenutzte Schlüssel:" -ForegroundColor Yellow
	$unused | Sort-Object Key | ForEach-Object { Write-Host "- $($_.Key)" }
} else {
	Write-Host "Keine unbenutzten Ressourcen gefunden." -ForegroundColor Green
}

# Ausgabe in Datei
$outFile = "tools/appresources_usage_report.txt"
"Report generated: $(Get-Date)" | Out-File $outFile -Encoding UTF8
"Total resources: $($result.Count)" | Out-File $outFile -Append -Encoding UTF8
"Used: $($result.Count - $unused.Count)" | Out-File $outFile -Append -Encoding UTF8
"Unused: $($unused.Count)" | Out-File $outFile -Append -Encoding UTF8
"" | Out-File $outFile -Append -Encoding UTF8
if ($unused.Count -gt 0) {
	"Unused keys:" | Out-File $outFile -Append -Encoding UTF8
	$unused | Sort-Object Key | ForEach-Object { "- $($_.Key)" | Out-File $outFile -Append -Encoding UTF8 }
} else {
	"No unused keys." | Out-File $outFile -Append -Encoding UTF8
}

Write-Host "Report written to $outFile" -ForegroundColor Cyan
