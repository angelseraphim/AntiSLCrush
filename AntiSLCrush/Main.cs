using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using System;

namespace AntiSLCrush
{
    public class Main : Plugin
    {
        public override string Name => "AntiSLCrush";
        public override string Author => "angelseraphim.";
        public override string Description => "AntiSLCrush";
        public override Version Version => new Version(2, 3, 2);
        public override Version RequiredApiVersion => new Version(1, 0, 2);

        private static Harmony harmony;

        public override void Enable()
        {
            harmony = new Harmony(Name);
            harmony.PatchAll();
        }

        public override void Disable()
        {
            harmony.UnpatchAll();
            harmony = null;
        }
    }
}
