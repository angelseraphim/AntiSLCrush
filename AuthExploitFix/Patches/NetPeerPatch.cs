using HarmonyLib;
using LiteNetLib;
using NorthwoodLib.Pools;
using System.Collections.Generic;
using System.Reflection.Emit;
using static HarmonyLib.AccessTools;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetPeer), nameof(NetPeer.ProcessPacket))]

    internal static class NetPeerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 5;
            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(NetPacket), nameof(NetPacket.ChannelId)))) + offset;

            newInstructions[index].opcode = OpCodes.Blt;

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
