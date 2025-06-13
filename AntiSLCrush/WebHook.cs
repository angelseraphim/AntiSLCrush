using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System.Net.Http;
using System.Text;
using Utf8Json;

namespace AntiSLCrush
{
    internal static class WebHook
    {
        internal static async void Send(string message)
        {
            if (string.IsNullOrEmpty(Main.config.DiscordWebHook))
            {
                Logger.Error("Webhook url is empty");
                return;
            }

            using (HttpClient httpClient = new HttpClient())
            {
                var payload = new
                {
                    embeds = new[]
                    {
                        new
                        {
                            color = 0x00ff00,
                            title = Server.IpAddress + ':' + Server.Port,
                            description = message,
                        }
                    }
                };

                byte[] jsonBytes = JsonSerializer.Serialize(payload);
                string jsonPayload = Encoding.UTF8.GetString(jsonBytes);
                StringContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(Main.config.DiscordWebHook, httpContent);
            }
        }
    }
}
