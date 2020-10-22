using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using GTEventGenerator.Entities;
using GTEventGenerator.Utils;
using Humanizer;

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        #region Pane 1
        private void comboBox_CompleteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.CompleteType = (CompleteType)comboBox_CompleteType.SelectedIndex;

        }

        private void comboBox_FinishType_SelectedIndexChanged(object sender, EventArgs e)
            => CurrentEvent.RaceParameters.FinishType = (FinishType)comboBox_FinishType.SelectedIndex;

        private void numericUpDown_FinishVCoord_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_FinishVCoord.Value is null)
                numericUpDown_FinishVCoord.Value = 0;

            CurrentEvent.RaceParameters.EventGoalV = (int)numericUpDown_FinishVCoord.Value;
        }

        private void numericUpDown_FinishWidth_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_FinishWidth.Value is null)
                numericUpDown_FinishWidth.Value = 0;

            CurrentEvent.RaceParameters.EventGoalWidth = (int)numericUpDown_FinishWidth.Value;
        }

        private void numericUpDown_TimeToFinish_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_TimeToFinish.Value is null)
                numericUpDown_TimeToFinish.Value = 0;

            CurrentEvent.RaceParameters.TimeToFinish = TimeSpan.FromMilliseconds((double)numericUpDown_TimeToFinish.Value);
        }

        private void numericUpDown_TimeToStart_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_TimeToStart.Value is null)
                numericUpDown_TimeToStart.Value = 0;

            CurrentEvent.RaceParameters.TimeToStart = TimeSpan.FromMilliseconds((double)numericUpDown_TimeToStart.Value);
        }

        private void numericUpDown_LapsToFinish_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_LapsToFinish.Value is null)
                numericUpDown_LapsToFinish.Value = 0;

            CurrentEvent.RaceParameters.LapCount = (int)numericUpDown_LapsToFinish.Value;
        }

        private void numericUpDown_MinutesToFinish_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_MinutesToFinish.Value is null)
                numericUpDown_MinutesToFinish.Value = 0;

            CurrentEvent.RaceParameters.MinutesCount = (int)numericUpDown_MinutesToFinish.Value;
        }

        private void comboBox_GhostPresence_SelectedIndexChanged(object sender, EventArgs e)
            => CurrentEvent.RaceParameters.GhostPresenceType = (GhostPresenceType)comboBox_GhostPresence.SelectedIndex;

        private void comboBox_StartingType_SelectedIndexChanged(object sender, EventArgs e)
            => CurrentEvent.RaceParameters.StartType = (StartType)comboBox_StartingType.SelectedIndex;

        private void numericUpDown_TireConsumptionMultiplier_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_TireConsumptionMultiplier.Value is null)
                numericUpDown_TireConsumptionMultiplier.Value = 0;

            CurrentEvent.RaceParameters.TireUseMultiplier = (int)numericUpDown_TireConsumptionMultiplier.Value;
        }

        private void numericUpDown_FuelConsumptionMultiplier_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            if (numericUpDown_FuelConsumptionMultiplier.Value is null)
                numericUpDown_FuelConsumptionMultiplier.Value = 0;

            CurrentEvent.RaceParameters.FuelUseMultiplier = (int)numericUpDown_FuelConsumptionMultiplier.Value;
        }

        private void checkBox_EnableDamage_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            comboBox_DamageBehavior.IsEnabled = checkBox_EnableDamage.IsChecked.Value;
            if (!checkBox_EnableDamage.IsChecked.Value)
            {
                comboBox_DamageBehavior.SelectedIndex = 0;
                CurrentEvent.RaceParameters.BehaviorDamage = BehaviorDamageType.WEAK;
            }
        }

        private void comboBox_GhostType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.GhostType = (GhostType)comboBox_GhostType.SelectedIndex;
            comboBox_GhostPresence.IsEnabled = CurrentEvent.RaceParameters.GhostType != GhostType.NONE;
            if (comboBox_GhostType.SelectedIndex == 0)
            {
                comboBox_GhostPresence.SelectedIndex = 0;
                CurrentEvent.RaceParameters.GhostPresenceType = GhostPresenceType.NORMAL;
            }
        }

        #endregion Pane 1

        #region Pane 2 
        private void comboBox_GridSortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.GridSortType = (GridSortType)comboBox_GridSortType.SelectedIndex;
        }

        private void comboBox_LightingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.LightingMode = (LightingMode)comboBox_LightingMode.SelectedIndex;
        }

        private void comboBox_PenaltyLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.PenaltyLevel = (PenaltyLevel)comboBox_PenaltyLevel.SelectedIndex;
        }

        private void comboBox_SlipstreamBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.SlipstreamBehavior = (SlipstreamBehavior)comboBox_SlipstreamBehavior.SelectedIndex;
        }

        private void comboBox_RaceType_SelectedIndexChanged(object sender, EventArgs e)
            => CurrentEvent.RaceParameters.RaceType = (RaceType)comboBox_RaceType.SelectedIndex;


        private void comboBox_LineGhostRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.LineGhostRecordType = (LineGhostRecordType)comboBox_LineGhostRecordType.SelectedIndex;
            numericUpDown_MaxGhostLines.IsEnabled = CurrentEvent.RaceParameters.LineGhostRecordType != LineGhostRecordType.OFF;
            numericUpDown_MaxGhostLines.Value = numericUpDown_MaxGhostLines.IsEnabled ? 1 : 0;
        }

        private void numericUpDown_MaxGhostLines_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            CurrentEvent.RaceParameters.LineGhostPlayMax = numericUpDown_MaxGhostLines.Value;
        }

        private void comboBox_Flagset_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent.RaceParameters.Flagset = (Flagset)comboBox_Flagset.SelectedIndex;
        }
        #endregion

        #region Pane 3
        private void checkBox_DisableReplayRecord_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.DisableRecordingReplay = chk.IsChecked.Value;
        }

        private void checkBox_AcademyEvent_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.AcademyEvent = chk.IsChecked.Value;
        }

        private void checkBox_Accumulation_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.Accumulation = chk.IsChecked.Value;

            numericUpDown_TireConsumptionMultiplier.IsEnabled = CurrentEvent.RaceParameters.Accumulation;
            numericUpDown_FuelConsumptionMultiplier.IsEnabled = CurrentEvent.RaceParameters.Accumulation;
            if (!CurrentEvent.RaceParameters.Accumulation)
            {
                numericUpDown_TireConsumptionMultiplier.Value = 0;
                numericUpDown_FuelConsumptionMultiplier.Value = 0;
            }
        }

        private void checkBox_CoDriver_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.AllowCoDriver = chk.IsChecked.Value;
        }

        private void checkBox_AutostartPitout_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.AutostartPitout = chk.IsChecked.Value;
        }

        private void checkBox_PaceNote_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.DisableRecordingReplay = chk.IsChecked.Value;
        }

        private void checkBox_ImmediateFinish_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.ImmediateFinish = chk.IsChecked.Value;
        }

        private void checkBox_BoostFlag_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.BoostFlag = chk.IsChecked.Value;
        }

        private void checkBox_PenaltyNoReset_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.PenaltyNoLevel = chk.IsChecked.Value;
        }

        private void checkBox_OnlineOn_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.OnlineOn = chk.IsChecked.Value;
        }

        private void checkBox_GhostEnabled_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            CurrentEvent.RaceParameters.WithGhost = chk.IsChecked.Value;
        }
        #endregion

        #region Pane 4

        private void comboBox_DecisiveWeather_SelectedIndexChanged(object sender, EventArgs e)
            => CurrentEvent.RaceParameters.DecisiveWeather = (DecisiveWeatherType)comboBox_DecisiveWeather.SelectedIndex;

        #endregion

        public void PopulateParameters()
        {
            PrePopulateComboBoxes();

            numericUpDown_FinishVCoord.Value = CurrentEvent.RaceParameters.EventGoalV != null ? (int)CurrentEvent.RaceParameters.EventGoalV.Value : -1;
            numericUpDown_FinishWidth.Value = CurrentEvent.RaceParameters.EventGoalWidth != null ? (int)CurrentEvent.RaceParameters.EventGoalWidth.Value : -1;
            numericUpDown_TimeToFinish.Value = (int)CurrentEvent.RaceParameters.TimeToFinish.TotalMilliseconds;
            numericUpDown_TimeToStart.Value = (int)CurrentEvent.RaceParameters.TimeToStart.TotalMilliseconds;
            numericUpDown_LapsToFinish.Value = CurrentEvent.RaceParameters.LapCount;
            numericUpDown_MinutesToFinish.Value = CurrentEvent.RaceParameters.MinutesCount;
            numericUpDown_TireConsumptionMultiplier.Value = CurrentEvent.RaceParameters.TireUseMultiplier;
            numericUpDown_FuelConsumptionMultiplier.Value = CurrentEvent.RaceParameters.FuelUseMultiplier;

            comboBox_CompleteType.SelectedIndex = (int)CurrentEvent.RaceParameters.CompleteType;
            comboBox_FinishType.SelectedIndex = (int)CurrentEvent.RaceParameters.FinishType;
            comboBox_DamageBehavior.SelectedIndex = (int)CurrentEvent.RaceParameters.BehaviorDamage;
            comboBox_GhostType.SelectedIndex = (int)CurrentEvent.RaceParameters.GhostType;
            comboBox_GhostPresence.SelectedIndex = (int)CurrentEvent.RaceParameters.GhostPresenceType;

            comboBox_GridSortType.SelectedIndex = (int)CurrentEvent.RaceParameters.GridSortType;
            comboBox_LightingMode.SelectedIndex = (int)CurrentEvent.RaceParameters.LightingMode;
            comboBox_PenaltyLevel.SelectedIndex = (int)CurrentEvent.RaceParameters.PenaltyLevel;
            comboBox_SlipstreamBehavior.SelectedIndex = (int)CurrentEvent.RaceParameters.SlipstreamBehavior;
            comboBox_LineGhostRecordType.SelectedIndex = (int)CurrentEvent.RaceParameters.LineGhostRecordType;
            numericUpDown_MaxGhostLines.Value = CurrentEvent.RaceParameters.LineGhostPlayMax;
            comboBox_Flagset.SelectedIndex = (int)CurrentEvent.RaceParameters.Flagset;
            comboBox_StartingType.SelectedIndex = (int)CurrentEvent.RaceParameters.StartType;
            comboBox_RaceType.SelectedIndex = (int)CurrentEvent.RaceParameters.RaceType;
            comboBox_DecisiveWeather.SelectedIndex = (int)CurrentEvent.RaceParameters.DecisiveWeather;
            cb_StartTime.IsChecked = CurrentEvent.RaceParameters.Date != null;

            CurrentEvent.RaceParameters.NeedsPopulating = false;
        }

        private void PrePopulateComboBoxes()
        {
            if (comboBox_CompleteType.Items.Count > 0)
                return; // Assume it was already all filled in

            var types = (CompleteType[])Enum.GetValues(typeof(CompleteType));
            for (int i = 0; i < types.Length; i++)
            {
                var comp = (CompleteType)i;
                string compName = comp.Humanize();
                comboBox_CompleteType.Items.Add(compName);
            }

            var finishTypes = (FinishType[])Enum.GetValues(typeof(FinishType));
            for (int i = 0; i < finishTypes.Length; i++)
            {
                var fin = (FinishType)i;
                string finName = fin.Humanize();
                comboBox_FinishType.Items.Add(finName);
            }

            var behaviorDamageTypes = (BehaviorDamageType[])Enum.GetValues(typeof(BehaviorDamageType));
            for (int i = 0; i < behaviorDamageTypes.Length; i++)
            {
                var behaviorDamageType = (BehaviorDamageType)i;
                string behaviorDamageTypeName = behaviorDamageType.Humanize();
                comboBox_DamageBehavior.Items.Add(behaviorDamageTypeName);
            }

            var ghostTypes = (GhostType[])Enum.GetValues(typeof(GhostType));
            for (int i = 0; i < ghostTypes.Length; i++)
            {
                var ghostType = (GhostType)i;
                string ghostTypeName = ghostType.Humanize();
                comboBox_GhostType.Items.Add(ghostTypeName);
            }

            var ghostPresenceTypes = (GhostPresenceType[])Enum.GetValues(typeof(GhostPresenceType));
            for (int i = 0; i < ghostPresenceTypes.Length; i++)
            {
                var ghostPresenceType = (GhostPresenceType)i;
                string ghostPresenceTypeName = ghostPresenceType.Humanize();
                comboBox_GhostPresence.Items.Add(ghostPresenceTypeName);
            }

            var gridSortTypes = (GridSortType[])Enum.GetValues(typeof(GridSortType));
            for (int i = 0; i < gridSortTypes.Length; i++)
            {
                var gridSortType = (GridSortType)i;
                string gridSortTypeName = gridSortType.Humanize();
                comboBox_GridSortType.Items.Add(gridSortTypeName);
            }

            var lightingModes = (LightingMode[])Enum.GetValues(typeof(LightingMode));
            for (int i = 0; i < lightingModes.Length; i++)
            {
                var lightingMode = (LightingMode)i;
                string lightingModeName = lightingMode.Humanize();
                comboBox_LightingMode.Items.Add(lightingModeName);
            }

            var penalties = (PenaltyLevel[])Enum.GetValues(typeof(PenaltyLevel));
            for (int i = 0; i < penalties.Length; i++)
            {
                var p = (PenaltyLevel)i;
                string pName = p.Humanize();
                comboBox_PenaltyLevel.Items.Add(pName);
            }
            var slipstreams = (SlipstreamBehavior[])Enum.GetValues(typeof(SlipstreamBehavior));
            for (int i = 0; i < slipstreams.Length; i++)
            {
                var s = (SlipstreamBehavior)i;
                string sName = s.Humanize();
                comboBox_SlipstreamBehavior.Items.Add(sName);
            }

            var lineRecordTypes = (LineGhostRecordType[])Enum.GetValues(typeof(LineGhostRecordType));
            for (int i = 0; i < lineRecordTypes.Length; i++)
            {
                var l = (LineGhostRecordType)i;
                string lName = l.Humanize();
                comboBox_LineGhostRecordType.Items.Add(lName);
            }

            var flagsets = (Flagset[])Enum.GetValues(typeof(Flagset));
            for (int i = 0; i < flagsets.Length; i++)
            {
                var f = (Flagset)i;
                string fName = f.Humanize();
                comboBox_Flagset.Items.Add(fName);
            }

            var startTypes = (StartType[])Enum.GetValues(typeof(StartType));
            for (int i = 0; i < startTypes.Length; i++)
            {
                var s = (StartType)i;
                string sName = s.Humanize();
                comboBox_StartingType.Items.Add(sName);
            }

            var raceTypes = (RaceType[])Enum.GetValues(typeof(RaceType));
            for (int i = 0; i < raceTypes.Length; i++)
            {
                var r = (RaceType)i;
                string rName = r.Humanize();
                comboBox_RaceType.Items.Add(rName);
            }

            var weatherTypes = (DecisiveWeatherType[])Enum.GetValues(typeof(DecisiveWeatherType));
            for (int i = 0; i < weatherTypes.Length; i++)
            {
                var w = (DecisiveWeatherType)i;
                string wName = w.Humanize();
                comboBox_DecisiveWeather.Items.Add(wName);
            }
        }
    }
}
 