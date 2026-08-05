# SPT Mods

Monorepo: вихідники, релізні архіви та інсталяційний патч для SPT **4.0.13**.

Ціль: **SPT 4.0.13**.

## Структура

```
SPT mods/
├── README.md
├── README.uk.md
├── docs/
├── mods/
│   ├── owned/      # власні оригінальні моди
│   ├── forks/      # підтримувані форки upstream
│   └── external/   # сторонні сорси з ModPack
├── mods_files/     # останні релізні zip (+ DESCRIPTION.md EN/UK), та сама група
└── mods_patch/     # ModPack zip + скрипти встановлення / оновлення
```

## Автоматичне встановлення / оновлення

Рекомендований спосіб поставити або оновити повний loadout на чистому **SPT 4.0.13**.

1. Завантаж реліз інсталера: [`mods_pack_installer_4.0.13`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer_4.0.13) (асет `SPT-4.0.13-ModsPack-Installer.zip`).
2. Розпакуй **обидва** файли `manage-modpack.cmd` і `manage-modpack.ps1` у корінь SPT (туди, де `EscapeFromTarkov.exe` / `SPT.Server.exe`).
3. Запусти `manage-modpack.cmd` і обери пункт меню:
   - **1** — Автооновлення: видалити поточні моди, завантажити останній ModPack з GitHub, встановити
   - **2** — Лише очищення модів (`BepInEx/plugins`, `BepInEx/patchers`, `user/mods`, службові файли пакета)
   - **3** — Встановити моди (локальний zip поруч зі скриптом або завантаження, якщо архіву немає)
   - **4** — Встановити SVM з офіційного релізу [GhostFenixx/svm-csharp](https://github.com/GhostFenixx/svm-csharp) (не входить у збірку; PUSL забороняє редистрибуцію)
   - **5** — Вихід

Для пунктів **1** / **4** і для **3** без локального `SPT-4.0.13-ModPack.zip` потрібен інтернет. Сам ModPack — на [GitHub Releases](https://github.com/gadjed/SPT-4.0.13-Mods/releases).

macOS / Linux: той самий процес через `mods_patch/manage-modpack.sh` (`./manage-modpack.sh "/шлях/до/SPT"`).

## Моди

### Власні (`mods/owned/`)

| Тека | Опис | Джерело |
|------|------|---------|
| `MedRebalance` | Ребаланс медицини — швидка хірургія, continuous limbs, scratch heal, cancel on damage | [gadjed/MedRebalance-SPT-mod](https://github.com/gadjed/MedRebalance-SPT-mod) |
| `AutoMedHotkeys` | Автоприв’язка медів до слотів 4/5/6 (схрон + рейд) | [gadjed/Auto-med-hotkeys-SPT-mod](https://github.com/gadjed/Auto-med-hotkeys-SPT-mod) |
| `FastTaxi` | Коротший час очікування таксі / авто | [gadjed/FastTaxi-SPT-mod](https://github.com/gadjed/FastTaxi-SPT-mod) |
| `InsuranceControl` | Контроль повернення страховки | [gadjed/Insurance-refund-SPT-mod](https://github.com/gadjed/Insurance-refund-SPT-mod) |
| `ModInventory` | API інвентаря модів хоста для delta-sync клієнтів | [gadjed/ModInventory-SPT-mod](https://github.com/gadjed/ModInventory-SPT-mod) |
| `QuickSearch` | Швидший пошук контейнерів (клієнт) | [gadjed/Quick-search-SPT-mod](https://github.com/gadjed/Quick-search-SPT-mod) |
| `InsureAllPrapor` | Кнопка на схроні — застрахувати лутаут у Прапора | [gadjed/Insure-all-prapor-SPT-mod](https://github.com/gadjed/Insure-all-prapor-SPT-mod) |
| `YellowFlareCurse` | Прокляття жовтої сигнальної ракети | [gadjed/Yellow-flare-curse-SPT-mod](https://github.com/gadjed/Yellow-flare-curse-SPT-mod) |
| `PackItems` | Контекстне меню схрону — скласти підходящі предмети у кейс | [gadjed/Pack-items-SPT-mod](https://github.com/gadjed/Pack-items-SPT-mod) |

### Форки (`mods/forks/`)

| Тека | Опис | Джерело |
|------|------|---------|
| `SAIN` | SAIN StealthEngage (обережний вступ ЧВК у бій за пострілами) | [gadjed/SAIN-StealthEngage-SPT-mod](https://github.com/gadjed/SAIN-StealthEngage-SPT-mod) |
| `SPTQuestingBots` | QuestingBots Continuous (з колишнім Scav Population; замість DanW) | [gadjed/QuestingBots-Continuous-SPT-mod](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod) |
| `Saria-4.x.x` | Торговець Saria 2.0 | [gadjed/SariaTrader2.0-SPT-mod](https://github.com/gadjed/SariaTrader2.0-SPT-mod) |
| `SPT-Waypoints` | Waypoints — база для локального форку (DrakiaXYZ 1.8.2) | [DrakiaXYZ/SPT-Waypoints](https://github.com/DrakiaXYZ/SPT-Waypoints) |

### Зовнішні (`mods/external/`)

| Тека | Мод | Джерело |
|------|-----|---------|
| `AmandsGraphics` | Amands's Graphics (клієнт; 1.7.0 / SPT 4.0) | [Amands2Mello/AmandsGraphics](https://github.com/Amands2Mello/AmandsGraphics) · [Forge](https://forge.sp-tarkov.com/mod/592/amandss-graphics) |
| `FastSellInFlea` | Fast Sell In Flea (клієнт; 1.2.0) | [Katrin0522/SPT-FastInFleaSell](https://github.com/Katrin0522/SPT-FastInFleaSell) |
| `EnableLabyrinth` | Enable Labyrinth | [acidphantasm/enablelabyrinth-csharp](https://github.com/acidphantasm/enablelabyrinth-csharp) |
| `Skipper` | Skipper — пропуск квестів (клієнт; 1.1.4) | [acidphantasm/SPT-Skipper](https://github.com/acidphantasm/SPT-Skipper) · [Forge](https://forge.sp-tarkov.com/mod/1343/skipper) |
| `GildedKeyStorage` | Gilded Key Storage | [DrakiaXYZ/SPT-GildedKeyStorage-CSharp](https://github.com/DrakiaXYZ/SPT-GildedKeyStorage-CSharp) |
| `LiveFleaPrices` | Live Flea Prices | [DrakiaXYZ/SPT-LiveFleaPrices-CSharp](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices-CSharp) |
| `Fika-Server` | Fika server | [project-fika/Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp) |
| `Fika-Plugin` | Fika client | [project-fika/Fika-Plugin](https://github.com/project-fika/Fika-Plugin) |
| `MoreBotsAPI` | MoreBotsAPI | [TacticalToaster/MoreBotsAPI](https://github.com/TacticalToaster/MoreBotsAPI) |
| `MoreCheckmarks` | MoreCheckmarks (+ backend) | [TommySoucy/MoreCheckmarks](https://github.com/TommySoucy/MoreCheckmarks) |
| `DynamicMaps` | Dynamic Maps | [acidphantasm/SPT-DynamicMaps](https://github.com/acidphantasm/SPT-DynamicMaps) |
| `LootingBots` | Looting Bots | [Skwizzy/SPT-LootingBots](https://github.com/Skwizzy/SPT-LootingBots) |
| `UIFixes` | UI Fixes | [tyfon7/UIFixes](https://github.com/tyfon7/UIFixes) |
| `UnbreakableKeys` | Unbreakable Keys | [Toha3673/unbreakableKeys](https://github.com/Toha3673/unbreakableKeys) |
| `SPT-BigBrain` | BigBrain (SAIN / bot deps) | [DrakiaXYZ/SPT-BigBrain](https://github.com/DrakiaXYZ/SPT-BigBrain) |

## Примітки щодо збірки

- **ModPack 4.0.13** (`v1.2.13+`) — повний server+client loadout (див. MANIFEST у zip), включно з **Saria 2.0.5** (BlackRock, набої/магазини SPEAR 6.8), **PackItems**, **InsureAllPrapor**, **ModInventory**, **Skipper** (пропуск квестів) і **AmandsGraphics 1.7.0** (яскравість / post FX)
- Використовуйте **`SPTQuestingBots`** (Continuous); DanW QuestingBots і окремий **Scav Population** не включені
- Краще **`SAIN` StealthEngage** замість стокового SAIN (ті самі шляхи / GUID); manage-modpack прибирає альтернативні теки SAIN
- **SVM не редистрибутується** (upstream PUSL). Опційна установка через пункт меню **4**
- Пакт: `mods_patch/SPT-4.0.13-ModPack.zip` (GitHub Releases) — або інсталер вище
- Nested layout SPT 4.x: серверні моди в `SPT/user/mods/` (інсталер синхронізує; паки вже з цим шляхом). Не лишайте DanW `QuestingBots` поруч із Continuous.
- Не використовуйте зламаний ModPack `v1.2.5`

## Документація розробки

- [Нотатки: стек ШІ ботів](docs/bot-ai-stack-notes.uk.md) — Waypoints / BigBrain / SAIN / QuestingBots (тези, Q&A, бій і групи)
