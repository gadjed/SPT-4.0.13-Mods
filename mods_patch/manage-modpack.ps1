# SPT 4.0.13 Mod Pack Manager
# Керування встановленням / оновленням / очищенням модів збірки.
# Зазвичай запускається через manage-modpack.cmd (bootstrap з GitHub).
# Прямий запуск: powershell -ExecutionPolicy Bypass -File .\manage-modpack.ps1 [шлях_до_SPT]
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$SptRoot
)

$ErrorActionPreference = "Stop"

$Repo = "gadjed/SPT-4.0.13-Mods"
$AssetName = "SPT-4.0.13-ModPack.zip"
$SvmRepo = "GhostFenixx/svm-csharp"
$SvmAssetName = "SVM.Server.Value.Modifier.zip"
$ScriptPath = $MyInvocation.MyCommand.Path
$ScriptDir = Split-Path -Parent $ScriptPath

$PackRootFiles = @("INSTALL.txt", "MANIFEST.txt")
# Greed.exe + [SVM] Server Value Modifier are user-owned (presets); never wipe on clean/update.
$PackManagedDlls = @(
    "Unity.InternalAPIEngineBridge.003.dll",
    "Unity.VectorGraphics.dll"
)

# Filled by Initialize-GamePaths — always the SPT game root, even if this .ps1 lives in .modpack-cache.
$script:ResolvedSptRoot = $null
$script:CacheDir = $null
$script:LocalZip = $null
$script:ScriptSource = "local"

function Write-Info  { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Ok    { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Warn  { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }
function Write-Err   { param([string]$Message) Write-Host $Message -ForegroundColor Red }

function Pause-Enter {
    Write-Host ""
    Read-Host "Натисніть Enter, щоб продовжити" | Out-Null
}

function Confirm-Yes {
    param([string]$Prompt = "Продовжити?")
    $answer = Read-Host "$Prompt [y/N]"
    return ($answer -match '^(?i)(y|yes|т|так)$')
}

function Normalize-SptPath {
    param([string]$Dir)
    if ([string]::IsNullOrWhiteSpace($Dir)) { return $null }
    # cmd "D:\SPT\" escapes the closing quote → PowerShell may see D:\SPT"
    $clean = $Dir.Trim().Trim('"').TrimEnd('\', '/')
    if ([string]::IsNullOrWhiteSpace($clean)) { return $null }
    return $clean
}

function Test-SptRoot {
    param([string]$Dir)
    $Dir = Normalize-SptPath $Dir
    if ([string]::IsNullOrWhiteSpace($Dir)) { return $false }
    if (-not (Test-Path -LiteralPath $Dir)) { return $false }
    $eft = Join-Path $Dir "EscapeFromTarkov.exe"
    $server = Join-Path $Dir "SPT.Server.exe"
    $bep = Join-Path $Dir "BepInEx"
    $user = Join-Path $Dir "user"
    return (Test-Path -LiteralPath $eft) -or (Test-Path -LiteralPath $server) -or `
        ((Test-Path -LiteralPath $bep) -and (Test-Path -LiteralPath $user))
}

function Get-CacheParentGameRoot {
    # Bootstrap stores the live script at <game>\.modpack-cache\manage-modpack.ps1
    if ((Split-Path -Leaf $ScriptDir) -ne ".modpack-cache") { return $null }
    $parent = Split-Path -Parent $ScriptDir
    if (Test-SptRoot $parent) { return $parent }
    return $null
}

function Initialize-GamePaths {
    if ($script:ResolvedSptRoot) { return }

    $candidate = $null
    $paramRoot = Normalize-SptPath $script:SptRoot
    $envRoot = Normalize-SptPath $env:SPT_ROOT

    if ($paramRoot -and (Test-SptRoot $paramRoot)) {
        $candidate = $paramRoot
    }
    elseif ($envRoot -and (Test-SptRoot $envRoot)) {
        $candidate = $envRoot
    }
    elseif (Test-SptRoot (Get-Location).Path) {
        $candidate = (Get-Location).Path
    }
    else {
        $fromCache = Get-CacheParentGameRoot
        if ($fromCache) {
            $candidate = $fromCache
        }
        elseif (Test-SptRoot $ScriptDir) {
            $candidate = $ScriptDir
        }
    }

    if (-not $candidate) {
        Write-Host ""
        Write-Warn "Вкажіть шлях до кореня SPT (де EscapeFromTarkov.exe / SPT.Server.exe)."
        $inputPath = Normalize-SptPath (Read-Host "Шлях SPT")
        if (-not (Test-SptRoot $inputPath)) {
            Write-Err "Не схоже на корінь SPT: $inputPath"
            throw "Invalid SPT root"
        }
        $candidate = $inputPath
    }

    $script:ResolvedSptRoot = (Resolve-Path -LiteralPath (Normalize-SptPath $candidate)).Path
    $script:CacheDir = Join-Path $script:ResolvedSptRoot ".modpack-cache"
    $script:LocalZip = Join-Path $script:ResolvedSptRoot $AssetName
    New-Item -ItemType Directory -Force -Path $script:CacheDir | Out-Null

    if ((Split-Path -Leaf $ScriptDir) -eq ".modpack-cache") {
        $script:ScriptSource = "github-cache"
    }
    else {
        $script:ScriptSource = "local"
    }
}

function Resolve-SptRootPath {
    Initialize-GamePaths
    return $script:ResolvedSptRoot
}

function Get-ModpackCacheDir {
    Initialize-GamePaths
    return $script:CacheDir
}

function Get-LocalModPackZip {
    Initialize-GamePaths
    return $script:LocalZip
}

function Ensure-SptDirs {
    param([string]$Root)
    New-Item -ItemType Directory -Force -Path (Join-Path $Root "BepInEx\plugins") | Out-Null
    New-Item -ItemType Directory -Force -Path (Join-Path $Root "BepInEx\patchers") | Out-Null
    New-Item -ItemType Directory -Force -Path (Join-Path $Root "user\mods") | Out-Null
    if (Test-Path -LiteralPath (Join-Path $Root "SPT\SPT.Server.exe")) {
        New-Item -ItemType Directory -Force -Path (Join-Path $Root "SPT\user\mods") | Out-Null
    }
}

function Get-ServerModRoots {
    param([string]$Root)
    $roots = @()
    $nested = Join-Path $Root "SPT\user\mods"
    $flat = Join-Path $Root "user\mods"
    if (Test-Path -LiteralPath (Join-Path $Root "SPT\SPT.Server.exe")) {
        $roots += $nested
    }
    if (Test-Path -LiteralPath $flat) {
        $roots += $flat
    }
    elseif (-not $roots) {
        $roots += $flat
    }
    return $roots
}

function Sync-NestedServerMods {
    param([string]$Root)
    $nestedServer = Join-Path $Root "SPT\SPT.Server.exe"
    if (-not (Test-Path -LiteralPath $nestedServer)) { return }

    $rootMods = Join-Path $Root "user\mods"
    $nestedMods = Join-Path $Root "SPT\user\mods"
    New-Item -ItemType Directory -Force -Path $nestedMods | Out-Null

    # Prefer pack layout SPT\user\mods; also mirror root user\mods for older zips.
    if (Test-Path -LiteralPath $rootMods) {
        $items = Get-ChildItem -LiteralPath $rootMods -Force -ErrorAction SilentlyContinue
        if ($items) {
            Copy-Item -Path (Join-Path $rootMods "*") -Destination $nestedMods -Recurse -Force
            Write-Ok "  синхронізовано user\mods → SPT\user\mods"
        }
    }
}

function Remove-LegacyQuestingConflicts {
    param([string]$Root)
    foreach ($modsDir in (Get-ServerModRoots -Root $Root)) {
        if (-not (Test-Path -LiteralPath $modsDir)) { continue }
        $continuous = Join-Path $modsDir "QuestingBotsContinuous"
        if (-not (Test-Path -LiteralPath $continuous)) { continue }

        foreach ($legacy in @("QuestingBots", "ScavPopulation", "Scav-Population", "zSolarint-ScavPopulation")) {
            $path = Join-Path $modsDir $legacy
            if (Test-Path -LiteralPath $path) {
                Remove-Item -LiteralPath $path -Recurse -Force
                Write-Ok "  видалено конфлікт $legacy з $($modsDir.Substring($Root.Length).TrimStart('\','/'))"
            }
        }
    }

    $legacyClient = Join-Path $Root "BepInEx\plugins\QuestingBots"
    if ((Test-Path -LiteralPath (Join-Path $Root "BepInEx\plugins\QuestingBotsContinuous")) -and (Test-Path -LiteralPath $legacyClient)) {
        Remove-Item -LiteralPath $legacyClient -Recurse -Force
        Write-Ok "  видалено конфліктний BepInEx\plugins\QuestingBots"
    }
}

function Remove-LegacyMedConflicts {
    param([string]$Root)
    # Fast Surgery / Continuous Healing / AutoMedHotkeys / DefibAllyRevive / MedRebalance
    # were folded into MedSuite (4.0.13). Strip leftovers from older packs.
    $hasMedSuite = $false
    foreach ($modsDir in (Get-ServerModRoots -Root $Root)) {
        if (Test-Path -LiteralPath (Join-Path $modsDir "MedSuite")) { $hasMedSuite = $true; break }
    }
    if (-not $hasMedSuite -and (Test-Path -LiteralPath (Join-Path $Root "BepInEx\plugins\MedSuite.Client.dll"))) {
        $hasMedSuite = $true
    }

    foreach ($modsDir in (Get-ServerModRoots -Root $Root)) {
        if (-not (Test-Path -LiteralPath $modsDir)) { continue }
        $serverLegacy = @("FastSurgery")
        if ($hasMedSuite) {
            $serverLegacy += @("MedRebalance", "DefibAllyRevive")
        }
        foreach ($legacy in $serverLegacy) {
            $path = Join-Path $modsDir $legacy
            if (Test-Path -LiteralPath $path) {
                Remove-Item -LiteralPath $path -Recurse -Force
                Write-Ok "  видалено застарілий $legacy з $($modsDir.Substring($Root.Length).TrimStart('\','/'))"
            }
        }
    }

    $plugins = Join-Path $Root "BepInEx\plugins"
    $clientLegacy = @(
        "ContinuousHealing",
        "ContinuousHealing.dll",
        "FastSurgery.Client.dll",
        "FastSurgery.dll",
        "InsureAllPrapor.dll"
    )
    if ($hasMedSuite) {
        $clientLegacy += @(
            "AutoMedHotkeys.dll",
            "DefibAllyRevive.dll",
            "MedRebalance.Client.dll",
            "MedRebalance.dll"
        )
    }
    foreach ($legacy in $clientLegacy) {
        $path = Join-Path $plugins $legacy
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Recurse -Force
            Write-Ok "  видалено застарілий BepInEx\plugins\$legacy"
        }
    }
}

function Remove-LegacySainConflicts {
    param([string]$Root)
    # StealthEngage is a drop-in for BepInEx\plugins\SAIN + Solarint-SAIN-ServerMod.
    # Strip alternate install names that would double-load beside the fork.
    $sainPlugin = Join-Path $Root "BepInEx\plugins\SAIN"
    if (-not (Test-Path -LiteralPath $sainPlugin)) { return }

    foreach ($modsDir in (Get-ServerModRoots -Root $Root)) {
        if (-not (Test-Path -LiteralPath $modsDir)) { continue }
        foreach ($legacy in @("SAIN", "SAIN-ServerMod", "SAINServerMod", "Solarint-SAIN", "SAIN-StealthEngage")) {
            $path = Join-Path $modsDir $legacy
            if (Test-Path -LiteralPath $path) {
                Remove-Item -LiteralPath $path -Recurse -Force
                Write-Ok "  видалено конфліктний $legacy з $($modsDir.Substring($Root.Length).TrimStart('\','/'))"
            }
        }
    }

    $plugins = Join-Path $Root "BepInEx\plugins"
    foreach ($legacy in @("SAIN.dll", "SAIN-StealthEngage", "SAIN-StealthEngage.dll", "Solarint-SAIN")) {
        $path = Join-Path $plugins $legacy
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Recurse -Force
            Write-Ok "  видалено конфліктний BepInEx\plugins\$legacy"
        }
    }
}

function Clear-Mods {
    param([string]$Root)
    Write-Info "Очищення модів у: $Root"

    # Never delete SPT's own BepInEx modules (spt-core.dll lives here).
    $preservePluginNames = @("spt")
    # SVM is installed separately; keep presets / Greed state across pack updates.
    $preserveModFolderNames = @("[SVM] Server Value Modifier")

    $pluginsDir = Join-Path $Root "BepInEx\plugins"
    if (Test-Path -LiteralPath $pluginsDir) {
        Get-ChildItem -LiteralPath $pluginsDir -Force | Where-Object {
            $preservePluginNames -notcontains $_.Name
        } | Remove-Item -Recurse -Force
        Write-Ok "  очищено BepInEx\plugins (збережено: $($preservePluginNames -join ', '))"
    }

    $patchersDir = Join-Path $Root "BepInEx\patchers"
    if (Test-Path -LiteralPath $patchersDir) {
        Get-ChildItem -LiteralPath $patchersDir -Force | Remove-Item -Recurse -Force
        Write-Ok "  очищено BepInEx\patchers"
    }

    foreach ($modsRel in @("user\mods", "SPT\user\mods")) {
        $dir = Join-Path $Root $modsRel
        if (-not (Test-Path -LiteralPath $dir)) { continue }
        Get-ChildItem -LiteralPath $dir -Force | Where-Object {
            $preserveModFolderNames -notcontains $_.Name
        } | Remove-Item -Recurse -Force
        $kept = @(Get-ChildItem -LiteralPath $dir -Force -ErrorAction SilentlyContinue |
            Where-Object { $preserveModFolderNames -contains $_.Name } |
            ForEach-Object { $_.Name })
        if ($kept.Count -gt 0) {
            Write-Ok "  очищено $modsRel (збережено: $($kept -join ', '))"
        }
        else {
            Write-Ok "  очищено $modsRel"
        }
    }

    foreach ($f in $PackRootFiles) {
        $path = Join-Path $Root $f
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Force
            Write-Ok "  видалено $f"
        }
    }

    foreach ($dll in $PackManagedDlls) {
        $path = Join-Path $Root "EscapeFromTarkov_Data\Managed\$dll"
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Force
            Write-Ok "  видалено EscapeFromTarkov_Data\Managed\$dll"
        }
    }

    Write-Ok "Очищення завершено."
}

function Get-LatestModPack {
    param([string]$Dest)
    $destDir = Split-Path -Parent $Dest
    New-Item -ItemType Directory -Force -Path $destDir | Out-Null
    Write-Info "Завантаження останнього релізу з GitHub ($Repo)..."

    if (Get-Command gh -ErrorAction SilentlyContinue) {
        if (Test-Path -LiteralPath $Dest) { Remove-Item -LiteralPath $Dest -Force }
        # Find any release that contains this asset (4.0.13 pack is not always on "latest").
        $tag = $null
        $listJson = & gh release list -R $Repo --limit 40 --json tagName,isLatest
        $tags = ($listJson | ConvertFrom-Json).tagName
        foreach ($t in $tags) {
            $assetsJson = & gh release view $t -R $Repo --json assets
            $names = @(($assetsJson | ConvertFrom-Json).assets | ForEach-Object { $_.name })
            if ($names -contains $AssetName) { $tag = $t; break }
        }
        if (-not $tag) {
            throw "Не знайдено асет $AssetName у релізах $Repo."
        }
        & gh release download $tag -R $Repo -p $AssetName -D $destDir --clobber
        $downloaded = Join-Path $destDir $AssetName
        if ($downloaded -ne $Dest -and (Test-Path -LiteralPath $downloaded)) {
            Move-Item -LiteralPath $downloaded -Destination $Dest -Force
        }
    }
    else {
        $api = "https://api.github.com/repos/$Repo/releases?per_page=40"
        $headers = @{ "User-Agent" = "SPT-ModPack-Manager"; "Accept" = "application/vnd.github+json" }
        $releases = Invoke-RestMethod -Uri $api -Headers $headers
        $asset = $null
        foreach ($release in $releases) {
            $asset = $release.assets | Where-Object { $_.name -eq $AssetName } | Select-Object -First 1
            if ($asset) { break }
        }
        if (-not $asset) {
            throw "Не знайдено асет $AssetName у релізах $Repo."
        }
        Write-Info "URL: $($asset.browser_download_url)"
        Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $Dest -UseBasicParsing
    }

    if (-not (Test-Path -LiteralPath $Dest)) {
        throw "Завантаження не вдалося."
    }
    Write-Ok "Завантажено: $Dest"
    return $Dest
}

function Get-LatestSvm {
    param([string]$Dest)
    $destDir = Split-Path -Parent $Dest
    New-Item -ItemType Directory -Force -Path $destDir | Out-Null
    Write-Info "Завантаження SVM з офіційного релізу ($SvmRepo)..."

    if (Get-Command gh -ErrorAction SilentlyContinue) {
        if (Test-Path -LiteralPath $Dest) { Remove-Item -LiteralPath $Dest -Force }
        & gh release download -R $SvmRepo -p $SvmAssetName -D $destDir --clobber
        $downloaded = Join-Path $destDir $SvmAssetName
        if ($downloaded -ne $Dest -and (Test-Path -LiteralPath $downloaded)) {
            Move-Item -LiteralPath $downloaded -Destination $Dest -Force
        }
    }
    else {
        $api = "https://api.github.com/repos/$SvmRepo/releases/latest"
        $headers = @{ "User-Agent" = "SPT-ModPack-Manager"; "Accept" = "application/vnd.github+json" }
        $release = Invoke-RestMethod -Uri $api -Headers $headers
        $asset = $release.assets | Where-Object { $_.name -eq $SvmAssetName } | Select-Object -First 1
        if (-not $asset) {
            throw "Не знайдено асет $SvmAssetName у latest release $SvmRepo."
        }
        Write-Info "URL: $($asset.browser_download_url)"
        Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $Dest -UseBasicParsing
    }

    if (-not (Test-Path -LiteralPath $Dest)) {
        throw "Завантаження SVM не вдалося."
    }
    Write-Ok "Завантажено: $Dest"
    return $Dest
}

function Resolve-PackZip {
    param([bool]$PreferDownload = $false)

    $cacheDir = Get-ModpackCacheDir
    $localZip = Get-LocalModPackZip

    if ($PreferDownload) {
        $zip = Join-Path $cacheDir $AssetName
        return (Get-LatestModPack -Dest $zip)
    }

    if (Test-Path -LiteralPath $localZip) {
        Write-Info "Використовую локальний архів: $localZip"
        return $localZip
    }

    Write-Warn "Локальний $AssetName не знайдено — завантажую з GitHub."
    $zip = Join-Path $cacheDir $AssetName
    return (Get-LatestModPack -Dest $zip)
}

function Assert-Net9ServerModsOnly {
    param([string]$SearchRoot)
    # SPT 4.0.13 runs on net9. net10 DLLs (SPT 4.1 builds) crash DI with System.Runtime 10.0.0.0.
    $bad = @()
    foreach ($modsDir in @(
            (Join-Path $SearchRoot "user\mods"),
            (Join-Path $SearchRoot "SPT\user\mods")
        )) {
        if (-not (Test-Path -LiteralPath $modsDir)) { continue }
        Get-ChildItem -LiteralPath $modsDir -Recurse -Filter "*.dll" -File -ErrorAction SilentlyContinue | ForEach-Object {
            $bytes = [System.IO.File]::ReadAllBytes($_.FullName)
            $ascii = [System.Text.Encoding]::ASCII.GetString($bytes)
            if ($ascii.Contains(".NETCoreApp,Version=v10")) {
                $bad += $_.FullName.Substring($SearchRoot.Length).TrimStart('\', '/')
            }
        }
    }
    if ($bad.Count -gt 0) {
        Write-Err "Відхилено: у пакеті є серверні DLL під .NET 10 (SPT 4.1), а цей інсталер лише для SPT 4.0.13:"
        $bad | ForEach-Object { Write-Err "  $_" }
        throw "ModPack містить net10 серверні моди. Потрібен SPT-4.0.13-ModPack з net9 серверними DLL (YellowFlareCurse 1.4.4+)."
    }
}

function Install-Mods {
    param(
        [string]$Root,
        [string]$ZipPath
    )

    if (-not (Test-Path -LiteralPath $ZipPath)) {
        throw "Архів не знайдено: $ZipPath"
    }

    Ensure-SptDirs -Root $Root
    $tmp = Join-Path ([System.IO.Path]::GetTempPath()) ("spt-modpack-" + [guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Force -Path $tmp | Out-Null

    try {
        Write-Info "Розпакування $ZipPath..."
        Expand-Archive -LiteralPath $ZipPath -DestinationPath $tmp -Force

        $entries = Get-ChildItem -LiteralPath $tmp -Force
        $src = $tmp
        if ($entries.Count -eq 1 -and $entries[0].PSIsContainer) {
            $name = $entries[0].Name
            if ($name -eq "SPT-4.0.13-ModPack" -or (Test-Path (Join-Path $entries[0].FullName "BepInEx")) -or (Test-Path (Join-Path $entries[0].FullName "user")) -or (Test-Path (Join-Path $entries[0].FullName "SPT"))) {
                if ($name -ne "BepInEx") {
                    $src = $entries[0].FullName
                }
            }
        }

        Assert-Net9ServerModsOnly -SearchRoot $src

        Write-Info "Копіювання в $Root..."
        Copy-Item -Path (Join-Path $src "*") -Destination $Root -Recurse -Force
        Sync-NestedServerMods -Root $Root
        Remove-LegacyQuestingConflicts -Root $Root
        Remove-LegacyMedConflicts -Root $Root
        Remove-LegacySainConflicts -Root $Root
        Assert-Net9ServerModsOnly -SearchRoot $Root
        Write-Ok "Встановлення завершено."
        Write-Info "SVM не входить у збірку. За потреби оберіть пункт меню «Встановити SVM»."
    }
    finally {
        if (Test-Path -LiteralPath $tmp) {
            Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

function Install-SvmFromZip {
    param(
        [string]$Root,
        [string]$ZipPath
    )

    if (-not (Test-Path -LiteralPath $ZipPath)) {
        throw "Архів SVM не знайдено: $ZipPath"
    }

    Ensure-SptDirs -Root $Root
    $tmp = Join-Path ([System.IO.Path]::GetTempPath()) ("spt-svm-" + [guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Force -Path $tmp | Out-Null

    try {
        Write-Info "Розпакування $ZipPath..."
        Expand-Archive -LiteralPath $ZipPath -DestinationPath $tmp -Force

        $entries = Get-ChildItem -LiteralPath $tmp -Force
        $src = $tmp
        if ($entries.Count -eq 1 -and $entries[0].PSIsContainer) {
            $inner = $entries[0].FullName
            if ((Test-Path (Join-Path $inner "Greed.exe")) -or (Test-Path (Join-Path $inner "SPT")) -or (Test-Path (Join-Path $inner "user"))) {
                $src = $inner
            }
        }

        Write-Info "Копіювання SVM у $Root..."
        Copy-Item -Path (Join-Path $src "*") -Destination $Root -Recurse -Force
        Sync-NestedServerMods -Root $Root
        Write-Ok "SVM встановлено з офіційного релізу $SvmRepo."
        Write-Warn "Відкрийте Greed.exe у корені SPT, оберіть пресет і Save/Apply."
        Write-Warn "Ліцензія SVM (PUSL): лише personal use; не поширюйте архів/мод далі."
    }
    finally {
        if (Test-Path -LiteralPath $tmp) {
            Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

function Invoke-Clean {
    $root = Resolve-SptRootPath
    Write-Host ""
    Write-Warn "Буде видалено моди з BepInEx\plugins (крім spt\), BepInEx\patchers, user\mods"
    Write-Warn "та службові файли пакета (INSTALL/MANIFEST). Greed.exe і [SVM] зберігаються."
    Write-Info "SPT: $root"
    if (-not (Confirm-Yes "Очистити моди?")) {
        Write-Warn "Скасовано."
        return
    }
    Clear-Mods -Root $root
}

function Invoke-Install {
    $root = Resolve-SptRootPath
    Write-Host ""
    Write-Info "SPT: $root"

    $localZip = Get-LocalModPackZip
    if (Test-Path -LiteralPath $localZip) {
        Write-Info "Знайдено локальний архів: $localZip"
        if (Confirm-Yes "Завантажити свіжіший реліз з GitHub замість локального?") {
            $zip = Resolve-PackZip -PreferDownload $true
        }
        else {
            $zip = Resolve-PackZip -PreferDownload $false
        }
    }
    else {
        $zip = Resolve-PackZip -PreferDownload $false
    }

    if (-not (Confirm-Yes "Встановити моди з $(Split-Path -Leaf $zip)?")) {
        Write-Warn "Скасовано."
        return
    }
    Install-Mods -Root $root -ZipPath $zip
}

function Invoke-Update {
    $root = Resolve-SptRootPath
    Write-Host ""
    Write-Warn "Автоматичне оновлення:"
    Write-Warn "  1) видалити всі поточні моди"
    Write-Warn "  2) завантажити останній SPT-4.0.13-ModPack.zip з GitHub"
    Write-Warn "  3) встановити збірку"
    Write-Info "SPT: $root"
    Write-Info "Repo: https://github.com/$Repo"
    if (-not (Confirm-Yes "Оновити зараз?")) {
        Write-Warn "Скасовано."
        return
    }
    Clear-Mods -Root $root
    $zip = Resolve-PackZip -PreferDownload $true
    Install-Mods -Root $root -ZipPath $zip
    Write-Ok "Актуалізація завершена."
}

function Invoke-InstallSvm {
    $root = Resolve-SptRootPath
    Write-Host ""
    Write-Info "SPT: $root"
    Write-Info "Джерело: https://github.com/$SvmRepo/releases (офіційний асет $SvmAssetName)"
    Write-Warn "SVM не входить у ModPack: ліцензія забороняє редистрибуцію."
    Write-Warn "Цей пункт лише завантажує архів з upstream для вашого personal use."
    if (-not (Confirm-Yes "Завантажити й встановити останній SVM?")) {
        Write-Warn "Скасовано."
        return
    }
    $zip = Join-Path (Get-ModpackCacheDir) $SvmAssetName
    Get-LatestSvm -Dest $zip | Out-Null
    Install-SvmFromZip -Root $root -ZipPath $zip
}

function Show-Menu {
    Clear-Host
    Initialize-GamePaths
    Write-Host "========================================" -ForegroundColor White
    Write-Host "  SPT 4.0.13 Mod Pack — менеджер" -ForegroundColor White
    Write-Host "========================================" -ForegroundColor White
    Write-Host "  Репозиторій: $Repo"
    if ($script:ScriptSource -eq "github-cache") {
        Write-Host "  Скрипт: з GitHub (кеш .modpack-cache)" -ForegroundColor DarkGray
    }
    else {
        Write-Host "  Скрипт: локальний файл" -ForegroundColor DarkGray
    }
    $localZip = Get-LocalModPackZip
    if (Test-Path -LiteralPath $localZip) {
        Write-Host "  Локальний zip: є ($localZip)"
    }
    else {
        Write-Host "  Локальний zip: немає"
    }
    if ($env:SPT_ROOT) {
        Write-Host "  SPT_ROOT: $($env:SPT_ROOT)"
    }
    Write-Host ""
    Write-Host "  1) Автоматичне оновлення (очистити + остання збірка)"
    Write-Host "  2) Очищення від модів"
    Write-Host "  3) Встановити моди"
    Write-Host "  4) Встановити SVM (офіційний реліз GhostFenixx)"
    Write-Host "  5) Вихід"
    Write-Host ""
}

$normalizedParam = Normalize-SptPath $SptRoot
if ($normalizedParam) {
    $SptRoot = $normalizedParam
    $env:SPT_ROOT = $normalizedParam
}

while ($true) {
    Show-Menu
    $choice = Read-Host "Оберіть пункт [1-5]"
    switch ($choice) {
        "1" { try { Invoke-Update } catch { Write-Err $_.Exception.Message }; Pause-Enter }
        "2" { try { Invoke-Clean } catch { Write-Err $_.Exception.Message }; Pause-Enter }
        "3" { try { Invoke-Install } catch { Write-Err $_.Exception.Message }; Pause-Enter }
        "4" { try { Invoke-InstallSvm } catch { Write-Err $_.Exception.Message }; Pause-Enter }
        "5" { Write-Info "Вихід."; exit 0 }
        default { Write-Err "Невірний вибір."; Pause-Enter }
    }
}
