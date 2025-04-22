using System;
using BepInEx;
using HarmonyLib;
using MonoMod.Cil;
using RoR2;

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
        public const string PluginVersion = "1.0.0";
        public void Awake()
        {
            Log.Init(Logger);
            ConfigOptions.BindConfigOptions(Config);
            if (ModSupport.RiskOfOptionsMod.ModIsRunning)
            {
                ModSupport.RiskOfOptionsMod.AddOptions();
            }


            if (ConfigOptions.EnableWRBGoldScaling.Value)
            {
                On.RoR2.DeathRewards.OnKilledServer += Main.Hooks.DeathRewards_OnKilledServer;
            }
            if (ConfigOptions.EnableWRBMultiplayerCostScaling.Value)
            {
                IL.RoR2.Run.GetDifficultyScaledCost_int_float += Main.Hooks.Run_GetDifficultyScaledCost_int_float;
            }


            if (ConfigOptions.EnableSS2Support.Value && ModSupport.Starstorm2.ModIsRunning)
            {
                Harmony harmony = new(PluginGUID);


                if (ConfigOptions.SS2Empyrean_EnableChange.Value)
                {
                    Log.Info("Patching SS2's empyrean gold reward...");
                    if (ModSupport.Starstorm2.IsBetaVersion)
                    {
                        Log.Warning("Version number detected is the same as the beta's version! If this still errors out, either this mod is broken or you may have to update your version of Starstorm 2!");
                        harmony.CreateClassProcessor(typeof(ModSupport.Starstorm2.EmpyreanGoldReward_Beta)).Patch();
                    }
                    else
                    {
                        harmony.CreateClassProcessor(typeof(ModSupport.Starstorm2.EmpyreanGoldReward)).Patch();
                    }
                }


                if (ConfigOptions.SS2Ethereal_EnableChange.Value)
                {
                    Log.Info("Patching SS2 beta's ethereal gold reward...");
                    harmony.CreateClassProcessor(typeof(ModSupport.Starstorm2.EtherealGoldReward)).Patch();
                }


                if (ConfigOptions.SS2Ultra_EnableChange.Value)
                {
                    Log.Info("Patching SS2 beta's ultra gold reward...");
                    harmony.CreateClassProcessor(typeof(ModSupport.Starstorm2.UltraGoldReward)).Patch();
                }
            }
        }
    }
}