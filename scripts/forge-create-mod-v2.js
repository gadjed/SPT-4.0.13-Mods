async (page) => {
  const FORGE_MOD_INDEX = 0; // 0..4
  const PACK_REPO = "https://github.com/gadjed/SPT-4.0.13-Mods";
  const INSTALLER =
    "https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer_4.0.13";

  const mods = [
    {
      name: "Med Rebalance",
      guid: "gadjed.medrebalance",
      teaser: "SPT 4.0.13 вЂ” medicine rebalance: fast CMS/splints, continuous heal, scratch top-ups, cancel on damage.",
      category: "Items",
      source: "https://github.com/gadjed/MedRebalance-SPT-mod",
      profileSafe: true,
      description: [
        "**Medicine rebalance** (formerly Fast Surgery): shorter surgical/splint use time (default **5s**), continuous multi-limb healing, scratch top-ups from medkit resource, cancel-on-damage.",
        "",
        "## Compatibility",
        "- **SPT 4.0.13** вЂ” current `v1.3.0` combo (server + client)",
        "- Older archives under previous name: [v1.2.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.2.0) / [v1.1.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.1.0) (`FastSurgery-*.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "SPT/user/mods/MedRebalance/",
        "BepInEx/plugins/MedRebalance.Client.dll",
        "```",
        "",
        "Config: `SPT/user/mods/MedRebalance/config.json` (`UseTimeSeconds`, per-item toggles). F12 for client options.",
        "",
        "Remove standalone Continuous Healing and old Fast Surgery folders.",
        "",
        "## Full mod pack + auto installer",
        "Included in my SPT loadout assembly with an automatic install/update manager:",
        "- Pack repository: " + PACK_REPO,
        "- Installer release: " + INSTALLER,
        "",
        "## Source",
        "https://github.com/gadjed/MedRebalance-SPT-mod",
      ].join("\n"),
    },
    {
      name: "Fast Taxi",
      guid: "gadjed.fasttaxi",
      teaser:
        "SPT 4.0.13 вЂ” shorter paid car/taxi extract wait (default 8s).",
      category: "Locations",
      source: "https://github.com/gadjed/FastTaxi-SPT-mod",
      profileSafe: true,
      description: [
        "Shortens paid car/taxi extract wait times (typically ~60s в†’ configurable, default **8 seconds**).",
        "",
        "## Compatibility",
        "- **SPT 4.0.13** вЂ” [v1.0.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.0.0) (`FastTaxi-1.0.0.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "user/mods/FastTaxi/",
        "```",
        "",
        "Config: `user/mods/FastTaxi/config.json` (`WaitTimeSeconds`).",
        "",
        "## Full mod pack + auto installer",
        "Included in my SPT loadout assembly with an automatic install/update manager:",
        "- Pack repository: " + PACK_REPO,
        "- Installer release: " + INSTALLER,
        "",
        "## Source",
        "https://github.com/gadjed/FastTaxi-SPT-mod",
      ].join("\n"),
    },
    {
      name: "Insurance Control",
      guid: "gadjed.insurancerefund",
      teaser:
        "SPT 4.0.13 — insurance return rules + Insure All stash button (F12).",
      category: "Scripting",
      source: "https://github.com/gadjed/Insurance-refund-SPT-mod",
      profileSafe: false,
      description: [
        "Server: insurance return timing, lost chance, magazine/container contents.",
        "Client: **Insure All** stash button (Prapor or Therapist) with F12 layout settings.",
        "",
        "## Compatibility",
        "- **SPT 4.0.13** — [v1.1.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.1.0) (`InsuranceControl-1.1.0.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "SPT/user/mods/InsuranceControl/",
        "BepInEx/plugins/InsuranceControl.Client.dll",
        "```",
        "",
        "Remove old `InsureAllPrapor.dll` if present.",
        "",
        "Client config: F12 / `BepInEx/config/gadjed.insurancerefund.cfg`.",
        "Server config: `SPT/user/mods/InsuranceControl/config.json`.",
        "",
        "## Full mod pack + auto installer",
        "Included in my SPT loadout assembly with an automatic install/update manager:",
        "- Pack repository: " + PACK_REPO,
        "- Installer release: " + INSTALLER,
        "",
        "## Source",
        "https://github.com/gadjed/Insurance-refund-SPT-mod",
      ].join("\n"),
    },
    {
      name: "Quick Search",
      guid: "gadjed.quicksearch",
      teaser:
        "SPT 4.0.13 вЂ” search containers and corpses faster (default 3Г—).",
      category: "Scripting",
      source: "https://github.com/gadjed/Quick-search-SPT-mod",
      profileSafe: true,
      description: [
        "Speeds up searching containers and corpses by a configurable multiplier (default **3Г—**).",
        "",
        "## Compatibility",
        "- **SPT 4.0.13** вЂ” [v1.0.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.0.0) (`QuickSearch-1.0.0.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "BepInEx/plugins/QuickSearch.dll",
        "```",
        "",
        "Config: `BepInEx/config/gadjed.quicksearch.cfg` / F12 (`SearchSpeedMultiplier`).",
        "",
        "## Full mod pack + auto installer",
        "Included in my SPT loadout assembly with an automatic install/update manager:",
        "- Pack repository: " + PACK_REPO,
        "- Installer release: " + INSTALLER,
        "",
        "## Source",
        "https://github.com/gadjed/Quick-search-SPT-mod",
      ].join("\n"),
    },
    {
      name: "Yellow Flare Curse",
      guid: "gadjed.yellowflarecurse",
      teaser:
        "SPT 4.0.13 вЂ” yellow RSP-30 flare curse + delayed high-value airdrop.",
      category: "Overhauls",
      source: "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
      profileSafe: true,
      description: [
        "Using an RSP-30 Yellow (once per raid) curses nearby scavs/PMCs, then triggers a high-value airdrop after a delay.",
        "",
        "## Compatibility",
        "- **SPT 4.0.13** вЂ” [v1.4.5](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.4.5) (`YellowFlareCurse-1.4.5.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "user/mods/YellowFlareCurse/",
        "BepInEx/plugins/YellowFlareCurse.Client.dll",
        "```",
        "",
        "Server config: `user/mods/YellowFlareCurse/config.json`. Client settings in F12.",
        "",
        "Needs maps with airdrop points. SAIN optional for harder cursed fights.",
        "",
        "## Full mod pack + auto installer",
        "Included in my SPT loadout assembly with an automatic install/update manager:",
        "- Pack repository: " + PACK_REPO,
        "- Installer release: " + INSTALLER,
        "",
        "## Source",
        "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
      ].join("\n"),
    },
  ];

  const mod = mods[FORGE_MOD_INDEX];
  if (!mod) throw new Error("Invalid mod index " + FORGE_MOD_INDEX);

  await page.goto("https://forge.sp-tarkov.com/mod/create", {
    waitUntil: "networkidle",
  });
  await page.waitForTimeout(1000);

  await page.locator('input[name="name"]').fill(mod.name);
  await page.locator('input[name="guid"]').fill(mod.guid);
  await page.locator('input[name="teaser"]').fill(mod.teaser);
  await page.locator('textarea[name="description"]').fill(mod.description);

  await page.getByRole("combobox", { name: /License/i }).click();
  await page.getByRole("option", { name: "MIT License", exact: true }).click();
  await page.waitForTimeout(200);

  await page.getByRole("combobox", { name: /Category/i }).click();
  await page.getByRole("option", { name: mod.category, exact: true }).click();
  await page.waitForTimeout(500);

  const sourceBoxes = page.locator('input[type="url"]');
  await sourceBoxes.nth(0).fill(mod.source);
  const labels = page.getByPlaceholder("Label (optional)");
  await labels.nth(0).fill("Source");
  await page.getByRole("button", { name: /Add another link/i }).click();
  await page.waitForTimeout(400);
  await sourceBoxes.nth(1).fill(PACK_REPO);
  await labels.nth(1).fill("Mod pack + auto installer");

  await page.getByRole("button", { name: "Now", exact: true }).click();
  await page.waitForTimeout(400);

  // Uncheck AI if checked (requires extra disclosure text)
  const ai = page.getByRole("checkbox", { name: /Contains AI Content/i });
  if (await ai.isChecked()) await ai.uncheck();

  const profile = page.getByRole("checkbox", {
    name: /Disable Profile Binding Notice/i,
  });
  if ((await profile.count()) > 0 && mod.profileSafe) {
    if (!(await profile.isChecked())) await profile.check();
  }

  await page.getByRole("button", { name: "Create Mod", exact: true }).click();
  await page.waitForTimeout(3000);

  const alertText = await page
    .locator("main")
    .innerText()
    .catch(() => "");

  return {
    index: FORGE_MOD_INDEX,
    name: mod.name,
    url: page.url(),
    title: await page.title(),
    ok: /\/mod\/\d+\//.test(page.url()),
    snippet: alertText.slice(0, 1500),
  };
}
