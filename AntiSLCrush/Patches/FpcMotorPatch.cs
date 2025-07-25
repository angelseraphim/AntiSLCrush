using HarmonyLib;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdatePosition))]
    internal static class FpcMotorPatch
    {
        private static bool Prefix(FpcMotor __instance, ref bool sendJump)
        {
            if (__instance.MainModule.CharController == null)
            {
                Main.Log($"[FpcMotorPatch] {__instance.Hub?.nicknameSync?.MyNick ?? "NULL"} does not have CharController");

                __instance.MoveDirection = Vector3.zero;
                sendJump = false;
                return false;
            }

            return true;
        }
    }
}
