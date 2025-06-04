using System.ComponentModel;

namespace AntiSLCrush
{
    public class Config
    {
        public bool ShowAddPlayerCheckLogs { get; set; } = true;
        public bool ShowBroadcastToConnectionLogs { get; set; } = true;

        [Description("Anti DDoS (SL bots method) config")]
        public bool ShowSuspiciousPacketLogs { get; set; } = true;
        public bool ShowBannedIpLogs { get; set; } = true;
        public bool BanIp { get; set; } = true;
    }
}
