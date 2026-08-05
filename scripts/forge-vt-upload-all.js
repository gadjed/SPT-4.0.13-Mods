async (page) => {
  const dir = "/Users/alexsukhykh/projects/SPT mods/scripts/vt-uploads";
  const files = [
    "MedRebalance-1.3.0.zip",
    "FastTaxi-1.0.0.zip",
    "InsuranceControl-1.0.1.zip",
    "QuickSearch-1.0.0.zip",
    "InsureAllPrapor-1.0.3.zip",
    "YellowFlareCurse-1.4.5.zip",
  ];
  const results = {};
  for (const file of files) {
    const path = dir + "/" + file;
    await page.goto("https://www.virustotal.com/gui/home/upload", {
      waitUntil: "domcontentloaded",
    });
    await page.waitForTimeout(1500);
    const [chooser] = await Promise.all([
      page.waitForEvent("filechooser", { timeout: 25000 }),
      page.getByRole("button", { name: "Choose file" }).click(),
    ]);
    await chooser.setFiles(path);
    await page.waitForTimeout(2500);
    const confirm = page.getByRole("button", { name: /Confirm upload/i });
    if (await confirm.count()) {
      await confirm.first().click().catch(() => {});
    }
    let hashUrl = null;
    for (let t = 0; t < 90; t++) {
      await page.waitForTimeout(2000);
      const m = page.url().match(/\/gui\/file\/([a-f0-9]{64})/i);
      if (m) {
        hashUrl = "https://www.virustotal.com/gui/file/" + m[1];
        break;
      }
    }
    results[file] = hashUrl || page.url();
  }
  return results;
}
