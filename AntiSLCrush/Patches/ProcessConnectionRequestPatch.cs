﻿using HarmonyLib;
using LabApi.Features.Console;
using LiteNetLib;
using System;

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

                if (filteretConnectionCount == 0)
                {
                    byte[] data = new byte[request.Data.AvailableBytes];
                    Buffer.BlockCopy(request.Data._data, request.Data._position, data, 0, request.Data.AvailableBytes);
                    string hex = BitConverter.ToString(data).Replace("-", "");
                    WebHook.Send($"Suspicious packet filtered! Please inform the plugin author about this data: {hex}");
                }

                filteretConnectionCount++;

                return false;
            }

            return true;
        }
    }
}