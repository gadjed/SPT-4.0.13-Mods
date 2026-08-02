async (page) => {
  // All versions for Med Rebalance (formerly Fast Surgery), Fast Taxi, Insurance Control, Quick Search
  // Yellow Flare Curse intentionally left as Unknown
  const targets = [
    { modId: 2869, versionId: 14349, version: "1.2.0" },
    { modId: 2869, versionId: 14350, version: "1.1.0" },
    { modId: 2870, versionId: 14351, version: "1.1.0" },
    { modId: 2870, versionId: 14352, version: "1.0.0" },
    { modId: 2871, versionId: 14353, version: "1.1.0" },
    { modId: 2871, versionId: 14354, version: "1.0.0" },
    { modId: 2872, versionId: 14355, version: "1.1.0" },
    { modId: 2872, versionId: 14356, version: "1.0.0" },
  ];

  const report = [];

  for (const t of targets) {
    const editUrl =
      "https://forge.sp-tarkov.com/mod/" +
      t.modId +
      "/version/" +
      t.versionId +
      "/edit";
    await page.goto(editUrl, { waitUntil: "networkidle" });
    await page.waitForTimeout(900);

    if ((await page.title()).includes("Not Found")) {
      report.push({ ...t, ok: false, error: "edit page 404" });
      continue;
    }

    const fika = page
      .getByRole("combobox")
      .filter({ hasText: /Compatible|Incompatible|Unknown/i })
      .first();
    await fika.click();
    await page.waitForTimeout(300);
    await page.getByRole("option", { name: "Compatible", exact: true }).click();
    await page.waitForTimeout(300);

    await page.getByRole("button", { name: "Save Changes", exact: true }).click();
    await page.waitForTimeout(3000);

    report.push({
      ...t,
      url: page.url(),
      ok: !page.url().includes("/edit"),
      title: await page.title(),
    });
  }

  return report;
}
