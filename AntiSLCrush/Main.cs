using HarmonyLib;
using LabApi.Features.Console;
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
        public override Version Version => new Version(2, 4, 4);
        public override Version RequiredApiVersion => new Version(1, 0, 2);

        internal static Main main;
        internal static Config config;
        private static Harmony harmony;

        public override void Enable()
        {
            main = this;
            config = Config;
            harmony = new Harmony(Name);

            harmony.PatchAll();
        }

        public override void Disable()
        {
            harmony.UnpatchAll();

            main = null;
            config = null;
            harmony = null;
        }

        internal static void BanHexAtSystemLevel(string hex, int port, string reason)
        {
            Logger.Warn($"{hex} will be banned with iptables. Reason: {reason}");

            int byteCount = hex.Length / 2;
            int totalLength = 20 + 8 + byteCount;

            if (hex.Length > 254)
                hex = hex.Substring(0, 254);

            string hexString = hex.ToLowerInvariant();

            string command = $"sudo iptables -A INPUT -p udp --dport {port} -m length --length {totalLength}:{totalLength} -m string --algo bm --hex-string \"|{hexString}|\" -j DROP";

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
                    Logger.Warn($"{hex} succesfully banned with IPTables! Reason: {reason}");
                    WebHook.Send($"{hex} succesfully banned with IPTables! Reason: {reason}");
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
