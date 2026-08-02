async (page) => {
  // Fill via page URL containing mod id, or pass args below
  const args = {
    modId: 2869,
    version: "1.2.0",
    sptConstraint: "~4.1.0",
    download:
      "https://github.com/gadjed/MedRebalance-SPT-mod/releases/download/v1.3.0/MedRebalance-1.3.0.zip",
    description:
      "SPT **4.1.0** release.\n\n- Ported to SPT 4.1.0 (`IModMetadata`, `OnLoadAsync`, `TemplateTable`, net10.0)\n- Direct download: MedRebalance-1.3.0.zip\n\nOlder SPT 4.0.13 build: [v1.1.0](https://github.com/gadjed/MedRebalance-SPT-mod/releases/tag/v1.1.0)",
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

  // Publish now if button exists
  const nowBtn = page.getByRole("button", { name: "Now", exact: true });
  if ((await nowBtn.count()) > 0) {
    await nowBtn.click();
    await page.waitForTimeout(300);
  }

  // Prefer Compatible Unknown / leave Fika as-is unless needed
  const submit = page
    .getByRole("button", { name: /Create Version|Create Mod Version|Submit/i })
    .first();
  await submit.click();
  await page.waitForTimeout(3500);

  const main = await page.locator("main").innerText().catch(() => "");
  return {
    url: page.url(),
    title: await page.title(),
    ok: !page.url().includes("/version/create"),
    snippet: main.slice(0, 1800),
  };
}
