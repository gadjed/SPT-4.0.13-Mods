#!/usr/bin/env bash
# SPT 4.0.13 Mod Pack Manager
# Зазвичай: тонкий bootstrap тягне актуальну копію з GitHub у .modpack-cache і exec.
set -euo pipefail

REPO="gadjed/SPT-4.0.13-Mods"
ASSET_NAME="SPT-4.0.13-ModPack.zip"
SVM_REPO="GhostFenixx/svm-csharp"
SVM_ASSET_NAME="SVM.Server.Value.Modifier.zip"
SCRIPT_URL="https://raw.githubusercontent.com/gadjed/SPT-4.0.13-Mods/main/mods_patch/manage-modpack.sh"
SCRIPT_PATH="$(readlink -f "${BASH_SOURCE[0]}" 2>/dev/null || realpath "${BASH_SOURCE[0]}" 2>/dev/null || echo "${BASH_SOURCE[0]}")"
SCRIPT_DIR="$(cd "$(dirname "$SCRIPT_PATH")" && pwd)"

is_spt_root() {
  local dir="$1"
  [[ -f "${dir}/EscapeFromTarkov.exe" ]] || [[ -f "${dir}/SPT.Server.exe" ]] || [[ -d "${dir}/BepInEx" && -d "${dir}/user" ]]
}

guess_game_root() {
  if [[ -n "${1:-}" ]] && is_spt_root "$1"; then
    printf '%s' "$1"
    return 0
  fi
  if [[ -n "${SPT_ROOT:-}" ]] && is_spt_root "$SPT_ROOT"; then
    printf '%s' "$SPT_ROOT"
    return 0
  fi
  if is_spt_root "$PWD"; then
    printf '%s' "$PWD"
    return 0
  fi
  if [[ "$(basename "$SCRIPT_DIR")" == ".modpack-cache" ]] && is_spt_root "$(dirname "$SCRIPT_DIR")"; then
    printf '%s' "$(dirname "$SCRIPT_DIR")"
    return 0
  fi
  if is_spt_root "$SCRIPT_DIR"; then
    printf '%s' "$SCRIPT_DIR"
    return 0
  fi
  return 1
}

# Bootstrap once: download latest script from GitHub and re-exec from cache.
if [[ "${MODPACK_BOOTSTRAPPED:-0}" != "1" ]]; then
  GAME_ROOT="$(guess_game_root "${1:-}" || true)"
  if [[ -z "${GAME_ROOT:-}" ]]; then
    GAME_ROOT="$PWD"
  fi
  CACHE_DIR="${GAME_ROOT}/.modpack-cache"
  REMOTE_SCRIPT="${CACHE_DIR}/manage-modpack.sh"
  mkdir -p "$CACHE_DIR"
  echo "[modpack] Завантаження актуального manage-modpack.sh з GitHub..."
  tmp="${REMOTE_SCRIPT}.tmp"
  if command -v curl >/dev/null 2>&1 && curl -fsSL -H 'Cache-Control: no-cache' -A 'SPT-ModPack-Bootstrap' "$SCRIPT_URL" -o "$tmp"; then
    if [[ "$(wc -c < "$tmp")" -ge 200 ]]; then
      mv -f "$tmp" "$REMOTE_SCRIPT"
      chmod +x "$REMOTE_SCRIPT" || true
      echo "[modpack] Скрипт оновлено."
    else
      rm -f "$tmp"
      echo "[modpack] Порожнє завантаження — fallback."
    fi
  elif command -v wget >/dev/null 2>&1 && wget -q -O "$tmp" "$SCRIPT_URL"; then
    mv -f "$tmp" "$REMOTE_SCRIPT"
    chmod +x "$REMOTE_SCRIPT" || true
    echo "[modpack] Скрипт оновлено."
  else
    rm -f "$tmp" 2>/dev/null || true
    echo "[modpack] Не вдалося завантажити — локальний/кеш fallback."
    if [[ ! -f "$REMOTE_SCRIPT" && -f "$SCRIPT_PATH" ]]; then
      cp -f "$SCRIPT_PATH" "$REMOTE_SCRIPT"
    fi
  fi
  if [[ ! -f "$REMOTE_SCRIPT" ]]; then
    echo "[modpack] ПОМИЛКА: немає скрипта менеджера." >&2
    exit 1
  fi
  export MODPACK_BOOTSTRAPPED=1
  export SPT_ROOT="$GAME_ROOT"
  exec bash "$REMOTE_SCRIPT" "$GAME_ROOT" "$@"
fi

# --- running bootstrapped copy ---
GAME_ROOT="$(guess_game_root "${1:-}" || true)"
if [[ -z "${GAME_ROOT:-}" ]]; then
  echo "Не вдалося визначити корінь SPT." >&2
  exit 1
fi
export SPT_ROOT="$GAME_ROOT"
CACHE_DIR="${GAME_ROOT}/.modpack-cache"
LOCAL_ZIP="${GAME_ROOT}/${ASSET_NAME}"
mkdir -p "$CACHE_DIR"

# Файли з кореня SPT, що приходять із пакета (Greed.exe не чіпаємо — користувацький SVM)
PACK_ROOT_FILES=(INSTALL.txt MANIFEST.txt)
PRESERVE_MOD_FOLDERS=("[SVM] Server Value Modifier")
PACK_MANAGED_DLLS=(
  Unity.InternalAPIEngineBridge.003.dll
  Unity.VectorGraphics.dll
)

RED=$'\033[31m'
GREEN=$'\033[32m'
YELLOW=$'\033[33m'
CYAN=$'\033[36m'
BOLD=$'\033[1m'
RESET=$'\033[0m'

info()  { printf '%s%s%s\n' "$CYAN" "$*" "$RESET"; }
ok()    { printf '%s%s%s\n' "$GREEN" "$*" "$RESET"; }
warn()  { printf '%s%s%s\n' "$YELLOW" "$*" "$RESET"; }
err()   { printf '%s%s%s\n' "$RED" "$*" "$RESET" >&2; }

pause() {
  printf '\n'
  read -r -p "Натисніть Enter, щоб продовжити..." _
}

confirm() {
  local prompt="${1:-Продовжити?}"
  local answer
  read -r -p "${prompt} [y/N]: " answer
  case "${answer:-}" in
    y|Y|yes|YES|т|Т|так|Так|ТАК) return 0 ;;
    *) return 1 ;;
  esac
}

resolve_spt_root() {
  local guessed
  if guessed="$(guess_game_root "${1:-}")"; then
    printf '%s' "$guessed"
    return 0
  fi
  if [[ -n "${SPT_ROOT:-}" ]] && is_spt_root "$SPT_ROOT"; then
    printf '%s' "$SPT_ROOT"
    return 0
  fi
  if is_spt_root "$PWD"; then
    printf '%s' "$PWD"
    return 0
  fi

  local input
  printf '\n' >&2
  warn "Вкажіть шлях до кореня SPT (де EscapeFromTarkov.exe / SPT.Server.exe)." >&2
  read -r -p "Шлях SPT: " input
  input="${input/#\~/$HOME}"
  if [[ -z "$input" ]] || ! is_spt_root "$input"; then
    err "Не схоже на корінь SPT: ${input:-<порожньо>}"
    return 1
  fi
  printf '%s' "$input"
}

ensure_dirs() {
  local root="$1"
  mkdir -p "${root}/BepInEx/plugins" "${root}/BepInEx/patchers" "${root}/user/mods"
  if [[ -f "${root}/SPT/SPT.Server.exe" ]]; then
    mkdir -p "${root}/SPT/user/mods"
  fi
}

sync_nested_server_mods() {
  local root="$1"
  if [[ ! -f "${root}/SPT/SPT.Server.exe" ]]; then
    return 0
  fi
  mkdir -p "${root}/SPT/user/mods"
  if [[ -d "${root}/user/mods" ]] && [[ -n "$(ls -A "${root}/user/mods" 2>/dev/null || true)" ]]; then
    if command -v rsync >/dev/null 2>&1; then
      rsync -a --exclude '.DS_Store' "${root}/user/mods/" "${root}/SPT/user/mods/"
    else
      cp -R "${root}/user/mods/." "${root}/SPT/user/mods/"
    fi
    ok "  синхронізовано user/mods → SPT/user/mods"
  fi
}

remove_legacy_questing_conflicts() {
  local root="$1"
  local mods_dir
  for mods_dir in "${root}/SPT/user/mods" "${root}/user/mods"; do
    [[ -d "$mods_dir" ]] || continue
    [[ -d "${mods_dir}/QuestingBotsContinuous" ]] || continue
    local legacy
    for legacy in QuestingBots ScavPopulation Scav-Population zSolarint-ScavPopulation; do
      if [[ -e "${mods_dir}/${legacy}" ]]; then
        rm -rf "${mods_dir}/${legacy}"
        ok "  видалено конфлікт ${legacy} з ${mods_dir#${root}/}"
      fi
    done
  done
  if [[ -d "${root}/BepInEx/plugins/QuestingBotsContinuous" && -d "${root}/BepInEx/plugins/QuestingBots" ]]; then
    rm -rf "${root}/BepInEx/plugins/QuestingBots"
    ok "  видалено конфліктний BepInEx/plugins/QuestingBots"
  fi
}

remove_legacy_med_conflicts() {
  local root="$1"
  local mods_dir
  for mods_dir in "${root}/SPT/user/mods" "${root}/user/mods"; do
    [[ -d "$mods_dir" ]] || continue
    if [[ -e "${mods_dir}/FastSurgery" ]]; then
      rm -rf "${mods_dir}/FastSurgery"
      ok "  видалено застарілий FastSurgery з ${mods_dir#${root}/}"
    fi
  done
  local plugins="${root}/BepInEx/plugins"
  [[ -d "$plugins" ]] || return 0
  local legacy
  for legacy in ContinuousHealing ContinuousHealing.dll FastSurgery.Client.dll FastSurgery.dll; do
    if [[ -e "${plugins}/${legacy}" ]]; then
      rm -rf "${plugins}/${legacy}"
      ok "  видалено застарілий BepInEx/plugins/${legacy}"
    fi
  done
}

remove_legacy_sain_conflicts() {
  local root="$1"
  # Only clean aliases when a proper SAIN plugin folder is present (StealthEngage drop-in).
  if [[ ! -d "${root}/BepInEx/plugins/SAIN" ]]; then
    return 0
  fi

  local mods_dir
  for mods_dir in "${root}/SPT/user/mods" "${root}/user/mods"; do
    [[ -d "$mods_dir" ]] || continue
    local legacy
    for legacy in SAIN SAIN-ServerMod SAINServerMod Solarint-SAIN SAIN-StealthEngage; do
      if [[ -e "${mods_dir}/${legacy}" ]]; then
        rm -rf "${mods_dir}/${legacy}"
        ok "  видалено конфліктний ${legacy} з ${mods_dir#${root}/}"
      fi
    done
  done

  local plugins="${root}/BepInEx/plugins"
  [[ -d "$plugins" ]] || return 0
  local legacy
  for legacy in SAIN.dll SAIN-StealthEngage SAIN-StealthEngage.dll Solarint-SAIN; do
    if [[ -e "${plugins}/${legacy}" ]]; then
      rm -rf "${plugins}/${legacy}"
      ok "  видалено конфліктний BepInEx/plugins/${legacy}"
    fi
  done
}

clear_mods() {
  local root="$1"
  info "Очищення модів у: ${root}"

  # Never delete SPT's own BepInEx modules (spt-core.dll lives here).
  if [[ -d "${root}/BepInEx/plugins" ]]; then
    find "${root}/BepInEx/plugins" -mindepth 1 -maxdepth 1 ! -name 'spt' -exec rm -rf {} +
    ok "  очищено BepInEx/plugins/ (збережено: spt)"
  fi
  if [[ -d "${root}/BepInEx/patchers" ]]; then
    find "${root}/BepInEx/patchers" -mindepth 1 -maxdepth 1 -exec rm -rf {} +
    ok "  очищено BepInEx/patchers/"
  fi

  local mods_dir name keep
  for mods_dir in "${root}/user/mods" "${root}/SPT/user/mods"; do
    [[ -d "$mods_dir" ]] || continue
    keep=0
    while IFS= read -r -d '' name; do
      local base
      base="$(basename "$name")"
      local skip=0
      local p
      for p in "${PRESERVE_MOD_FOLDERS[@]}"; do
        if [[ "$base" == "$p" ]]; then skip=1; keep=1; break; fi
      done
      if [[ "$skip" -eq 0 ]]; then
        rm -rf "$name"
      fi
    done < <(find "$mods_dir" -mindepth 1 -maxdepth 1 -print0 2>/dev/null)
    if [[ "$keep" -eq 1 ]]; then
      ok "  очищено ${mods_dir#"${root}/"}/ (збережено: ${PRESERVE_MOD_FOLDERS[*]})"
    else
      ok "  очищено ${mods_dir#"${root}/"}/"
    fi
  done

  local f
  for f in "${PACK_ROOT_FILES[@]}"; do
    if [[ -e "${root}/${f}" ]]; then
      rm -f "${root}/${f}"
      ok "  видалено ${f}"
    fi
  done

  local dll
  for dll in "${PACK_MANAGED_DLLS[@]}"; do
    if [[ -e "${root}/EscapeFromTarkov_Data/Managed/${dll}" ]]; then
      rm -f "${root}/EscapeFromTarkov_Data/Managed/${dll}"
      ok "  видалено EscapeFromTarkov_Data/Managed/${dll}"
    fi
  done

  ok "Очищення завершено."
}

download_latest() {
  local dest="$1"
  mkdir -p "$(dirname "$dest")"
  info "Завантаження релізу з GitHub (${REPO}, асет ${ASSET_NAME})..."

  if command -v gh >/dev/null 2>&1; then
    rm -f "$dest"
    local tag=""
    local t
    while IFS= read -r t; do
      if gh release view "$t" -R "$REPO" --json assets -q ".assets[].name" | grep -Fxq "$ASSET_NAME"; then
        tag="$t"
        break
      fi
    done < <(gh release list -R "$REPO" --limit 40 --json tagName -q '.[].tagName')
    if [[ -z "$tag" ]]; then
      err "Не знайдено асет ${ASSET_NAME} у релізах ${REPO}."
      return 1
    fi
    gh release download "$tag" -R "$REPO" -p "$ASSET_NAME" -D "$(dirname "$dest")" --clobber
    if [[ "$(basename "$dest")" != "$ASSET_NAME" ]]; then
      mv -f "$(dirname "$dest")/${ASSET_NAME}" "$dest"
    fi
  else
    local api_url="https://api.github.com/repos/${REPO}/releases?per_page=40"
    local zip_url
    zip_url="$(curl -fsSL "$api_url" | python3 -c '
import json,sys
data=json.load(sys.stdin)
name=sys.argv[1]
for release in data:
    for a in release.get("assets", []):
        if a.get("name")==name:
            print(a["browser_download_url"]); raise SystemExit
sys.exit(1)
' "$ASSET_NAME")"
    if [[ -z "${zip_url:-}" ]]; then
      err "Не знайдено асет ${ASSET_NAME} у релізах ${REPO}."
      return 1
    fi
    info "URL: ${zip_url}"
    curl -fL --progress-bar -o "$dest" "$zip_url"
  fi

  if [[ ! -f "$dest" ]]; then
    err "Завантаження не вдалося."
    return 1
  fi
  ok "Завантажено: ${dest}"
}

download_svm_latest() {
  local dest="$1"
  mkdir -p "$(dirname "$dest")"
  info "Завантаження SVM з офіційного релізу (${SVM_REPO})..."

  if command -v gh >/dev/null 2>&1; then
    rm -f "$dest"
    gh release download -R "$SVM_REPO" -p "$SVM_ASSET_NAME" -D "$(dirname "$dest")" --clobber
    if [[ "$(basename "$dest")" != "$SVM_ASSET_NAME" ]]; then
      mv -f "$(dirname "$dest")/${SVM_ASSET_NAME}" "$dest"
    fi
  else
    local api_url="https://api.github.com/repos/${SVM_REPO}/releases/latest"
    local zip_url
    zip_url="$(curl -fsSL "$api_url" | python3 -c '
import json,sys
data=json.load(sys.stdin)
name=sys.argv[1]
for a in data.get("assets", []):
    if a.get("name")==name:
        print(a["browser_download_url"]); break
else:
    sys.exit(1)
' "$SVM_ASSET_NAME")"
    if [[ -z "${zip_url:-}" ]]; then
      err "Не знайдено асет ${SVM_ASSET_NAME} у latest release ${SVM_REPO}."
      return 1
    fi
    info "URL: ${zip_url}"
    curl -fL --progress-bar -o "$dest" "$zip_url"
  fi

  if [[ ! -f "$dest" ]]; then
    err "Завантаження SVM не вдалося."
    return 1
  fi
  ok "Завантажено: ${dest}"
}

resolve_pack_zip() {
  local prefer_download="${1:-0}"
  local zip_path=""

  if [[ "$prefer_download" == "1" ]]; then
    zip_path="${CACHE_DIR}/${ASSET_NAME}"
    download_latest "$zip_path"
    printf '%s' "$zip_path"
    return 0
  fi

  if [[ -f "$LOCAL_ZIP" ]]; then
    info "Використовую локальний архів: ${LOCAL_ZIP}"
    printf '%s' "$LOCAL_ZIP"
    return 0
  fi

  warn "Локальний ${ASSET_NAME} не знайдено — завантажую з GitHub."
  zip_path="${CACHE_DIR}/${ASSET_NAME}"
  download_latest "$zip_path"
  printf '%s' "$zip_path"
}

install_mods() {
  local root="$1"
  local zip_path="$2"
  local tmp

  if [[ ! -f "$zip_path" ]]; then
    err "Архів не знайдено: ${zip_path}"
    return 1
  fi

  ensure_dirs "$root"
  tmp="$(mktemp -d "${TMPDIR:-/tmp}/spt-modpack.XXXXXX")"
  info "Розпакування ${zip_path}..."
  unzip -q "$zip_path" -d "$tmp"

  # Підтримка як «вміст у корені zip», так і «одна тека всередині»
  local src="$tmp"
  local entries=()
  while IFS= read -r -d '' e; do
    entries+=("$e")
  done < <(find "$tmp" -mindepth 1 -maxdepth 1 -print0)

  if [[ ${#entries[@]} -eq 1 && -d "${entries[0]}" ]]; then
    local name
    name="$(basename "${entries[0]}")"
    if [[ "$name" == SPT-4.0.13-ModPack || "$name" == BepInEx || -d "${entries[0]}/BepInEx" || -d "${entries[0]}/user" ]]; then
      if [[ "$name" != BepInEx ]]; then
        src="${entries[0]}"
      fi
    fi
  fi

  info "Копіювання в ${root}..."
  if command -v rsync >/dev/null 2>&1; then
    rsync -a --exclude '.DS_Store' "${src}/" "${root}/"
  else
    ditto "$src" "$root" 2>/dev/null || cp -R "${src}/." "${root}/"
  fi

  sync_nested_server_mods "$root"
  remove_legacy_questing_conflicts "$root"
  remove_legacy_med_conflicts "$root"
  remove_legacy_sain_conflicts "$root"

  rm -rf "$tmp"
  ok "Встановлення завершено."
  info "SVM не входить у збірку. За потреби оберіть пункт меню «Встановити SVM»."
}

install_svm() {
  local root="$1"
  local zip_path="$2"
  local tmp

  if [[ ! -f "$zip_path" ]]; then
    err "Архів SVM не знайдено: ${zip_path}"
    return 1
  fi

  ensure_dirs "$root"
  tmp="$(mktemp -d "${TMPDIR:-/tmp}/spt-svm.XXXXXX")"
  info "Розпакування ${zip_path}..."
  unzip -q "$zip_path" -d "$tmp"

  local src="$tmp"
  local entries=()
  while IFS= read -r -d '' e; do
    entries+=("$e")
  done < <(find "$tmp" -mindepth 1 -maxdepth 1 -print0)

  # Офіційний архів: Greed.exe + SPT/user/mods/[SVM]...
  if [[ ${#entries[@]} -eq 1 && -d "${entries[0]}" ]]; then
    local name
    name="$(basename "${entries[0]}")"
    if [[ -f "${entries[0]}/Greed.exe" || -d "${entries[0]}/SPT" || -d "${entries[0]}/user" ]]; then
      src="${entries[0]}"
    fi
  fi

  info "Копіювання SVM у ${root}..."
  if command -v rsync >/dev/null 2>&1; then
    rsync -a --exclude '.DS_Store' "${src}/" "${root}/"
  else
    ditto "$src" "$root" 2>/dev/null || cp -R "${src}/." "${root}/"
  fi

  sync_nested_server_mods "$root"

  rm -rf "$tmp"
  ok "SVM встановлено з офіційного релізу ${SVM_REPO}."
  warn "Відкрийте Greed.exe у корені SPT, оберіть пресет і Save/Apply."
  warn "Ліцензія SVM (PUSL): лише personal use; не поширюйте архів/мод далі."
}

do_clean() {
  local root
  root="$(resolve_spt_root)" || return 1
  printf '\n'
  warn "Буде видалено моди з BepInEx/plugins (крім spt/), BepInEx/patchers, user/mods"
  warn "та службові файли пакета (INSTALL/MANIFEST). Greed.exe і [SVM] зберігаються."
  info "SPT: ${root}"
  confirm "Очистити моди?" || { warn "Скасовано."; return 0; }
  clear_mods "$root"
}

do_install() {
  local root zip_path
  root="$(resolve_spt_root)" || return 1
  printf '\n'
  info "SPT: ${root}"
  if [[ -f "$LOCAL_ZIP" ]]; then
    info "Знайдено локальний архів: ${LOCAL_ZIP}"
    if confirm "Завантажити свіжіший реліз з GitHub замість локального?"; then
      zip_path="$(resolve_pack_zip 1)"
    else
      zip_path="$(resolve_pack_zip 0)"
    fi
  else
    zip_path="$(resolve_pack_zip 0)"
  fi
  confirm "Встановити моди з $(basename "$zip_path")?" || { warn "Скасовано."; return 0; }
  install_mods "$root" "$zip_path"
}

do_update() {
  local root zip_path
  root="$(resolve_spt_root)" || return 1
  printf '\n'
  warn "Автоматичне оновлення:"
  warn "  1) видалити всі поточні моди"
  warn "  2) завантажити останній SPT-4.0.13-ModPack.zip з GitHub"
  warn "  3) встановити збірку"
  info "SPT: ${root}"
  info "Repo: https://github.com/${REPO}"
  confirm "Оновити зараз?" || { warn "Скасовано."; return 0; }
  clear_mods "$root"
  zip_path="$(resolve_pack_zip 1)"
  install_mods "$root" "$zip_path"
  ok "Актуалізація завершена."
}

do_install_svm() {
  local root zip_path
  root="$(resolve_spt_root)" || return 1
  printf '\n'
  info "SPT: ${root}"
  info "Джерело: https://github.com/${SVM_REPO}/releases (офіційний асет ${SVM_ASSET_NAME})"
  warn "SVM не входить у ModPack: ліцензія забороняє редистрибуцію."
  warn "Цей пункт лише завантажує архів з upstream для вашого personal use."
  confirm "Завантажити й встановити останній SVM?" || { warn "Скасовано."; return 0; }
  zip_path="${CACHE_DIR}/${SVM_ASSET_NAME}"
  download_svm_latest "$zip_path"
  install_svm "$root" "$zip_path"
}

show_menu() {
  clear 2>/dev/null || true
  printf '%s\n' "${BOLD}========================================${RESET}"
  printf '%s\n' "${BOLD}  SPT 4.0.13 Mod Pack — менеджер${RESET}"
  printf '%s\n' "${BOLD}========================================${RESET}"
  printf '  Репозиторій: %s\n' "$REPO"
  printf '  Локальний zip: %s\n' "$( [[ -f "$LOCAL_ZIP" ]] && echo "є (${LOCAL_ZIP})" || echo "немає" )"
  if [[ -n "${SPT_ROOT:-}" ]]; then
    printf '  SPT_ROOT: %s\n' "$SPT_ROOT"
  fi
  printf '\n'
  printf '  1) Автоматичне оновлення (очистити + остання збірка)\n'
  printf '  2) Очищення від модів\n'
  printf '  3) Встановити моди\n'
  printf '  4) Встановити SVM (офіційний реліз GhostFenixx)\n'
  printf '  5) Вихід\n'
  printf '\n'
}

main() {
  if [[ $# -ge 1 && -n "$1" && "$1" != "-" ]]; then
    export SPT_ROOT="$1"
  fi

  while true; do
    show_menu
    local choice
    read -r -p "Оберіть пункт [1-5]: " choice
    case "${choice:-}" in
      1) do_update; pause ;;
      2) do_clean; pause ;;
      3) do_install; pause ;;
      4) do_install_svm; pause ;;
      5) info "Вихід."; exit 0 ;;
      *) err "Невірний вибір."; pause ;;
    esac
  done
}

main "$@"
