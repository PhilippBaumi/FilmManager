# FilmManager

FilmManager ist eine .NET MAUI (.NET 10) App zum Durchsuchen und Verwalten von Filmen und Serien‑Einträgen mithilfe der TMDb API.
Kurzübersicht

- Sucht und zeigt Filme und Serien (TMDb).
- Detailansichten mit Bildern, Homepage‑Links und Empfehlungen.
- Anzeige von TMDb‑Sammlungen.
- Lokale Persistenz für zwei Listen: "Watchlist" und "Watched" (SQLite).
- Import/Export der Listen als CSV, JSON oder DOCX.
- Sprachwahl (Deutsch/Englisch) über die Einstellungen.
- Läuft als .NET MAUI App auf unterstützten Plattformen (Windows, Android).

Funktionen (kurze Erklärung)

- Suche (TMDb)
  - Suche nach Filmen oder Serien über die TMDb API. Ergebnisliste wird in einer CollectionView angezeigt.

- Übersicht (Overview)
  - Zeigt Suchergebnisse bzw. übergebene Listen als Poster/Backdrops an. Ausgewählte Einträge öffnen ein Options‑Popup für weitere Aktionen.

- Detailansicht
  - Zeigt umfangreiche Details eines ausgewählten Films/Serie (Titel, Beschreibung, Bilder, Bewertungen, Homepage‑Link, Credits, Empfehlungen). Ausgewählte Empfehlungen lassen sich direkt öffnen.

- TMDb‑Sammlungen
  - Suche nach Collections (z. B. Reihen) und Anzeige der zugehörigen Filme.

- Watchlist & Watched (lokale Speicherung)
  - Zwei getrennte Listen werden in einer lokalen SQLite‑Datenbank gespeichert. In der UI können Einträge angezeigt, hinzugefügt bzw. entfernt werden (über Popups und Optionen).

- Import / Export
  - Exportiert die Listen in CSV, JSON oder DOCX‑Dateien.
  - Importiert CSV, JSON oder DOCX und überschreibt die jeweiligen lokalen Tabellen ("Watchlist" / "Watched").
  - Export/Import nutzt Dateipfade im App‑Datenverzeichnis (plattformabhängig).

- Spracheinstellungen
  - UI‑Texte können auf Deutsch oder Englisch umgestellt werden. Nach Änderung wird zur Startseite navigiert, damit die neuen Ressourcen greifen.

Technische Hinweise

- TMDb API
  - Die App verwendet TMDb (The Movie Database) über die TMDbLib‑Bibliothek.

- Lokale Datenbank
  - SQLite wird direkt über Microsoft.Data.Sqlite und Dapper verwendet. Die Datenbank speichert die Felder, die von TMDb‑Suchobjekten (SearchMovie / SearchTv) bereitgestellt werden.

- Dateiformate
  - CSV: Semikolongetrennte Datei mit Header.
  - JSON: Jede Zeile ein JSON‑Eintrag mit Typkennzeichnung (Movie/Tv).
  - DOCX: Word‑Dokument mit pro Eintrag mehreren Absätzen (wird beim Import geparst).

Build & Ausführen

- Voraussetzungen: .NET 10 SDK, .NET MAUI Workload, passende IDE/Tooling (z. B. Visual Studio 2026 Community oder JetBrains Rider mit MAUI‑Support).
- Projekt öffnen und starten mit gewünschtem Target (z. B. Windows oder Android).
- CLI (Beispiel):
  - dotnet restore
  - dotnet build