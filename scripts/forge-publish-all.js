async (page) => {
  const PACK_REPO = "https://github.com/gadjed/SPT-4.0.13-Mods";
  const INSTALLER =
    "https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer_4.0.13";
  const vt = (hash) => "https://www.virustotal.com/gui/file/" + hash;

  const mods = [
    {
      create: false,
      modId: 2869,
      name: "Med Rebalance",
      versions: [
        {
          version: "1.1.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/MedRebalance-SPT-mod/releases/download/v1.3.0/MedRebalance-1.3.0.zip",
          virusTotal: vt(
            "b17235e756b9bb58b0423eb066de784cc4b09ef0edb53424aab9c41e1c6fcd72"
          ),
          description:
            "SPT **4.0.13** release.\n\nArchive: `MedRebalance-1.3.0.zip`.",
        },
      ],
    },
    {
      create: true,
      name: "Fast Taxi",
      guid: "gadjed.fasttaxi",
      teaser: "SPT 4.0.13 вЂ” shorter paid car/taxi extract wait (default 8s).",
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
      versions: [
        {
          version: "1.0.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/FastTaxi-SPT-mod/releases/download/v1.0.0/FastTaxi-1.0.0.zip",
          virusTotal: vt(
            "d2e486ca25cbdd9083c41ec0b73f8318f66608226e2a8664204b45da99d921bc"
          ),
          description: "SPT **4.0.13** release.\n\nArchive: `FastTaxi-1.0.0.zip`.",
        },
      ],
    },
    {
      create: true,
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
      versions: [
        {
          version: "1.1.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/Insurance-refund-SPT-mod/releases/download/v1.1.0/InsuranceControl-1.1.0.zip",
          virusTotal: vt(
            "2219f7b8262399d24a9174e21b912a2ebf78b9c92a2a16a5590304a8efcc3138"
          ),
          description:
            "SPT **4.0.13** — merged Insure All client + F12 settings.\n\nArchive: `InsuranceControl-1.1.0.zip`.",
        },
      ],
    },
    {
      create: true,
      name: "Quick Search",
      guid: "gadjed.quicksearch",
      teaser: "SPT 4.0.13 вЂ” search containers and corpses faster (default 3Г—).",
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
      versions: [
        {
          version: "1.0.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/Quick-search-SPT-mod/releases/download/v1.0.0/QuickSearch-1.0.0.zip",
          virusTotal: vt(
            "67d64df7698d7fb681aec2a0b8da8784f7785caf9fd6c875d16a63ba2bf505dd"
          ),
          description:
            "SPT **4.0.13** release.\n\nArchive: `QuickSearch-1.0.0.zip`.",
        },
      ],
    },
    {
      create: true,
      name: "Yellow Flare Curse",
      guid: "gadjed.yellowflarecurse",
      teaser:
        "SPT 4.0.13 вЂ” yellow flare curse: teleport hunt pack, Tagilla, high-value airdrop.",
      category: "Overhauls",
      source: "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
      profileSafe: true,
      description: [
        "Successful **RSP-30 Yellow** (once per raid) teleports scavs into a hunt ring, optionally spawns Tagilla / cultists, allies scav AI against you, then drops a forced high-value SUPPLY airdrop after a delay.",
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
        "Maps without airdrop points still run the hunt/bosses (airdrop skipped). SAIN optional.",
        "",
        "## Full mod pack + auto installer",
        "Included in my SPT loadout assembly with an automatic install/update manager:",
        "- Pack repository: " + PACK_REPO,
        "- Installer release: " + INSTALLER,
        "",
        "## Source",
        "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
      ].join("\n"),
      versions: [
        {
          version: "1.4.5",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/download/v1.4.5/YellowFlareCurse-1.4.5.zip",
          virusTotal: vt(
            "c0045f405da2b4b127d8d8d8ca2c6b44eda2a525f1a897618fce191c3ba6a188"
          ),
          description:
            "SPT **4.0.13** release.\n\n- SUPPLY crate + ForcedLoot (no Common/weapon junk)\n- Teleport/curse scavs only (PMCs excluded)\n- Optional cultist squad spawn\n\nArchive: `YellowFlareCurse-1.4.5.zip`.",
        },
      ],
    },
  ];

  async function createMod(mod) {
    await page.goto("https://forge.sp-tarkov.com/mod/create", {
      waitUntil: "networkidle",
    });
    await page.waitForTimeout(900);

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
    await page.waitForTimeout(300);

    const ai = page.getByRole("checkbox", { name: /Contains AI Content/i });
    if (await ai.isChecked()) await ai.uncheck();

    const profile = page.getByRole("checkbox", {
      name: /Disable Profile Binding Notice/i,
    });
    if ((await profile.count()) > 0 && mod.profileSafe) {
      if (!(await profile.isChecked())) await profile.check();
    }

    await page.getByRole("button", { name: "Create Mod", exact: true }).click();
    await page.waitForTimeout(3500);

    const m = page.url().match(/\/mod\/(\d+)\//);
    if (!m) {
      const main = await page.locator("main").innerText().catch(() => "");
      throw new Error(
        "Failed to create mod " + mod.name + " at " + page.url() + " :: " + main.slice(0, 400)
      );
    }
    return Number(m[1]);
  }

  async function createVersion(modId, ver) {
    await page.goto(
      "https://forge.sp-tarkov.com/mod/" + modId + "/version/create",
      { waitUntil: "networkidle" }
    );
    await page.waitForTimeout(800);

    await page.locator('input[name="version"]').fill(ver.version);
    await page.locator('textarea[name="description"]').fill(ver.description);
    await page.locator('input[name="link"]').fill(ver.download);
    await page.locator('input[name="sptVersionConstraint"]').fill(ver.spt);
    await page.waitForTimeout(700);
    await page.locator('input[name="virusTotalLinks.0.url"]').fill(ver.virusTotal);
    await page
      .locator('input[name="virusTotalLinks.0.label"]')
      .fill("Release archive");

    const nowBtn = page.getByRole("button", { name: "Now", exact: true });
    if (await nowBtn.count()) {
      await nowBtn.click();
      await page.waitForTimeout(300);
    }

    await page.getByRole("button", { name: "Create Version", exact: true }).click();
    await page.waitForTimeout(4000);

    const ok = !page.url().includes("/version/create");
    if (!ok) {
      const main = await page.locator("main").innerText().catch(() => "");
      throw new Error(
        "Failed version " +
          ver.version +
          " for mod " +
          modId +
          " :: " +
          main.slice(0, 500)
      );
    }
    return page.url();
  }

  const report = [];

  for (const mod of mods) {
    let modId = mod.modId;
    if (mod.create) {
      modId = await createMod(mod);
    }
    const versionUrls = [];
    for (const ver of mod.versions) {
      const url = await createVersion(modId, ver);
      versionUrls.push({ version: ver.version, url });
    }
    report.push({
      name: mod.name,
      modId,
      page: "https://forge.sp-tarkov.com/mod/" + modId,
      versions: versionUrls,
    });
  }

  return report;
}
