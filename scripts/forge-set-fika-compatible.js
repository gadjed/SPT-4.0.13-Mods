async (page) => {
  // All owned mods except Yellow Flare Curse
  const modIds = [2869, 2870, 2871, 2872];
  const report = [];

  for (const modId of modIds) {
    await page.goto("https://forge.sp-tarkov.com/mod/" + modId, {
      waitUntil: "domcontentloaded",
    });
    await page.waitForTimeout(900);

    // Collect version IDs from wire:click attributes
    const versionIds = await page.evaluate(() => {
      const html = document.documentElement.innerHTML;
      const ids = new Set();
      for (const m of html.matchAll(/deleteModVersion\\((\\d+)/g)) {
        ids.add(Number(m[1]));
      }
      for (const m of html.matchAll(/mod-version-action-[a-z]+-(\\d+)/g)) {
        ids.add(Number(m[1]));
      }
      for (const m of html.matchAll(/version-download-(\\d+)/g)) {
        ids.add(Number(m[1]));
      }
      return [...ids].sort((a, b) => a - b);
    });

    const updated = [];
    for (const versionId of versionIds) {
      await page.goto(
        "https://forge.sp-tarkov.com/mod/" +
          modId +
          "/version/" +
          versionId +
          "/edit",
        { waitUntil: "networkidle" }
      );
      await page.waitForTimeout(800);

      const fika = page
        .getByRole("combobox")
        .filter({ hasText: /Compatible|Incompatible|Unknown/i })
        .first();
      await fika.click();
      await page.waitForTimeout(250);
      await page.getByRole("option", { name: "Compatible", exact: true }).click();
      await page.waitForTimeout(300);

      await page.getByRole("button", { name: "Save Changes", exact: true }).click();
      await page.waitForTimeout(3000);

      const main = await page.locator("main").innerText().catch(() => "");
      updated.push({
        versionId,
        url: page.url(),
        ok: !page.url().includes("/edit") || /saved|updated|success/i.test(main),
        fikaLine: (main.match(/Fika[^\n]*/i) || [null])[0],
      });
    }

    report.push({ modId, versionIds, updated });
  }

  return report;
}
