# Questing Bots Continuous

| | |
|---|---|
| **Version** | v0.15.0 |
| **Type** | Client + server / Клієнт + сервер |
| **Source** | https://github.com/gadjed/QuestingBots-Continuous-SPT-mod |
| **Target** | SPT 4.1.0 |

---

## English

### Description
Bots pursue quests/objectives with continuous **scav** reinforcement waves (former Scav Population). Mid-raid PMC respawn/top-up has been removed — only initial PMC spawns.

### Settings
(`SPT/user/mods/QuestingBotsContinuous/config.json` + client F12 menu)
- **enabled**
- Large **questing** / **bot_spawns** trees (roles, hostility, spawn counts)
- **continuous_population.scav_reinforcements** — timed scav waves (~180s), map skip list (lab/labyrinth/hideout)
- PMC group size weights (biased toward 2–4)
- Stuck bots redirect toward a NavMesh point near a human player (no off-mesh teleport snap)
- **F12 actions** — Respawn All Bots (PScav refill), Remove All Corpses

### Notes
Install: extract into SPT **game root** → `SPT/user/mods/QuestingBotsContinuous/` + `BepInEx/plugins/QuestingBotsContinuous/`. Needs BigBrain ≥1.4.0 + Waypoints 1.8.2+. Do **not** stack with DanW QuestingBots or standalone Scav Population.

SPT **4.0.13** build: [v0.12.1](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod/releases/tag/v0.12.1).

### Changelog (0.15.0)
- Removed mid-raid PMC top-up entirely (config + client logic)
- Stuck/off-mesh recovery: redirect near player instead of teleport snap
- Performance: stagger DelayedUpdate timers, cheaper HiveMind, idle spawn poll, staggered scav waves
- Debug overlay / debug logging off by default; Forge zip layout `SPT/` + `BepInEx/`

---

## Українська

### Опис
Боти виконують квести/цілі; безперервні хвилі **скавів** (колишній Scav Population). Переспавн ЧВК mid-raid прибрано — лише початковий спавн PMC.

### Налаштування
(`SPT/user/mods/QuestingBotsContinuous/config.json` + клієнтське меню F12)
- **enabled**
- Великі дерева **questing** / **bot_spawns**
- **continuous_population.scav_reinforcements** — хвилі скавів (~180 с), пропуск lab/labyrinth/hideout
- Ваги розміру груп PMC (зсув до 2–4)
- Застряглі боти отримують ціль біля гравця (без телепорт-snap)
- **F12** — Respawn All Bots (рефіл PScav), Remove All Corpses

### Примітки
Встановлення: розпакувати в корінь SPT → `SPT/user/mods/QuestingBotsContinuous/` + `BepInEx/plugins/QuestingBotsContinuous/`. Потрібні BigBrain ≥1.4.0 + Waypoints 1.8.2+. Не ставити разом із DanW QuestingBots або окремим Scav Population.

Збірка для SPT **4.0.13**: [v0.12.1](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod/releases/tag/v0.12.1).

### Changelog (0.15.0)
- Повністю прибрано mid-raid PMC top-up
- Застрягання: редірект біля гравця замість телепорту
- Оптимізація мікрофрізів (stagger timers, HiveMind, scav waves)
- Debug за замовчуванням вимкнено; архів Forge-layout `SPT/` + `BepInEx/`
