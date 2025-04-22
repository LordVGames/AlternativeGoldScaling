using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;

namespace AlternativeGoldScaling
{
    public static class ConfigOptions
    {
        #region WRB Gold Reward Options
        private const string _wrbGoldRewardSectionName = "WRB - Enemy gold reward scaling";
        public static ConfigEntry<bool> EnableWRBGoldScaling;

        public static ConfigEntry<float> BaseGoldScalingMultiplier;
        public static ConfigEntry<float> LoopMultiplier;
        public static ConfigEntry<float> StageDivisor;
        public static ConfigEntry<float> StageClearCountMultiplier;
        public static ConfigEntry<float> StageMultiplier;
        public static ConfigEntry<float> SquareRootMultiplier;
        public static ConfigEntry<float> StageAndLoopMultiplier;
        #endregion

        #region WRB Multiplayer Scaled Cost Options
        private const string _wrbMultiplayerCostSectionName = "WRB - Multiplayer cost scaling";
        public static ConfigEntry<bool> EnableWRBMultiplayerCostScaling;

        public static ConfigEntry<float> BaseMultiplayerCostMultiplier;
        public static ConfigEntry<float> PerPlayerCostMultiplier;
        #endregion

        #region SS2 Support
        private const string _ss2EntireSectionName = "Starstorm 2 - Late-game elites support";
        public static ConfigEntry<bool> EnableSS2Support;

        private const string _ss2EmpyreanSectionName = "Starstorm 2 - Empyrean Gold Reward";
        public static ConfigEntry<bool> SS2Empyrean_EnableChange;
        public static ConfigEntry<float> SS2Empyrean_NerfBaseMultiplier;
        public static ConfigEntry<int> SS2Empyrean_StageOfNerfStart;
        public static ConfigEntry<float> SS2Empyrean_NerfPerStageMultiplier;

        private const string _ss2EtherealSectionName = "Starstorm 2 - Ethereal Gold Reward";
        public static ConfigEntry<bool> SS2Ethereal_EnableChange;
        public static ConfigEntry<float> SS2Ethereal_NerfBaseMultiplier;
        public static ConfigEntry<float> SS2Ethereal_EtherealsUsedMultiplier;
        public static ConfigEntry<int> SS2Ethereal_StageOfNerfStart;
        public static ConfigEntry<float> SS2Ethereal_NerfPerStageMultiplier;

        private const string _ss2UltraSectionName = "Starstorm 2 - Ultra Gold Reward";
        public static ConfigEntry<bool> SS2Ultra_EnableChange;
        public static ConfigEntry<float> SS2Ultra_NerfBaseMultiplier;
        public static ConfigEntry<float> SS2Ultra_EtherealsUsedMultiplier;
        public static ConfigEntry<int> SS2Ultra_StageOfNerfStart;
        public static ConfigEntry<float> SS2Ultra_NerfPerStageMultiplier;
        #endregion

        internal static void BindConfigOptions(ConfigFile config)
        {
            #region WRB Gold Reward Options
            EnableWRBGoldScaling = config.Bind<bool>(
                _wrbGoldRewardSectionName,
                "Enable", true,
                "Enable using the formula from Well-Rounded Balance (WRB) for scaling gold from enemy kills?\n\nFormula for gold reward: Minimum value between Vanilla Gold Reward * Base Multiplier and Base Multiplier * ((Vanilla Gold Reward / (Stage Divisor + (Stage Clear Count * Stage Clear Count Multiplier))) + Square Root(Square Root Multiplier * (Stage And Loop Multiplier + (Stage Clear Count * Stage Multiplier + Loop Clear Count * Loop Multiplier))))"
            );

            BaseGoldScalingMultiplier = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Base Multiplier", 1f,
                "WRB's default value is 0.75"
            );
            LoopMultiplier = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Loop Multiplier", -700f,
                "WRB's default value is unchanged"
            );
            StageDivisor = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Stage Divisor", 2f,
                "WRB's default value is 3"
            );
            StageClearCountMultiplier = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Stage Clear Count Multiplier", 0.5f,
                "WRB's default value is 0.25"
            );
            StageMultiplier = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Stage Multiplier", 150f,
                "WRB's default value is unchanged"
            );
            SquareRootMultiplier = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Square Root Multiplier", 6f,
                "WRB's default value is unchanged"
            );
            StageAndLoopMultiplier = config.Bind<float>(
                _wrbGoldRewardSectionName,
                "Stage and Loop Multiplier", 275f,
                "WRB's default value is unchanged"
            );
            #endregion

            #region WRB Multiplayer Scaled Cost Options
            EnableWRBMultiplayerCostScaling = config.Bind<bool>(
                _wrbMultiplayerCostSectionName,
                "Enable", true,
                "Enable using the formula from Well-Rounded Balance (WRB) for scaling the cost of buyable things in multiplayer?\n\nFormula for cost scaling: Base Multiplier + (Per-Player Multiplier / Square Root(Player Count))"
            );

            BaseMultiplayerCostMultiplier = config.Bind<float>(
                _wrbMultiplayerCostSectionName,
                "Base Multiplier", 1f,
                "WRB's default value is unchanged"
            );
            PerPlayerCostMultiplier = config.Bind<float>(
                _wrbMultiplayerCostSectionName,
                "Per-Player cost multiplier", 0.25f,
                "WRB's default value is unchanged"
            );
            #endregion

            #region SS2 Support
            EnableSS2Support = config.Bind<bool>(
                _ss2EntireSectionName,
                "Enable", true,
                "Enable special support for nerfing the gold reward from some late-game elites from Starstorm 2? The gold reward nerf for these elites are ON TOP OF the existing gold nerf from WRB if it's enabled.\n\nFormula for gold reward: Highest value between 1 and (Base Multiplier + IF APPLICABLE (Ethereal Multiplier * Ethereals Used Count)) / (1 + (Highest value between 0 and ((Stage Clear Count + 1) - Starting Stage of Nerf) * Per-Stage Multiplier))"
            );



            SS2Empyrean_EnableChange = config.Bind<bool>(
                _ss2EmpyreanSectionName,
                "Enable Empyrean Gold Reward Change", true,
                ""
            );
            SS2Empyrean_NerfBaseMultiplier = config.Bind<float>(
                _ss2EmpyreanSectionName,
                "Empyrean - Base Multiplier", 15f,
                ""
            );
            SS2Empyrean_StageOfNerfStart = config.Bind<int>(
                _ss2EmpyreanSectionName,
                "Empyrean - Starting stage of nerf", 11,
                ""
            );
            SS2Empyrean_NerfPerStageMultiplier = config.Bind<float>(
                _ss2EmpyreanSectionName,
                "Empyrean - Per-Stage multiplier", 0.5f,
                ""
            );



            SS2Ethereal_EnableChange = config.Bind<bool>(
                _ss2EtherealSectionName,
                "Enable Ethereal Gold Reward Change", false,
                "ONLY ENABLE IF YOU'RE PLAYING WITH THE STARSTORM 2 BETA"
            );
            SS2Ethereal_NerfBaseMultiplier = config.Bind<float>(
                _ss2EtherealSectionName,
                "Ethereal - Base Multiplier", 2f,
                ""
            );
            SS2Ethereal_EtherealsUsedMultiplier = config.Bind<float>(
                _ss2EtherealSectionName,
                "Ethereal - Ethereals-used Multiplier", 2f,
                ""
            );
            SS2Ethereal_StageOfNerfStart = config.Bind<int>(
                _ss2EtherealSectionName,
                "Ethereal - Starting stage of nerf", 8,
                ""
            );
            SS2Ethereal_NerfPerStageMultiplier = config.Bind<float>(
                _ss2EtherealSectionName,
                "Ethereal - Per-Stage multiplier", 0.2f,
                ""
            );



            SS2Ultra_EnableChange = config.Bind<bool>(
                _ss2UltraSectionName,
                "Enable Ultra Gold Reward Change", false,
                "ONLY ENABLE IF YOU'RE PLAYING WITH THE STARSTORM 2 BETA"
            );
            SS2Ultra_NerfBaseMultiplier = config.Bind<float>(
                _ss2UltraSectionName,
                "Ultra - Base Multiplier", 20f,
                ""
            );
            SS2Ultra_EtherealsUsedMultiplier = config.Bind<float>(
                _ss2UltraSectionName,
                "Ultra - Ethereals-used Multiplier", 20f,
                ""
            );
            SS2Ultra_StageOfNerfStart = config.Bind<int>(
                _ss2UltraSectionName,
                "Ultra - Starting stage of nerf", 12,
                ""
            );
            SS2Ultra_NerfPerStageMultiplier = config.Bind<float>(
                _ss2UltraSectionName,
                "Ultra - Per-Stage multiplier", 0.6f,
                ""
            );
            #endregion
        }
    }
}