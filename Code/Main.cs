using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace AlternativeGoldScaling
{
    internal static class Main
    {
        internal static int CurrentStageNumber
        {
            get
            {
                return Run.instance.stageClearCount + 1;
            }
        }

        internal static uint GetWRBGoldReward(uint goldReward)
        {
            return Convert.ToUInt32(Mathf.Min(goldReward * ConfigOptions.BaseGoldScalingMultiplier.Value, ConfigOptions.BaseGoldScalingMultiplier.Value * ((goldReward / (ConfigOptions.StageDivisor.Value + (Run.instance.stageClearCount * ConfigOptions.StageClearCountMultiplier.Value))) + Mathf.Sqrt(ConfigOptions.SquareRootMultiplier.Value * (ConfigOptions.StageAndLoopMultiplier.Value + (Run.instance.stageClearCount * ConfigOptions.StageMultiplier.Value + Run.instance.loopClearCount * ConfigOptions.LoopMultiplier.Value))))));
        }

        internal static class Hooks
        {
            internal static void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
            {
                self.goldReward = GetWRBGoldReward(self.goldReward);
                orig(self, damageReport);
            }

            internal static void Run_GetDifficultyScaledCost_int_float(ILContext il)
            {
                ILCursor c = new(il);

                if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1.25f)))
                {
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>((orig) =>
                    {
                        int players = Run.instance.participatingPlayerCount;
                        float newScaling = ConfigOptions.BaseMultiplayerCostMultiplier.Value + (ConfigOptions.PerPlayerCostMultiplier.Value / Mathf.Sqrt(players));
                        return players <= 1 ? orig : newScaling;
                        // for testing
                        //return newScaling;
                    });
                }
                else
                {
                    Log.Error($"COULD NOT IL HOOK {il.Method.Name}");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                }
            }
        }

    }
}