#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
MOD_NAME="QuestingBotsContinuous"
MOD_VERSION="0.12.0"
DIST="$ROOT/Dist"
STAGE="$DIST/stage"
ZIP_NAME="${MOD_NAME}-${MOD_VERSION}.zip"

SERVER_DLL="$ROOT/Server/bin/Release/${MOD_NAME}-Server/${MOD_NAME}-Server/${MOD_NAME}-Server.dll"
# Fallback if OutputPath nesting differs
if [[ ! -f "$SERVER_DLL" ]]; then
  SERVER_DLL="$(find "$ROOT/Server/bin/Release" -name "${MOD_NAME}-Server.dll" | head -1)"
fi
CLIENT_DLL="$ROOT/Client/bin/Release/netstandard2.1/${MOD_NAME}-Client.dll"

if [[ ! -f "$SERVER_DLL" ]]; then
  echo "Missing server DLL. Build Server first." >&2
  exit 1
fi
if [[ ! -f "$CLIENT_DLL" ]]; then
  echo "Missing client DLL. Build Client first." >&2
  exit 1
fi

rm -rf "$DIST"
mkdir -p "$STAGE/user/mods/${MOD_NAME}"
mkdir -p "$STAGE/BepInEx/plugins/${MOD_NAME}"

cp "$SERVER_DLL" "$STAGE/user/mods/${MOD_NAME}/"
cp "$ROOT/Shared/Config/config.json" "$STAGE/user/mods/${MOD_NAME}/"
cp "$ROOT/Shared/Config/eftQuestSettings.json" "$STAGE/user/mods/${MOD_NAME}/"
cp "$ROOT/Shared/Config/zoneAndItemQuestPositions.json" "$STAGE/user/mods/${MOD_NAME}/"
cp "$ROOT/LICENSE" "$STAGE/user/mods/${MOD_NAME}/"
cp "$ROOT/NOTICE" "$STAGE/user/mods/${MOD_NAME}/"

cp "$CLIENT_DLL" "$STAGE/BepInEx/plugins/${MOD_NAME}/"
cp -R "$ROOT/Shared/Quests" "$STAGE/BepInEx/plugins/${MOD_NAME}/"

# Zip from stage so archive roots are user/ and BepInEx/
(
  cd "$STAGE"
  zip -r "$DIST/$ZIP_NAME" user BepInEx >/dev/null
)

echo "Created $DIST/$ZIP_NAME"
unzip -l "$DIST/$ZIP_NAME" | head -40
