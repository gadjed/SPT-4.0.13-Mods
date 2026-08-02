async (page) => {
  const args = {
    modId: 2869,
    version: "1.2.0",
    sptConstraint: "~4.1.0",
    download:
      "https://github.com/gadjed/MedRebalance-SPT-mod/releases/download/v1.3.0/MedRebalance-1.3.0.zip",
    virusTotal:
      "https://www.virustotal.com/gui/file/3b4152eac95eb849aa3569eef94ab3e07461998be43f37514f853407b024061d",
    description:
      "SPT **4.1.0** release.\n\n- Ported to SPT 4.1.0 (`IModMetadata`, `OnLoadAsync`, `TemplateTable`, net10.0)\n- Archive: `MedRebalance-1.3.0.zip`\n\nAlso available for SPT 4.0.13: [v1.1.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.1.0)",
  };

  await page.goto(
    "https://forge.sp-tarkov.com/mod/" + args.modId + "/version/create",
    { waitUntil: "networkidle" }
  );
  await page.waitForTimeout(800);

  await page.locator('input[name="version"]').fill(args.version);
  await page.locator('textarea[name="description"]').fill(args.description);
  await page.locator('input[name="link"]').fill(args.download);
  await page
    .locator('input[name="sptVersionConstraint"]')
    .fill(args.sptConstraint);
  await page.waitForTimeout(800);

  await page.locator('input[name="virusTotalLinks.0.url"]').fill(args.virusTotal);
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

  const main = await page.locator("main").innerText().catch(() => "");
  return {
    url: page.url(),
    title: await page.title(),
    ok: !page.url().includes("/version/create"),
    snippet: main.slice(0, 2000),
  };
}
