using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SS2;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2;

namespace AlternativeGoldScaling
{
    internal static class ModSupport
    {
        internal static class RiskOfOptionsMod
        {
            private static bool? _modexists;
            public static bool ModIsRunning
            {
                get
                {
                    if (_modexists == null)
                    {
                        _modexists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(RiskOfOptions.PluginInfo.PLUGIN_GUID);
                    }
                    return (bool)_modexists;
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void SetRiskOfOptionsDescription()
            {
                ModSettingsManager.SetModDescription("A standalone version of the Well-Rounded Balance mod's gold scaling, with some more values now configurable + some mod support.");
            }
        }

        internal static class Starstorm2
        {
            private static bool? _modexists;
            internal static bool ModIsRunning
            {
                get
                {
                    _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SS2Main.GUID);
                    return (bool)_modexists;
                }
            }
            private static bool? _isBetaVersion;
            internal static bool IsBetaVersion
            {
                get
                {
                    // turns out the old code for this did come back to bite me in the ass, so i made it better
                    _isBetaVersion ??= (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(SS2Main.GUID, out BepInEx.PluginInfo pluginInfo) && pluginInfo.Metadata.Version.ToString() == "0.6.16");
                    return (bool)_isBetaVersion;
                }
            }



            private enum EtherealEliteType
            {
                Ethereal,
                Ultra
            }
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static uint GetNewEtherealGoldReward(EtherealEliteType eliteType, uint goldReward)
            {
                EtherealBehavior etherealBehavior = EtherealBehavior.instance;
                float baseMultiplier = 0;
                float etherealsUsedMultiplier = 0;
                int nerfStartStage = 0;
                float perStageMultiplier = 0;


                switch (eliteType)
                {
                    case EtherealEliteType.Ethereal:
                        baseMultiplier = ConfigOptions.SS2Ethereal_NerfBaseMultiplier.Value;
                        etherealsUsedMultiplier = ConfigOptions.SS2Ethereal_EtherealsUsedMultiplier.Value;
                        nerfStartStage = ConfigOptions.SS2Ethereal_StageOfNerfStart.Value;
                        perStageMultiplier = ConfigOptions.SS2Ethereal_NerfPerStageMultiplier.Value;
                        break;
                    case EtherealEliteType.Ultra:
                        baseMultiplier = ConfigOptions.SS2Ultra_NerfBaseMultiplier.Value;
                        etherealsUsedMultiplier = ConfigOptions.SS2Ultra_EtherealsUsedMultiplier.Value;
                        nerfStartStage = ConfigOptions.SS2Ultra_StageOfNerfStart.Value;
                        perStageMultiplier = ConfigOptions.SS2Ultra_NerfPerStageMultiplier.Value;
                        break;
                }

                return goldReward *= Convert.ToUInt32(MathF.Max(1, (baseMultiplier + (etherealsUsedMultiplier * etherealBehavior.etherealsCompleted)) / (1 + (MathF.Max(0, (Main.CurrentStageNumber - nerfStartStage) * perStageMultiplier)))));
            }

            private static uint GetNewEmpyreanGoldReward(uint goldReward)
            {
                float baseMultiplier = ConfigOptions.SS2Empyrean_NerfBaseMultiplier.Value;
                int nerfStartStage = ConfigOptions.SS2Empyrean_StageOfNerfStart.Value;
                float perStageMultiplier = ConfigOptions.SS2Empyrean_NerfPerStageMultiplier.Value;

                return goldReward *= Convert.ToUInt32(MathF.Max(1, baseMultiplier / (1 + (MathF.Max(0, (Main.CurrentStageNumber - nerfStartStage) * perStageMultiplier)))));
            }

            [HarmonyPatch]
            internal class EmpyreanGoldReward
            {
                [HarmonyPatch(typeof(SS2.Components.CustomEliteDirector), "MakeEmpyrean")]
                [HarmonyILManipulator]
                internal static void Patch(ILContext il)
                {
                    ILCursor c = new(il);

                    if (!c.TryGotoNext(MoveType.AfterLabel,
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward")
                    ))
                    {
                        Log.Error($"COULD NOT IL HOOK {il.Method.Name}");
                        Log.Warning($"cursor is {c}");
                        Log.Warning($"il is {il}");
                        return;
                    }

                    c.Emit(OpCodes.Ldloc, 3);
                    c.EmitDelegate<Func<uint, DeathRewards, uint>>((oldGoldReward, deathRewards) =>
                    {
                        return GetNewEmpyreanGoldReward(deathRewards.goldReward);
                    });
                }
            }

            [HarmonyPatch]
            internal class EmpyreanGoldReward_Beta
            {
                [HarmonyPatch(typeof(SS2.Components.Empyrean), nameof(SS2.Components.Empyrean.MakeElite))]
                [HarmonyILManipulator]
                internal static void Patch(ILContext il)
                {
                    ILCursor c = new(il);

                    if (!c.TryGotoNext(MoveType.AfterLabel,
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward")
                    ))
                    {
                        Log.Error($"COULD NOT IL HOOK {il.Method.Name}");
                        Log.Warning($"cursor is {c}");
                        Log.Warning($"il is {il}");
                        return;
                    }

                    c.Emit(OpCodes.Ldloc, 3);
                    c.EmitDelegate<Func<uint, DeathRewards, uint>>((oldGoldReward, deathRewards) =>
                    {
                        return GetNewEmpyreanGoldReward(deathRewards.goldReward);
                    });
                }
            }

            [HarmonyPatch]
            internal class EtherealGoldReward
            {
                [HarmonyPatch(typeof(SS2.Components.Ethereal), nameof(SS2.Components.Ethereal.MakeElite))]
                [HarmonyILManipulator]
                internal static void Patch(ILContext il)
                {
                    ILCursor c = new(il);

                    if (!c.TryGotoNext(MoveType.Before,
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward")
                    ))
                    {
                        Log.Error($"COULD NOT IL HOOK {il.Method.Name}");
                        Log.Warning($"cursor is {c}");
                        Log.Warning($"il is {il}");
                        return;
                    }

                    c.Emit(OpCodes.Ldloc, 4);
                    c.EmitDelegate<Func<uint, DeathRewards, uint>>((oldGoldReward, deathRewards) =>
                    {
                        return GetNewEtherealGoldReward(EtherealEliteType.Ethereal, deathRewards.goldReward);
                    });
                }
            }

            [HarmonyPatch]
            internal class UltraGoldReward
            {
                [HarmonyPatch(typeof(SS2.Components.Ultra), nameof(SS2.Components.Ultra.MakeElite))]
                [HarmonyILManipulator]
                internal static void Patch(ILContext il)
                {
                    ILCursor c = new(il);

                    if (!c.TryGotoNext(MoveType.Before,
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward")
                    ))
                    {
                        Log.Error($"COULD NOT IL HOOK {il.Method.Name}");
                        Log.Warning($"cursor is {c}");
                        Log.Warning($"il is {il}");
                        return;
                    }

                    c.Emit(OpCodes.Ldloc, 4);
                    c.EmitDelegate<Func<uint, DeathRewards, uint>>((oldGoldReward, deathRewards) =>
                    {
                        return GetNewEtherealGoldReward(EtherealEliteType.Ultra, deathRewards.goldReward);
                    });
                }
            }
        }
    }
}
