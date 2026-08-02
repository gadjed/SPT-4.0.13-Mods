SPT 4.0.13 - Mods Pack Installer
================================

1. Copy ONLY manage-modpack.cmd into the SPT game root
   (folder with EscapeFromTarkov.exe; SPT.Server.exe may be in SPT\).
   Optional: keep a local manage-modpack.ps1 as offline fallback.
2. Run manage-modpack.cmd

On every launch the .cmd downloads the latest manage-modpack.ps1 from GitHub
(raw main branch) into .modpack-cache\ and executes it. You do not need to
manually replace the manager script after updates.

Menu:
  1 - Auto-update (clean + download SPT-4.0.13-ModPack.zip from GitHub + install)
  2 - Clean mods only (keeps Greed.exe and [SVM] presets)
  3 - Install mods (local SPT-4.0.13-ModPack.zip beside scripts, or download)
  4 - Install SVM from official GhostFenixx/svm-csharp (not bundled)
  5 - Exit

Nested layout: server mods sync to SPT\user\mods.
Flat layout: server mods sync to user\mods when needed.
Removes leftover DanW QuestingBots when QuestingBotsContinuous is present.
Removes FastSurgery / ContinuousHealing leftovers.

Downloads the newest GitHub release that contains SPT-4.0.13-ModPack.zip.

Repo: https://github.com/gadjed/SPT-4.0.13-Mods
Script source: https://raw.githubusercontent.com/gadjed/SPT-4.0.13-Mods/main/mods_patch/manage-modpack.ps1
ModPack releases: https://github.com/gadjed/SPT-4.0.13-Mods/releases
