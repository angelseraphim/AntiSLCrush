using HarmonyLib;
using LabApi.Features.Console;
using LiteNetLib;
using System;
using System.Collections.Generic;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal class ProcessConnectionRequestPatch
    {
        internal static int filteretConnectionCount = 0;
        internal static readonly HashSet<string> bannedIp = new HashSet<string>();

        private static bool Prefix(CustomLiteNetLib4MirrorTransport __instance, ConnectionRequest request)
        {
            string ip = request.RemoteEndPoint.Address.ToString();

            if (bannedIp.Contains(ip))
            {
                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)12);
                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);

                if (Main.config.ShowBannedIpLogs)
                    Logger.Warn($"Banned IP {ip} rejected as bot");

                return false;
            }

            //byte[] array = new byte[request.Data.AvailableBytes];
            //Buffer.BlockCopy(request.Data._data, request.Data._position, array, 0, request.Data.AvailableBytes);

            if (request.Data.AvailableBytes < 50)
            {
                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)12);
                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);

                if (Main.config.BanIp)
                    bannedIp.Add(ip);

                if (Main.config.ShowSuspiciousPacketLogs)
                    Logger.Warn($"Too short handshake packet ({request.Data.AvailableBytes} bytes) from {ip} rejected as bot.");

                filteretConnectionCount++;

                return false;
            }

            return true;
        }
    }
}
