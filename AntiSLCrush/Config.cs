using System.ComponentModel;

namespace AntiSLCrush
{
    public class Config
    {
        public bool Debug { get; set; } = true;

        [Description("Maximum number of NULLs in NetworkServer.BroadcastToConnection after which the server will restart. (0 for disable)")]
        public int MaxNullBroadcastCount { get; set; } = 10;

        [Description("Anti SL bots")]
        public string DiscordWebHook { get; set; } = string.Empty;
        public bool ShowSuspiciousPacketLogs { get; set; } = true;
        public bool BanHex { get; set; } = true;
        public bool BanIp { get; set; } = true;
    }
}
