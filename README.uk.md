# SPT Mods

Monorepo: вихідники, релізні архіви та інсталяційний патч для SPT **4.1.0**.

Ціль: **SPT 4.1.0**.

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

Рекомендований спосіб поставити або оновити власні моди на чистому **SPT 4.1.0**.

1. Завантаж реліз інсталера: [`mods_pack_installer`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer) (асет `SPT-4.1.0-ModsPack-Installer.zip`).
2. Розпакуй **обидва** файли `manage-modpack.cmd` і `manage-modpack.ps1` у корінь SPT (туди, де `EscapeFromTarkov.exe` / `SPT.Server.exe`).
3. Запусти `manage-modpack.cmd` і обери пункт меню:
   - **1** — Автооновлення: видалити поточні моди, завантажити останній ModPack з GitHub, встановити
   - **2** — Лише очищення модів (`BepInEx/plugins`, `BepInEx/patchers`, `user/mods`, службові файли пакета)
   - **3** — Встановити моди (локальний zip поруч зі скриптом або завантаження, якщо архіву немає)
   - **4** — Встановити SVM з офіційного релізу [GhostFenixx/svm-csharp](https://github.com/GhostFenixx/svm-csharp) (не входить у збірку; PUSL забороняє редистрибуцію)
   - **5** — Вихід

Для пунктів **1** / **4** і для **3** без локального `SPT-4.1.0-ModPack.zip` потрібен інтернет. Сам ModPack — на [GitHub Releases](https://github.com/gadjed/SPT-4.0.13-Mods/releases).

macOS / Linux: той самий процес через `mods_patch/manage-modpack.sh` (`./manage-modpack.sh "/шлях/до/SPT"`).

## Моди

### Власні / підтримуються тут (SPT 4.1.0)

| Тека | Опис | Джерело |
|------|------|---------|
| `MedRebalance` | Ребаланс медицини (швидка хірургія + continuous / scratch heal; **SPT 4.0.13**) | [gadjed/MedRebalance-SPT-mod](https://github.com/gadjed/MedRebalance-SPT-mod) |
| `FastTaxi` | Коротший час очікування таксі / авто | [gadjed/FastTaxi-SPT-mod](https://github.com/gadjed/FastTaxi-SPT-mod) |
| `InsuranceControl` | Контроль повернення страховки | [gadjed/Insurance-refund-SPT-mod](https://github.com/gadjed/Insurance-refund-SPT-mod) |
| `QuickSearch` | Швидший пошук контейнерів (клієнт) | [gadjed/Quick-search-SPT-mod](https://github.com/gadjed/Quick-search-SPT-mod) |
| `Saria-4.x.x` | Торговець Saria 2.0 | [gadjed/SariaTrader2.0-SPT-mod](https://github.com/gadjed/SariaTrader2.0-SPT-mod) |
| `SPTQuestingBots` | QuestingBots Continuous (з колишнім Scav Population; замість DanW) | [gadjed/QuestingBots-Continuous-SPT-mod](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod) |
| `YellowFlareCurse` | Прокляття жовтої сигнальної ракети | [gadjed/Yellow-flare-curse-SPT-mod](https://github.com/gadjed/Yellow-flare-curse-SPT-mod) |

### Власні / підтримуються тут (лише SPT 4.0.13)

| Тека | Опис | Джерело |
|------|------|---------|
| `MedRebalance` | v1.3.0 server+client: ребаланс медицини — швидка хірургія, continuous limbs, scratch heal, cancel on damage | [gadjed/MedRebalance-SPT-mod](https://github.com/gadjed/MedRebalance-SPT-mod) |
| `AutoMedHotkeys` | Автоприв’язка медів до слотів 4/5/6 (схрон + рейд) | [gadjed/Auto-med-hotkeys-SPT-mod](https://github.com/gadjed/Auto-med-hotkeys-SPT-mod) |

### Сторонні (сорси збережені; у паку 4.1.0 поки немає)

Повний колишній loadout під 4.0.13 (Fika, SAIN, UIFixes, BigBrain, …) лишається в `mods/` / `mods_files/` як референс. У ModPack для **SPT 4.1.0** вони **не** входять, доки upstream не випустить порти під 4.1.

| Тека | Мод | Джерело |
|------|-----|---------|
| `FastSellInFlea` | Fast Sell In Flea (клієнт; 1.2.0 = SPT 4.0) | [Katrin0522/SPT-FastInFleaSell](https://github.com/Katrin0522/SPT-FastInFleaSell) |
| `EnableLabyrinth` | Enable Labyrinth | [acidphantasm/enablelabyrinth-csharp](https://github.com/acidphantasm/enablelabyrinth-csharp) |
| `GildedKeyStorage` | Gilded Key Storage | [DrakiaXYZ/SPT-GildedKeyStorage-CSharp](https://github.com/DrakiaXYZ/SPT-GildedKeyStorage-CSharp) |
| `LiveFleaPrices` | Live Flea Prices | [DrakiaXYZ/SPT-LiveFleaPrices-CSharp](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices-CSharp) |
| `Fika-Server` | Fika server | [project-fika/Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp) |
| `Fika-Plugin` | Fika client | [project-fika/Fika-Plugin](https://github.com/project-fika/Fika-Plugin) |
| `MoreBotsAPI` | MoreBotsAPI | [TacticalToaster/MoreBotsAPI](https://github.com/TacticalToaster/MoreBotsAPI) |
| `MoreCheckmarks` | MoreCheckmarks (+ backend) | [TommySoucy/MoreCheckmarks](https://github.com/TommySoucy/MoreCheckmarks) |
| `DynamicMaps` | Dynamic Maps | [acidphantasm/SPT-DynamicMaps](https://github.com/acidphantasm/SPT-DynamicMaps) |
| `LootingBots` | Looting Bots | [Skwizzy/SPT-LootingBots](https://github.com/Skwizzy/SPT-LootingBots) |
| `SAIN` | SAIN | [ArchangelWTF/SAIN](https://github.com/ArchangelWTF/SAIN) |
| `UIFixes` | UI Fixes | [tyfon7/UIFixes](https://github.com/tyfon7/UIFixes) |
| `UnbreakableKeys` | Unbreakable Keys | [Toha3673/unbreakableKeys](https://github.com/Toha3673/unbreakableKeys) |
| `SPT-BigBrain` | BigBrain (SAIN / bot deps) | [DrakiaXYZ/SPT-BigBrain](https://github.com/DrakiaXYZ/SPT-BigBrain) |

## Примітки щодо збірки

- **ModPack 4.1.0** містить лише власні моди (див. MANIFEST у zip)
- Використовуйте **`SPTQuestingBots`** (Continuous); DanW QuestingBots і окремий **Scav Population** не включені
- **SVM не редистрибутується** (upstream PUSL). Опційна установка через пункт меню **4**
- Пакт: `mods_patch/SPT-4.1.0-ModPack.zip` (GitHub Releases) — або інсталер вище
- Nested layout SPT 4.x: серверні моди в `SPT/user/mods/` (інсталер синхронізує; паки вже з цим шляхом). Не лишайте DanW `QuestingBots` поруч із Continuous.
- Історичний **SPT 4.0.13**: ModPack `v1.2.x` + стабільний інсталер [`mods_pack_installer_4.0.13`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer_4.0.13) (`SPT-4.0.13-ModsPack-Installer.zip`); включає **MedRebalance 1.3.0**, **YellowFlareCurse**, **AutoMedHotkeys**, **FastSellInFlea**

