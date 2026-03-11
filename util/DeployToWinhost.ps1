param(
    [switch]$ApiOnly,
    [switch]$ReactOnly
)

# --- Configuration ---------------------------------------------------
$SolutionRoot  = (Resolve-Path "$PSScriptRoot\..").Path
$NetrcFile     = "C:/tmp/ftpnetrc"
$FtpBase       = "ftp://rodj.me"
$ApiPublishDir = "$SolutionRoot\publish-api"
$FtpApiDir     = "$FtpBase/LibraryApi"
$FtpReactDir   = "$FtpBase/library"

if ($env:RODJBUILD) {
    $ReactDistDir = "$env:RODJBUILD\paiwares\LibraryReact\dist"
} else {
    $ReactDistDir = "$SolutionRoot\LibraryReact\dist"
}

# --- Helpers ---------------------------------------------------------
function Upload-File($localPath, $ftpPath) {
    curl.exe --netrc-file $NetrcFile --ftp-create-dirs -T $localPath $ftpPath --silent --show-error
    if ($LASTEXITCODE -ne 0) { Write-Error "Upload failed: $localPath"; exit 1 }
}

function Upload-Directory($localDir, $ftpDir) {
    $files = Get-ChildItem -Recurse -File $localDir
    $i = 0
    foreach ($file in $files) {
        $i++
        $rel = $file.FullName.Substring($localDir.Length).TrimStart('\', '/').Replace('\', '/')
        Write-Host "  [$i/$($files.Count)] $rel"
        Upload-File $file.FullName "$ftpDir/$rel"
    }
}

function Delete-FtpFile($dir, $filename) {
    # +DELE sends the command AFTER curl has CWD'd into the directory
    curl.exe --netrc-file $NetrcFile "$FtpBase/$dir/" --list-only --quote "+DELE $filename" --silent --show-error
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  WARNING: Could not delete $filename -- remove it manually via FTP" -ForegroundColor Yellow
    }
}

# --- API deployment --------------------------------------------------
if (-not $ReactOnly) {

    Write-Host "`n=== [1/5] dotnet publish ===" -ForegroundColor Cyan
    Push-Location $SolutionRoot
    dotnet publish LibraryApi -c Release -o $ApiPublishDir
    if ($LASTEXITCODE -ne 0) { Pop-Location; Write-Error "Publish failed"; exit 1 }
    Pop-Location

    Write-Host "`n=== [2/5] Fix web.config (outofprocess) ===" -ForegroundColor Cyan
    $webConfigPath = "$ApiPublishDir\web.config"
    $original = Get-Content $webConfigPath -Raw
    $fixed = $original -replace 'hostingModel="inprocess"', 'hostingModel="outofprocess"'
    if ($original -eq $fixed) {
        Write-Host "  WARNING: hostingModel not changed -- verify web.config manually" -ForegroundColor Yellow
    } else {
        Set-Content $webConfigPath $fixed
        Write-Host "  inprocess -> outofprocess"
    }

    Write-Host "`n=== [3/5] Upload app_offline.htm (unlock DLLs) ===" -ForegroundColor Cyan
    $tmpOffline = [System.IO.Path]::GetTempFileName()
    Set-Content $tmpOffline "<html><body>Updating, please wait...</body></html>"
    Upload-File $tmpOffline "$FtpApiDir/app_offline.htm"
    Remove-Item $tmpOffline
    Write-Host "  Waiting 3s for IIS to release file locks..."
    Start-Sleep 3

    Write-Host "`n=== [4/5] Upload API files ===" -ForegroundColor Cyan
    Upload-Directory $ApiPublishDir $FtpApiDir

    Write-Host "`n=== [5/5] Remove app_offline.htm ===" -ForegroundColor Cyan
    Delete-FtpFile "LibraryApi" "app_offline.htm"
    Write-Host "  Done -- IIS will restart the app"
}

# --- React deployment ------------------------------------------------
if (-not $ApiOnly) {

    Write-Host "`n=== [R1/R2] npm run build ===" -ForegroundColor Cyan
    Push-Location "$SolutionRoot\LibraryReact"
    npm run build
    if ($LASTEXITCODE -ne 0) { Pop-Location; Write-Error "React build failed"; exit 1 }
    Pop-Location

    Write-Host "`n=== [R2/R2] Upload React files ===" -ForegroundColor Cyan
    Upload-Directory $ReactDistDir $FtpReactDir
}

# --- Done ------------------------------------------------------------
Write-Host "`n=== Deployment complete ===" -ForegroundColor Green
if (-not $ReactOnly) { Write-Host "  API:   https://rodj.me/LibraryApi/api/books" }
if (-not $ApiOnly)   { Write-Host "  React: https://rodj.me/library/" }
