using HarmonyLib;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using System;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.PopulateDummyActions))]
    internal class PopulateDummyActionsPatch
    {
        internal static int nullCount = 0;

        private static bool Prefix(PlayerRoleManager __instance, Action<DummyAction> actionAdder, Action<string> categoryAdder)
        {
            IRootDummyActionProvider[] dummyProviders = __instance._dummyProviders;

            if (__instance._dummyProviders == null)
                return false;

            for (int i = 0; i < dummyProviders.Length; i++)
            {
                if (dummyProviders[i] == null)
                {
                    nullCount++;
                    continue;
                }

                dummyProviders[i].PopulateDummyActions(actionAdder, categoryAdder);
            }

            __instance.DummyActionsDirty = false;
            return false;
        }
    }
}
