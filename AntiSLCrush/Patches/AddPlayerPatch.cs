using HarmonyLib;
using LabApi.Features.Wrappers;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.AddPlayer))]
    internal static class AddPlayerPatch
    {
        private static void Prefix(ReferenceHub referenceHub)
        {
            if (referenceHub == null)
                return;

            if (Player.Dictionary.ContainsKey(referenceHub))
            {
                Main.Log($"[AddPlayerPatch] {referenceHub.nicknameSync.MyNick} already in Dictionary");
                Player.Dictionary.Remove(referenceHub);
            }
        }
    }
}
