using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.AddPlayer))]
    internal class AddPlayerPatch
    {
        internal static int Count = 0;

        private static void Prefix(ReferenceHub referenceHub)
        {
            if (referenceHub == null)
                return;

            if (Player.Dictionary.ContainsKey(referenceHub))
            {
                Count++;
                Logger.Warn($"{referenceHub.nicknameSync.MyNick} already in Dictionary");
                Player.Dictionary.Remove(referenceHub);
            }
        }
    }
}
