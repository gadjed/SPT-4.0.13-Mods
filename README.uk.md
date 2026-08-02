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
| `FastSurgery` | Швидша хірургія / шини | [gadjed/FastSurgery-SPT-mod](https://github.com/gadjed/FastSurgery-SPT-mod) |
| `FastTaxi` | Коротший час очікування таксі / авто | [gadjed/FastTaxi-SPT-mod](https://github.com/gadjed/FastTaxi-SPT-mod) |
| `InsuranceControl` | Контроль повернення страховки | [gadjed/Insurance-refund-SPT-mod](https://github.com/gadjed/Insurance-refund-SPT-mod) |
| `QuickSearch` | Швидший пошук контейнерів (клієнт) | [gadjed/Quick-search-SPT-mod](https://github.com/gadjed/Quick-search-SPT-mod) |
| `Saria-4.x.x` | Торговець Saria 2.0 | [gadjed/SariaTrader2.0-SPT-mod](https://github.com/gadjed/SariaTrader2.0-SPT-mod) |
| `SPTQuestingBots` | QuestingBots Continuous (з колишнім Scav Population; замість DanW) | [gadjed/QuestingBots-Continuous-SPT-mod](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod) |
| `YellowFlareCurse` | Прокляття жовтої сигнальної ракети | [gadjed/Yellow-flare-curse-SPT-mod](https://github.com/gadjed/Yellow-flare-curse-SPT-mod) |

### Сторонні (сорси збережені; у паку 4.1.0 поки немає)

Повний колишній loadout під 4.0.13 (Fika, SAIN, UIFixes, BigBrain, …) лишається в `mods/` / `mods_files/` як референс. У ModPack для **SPT 4.1.0** вони **не** входять, доки upstream не випустить порти під 4.1.

## Примітки щодо збірки

- **ModPack 4.1.0** містить лише власні моди (див. MANIFEST у zip)
- Використовуйте **`SPTQuestingBots`** (Continuous); DanW QuestingBots і окремий **Scav Population** не включені
- **SVM не редистрибутується** (upstream PUSL). Опційна установка через пункт меню **4**
- Пакт: `mods_patch/SPT-4.1.0-ModPack.zip` (GitHub Releases) — або інсталер вище
- Nested layout SPT 4.x: серверні моди в `SPT/user/mods/` (інсталер синхронізує; паки вже з цим шляхом). Не лишайте DanW `QuestingBots` поруч із Continuous.
- Історичний **SPT 4.0.13**: ModPack `v1.2.x` + стабільний інсталер [`mods_pack_installer_4.0.13`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer_4.0.13) (`SPT-4.0.13-ModsPack-Installer.zip`)

