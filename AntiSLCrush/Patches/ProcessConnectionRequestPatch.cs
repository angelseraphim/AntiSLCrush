using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LiteNetLib;
using System;
using System.Collections.Generic;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal class ProcessConnectionRequestPatch
    {
        internal static int filteredConnectionCount = 0;
        internal static readonly Dictionary<string, string> IpToHex = new Dictionary<string, string>();
        internal static readonly Dictionary<string, int> MaxSeenHexCount = new Dictionary<string, int>();
        internal static readonly HashSet<string> SeenHex = new HashSet<string>(); // Зачем 3 листа? Так надо.

        private static bool Prefix(CustomLiteNetLib4MirrorTransport __instance, ConnectionRequest request)
        {
            byte[] data = new byte[request.Data.AvailableBytes];
            Buffer.BlockCopy(request.Data._data, request.Data._position, data, 0, request.Data.AvailableBytes);
            string hex = BitConverter.ToString(data).Replace("-", "");
            string ip = request.RemoteEndPoint.Address.ToString();

            if (request.Data.AvailableBytes < 50)
            {
                if (Main.config.ShowSuspiciousPacketLogs)
                    Logger.Warn($"Too short handshake packet ({request.Data.AvailableBytes} bytes) from {ip} rejected as bot.");

                if (Main.config.BanHex)
                {
                    Main.BanHexAtSystemLevel(hex, Server.Port, "Too short handshake packet");
                    return false;
                }

                if (filteredConnectionCount == 0)
                {
                    WebHook.Send($"@everyone Suspicious packet filtered from {ip}! Please inform the plugin author about this data: {hex}");
                }

                filteredConnectionCount++;
                return false;
            }

            if (IpToHex.TryGetValue(ip, out var oldHex))
            {
                if (oldHex != hex)
                {
                    IpToHex.Remove(ip);
                    SeenHex.Remove(oldHex);

                    if (MaxSeenHexCount.ContainsKey(ip))
                        MaxSeenHexCount.Remove(ip);

                    return true;
                }

                if (MaxSeenHexCount.TryGetValue(ip, out int count))
                {
                    if (count < 5) //I'm too lazy to explain why this is necessary, but it is necessary.
                    {
                        Main.BanHexAtSystemLevel(hex, Server.Port, "Same HEX from same IP"); //Ну ладно, обьясню на русском. При входе игрок отправляет 2 раза разные HEX, и если одинаковых HEX много, то баним.
                        return false;
                    }
                }

                if (!MaxSeenHexCount.ContainsKey(ip))
                    MaxSeenHexCount[ip] = 1;
                else
                    MaxSeenHexCount[ip]++;

                return true;
            }

            if (SeenHex.Contains(hex))
            {
                if (filteredConnectionCount == 0)
                {
                    WebHook.Send($"@everyone Suspicious packet filtered from {ip}! Please inform the plugin author about this data: {hex}");

                    if (Main.config.BanHex)
                    {
                        Main.BanHexAtSystemLevel(hex, Server.Port, "Same HEX from other IP");
                    }
                }

                filteredConnectionCount++;
                return false;
            }

            IpToHex[ip] = hex;
            SeenHex.Add(hex);

            return true;
        }
    }
}