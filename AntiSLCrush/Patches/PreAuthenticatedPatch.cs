using HarmonyLib;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(PlayerEvents), nameof(PlayerEvents.OnPreAuthenticated))]
    internal static class PreAuthenticatedPatch
    {
        private static void Prefix(PlayerPreAuthenticatedEventArgs ev)
        {
            if (ProcessConnectionRequestPatch.IpToHex.TryGetValue(ev.IpAddress, out string hex))
            {
                if (ProcessConnectionRequestPatch.SeenHex.Contains(hex))
                    ProcessConnectionRequestPatch.SeenHex.Remove(hex);

                ProcessConnectionRequestPatch.IpToHex.Remove(ev.IpAddress);
            }

            if (ProcessConnectionRequestPatch.MaxSeenHexCount.ContainsKey(ev.IpAddress))
                ProcessConnectionRequestPatch.MaxSeenHexCount.Remove(ev.IpAddress);
        }
    }
}
