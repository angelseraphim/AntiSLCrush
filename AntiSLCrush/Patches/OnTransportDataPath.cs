using HarmonyLib;
using Mirror;
using System;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetworkServer), nameof(NetworkServer.OnTransportData))]
    internal static class OnTransportDataPath
    {
        private static bool Prefix(int connectionId, ArraySegment<byte> data, int channelId)
        {
            if (!NetworkServer.connections.TryGetValue(connectionId, out var connection))
            {
                Main.Log($"[OnTransportDataPath] HandleData Unknown connectionId:{connectionId}, disconnecting.");

                if (Transport.active != null)
                {
                    Transport.active.ServerDisconnect(connectionId);
                }
                else
                {
                    Main.Log("[OnTransportDataPath] No active transport found. Cannot disconnect.");
                }

                return false;
            }

            return true;
        }
    }
}
