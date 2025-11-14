using HarmonyLib;
using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.DetourTypes;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using SS2;
using SS2.Components;
using SS2.Items;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

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
                    _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(RiskOfOptions.PluginInfo.PLUGIN_GUID);
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
            [MonoDetourTargets(typeof(Empyrean))]
            private static class EmpyreanGoldReward
            {
                [MonoDetourHookInitialize]
                internal static void Setup()
                {
                    if (!ConfigOptions.SS2Empyrean_EnableChange.Value)
                    {
                        return;
                    }

                    MonoDetourHooks.SS2.Components.Empyrean.MakeElite.ILHook(ReplaceEmpyreanGoldReward);
                }


                private static void ReplaceEmpyreanGoldReward(ILManipulationInfo info)
                {
                    ILWeaver w = new(info);

                    w.MatchRelaxed(
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward") && w.SetCurrentTo(x)
                    ).ThrowIfFailure();
                    w.InsertBeforeCurrent(
                        w.Create(OpCodes.Ldloc_3), // DeathRewards
                        w.CreateCall(SetNewEmpyreanGoldReward)
                    );
                }
                private static uint SetNewEmpyreanGoldReward(uint oldGoldReward, DeathRewards deathRewards)
                {
                    float baseMultiplier = ConfigOptions.SS2Empyrean_NerfBaseMultiplier.Value;
                    int nerfStartStage = ConfigOptions.SS2Empyrean_StageOfNerfStart.Value;
                    float perStageMultiplier = ConfigOptions.SS2Empyrean_NerfPerStageMultiplier.Value;

                    return deathRewards.goldReward *= Convert.ToUInt32(MathF.Max(1, baseMultiplier / (1 + (MathF.Max(0, (MainChanges.CurrentStageNumber - nerfStartStage) * perStageMultiplier)))));
                }
            }


            [MonoDetourTargets(typeof(Ethereal))]
            private static class EtherealGoldReward
            {
                [MonoDetourHookInitialize]
                internal static void Setup()
                {
                    if (!ConfigOptions.SS2Ethereal_EnableChange.Value)
                    {
                        return;
                    }

                    MonoDetourHooks.SS2.Components.Ethereal.MakeElite.ILHook(ReplaceEtherealGoldReward);
                }


                private static void ReplaceEtherealGoldReward(ILManipulationInfo info)
                {
                    ILWeaver w = new(info);

                    w.MatchRelaxed(
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward") && w.SetCurrentTo(x)
                    ).ThrowIfFailure();
                    w.InsertBeforeCurrent(
                        w.Create(OpCodes.Ldloc_3), // DeathRewards
                        w.CreateCall(SetNewEtherealGoldReward)
                    );
                }
                private static uint SetNewEtherealGoldReward(uint oldGoldReward, DeathRewards deathRewards)
                {
                    EtherealBehavior etherealBehavior = EtherealBehavior.instance;
                    float baseMultiplier = ConfigOptions.SS2Ethereal_NerfBaseMultiplier.Value;
                    float etherealsUsedMultiplier = ConfigOptions.SS2Ethereal_EtherealsUsedMultiplier.Value;
                    int nerfStartStage = ConfigOptions.SS2Ethereal_StageOfNerfStart.Value;
                    float perStageMultiplier = ConfigOptions.SS2Ethereal_NerfPerStageMultiplier.Value;

                    return deathRewards.goldReward *= Convert.ToUInt32(MathF.Max(1, (baseMultiplier + (etherealsUsedMultiplier * etherealBehavior.etherealsCompleted)) / (1 + (MathF.Max(0, (MainChanges.CurrentStageNumber - nerfStartStage) * perStageMultiplier)))));
                }
            }


            [MonoDetourTargets(typeof(Ultra))]
            private static class UltraGoldReward
            {
                [MonoDetourHookInitialize]
                internal static void Setup()
                {
                    if (!ConfigOptions.SS2Ultra_EnableChange.Value)
                    {
                        return;
                    }

                    MonoDetourHooks.SS2.Components.Ultra.MakeElite.ILHook(ReplaceUltraGoldReward);
                }


                private static void ReplaceUltraGoldReward(ILManipulationInfo info)
                {
                    ILWeaver w = new(info);

                    w.MatchRelaxed(
                        x => x.MatchCallvirt<DeathRewards>("set_goldReward") && w.SetCurrentTo(x)
                    ).ThrowIfFailure();
                    w.InsertBeforeCurrent(
                        w.Create(OpCodes.Ldloc_3), // DeathRewards
                        w.CreateCall(GetNewUltraGoldReward)
                    );
                }
                private static uint GetNewUltraGoldReward(uint oldGoldReward, DeathRewards deathRewards)
                {
                    EtherealBehavior etherealBehavior = EtherealBehavior.instance;
                    float baseMultiplier = ConfigOptions.SS2Ultra_NerfBaseMultiplier.Value;
                    float etherealsUsedMultiplier = ConfigOptions.SS2Ultra_EtherealsUsedMultiplier.Value;
                    int nerfStartStage = ConfigOptions.SS2Ultra_StageOfNerfStart.Value;
                    float perStageMultiplier = ConfigOptions.SS2Ultra_NerfPerStageMultiplier.Value;

                    return deathRewards.goldReward *= Convert.ToUInt32(MathF.Max(1, (baseMultiplier + (etherealsUsedMultiplier * etherealBehavior.etherealsCompleted)) / (1 + (MathF.Max(0, (MainChanges.CurrentStageNumber - nerfStartStage) * perStageMultiplier)))));
                }
            }
        }
    }
}