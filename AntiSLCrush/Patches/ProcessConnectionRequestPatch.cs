using HarmonyLib;
using LabApi.Features.Console;
using LiteNetLib;
using System;
using System.Collections.Generic;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal static class ProcessConnectionRequestPatch
    {
        internal static int FilteredConnectionCount = 0;
        internal static readonly Dictionary<string, string> IpToHex = new Dictionary<string, string>();
        internal static readonly Dictionary<string, int> MaxSeenHexCount = new Dictionary<string, int>();
        internal static readonly HashSet<string> SeenHex = new HashSet<string>(); // Зачем 3 листа? Так надо.

        private static bool Prefix(CustomLiteNetLib4MirrorTransport __instance, ConnectionRequest request)
        {
            string ip = request.RemoteEndPoint.Address.ToString();

            if (Main.BannedIp.Contains(ip))
                return false;

            byte[] data = new byte[request.Data.AvailableBytes];
            Buffer.BlockCopy(request.Data._data, request.Data._position, data, 0, request.Data.AvailableBytes);
            string hex = BitConverter.ToString(data).Replace("-", "");

            if (Main.BannedHEX.Contains(hex))
                return false;
            
            if (request.Data.AvailableBytes < 50)
            {
                if (Main.config.ShowSuspiciousPacketLogs)
                    Logger.Warn($"Too short handshake packet ({request.Data.AvailableBytes} bytes) from {ip} rejected as bot.");

                if (Main.config.BanHex)
                {
                    Main.BanHexAtSystemLevel(hex, "Too short handshake packet");
                    return false;
                }

                if (FilteredConnectionCount == 0)
                    WebHook.Send($"@everyone Suspicious packet filtered from {ip}! Please inform the plugin author about this data: {hex}");

                FilteredConnectionCount++;
                return false;
            }

            if (IpToHex.ContainsKey(ip))
            {
                if (MaxSeenHexCount.TryGetValue(ip, out int count))
                {
                    if (count > 20) //I'm too lazy to explain why this is necessary, but it is necessary.
                    {
                        if (Main.config.BanHex)
                            Main.BanHexAtSystemLevel(hex, "Too many HEX from same IP"); //Ну ладно, обьясню на русском. При входе игрок отправляет 2 раза разные HEX, и если одинаковых HEX много, то баним.
                        
                        if (Main.config.BanIp)
                            Main.BanIpAtSystemLevel(ip, "Too many HEX from same IP");

                        return false;
                    }

                    MaxSeenHexCount[ip]++;
                }
                else
                {
                    MaxSeenHexCount[ip] = 1;
                }

                return true;
            }

            if (SeenHex.Contains(hex))
            {
                if (FilteredConnectionCount == 0)
                {
                    WebHook.Send($"@everyone Suspicious packet filtered from {ip}! Please inform the plugin author about this data: {hex}");

                    if (Main.config.BanHex)
                        Main.BanHexAtSystemLevel(hex, "Same HEX from other IP");
                }

                FilteredConnectionCount++;
                return false;
            }

            IpToHex[ip] = hex;
            SeenHex.Add(hex);

            return true;
        }
    }
}