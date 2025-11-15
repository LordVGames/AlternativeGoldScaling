using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.DetourTypes;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AlternativeGoldScaling
{
    internal static class MainChanges
    {
        internal static int CurrentStageNumber
        {
            get
            {
                return Run.instance.stageClearCount + 1;
            }
        }

        // how ominous
        [MonoDetourTargets(typeof(DeathRewards))]
        [MonoDetourTargets(typeof(Run))]
        internal static class Hooks
        {
            [MonoDetourHookInitialize]
            internal static void Setup()
            {
                if (ConfigOptions.EnableWRBGoldScaling.Value)
                {
                    MonoDetourHooks.RoR2.DeathRewards.OnKilledServer.Prefix(SetWRBGoldReward);
                    MonoDetourHooks.RoR2.Run.GetDifficultyScaledCost_System_Int32_System_Single.ILHook(SetNewMultiplayerCostScaling);
                }
            }


            private static void SetWRBGoldReward(DeathRewards self, ref DamageReport damageReport)
            {
                Log.BeforeAndAfter($"Before WRB Gold Reward: {self.goldReward}");
                self.goldReward = GetWRBGoldReward(self.goldReward);
                Log.BeforeAndAfter($"After WRB Gold Reward: {self.goldReward}");
            }
            internal static uint GetWRBGoldReward(uint goldReward)
            {
                return Convert.ToUInt32(Mathf.Min(goldReward * ConfigOptions.BaseGoldScalingMultiplier.Value, ConfigOptions.BaseGoldScalingMultiplier.Value * ((goldReward / (ConfigOptions.StageDivisor.Value + (Run.instance.stageClearCount * ConfigOptions.StageClearCountMultiplier.Value))) + Mathf.Sqrt(ConfigOptions.SquareRootMultiplier.Value * (ConfigOptions.StageAndLoopMultiplier.Value + (Run.instance.stageClearCount * ConfigOptions.StageMultiplier.Value + Run.instance.loopClearCount * ConfigOptions.LoopMultiplier.Value))))));
            }


            private static void SetNewMultiplayerCostScaling(ILManipulationInfo info)
            {
                ILWeaver w = new(info);

                w.MatchRelaxed(
                    x => x.MatchLdcR4(1.25f) && w.SetCurrentTo(x)
                ).ThrowIfFailure();
                w.InsertAfterCurrent(
                    w.CreateDelegateCall(
                        (float originalScaling) =>
                        {
                            int players = Run.instance.participatingPlayerCount;
                            float newScaling = ConfigOptions.BaseMultiplayerCostMultiplier.Value + (ConfigOptions.PerPlayerCostMultiplier.Value / Mathf.Sqrt(players));
                            return players <= 1 ? originalScaling : newScaling;
                            // for testing
                            //return newScaling;
                        }
                    )
                );
            }
        }
    }
}