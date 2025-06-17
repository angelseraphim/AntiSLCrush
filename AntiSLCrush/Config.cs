namespace AntiSLCrush
{
    public class Config
    {
        public string DiscordWebHook { get; set; } = string.Empty;
        public bool ShowAddPlayerCheckLogs { get; set; } = true;
        public bool ShowBroadcastToConnectionLogs { get; set; } = true;
        public bool ShowSuspiciousPacketLogs { get; set; } = true;
        public string AuthHexHeader { get; set; } = "050d00000083bdbf7b41bd8848000000001070a60fe75253c58d0000000000000000";
        public bool BanHex { get; set; } = true;
        public bool BanIp { get; set; } = true;
    }
}
