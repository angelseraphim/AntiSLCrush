using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms.Ammo;
using PlayerRoles.Spectating;
using Utils.Networking;
using static InventorySystem.Items.Firearms.Ammo.ReserveAmmoSync;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(ReserveAmmoSync), nameof(ReserveAmmoSync.UpdateDelta))]
    internal static class ReserveAmmoSyncUpdateDeltaPatch
    {
        private static bool Prefix()
        {
            foreach (AutosyncItem instance in AutosyncItem.Instances)
            {
                if (!TryUnpack(instance, out ReferenceHub owner, out ItemType ammoType))
                    continue;

                if (owner == null)
                {
                    Main.Log("[ReserveAmmoSyncUpdateDeltaPatch] Null");
                    continue;
                }

                int curAmmo = owner.inventory.GetCurAmmo(ammoType);
                LastSent orAdd = ServerLastSent.GetOrAdd(owner, () => new LastSent());
                if (orAdd.AmmoCount != curAmmo || orAdd.AmmoType != ammoType)
                {
                    orAdd.AmmoType = ammoType;
                    orAdd.AmmoCount = curAmmo;
                    new ReserveAmmoMessage(owner, ammoType).SendToHubsConditionally((ReferenceHub x) => x.roleManager.CurrentRole is SpectatorRole);
                }
            }

            return false;
        }
    }
}
