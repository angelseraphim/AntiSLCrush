using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.AddPlayer))]
    internal class AddPlayerPatch
    {
        private static void Prefix(ReferenceHub referenceHub)
        {
            if (Player.Dictionary.ContainsKey(referenceHub))
            {
                Logger.Warn($"{referenceHub.nicknameSync.MyNick} already in Dictionary");
                Player.Dictionary.Remove(referenceHub);
            }
        }
    }
}
