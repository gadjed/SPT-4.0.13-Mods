#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
MOD_NAME="SAIN-StealthEngage"
MOD_VERSION="4.4.4"
DIST="$ROOT/Dist"
STAGE="$DIST/stage"
ZIP_NAME="${MOD_NAME}-${MOD_VERSION}.zip"

CLIENT_DLL="$ROOT/Build/BepInEx/plugins/SAIN/SAIN.dll"
SERVER_DIR="$ROOT/Build/SPT/user/mods/Solarint-SAIN-ServerMod"

if [[ ! -f "$CLIENT_DLL" ]]; then
  echo "Missing client DLL. Build SAIN/SAIN.csproj -c Release first." >&2
  exit 1
fi
if [[ ! -f "$SERVER_DIR/SAINServerMod.dll" ]]; then
  echo "Missing server DLL. Build SAINServerMod/SAINServerMod.csproj -c Release first." >&2
  exit 1
fi

rm -rf "$DIST"
mkdir -p "$STAGE/BepInEx/plugins/SAIN"
mkdir -p "$STAGE/SPT/user/mods/Solarint-SAIN-ServerMod"

cp "$CLIENT_DLL" "$STAGE/BepInEx/plugins/SAIN/"
cp -R "$SERVER_DIR/." "$STAGE/SPT/user/mods/Solarint-SAIN-ServerMod/"
cp "$ROOT/LICENSE" "$STAGE/SPT/user/mods/Solarint-SAIN-ServerMod/"
cp "$ROOT/NOTICE" "$STAGE/SPT/user/mods/Solarint-SAIN-ServerMod/"
cp "$ROOT/README.md" "$STAGE/SPT/user/mods/Solarint-SAIN-ServerMod/"

(
  cd "$STAGE"
  zip -r "$DIST/$ZIP_NAME" BepInEx SPT >/dev/null
)

echo "Created $DIST/$ZIP_NAME"
unzip -l "$DIST/$ZIP_NAME"
