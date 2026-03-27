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

        public override void Load()
        {
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