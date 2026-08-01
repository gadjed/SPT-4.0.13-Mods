#!/usr/bin/env bash
# SPT 4.0.13 Mod Pack Manager
# Керування встановленням / оновленням / очищенням модів збірки.
set -euo pipefail

REPO="gadjed/SPT-4.0.13-Mods"
ASSET_NAME="SPT-4.0.13-ModPack.zip"
SVM_REPO="GhostFenixx/svm-csharp"
SVM_ASSET_NAME="SVM.Server.Value.Modifier.zip"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CACHE_DIR="${SCRIPT_DIR}/.modpack-cache"
LOCAL_ZIP="${SCRIPT_DIR}/${ASSET_NAME}"

# Файли з кореня SPT, що приходять із пакета / опційного SVM
PACK_ROOT_FILES=(Greed.exe INSTALL.txt MANIFEST.txt)
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

is_spt_root() {
  local dir="$1"
  [[ -f "${dir}/EscapeFromTarkov.exe" ]] || [[ -f "${dir}/SPT.Server.exe" ]] || [[ -d "${dir}/BepInEx" && -d "${dir}/user" ]]
}

resolve_spt_root() {
  if [[ -n "${SPT_ROOT:-}" ]] && is_spt_root "$SPT_ROOT"; then
    printf '%s' "$SPT_ROOT"
    return 0
  fi
  if is_spt_root "$PWD"; then
    printf '%s' "$PWD"
    return 0
  fi
  if is_spt_root "$SCRIPT_DIR"; then
    printf '%s' "$SCRIPT_DIR"
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
}

clear_mods() {
  local root="$1"
  info "Очищення модів у: ${root}"

  if [[ -d "${root}/BepInEx/plugins" ]]; then
    find "${root}/BepInEx/plugins" -mindepth 1 -maxdepth 1 -exec rm -rf {} +
    ok "  очищено BepInEx/plugins/"
  fi
  if [[ -d "${root}/BepInEx/patchers" ]]; then
    find "${root}/BepInEx/patchers" -mindepth 1 -maxdepth 1 -exec rm -rf {} +
    ok "  очищено BepInEx/patchers/"
  fi
  if [[ -d "${root}/user/mods" ]]; then
    find "${root}/user/mods" -mindepth 1 -maxdepth 1 -exec rm -rf {} +
    ok "  очищено user/mods/"
  fi
  if [[ -d "${root}/SPT/user/mods" ]]; then
    find "${root}/SPT/user/mods" -mindepth 1 -maxdepth 1 -exec rm -rf {} +
    ok "  очищено SPT/user/mods/"
  fi

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
  info "Завантаження останнього релізу з GitHub (${REPO})..."

  if command -v gh >/dev/null 2>&1; then
    rm -f "$dest"
    gh release download -R "$REPO" -p "$ASSET_NAME" -D "$(dirname "$dest")" --clobber
    if [[ "$(basename "$dest")" != "$ASSET_NAME" ]]; then
      mv -f "$(dirname "$dest")/${ASSET_NAME}" "$dest"
    fi
  else
    local api_url="https://api.github.com/repos/${REPO}/releases/latest"
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
' "$ASSET_NAME")"
    if [[ -z "${zip_url:-}" ]]; then
      err "Не знайдено асет ${ASSET_NAME} у latest release."
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

  rm -rf "$tmp"
  ok "SVM встановлено з офіційного релізу ${SVM_REPO}."
  warn "Відкрийте Greed.exe у корені SPT, оберіть пресет і Save/Apply."
  warn "Ліцензія SVM (PUSL): лише personal use; не поширюйте архів/мод далі."
}

do_clean() {
  local root
  root="$(resolve_spt_root)" || return 1
  printf '\n'
  warn "Буде видалено ВСІ моди з BepInEx/plugins, BepInEx/patchers, user/mods"
  warn "та службові файли пакета (INSTALL/MANIFEST, Greed.exe якщо був)."
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
