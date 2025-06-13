namespace AntiSLCrush
{
    public class Config
    {
        public string DiscordWebHook { get; set; } = string.Empty;
        public bool ShowAddPlayerCheckLogs { get; set; } = true;
        public bool ShowBroadcastToConnectionLogs { get; set; } = true;
        public bool ShowSuspiciousPacketLogs { get; set; } = true;
        public bool BanHex { get; set; } = true;   
    }
}
