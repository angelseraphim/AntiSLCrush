using HarmonyLib;
using Mirror;
using System;
using UnityEngine;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetworkServer), nameof(NetworkServer.OnTransportData))]
    internal class OnTransportDataPath
    {
        private static bool Prefix(int connectionId, ArraySegment<byte> data, int channelId)
        {
            if (!NetworkServer.connections.TryGetValue(connectionId, out var connection))
            {
                Debug.LogError($"[PATCH] HandleData Unknown connectionId:{connectionId}, disconnecting.");

                if (Transport.active != null)
                {
                    Transport.active.ServerDisconnect(connectionId);
                }
                else
                {
                    Debug.LogError("[PATCH] No active transport found. Cannot disconnect.");
                }

                return false;
            }

            return true;
        }
    }
}
