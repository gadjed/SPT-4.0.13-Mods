# SPT Mods

Monorepo: вихідники, релізні архіви та інсталяційний патч для SPT **4.0.13**.

Ціль: лише **SPT 4.0.13**. Не орієнтуватися на 4.1.0+ як на базову версію.

## Структура

```
SPT mods/
├── README.md
├── README.uk.md
├── mods/          # вихідний код кожного мода
├── mods_files/    # останні GitHub-релізи по модах (+ DESCRIPTION.md EN/UK)
└── mods_patch/    # ModPack zip + скрипти встановлення / оновлення
```

## Автоматичне встановлення / оновлення

Рекомендований спосіб поставити або оновити всю збірку на чистому **SPT 4.0.13**.

1. Завантаж реліз інсталера: [`mods_pack_installer`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer) (асет `SPT-4.0.13-ModsPack-Installer.zip`).
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

### Власні / підтримуються тут

| Тека | Опис | Джерело |
|------|------|---------|
| `FastSurgery` | Швидше застосування хірургії / шин | [gadjed/FastSurgery-SPT-mod](https://github.com/gadjed/FastSurgery-SPT-mod) |
| `FastTaxi` | Коротший час очікування авто / таксі-екстракту | [gadjed/FastTaxi-SPT-mod](https://github.com/gadjed/FastTaxi-SPT-mod) |
| `InsuranceControl` | Керування поверненням страховки | [gadjed/Insurance-refund-SPT-mod](https://github.com/gadjed/Insurance-refund-SPT-mod) |
| `QuickSearch` | Швидший обшук контейнерів (клієнт) | [gadjed/Quick-search-SPT-mod](https://github.com/gadjed/Quick-search-SPT-mod) |
| `Saria-4.x.x` | Торговець Saria 2.0 | [gadjed/SariaTrader2.0-SPT-mod](https://github.com/gadjed/SariaTrader2.0-SPT-mod) |
| `SPTQuestingBots` | QuestingBots Continuous (включно з колишнім Scav Population; замість DanW) | [gadjed/QuestingBots-Continuous-SPT-mod](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod) |
| `YellowFlareCurse` | Прокляття жовтої ракети | [gadjed/Yellow-flare-curse-SPT-mod](https://github.com/gadjed/Yellow-flare-curse-SPT-mod) |

### Сторонні (збірка сервера + залежності)

| Тека | Мод | Джерело |
|------|-----|---------|
| `EnableLabyrinth` | Enable Labyrinth | [acidphantasm/enablelabyrinth-csharp](https://github.com/acidphantasm/enablelabyrinth-csharp) |
| `GildedKeyStorage` | Gilded Key Storage 2.0.4 (C#) | [DrakiaXYZ/SPT-GildedKeyStorage-CSharp](https://github.com/DrakiaXYZ/SPT-GildedKeyStorage-CSharp) |
| `LiveFleaPrices` | Live Flea Prices 2.0.1 (C#) | [DrakiaXYZ/SPT-LiveFleaPrices-CSharp](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices-CSharp) |
| `Fika-Server` | Fika (сервер) | [project-fika/Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp) |
| `Fika-Plugin` | Fika (клієнт) | [project-fika/Fika-Plugin](https://github.com/project-fika/Fika-Plugin) |
| `MoreBotsAPI` | MoreBotsAPI | [TacticalToaster/MoreBotsAPI](https://github.com/TacticalToaster/MoreBotsAPI) |
| `MoreCheckmarks` | MoreCheckmarks (+ backend) | [TommySoucy/MoreCheckmarks](https://github.com/TommySoucy/MoreCheckmarks) |
| `DynamicMaps` | Dynamic Maps | [acidphantasm/SPT-DynamicMaps](https://github.com/acidphantasm/SPT-DynamicMaps) |
| `LootingBots` | Looting Bots | [Skwizzy/SPT-LootingBots](https://github.com/Skwizzy/SPT-LootingBots) |
| `SAIN` | SAIN | [ArchangelWTF/SAIN](https://github.com/ArchangelWTF/SAIN) |
| `UIFixes` | UI Fixes | [tyfon7/UIFixes](https://github.com/tyfon7/UIFixes) |
| `UnbreakableKeys` | Unbreakable Keys | [Toha3673/unbreakableKeys](https://github.com/Toha3673/unbreakableKeys) |
| `SPT-BigBrain` | BigBrain (залежність SAIN / ботів) | [DrakiaXYZ/SPT-BigBrain](https://github.com/DrakiaXYZ/SPT-BigBrain) |
| `ContinuousHealing` | Continuous Healing | [Lacyway/ContinuousHealing](https://github.com/Lacyway/ContinuousHealing) |

## Примітки щодо збірки

- Використовується **`SPTQuestingBots`** (Continuous); оригінал DanW і окремий **Scav Population** не включені (population уже в Continuous)
- Є **`YellowFlareCurse`** і **`Saria-4.x.x`** 2.0.1
- **SVM не редистрибується** (upstream PUSL). Опційне встановлення — пункт **4** у manage-modpack з офіційного релізу GhostFenixx
- Інсталяційний пак: `mods_patch/SPT-4.0.13-ModPack.zip` (GitHub Releases) — або інсталер вище
- Скрипти менеджера: `mods_patch/manage-modpack.cmd` + `.ps1` (+ `.sh`)

## Примітки

- Для встановлення краще інсталер / реліз ModPack; це дерево — для читання й правки вихідного коду.
- Upstream-посилання наведені вище (вкладена git-історія окремих модів у monorepo не зберігається).
