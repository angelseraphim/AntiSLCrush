using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AntiSLCrush
{
    public class Main : Plugin<Config>
    {
        public override string Name => "AntiSLCrush";
        public override string Author => "angelseraphim.";
        public override string Description => "AntiSLCrush";
        public override Version Version => new Version(2, 3, 2);
        public override Version RequiredApiVersion => new Version(1, 0, 2);

        internal static Config config;
        private static Harmony harmony;

        public override void Enable()
        {
            config = new Config();
            harmony = new Harmony(Name);

            harmony.PatchAll();
        }

        public override void Disable()
        {
            harmony.UnpatchAll();

            config = null;
            harmony = null;
        }
    }
}
