async (page) => {
  const vt = (hash) => "https://www.virustotal.com/gui/file/" + hash;

  // SHA256 of repacked archives
  const hashes = {
    "MedRebalance-1.3.0.zip":
      "fdb5dd0a7289e656bf628ac303fe1cd42582e0efcff5bd67c60b76cd8d8723c5",
    "MedRebalance-1.3.0.zip":
      "54045ed639ef55ecfe44a8bbc8cfdbb5a819f4475b2f253f0b678685015585cf",
    "FastTaxi-1.1.0.zip":
      "d30e7894090178ce24e7039b9c55b022dfd61260fb9d0672914906d2b6b3048b",
    "FastTaxi-1.0.0.zip":
      "76f4f797bddee215a195bb1bc85ed45fc4b9940ae3b24804ae6ebd8c3a2a88e8",
    "InsuranceControl-1.1.0.zip":
      "e807b566a89a3da83265bf3206c2a4d583efd2defff54f75661da2a73bec149c",
    "InsuranceControl-1.0.0.zip":
      "a34191ea26ea1f5281d22e820e8487f7285407e33132dcfaa0fc528c1814b349",
    "QuickSearch-1.1.0.zip":
      "216d41164cf99717c1c21ed63abd18a765067fa3535d69908ab5ab9df4ada5a8",
    "QuickSearch-1.0.0.zip":
      "fa8ac67e29d7db779c6bf359f19f9e98406b586b5d79701c19c9938240e4cbab",
    "YellowFlareCurse-1.1.0.zip":
      "ef2609a47d88dd65c476632dcaeeb6043b44bc3b4a9b944e47913d0c6d9b554d",
    "YellowFlareCurse-1.0.0.zip":
      "26da4db2ca794dd87642af59d90a29cf350f5d4958defe2dde1ebb0ae0f38ebf",
  };

  const mods = [
    {
      modId: 2869,
      slug: "med-rebalance",
      source: "https://github.com/gadjed/MedRebalance-SPT-mod",
      description: [
        "**Medicine rebalance** (formerly Fast Surgery): shorter surgical/splint use time (default **5s**), continuous multi-limb healing, scratch top-ups, cancel-on-damage.",
        "",
        "## Compatibility",
        "- **SPT 4.1.0** — [v1.2.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.2.0) (`MedRebalance-1.3.0.zip`)",
        "- **SPT 4.0.13** — [v1.1.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.1.0) (`MedRebalance-1.3.0.zip`)",
        "",
        "## Install",
        "Extract into your SPT game root:",
        "```",
        "SPT/user/mods/MedRebalance/",
        "```",
        "",
        "Config: `SPT/user/mods/MedRebalance/config.json` (`UseTimeSeconds`, per-item toggles).",
        "",
        "Includes continuous multi-limb healing, scratch top-ups, and cancel-on-damage (client plugin). Remove standalone Continuous Healing.",
        "",
        "## Source",
        "https://github.com/gadjed/MedRebalance-SPT-mod",
      ].join("\n"),
      versions: [
        {
          versionId: 14349,
          version: "1.2.0",
          download:
            "https://github.com/gadjed/MedRebalance-SPT-mod/releases/download/v1.3.0/MedRebalance-1.3.0.zip",
          virusTotal: vt(hashes["MedRebalance-1.3.0.zip"]),
          description:
            "SPT **4.1.0** release.\n\n- Correct Forge package layout: `SPT/user/mods/MedRebalance/`\n- Archive: `MedRebalance-1.3.0.zip`\n\nFor SPT 4.0.13 use [v1.1.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.1.0).",
        },
        {
          versionId: 14350,
          version: "1.1.0",
          download:
            "https://github.com/gadjed/MedRebalance-SPT-mod/releases/download/v1.3.0/MedRebalance-1.3.0.zip",
          virusTotal: vt(hashes["MedRebalance-1.3.0.zip"]),
          description:
            "SPT **4.0.13** release.\n\n- Correct Forge package layout: `SPT/user/mods/MedRebalance/`\n- Archive: `MedRebalance-1.3.0.zip`\n\nFor SPT 4.1.0 use [v1.2.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.2.0).",
        },
      ],
    },
    {
      modId: 2870,
      slug: "fast-taxi",
      source: "https://github.com/gadjed/FastTaxi-SPT-mod",
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
        "SPT/user/mods/FastTaxi/",
        "```",
        "",
        "Config: `SPT/user/mods/FastTaxi/config.json` (`WaitTimeSeconds`).",
        "",
        "## Source",
        "https://github.com/gadjed/FastTaxi-SPT-mod",
      ].join("\n"),
      versions: [
        {
          versionId: 14351,
          version: "1.1.0",
          download:
            "https://github.com/gadjed/FastTaxi-SPT-mod/releases/download/v1.1.0/FastTaxi-1.1.0.zip",
          virusTotal: vt(hashes["FastTaxi-1.1.0.zip"]),
          description:
            "SPT **4.1.0** release.\n\n- Correct Forge package layout: `SPT/user/mods/FastTaxi/`\n- Archive: `FastTaxi-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          versionId: 14352,
          version: "1.0.0",
          download:
            "https://github.com/gadjed/FastTaxi-SPT-mod/releases/download/v1.0.0/FastTaxi-1.0.0.zip",
          virusTotal: vt(hashes["FastTaxi-1.0.0.zip"]),
          description:
            "SPT **4.0.13** release.\n\n- Correct Forge package layout: `SPT/user/mods/FastTaxi/`\n- Archive: `FastTaxi-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/FastTaxi-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
    {
      modId: 2871,
      slug: "insurance-control",
      source: "https://github.com/gadjed/Insurance-refund-SPT-mod",
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
        "SPT/user/mods/InsuranceControl/",
        "```",
        "",
        "Config: `SPT/user/mods/InsuranceControl/config.json`.",
        "",
        "Avoid stacking with other insurance-return mods.",
        "",
        "## Source",
        "https://github.com/gadjed/Insurance-refund-SPT-mod",
      ].join("\n"),
      versions: [
        {
          versionId: 14353,
          version: "1.1.0",
          download:
            "https://github.com/gadjed/Insurance-refund-SPT-mod/releases/download/v1.1.0/InsuranceControl-1.1.0.zip",
          virusTotal: vt(hashes["InsuranceControl-1.1.0.zip"]),
          description:
            "SPT **4.1.0** release.\n\n- Correct Forge package layout: `SPT/user/mods/InsuranceControl/`\n- Archive: `InsuranceControl-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          versionId: 14354,
          version: "1.0.0",
          download:
            "https://github.com/gadjed/Insurance-refund-SPT-mod/releases/download/v1.0.0/InsuranceControl-1.0.0.zip",
          virusTotal: vt(hashes["InsuranceControl-1.0.0.zip"]),
          description:
            "SPT **4.0.13** release.\n\n- Correct Forge package layout: `SPT/user/mods/InsuranceControl/`\n- Archive: `InsuranceControl-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
    {
      modId: 2872,
      slug: "quick-search",
      source: "https://github.com/gadjed/Quick-search-SPT-mod",
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
        "## Source",
        "https://github.com/gadjed/Quick-search-SPT-mod",
      ].join("\n"),
      versions: [
        {
          versionId: 14355,
          version: "1.1.0",
          download:
            "https://github.com/gadjed/Quick-search-SPT-mod/releases/download/v1.1.0/QuickSearch-1.1.0.zip",
          virusTotal: vt(hashes["QuickSearch-1.1.0.zip"]),
          description:
            "SPT **4.1.0** release.\n\n- Correct Forge package layout: `BepInEx/plugins/QuickSearch.dll`\n- Archive: `QuickSearch-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          versionId: 14356,
          version: "1.0.0",
          download:
            "https://github.com/gadjed/Quick-search-SPT-mod/releases/download/v1.0.0/QuickSearch-1.0.0.zip",
          virusTotal: vt(hashes["QuickSearch-1.0.0.zip"]),
          description:
            "SPT **4.0.13** release.\n\n- Correct Forge package layout: `BepInEx/plugins/QuickSearch.dll`\n- Archive: `QuickSearch-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/Quick-search-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
    {
      modId: 2873,
      slug: "yellow-flare-curse",
      source: "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
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
        "SPT/user/mods/YellowFlareCurse/",
        "BepInEx/plugins/YellowFlareCurse.Client.dll",
        "```",
        "",
        "Server config: `SPT/user/mods/YellowFlareCurse/config.json`. Client settings in F12.",
        "",
        "Needs maps with airdrop points. SAIN optional for harder cursed fights.",
        "",
        "## Source",
        "https://github.com/gadjed/Yellow-flare-curse-SPT-mod",
      ].join("\n"),
      versions: [
        {
          versionId: 14357,
          version: "1.1.0",
          download:
            "https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/download/v1.1.0/YellowFlareCurse-1.1.0.zip",
          virusTotal: vt(hashes["YellowFlareCurse-1.1.0.zip"]),
          description:
            "SPT **4.1.0** release.\n\n- Correct Forge package layout: `SPT/user/mods/...` + `BepInEx/plugins/...`\n- Archive: `YellowFlareCurse-1.1.0.zip`\n\nFor SPT 4.0.13 use [v1.0.0](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.0.0).",
        },
        {
          versionId: 14358,
          version: "1.0.0",
          download:
            "https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/download/v1.0.0/YellowFlareCurse-1.0.0.zip",
          virusTotal: vt(hashes["YellowFlareCurse-1.0.0.zip"]),
          description:
            "SPT **4.0.13** release.\n\n- Correct Forge package layout: `SPT/user/mods/...` + `BepInEx/plugins/...`\n- Archive: `YellowFlareCurse-1.0.0.zip`\n\nFor SPT 4.1.0 use [v1.1.0](https://github.com/gadjed/Yellow-flare-curse-SPT-mod/releases/tag/v1.1.0).",
        },
      ],
    },
  ];

  const report = { mods: [], versions: [] };

  async function saveWithWait() {
    // Honeypot: wait before save
    await page.waitForTimeout(2500);
    const save = page.getByRole("button", { name: /Save Changes|Save|Update/i }).first();
    await save.click();
    await page.waitForTimeout(3500);
  }

  for (const mod of mods) {
    const editUrl = `https://forge.sp-tarkov.com/mod/${mod.modId}/edit`;
    await page.goto(editUrl, { waitUntil: "networkidle" });
    await page.waitForTimeout(1200);

    if ((await page.title()).includes("Not Found") || page.url().includes("/login")) {
      report.mods.push({
        modId: mod.modId,
        ok: false,
        error: "edit unavailable",
        url: page.url(),
        title: await page.title(),
      });
      continue;
    }

    await page.locator('textarea[name="description"]').fill(mod.description);

    // Source links: keep only the mod repo (remove mod-pack / installer links)
    const urlInputs = page.locator('input[type="url"]');
    const labels = page.getByPlaceholder("Label (optional)");
    const n = await urlInputs.count();
    let sourceSlot = -1;
    for (let i = 0; i < n; i++) {
      const val = await urlInputs.nth(i).inputValue();
      if (val === mod.source || /github\.com\/gadjed\/(?!SPT-4\.0\.13-Mods)/i.test(val)) {
        sourceSlot = i;
        break;
      }
    }
    if (sourceSlot < 0) sourceSlot = 0;

    for (let i = 0; i < n; i++) {
      if (i === sourceSlot) {
        await urlInputs.nth(i).fill(mod.source);
        if ((await labels.count()) > i) await labels.nth(i).fill("Source");
      } else {
        await urlInputs.nth(i).fill("");
        if ((await labels.count()) > i) await labels.nth(i).fill("");
      }
    }

    await saveWithWait();
    report.mods.push({
      modId: mod.modId,
      ok: !page.url().includes("/edit") || /saved|updated|success/i.test(
        (await page.locator("body").innerText()).slice(0, 500)
      ),
      url: page.url(),
      title: await page.title(),
    });
  }

  for (const mod of mods) {
    for (const ver of mod.versions) {
      const editUrl = `https://forge.sp-tarkov.com/mod/${mod.modId}/version/${ver.versionId}/edit`;
      await page.goto(editUrl, { waitUntil: "networkidle" });
      await page.waitForTimeout(1200);

      if ((await page.title()).includes("Not Found") || page.url().includes("/login")) {
        report.versions.push({
          ...ver,
          modId: mod.modId,
          ok: false,
          error: "edit unavailable",
          url: page.url(),
        });
        continue;
      }

      await page.locator('textarea[name="description"]').fill(ver.description);
      await page.locator('input[name="link"]').fill(ver.download);
      await page.locator('input[name="virusTotalLinks.0.url"]').fill(ver.virusTotal);
      await page
        .locator('input[name="virusTotalLinks.0.label"]')
        .fill("Release archive");

      await saveWithWait();
      report.versions.push({
        modId: mod.modId,
        versionId: ver.versionId,
        version: ver.version,
        ok: true,
        url: page.url(),
        title: await page.title(),
        vt: ver.virusTotal,
      });
    }
  }

  return report;
}
