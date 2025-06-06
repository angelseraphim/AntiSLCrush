﻿using HarmonyLib;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetworkServer), nameof(NetworkServer.BroadcastToConnection))]
    internal class BroadcastToConnectionPatch
    {
        internal static int NullConnectionCount = 0;

        private static bool Prefix(NetworkConnectionToClient connection)
        {
            if (connection == null)
            {
                Logger.Error("Connection is null in BroadcastToConnectionPatch.");
                return false;
            }

            connection.observing.RemoveWhere(item =>
            {
                if (item == null || item.gameObject == null)
                {
                    Debug.LogWarning($"Removing null entry in observing list for connectionId={connection.connectionId}.");
                    NullConnectionCount++;

                    if (NullConnectionCount > 10)
                    {
                        Logger.Error("Too many nulls in NetworkConnectionToClient. Restarting server.");
                        Server.Restart();
                        return true;
                    }

                    return true;
                }

                return false;
            });

            foreach (NetworkIdentity item in connection.observing)
            {
                NetworkWriter networkWriter = NetworkServer.SerializeForConnection(item, connection);
                if (networkWriter != null)
                {
                    EntityStateMessage message = new EntityStateMessage
                    {
                        netId = item.netId,
                        payload = networkWriter.ToArraySegment()
                    };

                    connection.Send(message);
                }
            }

            return false;
        }
    }
}
