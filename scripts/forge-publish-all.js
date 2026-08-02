async (page) => {
  const PACK_REPO = "https://github.com/gadjed/SPT-4.0.13-Mods";
  const INSTALLER =
    "https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer";
  const vt = (hash) => "https://www.virustotal.com/gui/file/" + hash;

  const mods = [
    // Med Rebalance (Forge: was Fast Surgery) already created as 2869 — only add 4.0.13 version if needed
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
            "SPT **4.0.13** release.\n\nArchive: `MedRebalance-1.3.0.zip`\n\nFor SPT 4.1.0 use [v1.2.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.2.0).",
        },
      ],
    },
    {
      create: true,
      name: "Fast Taxi",
      guid: "gadjed.fasttaxi",
      teaser:
        "SPT 4.1.0 / 4.0.13 — shorter paid car/taxi extract wait (default 8s).",
      category: "Locations",
      source: "https://github.com/gadjed/FastTaxi-SPT-mod",
      profileSafe: true,
      description: [
        "Shortens paid car/taxi extract wait times (typically ~60s → configurable, default **8 seconds**).",
        "",
        "## Compatibility",
        "- **SPT 4.1.0** — [v1.1.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.1.0) (`FastTaxi-1.1.0.zip`)",
        "- **SPT 4.0.13** — [v1.0.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.0.0) (`FastTaxi-1.0.0.zip`)",
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
          version: "1.1.0",
          spt: "~4.1.0",
          download:
            "https://github.com/gadjed/FastTaxi-SPT-mod/releases/download/v1.1.0/FastTaxi-1.1.0.zip",
          virusTotal: vt(
            "4e8851d9e2fe491b41d6582594ac60d9ec5163f5e149851a5ac9c39f83e7f11c"
          ),
          description:
            "SPT **4.1.0** release.\n\n- Ported to SPT 4.1.0 (`IModMetadata`, `OnLoadAsync`, `LocationTable`, net10.0)\n- Archive: `FastTaxi-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          version: "1.0.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/FastTaxi-SPT-mod/releases/download/v1.0.0/FastTaxi-1.0.0.zip",
          virusTotal: vt(
            "d2e486ca25cbdd9083c41ec0b73f8318f66608226e2a8664204b45da99d921bc"
          ),
          description:
            "SPT **4.0.13** release.\n\nArchive: `FastTaxi-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
    {
      create: true,
      name: "Insurance Control",
      guid: "gadjed.insurancerefund",
      teaser:
        "SPT 4.1.0 / 4.0.13 — insurance return time, lost chance, magazine/container contents.",
      category: "Scripting",
      source: "https://github.com/gadjed/Insurance-refund-SPT-mod",
      profileSafe: false,
      description: [
        "Controls insurance return timing, lost chance, and whether magazines/containers return with contents.",
        "",
        "## Compatibility",
        "- **SPT 4.1.0** — [v1.1.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.1.0) (`InsuranceControl-1.1.0.zip`)",
        "- **SPT 4.0.13** — [v1.0.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.0.0) (`InsuranceControl-1.0.0.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "user/mods/InsuranceControl/",
        "```",
        "",
        "Config: `user/mods/InsuranceControl/config.json`.",
        "",
        "Avoid stacking with other insurance-return mods.",
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
          spt: "~4.1.0",
          download:
            "https://github.com/gadjed/Insurance-refund-SPT-mod/releases/download/v1.1.0/InsuranceControl-1.1.0.zip",
          virusTotal: vt(
            "44e6578fb7dde4c315296a125f35f265a764ca4732319aa38a3f104503d80b5c"
          ),
          description:
            "SPT **4.1.0** release.\n\n- Ported to SPT 4.1.0 (injectable configs/tables, Services.InRaid patch target)\n- Archive: `InsuranceControl-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          version: "1.0.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/Insurance-refund-SPT-mod/releases/download/v1.0.0/InsuranceControl-1.0.0.zip",
          virusTotal: vt(
            "94849dd67482b86b40176c9d4e4d708dcaa9200bf863e3406dac26b7411a00b4"
          ),
          description:
            "SPT **4.0.13** release.\n\nArchive: `InsuranceControl-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
    {
      create: true,
      name: "Quick Search",
      guid: "gadjed.quicksearch",
      teaser:
        "SPT 4.1.0 / 4.0.13 — search containers and corpses faster (default 3×).",
      category: "Scripting",
      source: "https://github.com/gadjed/Quick-search-SPT-mod",
      profileSafe: true,
      description: [
        "Speeds up searching containers and corpses by a configurable multiplier (default **3×**).",
        "",
        "## Compatibility",
        "- **SPT 4.1.0** — [v1.1.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.1.0) (`QuickSearch-1.1.0.zip`)",
        "- **SPT 4.0.13** — [v1.0.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.0.0) (`QuickSearch-1.0.0.zip`)",
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
          version: "1.1.0",
          spt: "~4.1.0",
          download:
            "https://github.com/gadjed/Quick-search-SPT-mod/releases/download/v1.1.0/QuickSearch-1.1.0.zip",
          virusTotal: vt(
            "ae58d9c0103d28692cff4c9771d4a4bde8555b45d3a416b26729c6383e4c08b2"
          ),
          description:
            "SPT **4.1.0** release.\n\n- IL-constant discovery of search state machines + legacy GClass fallback\n- Archive: `QuickSearch-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          version: "1.0.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/Quick-search-SPT-mod/releases/download/v1.0.0/QuickSearch-1.0.0.zip",
          virusTotal: vt(
            "67d64df7698d7fb681aec2a0b8da8784f7785caf9fd6c875d16a63ba2bf505dd"
          ),
          description:
            "SPT **4.0.13** release.\n\nArchive: `QuickSearch-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
    {
      create: true,
      name: "Yellow Flare Curse",
      guid: "gadjed.yellowflarecurse",
      teaser:
        "SPT 4.1.0 / 4.0.13 — yellow RSP-30 flare curse + delayed high-value airdrop.",
      category: "Overhauls",
      source: "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
      profileSafe: true,
      description: [
        "Using an RSP-30 Yellow (once per raid) curses nearby scavs/PMCs, then triggers a high-value airdrop after a delay.",
        "",
        "## Compatibility",
        "- **SPT 4.1.0** — [v1.1.0](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.1.0) (`YellowFlareCurse-1.1.0.zip`)",
        "- **SPT 4.0.13** — [v1.0.0](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.0.0) (`YellowFlareCurse-1.0.0.zip`)",
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
      versions: [
        {
          version: "1.1.0",
          spt: "~4.1.0",
          download:
            "https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/download/v1.1.0/YellowFlareCurse-1.1.0.zip",
          virusTotal: vt(
            "3f89f526aee85df1aeb94ceae4489659f524b50f8d54c2382f8f329828eb4627"
          ),
          description:
            "SPT **4.1.0** release.\n\n- Server airdrop config injection + client version bump\n- Archive: `YellowFlareCurse-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          version: "1.0.0",
          spt: "~4.0.13",
          download:
            "https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/download/v1.0.0/YellowFlareCurse-1.0.0.zip",
          virusTotal: vt(
            "44975c635008dc15a81d3669d2dd350b42ac4d285154cf01953e9bc34c0803fe"
          ),
          description:
            "SPT **4.0.13** release.\n\nArchive: `YellowFlareCurse-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.1.0).",
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
