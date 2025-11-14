using BepInEx;
using HarmonyLib;
using MonoDetour;
using RoR2;
using System;

namespace AlternativeGoldScaling
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(RiskOfOptions.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(SS2.SS2Main.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "LordVGames";
        public const string PluginName = "AlternativeGoldScaling";
        public const string PluginVersion = "1.0.3";
        public void Awake()
        {
            Log.Init(Logger);
            ConfigOptions.BindConfigOptions(Config);
            MonoDetourManager.InvokeHookInitializers(typeof(Plugin).Assembly);
        }
    }
}