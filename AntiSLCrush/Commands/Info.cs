using AntiSLCrush.Patches;
using CommandSystem;
using NorthwoodLib.Pools;
using System;
using System.Text;

namespace AntiSLCrush.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class Info : ICommand
    {
        public string Command { get; } = "anti_crush";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Security Information";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder builder = StringBuilderPool.Shared.Rent();

            builder.AppendLine("--- AntiSLCrush ---");
            builder.AppendLine($"Version: {Main.main.Version}");
            builder.AppendLine($"Player.AddPlayer method fix: {AddPlayerPatch.count}");
            builder.AppendLine($"NetworkServer.BroadcastToConnection: {BroadcastToConnectionPatch.NullConnectionCount}");
            builder.AppendLine($"NetworkServer.OnTransportData: {OnTransportDataPath.unknownCount}");
            builder.AppendLine($"PlayerRoleManager.PopulateDummyActions: {PopulateDummyActionsPatch.nullCount}");
            builder.AppendLine($"AntiSLBots filtered connection count: {ProcessConnectionRequestPatch.filteredConnectionCount}");

            response = builder.ToString();
            StringBuilderPool.Shared.Return(builder);
            return true;
        }
    }
}
