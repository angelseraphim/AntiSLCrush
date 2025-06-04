using HarmonyLib;
using LabApi.Features.Console;
using LiteNetLib;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal class ProcessConnectionRequestPatch
    {
        internal static int filteretConnectionCount = 0;

        private static bool Prefix(CustomLiteNetLib4MirrorTransport __instance, ConnectionRequest request)
        {
            if (request.Data.AvailableBytes < 50)
            {
                if (Main.config.ShowSuspiciousPacketLogs)
                    Logger.Warn($"Too short handshake packet ({request.Data.AvailableBytes} bytes) from {request.RemoteEndPoint.Address.ToString()} rejected as bot.");

                filteretConnectionCount++;

                return false;
            }

            return true;
        }
    }
}