param(
    [Parameter(HelpMessage = "Connection string to override the one in appsettings.json")]
    [string]$ConnectionString
)

Push-Location "$PSScriptRoot\.."

if ($ConnectionString) {
    dotnet ef database update --project Library.EF --startup-project LibraryApi --connection $ConnectionString
} else {
    dotnet ef database update --project Library.EF --startup-project LibraryApi
}

Pop-Location
