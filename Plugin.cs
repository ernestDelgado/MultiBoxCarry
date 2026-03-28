using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace MultiBoxCarry
{
    [BepInPlugin("com.yaboie88.multiboxcarry", "Multi Box Carry", "1.0.1")]
    public class Plugin : BasePlugin
    {
        internal static new BepInEx.Logging.ManualLogSource Log;
        private Harmony _harmony;

        private static bool _initialized = false;

        public override void Load()
        {
            if (_initialized)
            {
                base.Log.LogInfo("MultiBoxCarry already initalized. Skipping Duplicate.");
                return;
            }
            _initialized = true;

            Log = base.Log;
            Log.LogInfo("Multi Box Carry loading...");

            _harmony = new Harmony("com.yaboie88.multiboxcarry");
            _harmony.PatchAll();

            ClassInjector.RegisterTypeInIl2Cpp<BoxInventoryHUD>();

            var hudObject = new GameObject("MultiBoxCarry_HUD");
            Object.DontDestroyOnLoad(hudObject);
            hudObject.AddComponent<BoxInventoryHUD>();

            Log.LogInfo("Multi Box Carry loaded.");
        }
    }
}