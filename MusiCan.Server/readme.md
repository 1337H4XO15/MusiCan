# MusiCan Projekt

## Voraussetzungen

### NuGet-Pakete wiederherstellen
```PowerShell
dotnet restore
```

## Entity Framework Core Migrations

### Entity Framework Tools installieren (falls noch nicht installiert)
```PowerShell
dotnet tool install --global dotnet-ef
```

### 1. Erste Migration erstellen (ist bereits vorhanden)

Führen Sie den folgenden Befehl aus, um die initiale Migration zu erstellen:

```PowerShell
dotnet ef migrations add InitialCreate --project MusiCan.Server --verbose
```

**Parameter-Erklärung:**
- `migrations add InitialCreate`: Erstellt eine neue Migration namens "InitialCreate"
- `--project MusiCan.Server`: Spezifiziert das Projekt, in dem die Migration erstellt werden soll
- `--verbose`: Zeigt detaillierte Ausgaben während des Prozesses

### 2. Datenbank aktualisieren

Wenden Sie die Migration auf die Datenbank an:

```PowerShell
dotnet ef database update --project MusiCan.Server
```