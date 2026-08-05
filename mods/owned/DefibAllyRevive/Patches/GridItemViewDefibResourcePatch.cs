using System.Reflection;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DefibAllyRevive.Patches;

/// <summary>
/// Inventory grid can also show "0/0" for the vanilla defibrillator resource field.
/// </summary>
internal sealed class GridItemViewDefibResourcePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.UpdateInfo));
    }

    [PatchPostfix]
    private static void Postfix(GridItemView __instance)
    {
        if (__instance?.Item == null || !AllyUtil.IsDefibrillator(__instance.Item))
        {
            return;
        }

        try
        {
            foreach (var name in new[] { "_resourceValue", "ResourceValue", "_itemValue", "itemValue" })
            {
                var field = AccessTools.Field(__instance.GetType(), name)
                    ?? AccessTools.Field(typeof(GridItemView), name)
                    ?? AccessTools.Field(typeof(ItemView), name);
                if (field == null)
                {
                    continue;
                }

                var label = field.GetValue(__instance);
                if (label == null)
                {
                    continue;
                }

                var textProp = label.GetType().GetProperty("text");
                if (textProp == null || textProp.PropertyType != typeof(string))
                {
                    continue;
                }

                var current = textProp.GetValue(label) as string;
                if (string.IsNullOrEmpty(current))
                {
                    continue;
                }

                if (current.Contains("0/0") || current.Trim() == "0")
                {
                    textProp.SetValue(label, "1");
                    DefibAllyRevivePlugin.DebugLog($"Grid resource label '{current}' -> '1'");
                }

                return;
            }
        }
        catch
        {
            // Quick-slot patch remains the primary fix.
        }
    }
}
