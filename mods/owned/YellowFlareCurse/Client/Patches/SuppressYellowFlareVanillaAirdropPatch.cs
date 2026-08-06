using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace YellowFlareCurse.Client.Patches;

/// <summary>
/// Yellow RSP-30 normally schedules a vanilla flare airdrop via
/// <c>ClientGameWorld.method_21</c> → <c>FlareSuccessEventHandler(AirDropTemplateId)</c>.
/// That template is often unmapped in SPT → random WEAPON / COMMON («общей поддержки»).
/// When the curse owns the drop, suppress that vanilla path entirely.
/// </summary>
public class SuppressYellowFlareVanillaAirdropPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ClientGameWorld).GetMethod(
            "method_21",
            BindingFlags.Public | BindingFlags.Instance
        )!;
    }

    [PatchPrefix]
    public static bool PatchPrefix(AmmoTemplate ammoTemplate)
    {
        if (!YellowFlareCursePlugin.Enabled.Value)
        {
            return true;
        }

        // AmmoTemplate._id is MongoID — compare via string like FlareSuccessPatch.
        var tpl = ammoTemplate?._id.ToString() ?? string.Empty;
        if (tpl != YellowFlareCursePlugin.YellowFlareTemplateId)
        {
            return true;
        }

        ModLogger.Info(
            "Suppressing vanilla yellow-flare airdrop (AirDropTemplateId path) — "
                + "curse delayed airdrop owns the crate."
        );
        return false;
    }
}
