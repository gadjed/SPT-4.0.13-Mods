SPT 4.1.0 - Mods Pack Installer
===============================

Auto-installer for the SPT 4.1.0 ModPack (owned mods + Waypoints).

1. Unpack manage-modpack.cmd + manage-modpack.ps1 (+ manage-modpack.sh on Unix)
   into the SPT game root (folder with EscapeFromTarkov.exe; SPT.Server.exe may be in SPT\).
2. Run manage-modpack.cmd

Menu:
  1 - Auto-update (clean + download SPT-4.1.0-ModPack.zip from GitHub + install)
  2 - Clean mods only
  3 - Install mods (local SPT-4.1.0-ModPack.zip beside scripts, or download)
  4 - Install SVM from official GhostFenixx/svm-csharp (not bundled)
  5 - Exit

Nested layout: server mods sync to SPT\user\mods.
Removes leftover DanW QuestingBots when QuestingBotsContinuous is present.

Downloads the newest GitHub release that contains SPT-4.1.0-ModPack.zip
(currently v1.0.3+).

Repo: https://github.com/gadjed/SPT-4.0.13-Mods
ModPack releases: https://github.com/gadjed/SPT-4.0.13-Mods/releases
