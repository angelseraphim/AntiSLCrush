using System.Linq;
using HarmonyLib;
using LiteNetLib;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetPacket), nameof(NetPacket.Verify))]
    internal class NetPacketPatch
    {
        private static bool Prefix(ref bool __result, NetPacket __instance)
        {
            if (!__instance.RawData.Any())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
