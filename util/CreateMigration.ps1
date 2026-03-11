param(
    [Parameter(Mandatory = $true, HelpMessage = "Name for the new migration")]
    [string]$MigrationName
)

Push-Location "$PSScriptRoot\.."

dotnet ef migrations add $MigrationName --project Library.EF --startup-project LibraryApi

Pop-Location
