using HarmonyLib;
using LabApi.Features.Wrappers;
using Mirror;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetworkServer), nameof(NetworkServer.BroadcastToConnection))]
    internal static class BroadcastToConnectionPatch
    {
        internal static int NullConnectionCount = 0;

        private static bool Prefix(NetworkConnectionToClient connection)
        {
            if (connection == null)
            {
                Main.Log("[BroadcastToConnectionPatch] Connection is null in BroadcastToConnectionPatch.", true);
                return false;
            }

            connection.observing.RemoveWhere(item =>
            {
                if (item == null || item.gameObject == null)
                {
                    Main.Log($"[BroadcastToConnectionPatch] Removing null entry in observing list for connectionId={connection.connectionId}.");

                    if (Main.config.MaxNullBroadcastCount > 0)
                    {
                        NullConnectionCount++;

                        if (NullConnectionCount > Main.config.MaxNullBroadcastCount)
                        {
                            Main.Log("[BroadcastToConnectionPatch] Too many nulls in NetworkConnectionToClient. Restarting server.", true);
                            Server.Restart();
                        }
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
