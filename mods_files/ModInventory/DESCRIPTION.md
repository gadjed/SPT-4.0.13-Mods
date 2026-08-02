# ModInventory

| | |
|---|---|
| **Version** | v0.1.0 |
| **Type** | Server / Сервер |
| **Source** | https://github.com/gadjed/ModInventory-SPT-mod |
| **Target** | SPT 4.0.13 |

---

## English

### Description
Exposes the host mod inventory over HTTP so clients can delta-sync only changed files (`/modinventory/api/manifest` + `/file`).

### Settings
(`SPT/user/mods/ModInventory/config.json`)
- **scanRoots** — folders relative to game root to inventory
- **excludeModFolders** — mod folder names skipped (default: `ModInventory`)

### Notes
Install: `SPT/user/mods/ModInventory/`.

---

## Українська

### Опис
Віддає інвентар модів хоста через HTTP, щоб клієнти тягнули лише змінені файли (`/modinventory/api/manifest` + `/file`).

### Налаштування
(`SPT/user/mods/ModInventory/config.json`)
- **scanRoots** — теки відносно кореня гри для інвентаризації
- **excludeModFolders** — теки модів, які пропускаються (за замовчуванням: `ModInventory`)

### Примітки
Встановлення: `SPT/user/mods/ModInventory/`.
