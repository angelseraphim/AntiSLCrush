using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using System;

namespace AntiSLCrush
{
    public class Main : Plugin<Config>
    {
        public override string Name => "AntiSLCrush";
        public override string Author => "angelseraphim.";
        public override string Description => "AntiSLCrush";
        public override Version Version => new Version(2, 4, 4);
        public override Version RequiredApiVersion => new Version(1, 0, 2);

        internal static Main main;
        internal static Config config;
        private static Harmony harmony;

        public override void Enable()
        {
            main = this;
            config = Config;
            harmony = new Harmony(Name);

            harmony.PatchAll();
        }

        public override void Disable()
        {
            harmony.UnpatchAll();

            main = null;
            config = null;
            harmony = null;
        }
    }
}
