using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using System;
using System.Runtime.InteropServices;

namespace AntiSLCrush
{
    public class Main : Plugin<Config>
    {
        public override string Name => "AntiSLCrush";
        public override string Author => "angelseraphim.";
        public override string Description => "AntiSLCrush";
        public override Version Version => new Version(2, 5, 1);
        public override Version RequiredApiVersion => new Version(1, 0, 2);

        internal static Version PluginVersion;

        internal static Config config;
        private static Harmony harmony;

        public override void Enable()
        {
            config = Config;
            PluginVersion = Version;
            harmony = new Harmony(Name);

            harmony.PatchAll();
        }

        public override void Disable()
        {
            harmony.UnpatchAll();

            config = null;
            harmony = null;
        }

        internal static void BanIpAtSystemLevel(string ip, string reason)
        {
            Logger.Warn($"{ip} will be banned with iptables. Reason: {reason}");

            string command = $"iptables -A INPUT -s {ip} -j DROP";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "sudo";
                proc.StartInfo.Arguments = command;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();

                if (string.IsNullOrEmpty(error))
                {
                    Logger.Warn($"{ip} succesfully banned with IPTables! Reason: {reason}");
                    WebHook.Send($"{ip} succesfully banned with IPTables! Reason: {reason}");
                }
                else
                {
                    Logger.Warn($"Error while HEX banning. stdout: {output}, stderr: {error}");
                    WebHook.Send($"Error while HEX banning. stdout: {output}, stderr: {error}");
                }
            }
            else
            {
                Logger.Warn($"Not linux: {RuntimeInformation.OSDescription}");
            }
        }

        internal static void BanHexAtSystemLevel(string hex, string reason)
        {
            Logger.Warn($"{hex} will be banned with iptables. Reason: {reason}");

            int byteCount = hex.Length / 2;
            int totalLength = 20 + 8 + byteCount;

            if (hex.Length > 254)
                hex = hex.Substring(0, 254);

            string hexString = hex.ToLowerInvariant();

            string command = $"sudo iptables -A INPUT -p udp --dport {Server.Port} -m length --length {totalLength}:{totalLength} -m string --algo bm --hex-string \"|{hexString}|\" -j DROP";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "sudo";
                proc.StartInfo.Arguments = command;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();

                if (string.IsNullOrEmpty(error))
                {
                    Logger.Warn($"{hex} succesfully banned with IPTables! Length: {totalLength} Reason: {reason}");
                    WebHook.Send($"{hex} succesfully banned with IPTables! Length: {totalLength} Reason: {reason}");
                }
                else
                {
                    Logger.Warn($"Error while HEX banning. stdout: {output}, stderr: {error}");
                    WebHook.Send($"Error while HEX banning. stdout: {output}, stderr: {error}");
                }
            }
            else
            {
                Logger.Warn($"Not linux: {RuntimeInformation.OSDescription}");
            }
        }
    }
}
