using HarmonyLib;
using LiteNetLib;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(ConnectionRequest), nameof(ConnectionRequest.Reject), new[] { typeof(byte[]), typeof(int), typeof(int), typeof(bool) })]
    internal static class RejectPatch
    {
        private static void Prefix(ConnectionRequest __instance, byte[] rejectData, int start, int length, bool force)
        {
            if (!(rejectData[0] is byte code))
                return;

            if (code == 12 || code == 13)
                return;

            string ip = __instance.RemoteEndPoint.Address.ToString();

            if (ProcessConnectionRequestPatch.IpToHex.TryGetValue(ip, out string hex))
            {
                if (ProcessConnectionRequestPatch.SeenHex.Contains(hex))
                    ProcessConnectionRequestPatch.SeenHex.Remove(hex);

                ProcessConnectionRequestPatch.IpToHex.Remove(ip);
            }

            if (ProcessConnectionRequestPatch.MaxSeenHexCount.ContainsKey(ip))
                ProcessConnectionRequestPatch.MaxSeenHexCount.Remove(ip);
        }
    }
}
