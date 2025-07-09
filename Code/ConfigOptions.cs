using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using MiscFixes.Modules;

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

        #region SS2 Options
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
            if (ModSupport.RiskOfOptionsMod.ModIsRunning)
            {
                ModSupport.RiskOfOptionsMod.SetRiskOfOptionsDescription();
            }

            BindWRBGoldRewardOptions(config);
            BindWRBMultiplayerGoldScalingOptions(config);
            BindSS2Options(config);

            config.WipeConfig();
        }



        private static void BindWRBGoldRewardOptions(ConfigFile config)
        {
            EnableWRBGoldScaling = config.BindOption(
                _wrbGoldRewardSectionName,
                "Enable",
                "Enable using the formula from Well-Rounded Balance (WRB) for scaling gold from enemy kills?\n\nFormula for gold reward: Minimum value between Vanilla Gold Reward * Base Multiplier and Base Multiplier * ((Vanilla Gold Reward / (Stage Divisor + (Stage Clear Count * Stage Clear Count Multiplier))) + Square Root(Square Root Multiplier * (Stage And Loop Multiplier + (Stage Clear Count * Stage Multiplier + Loop Clear Count * Loop Multiplier))))",
                true,
                Extensions.ConfigFlags.RestartRequired
            );


            // TODO miscfixes please update i need more float precision than 2 digits
            BaseGoldScalingMultiplier = config.BindOptionSteppedSlider(
                _wrbGoldRewardSectionName,
                "Base Multiplier",
                "WRB's default value is 0.75",
                //1.125f,
                //0.005f,
                //0.005f, 2.5f
                1.13f,
                0.01f,
                0.01f, 2.5f
            );
            LoopMultiplier = config.BindOptionSteppedSlider(
                _wrbGoldRewardSectionName,
                "Loop Multiplier",
                "WRB's default value is unchanged",
                -660,
                5,
                -1000, 500
            );
            StageDivisor = config.BindOptionSlider(
                _wrbGoldRewardSectionName,
                "Stage Divisor",
                "WRB's default value is 3",
                2f
            );
            StageClearCountMultiplier = config.BindOptionSlider(
                _wrbGoldRewardSectionName,
                "Stage Clear Count Multiplier",
                "WRB's default value is 0.25",
                0.5f
            );
            StageMultiplier = config.BindOptionSteppedSlider(
                _wrbGoldRewardSectionName,
                "Stage Multiplier",
                "WRB's default value is unchanged",
                150,
                5,
                0, 300
            );
            SquareRootMultiplier = config.BindOptionSlider(
                _wrbGoldRewardSectionName,
                "Square Root Multiplier",
                "WRB's default value is unchanged",
                6f
            );
            StageAndLoopMultiplier = config.BindOptionSteppedSlider(
                _wrbGoldRewardSectionName,
                "Stage and Loop Multiplier",
                "WRB's default value is unchanged",
                275f,
                5,
                0, 500
            );
        }

        private static void BindWRBMultiplayerGoldScalingOptions(ConfigFile config)
        {
            EnableWRBMultiplayerCostScaling = config.BindOption(
                _wrbMultiplayerCostSectionName,
                "Enable",
                "Enable using the formula from Well-Rounded Balance (WRB) for scaling the cost of buyable things in multiplayer?\n\nFormula for cost scaling: Base Multiplier + (Per-Player Multiplier / Square Root(Player Count))",
                true
            );

            BaseMultiplayerCostMultiplier = config.BindOptionSlider(
                _wrbMultiplayerCostSectionName,
                "Base Multiplier",
                "WRB's default value is unchanged",
                1f
            );
            PerPlayerCostMultiplier = config.BindOptionSlider(
                _wrbMultiplayerCostSectionName,
                "Per-Player cost multiplier",
                "WRB's default value is unchanged",
                0.11f
            );
        }

        private static void BindSS2Options(ConfigFile config)
        {
            EnableSS2Support = config.BindOption(
                _ss2EntireSectionName,
                "Enable",
                "Enable special support for nerfing the gold reward from some late-game elites from Starstorm 2? The gold reward nerf for these elites are ON TOP OF the existing gold nerf from WRB if it's enabled.\n\nFormula for gold reward: Base Gold Reward * Highest value between 1 and (Base Multiplier + IF APPLICABLE (Ethereal Multiplier * Ethereals Used Count)) / (1 + (Highest value between 0 and ((Stage Clear Count + 1) - Starting Stage of Nerf) * Per-Stage Multiplier))",
                true,
                Extensions.ConfigFlags.RestartRequired
            );



            #region Empyrean
            SS2Empyrean_EnableChange = config.BindOption(
                _ss2EmpyreanSectionName,
                "Enable Empyrean Gold Reward Change",
                "",
                true,
                Extensions.ConfigFlags.RestartRequired
            );
            SS2Empyrean_NerfBaseMultiplier = config.BindOptionSlider(
                _ss2EmpyreanSectionName,
                "Empyrean - Base Multiplier",
                "",
                10f
            );
            SS2Empyrean_StageOfNerfStart = config.BindOptionSlider(
                _ss2EmpyreanSectionName,
                "Empyrean - Starting stage of nerf",
                "",
                9
            );
            SS2Empyrean_NerfPerStageMultiplier = config.BindOptionSlider(
                _ss2EmpyreanSectionName,
                "Empyrean - Per-Stage multiplier",
                "",
                0.5f
            );
            #endregion



            #region Ethereal
            SS2Ethereal_EnableChange = config.BindOption(
                _ss2EtherealSectionName,
                "Enable Ethereal Gold Reward Change",
                "THIS IS FOR THE BETA VERSION OF SS2",
                false,
                Extensions.ConfigFlags.RestartRequired
            );
            SS2Ethereal_NerfBaseMultiplier = config.BindOptionSlider(
                _ss2EtherealSectionName,
                "Ethereal - Base Multiplier",
                "",
                2f
            );
            SS2Ethereal_EtherealsUsedMultiplier = config.BindOptionSlider(
                _ss2EtherealSectionName,
                "Ethereal - Ethereals-used Multiplier",
                "",
                2f
            );
            SS2Ethereal_StageOfNerfStart = config.BindOptionSlider(
                _ss2EtherealSectionName,
                "Ethereal - Starting stage of nerf",
                "",
                8
            );
            SS2Ethereal_NerfPerStageMultiplier = config.BindOptionSlider(
                _ss2EtherealSectionName,
                "Ethereal - Per-Stage multiplier",
                "",
                0.35f
            );
            #endregion



            #region Ultra
            SS2Ultra_EnableChange = config.BindOption(
                _ss2UltraSectionName,
                "Enable Ultra Gold Reward Change",
                "THIS IS FOR THE BETA VERSION OF SS2",
                false,
                Extensions.ConfigFlags.RestartRequired
            );
            SS2Ultra_NerfBaseMultiplier = config.BindOptionSlider(
                _ss2UltraSectionName,
                "Ultra - Base Multiplier",
                "",
                15f
            );
            SS2Ultra_EtherealsUsedMultiplier = config.BindOptionSlider(
                _ss2UltraSectionName,
                "Ultra - Ethereals-used Multiplier",
                "",
                20f
            );
            SS2Ultra_StageOfNerfStart = config.BindOptionSlider(
                _ss2UltraSectionName,
                "Ultra - Starting stage of nerf",
                "",
                12
            );
            SS2Ultra_NerfPerStageMultiplier = config.BindOptionSlider(
                _ss2UltraSectionName,
                "Ultra - Per-Stage multiplier",
                "",
                0.8f
            );
            #endregion
        }
    }
}