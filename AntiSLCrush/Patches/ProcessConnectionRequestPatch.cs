using HarmonyLib;
using LabApi.Features.Console;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal class ProcessConnectionRequestPatch
    {
        private static readonly HashSet<IPAddress> ipBlackList = new HashSet<IPAddress>();

        public static bool Prefix(CustomLiteNetLib4MirrorTransport __instance, ConnectionRequest request)
        {
            IPAddress ip = request.RemoteEndPoint.Address;

            if (ipBlackList.Contains(ip))
            {
                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)12);
                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                Logger.Warn($"AntiSLCrush: Banned ip {ip} rejected as bot.");
                return false;
            }

            int rawBytes = request.Data.AvailableBytes;

            byte[] array = new byte[rawBytes];
            Buffer.BlockCopy(request.Data._data, request.Data._position, array, 0, request.Data.AvailableBytes);

            if (array.Length < 50)
            {
                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)12);
                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                ipBlackList.Add(ip);
                Logger.Warn($"AntiSLCrush: Too short handshake packet ({array.Length} bytes) from {ip} rejected as bot.");
                return false;
            }

            return true;
        }
    }
}
