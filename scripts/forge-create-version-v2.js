async (page) => {
  // One-shot helper: create a Forge version for Med Rebalance on SPT 4.0.13.
  const args = {
    modId: 2869,
    version: "1.1.0",
    sptConstraint: "~4.0.13",
    download:
      "https://github.com/gadjed/MedRebalance-SPT-mod/releases/download/v1.3.0/MedRebalance-1.3.0.zip",
    description:
      "SPT **4.0.13** release.\n\n- Archive: `MedRebalance-1.3.0.zip`",
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

  const nowBtn = page.getByRole("button", { name: "Now", exact: true });
  if ((await nowBtn.count()) > 0) {
    await nowBtn.click();
    await page.waitForTimeout(300);
  }

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
