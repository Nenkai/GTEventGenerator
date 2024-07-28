using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PDTools.Structures.MGameParameter;
using PDTools.Enums;

using Humanizer;

using GTEventMaker;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for RaceParameterView.xaml
/// </summary>
public partial class RaceParameterView : UserControl
{
    public RaceParameter Race => this.DataContext as RaceParameter;

    public RaceParameterView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Populate();
    }

    private void numericUpDown_RacersMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (!numericUpDown_RacersMax.Value.HasValue)
            return;

        Race.RacersMax = numericUpDown_RacersMax.Value.Value;
    }

    private void numericUpDown_EntryMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (!numericUpDown_EntryMax.Value.HasValue)
            return;

        Race.EntryMax = numericUpDown_EntryMax.Value.Value;
    }

    private void comboBox_CompleteType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Race.CompleteType = (CompleteType)comboBox_CompleteType.SelectedIndex;

    }

    private void comboBox_FinishType_SelectedIndexChanged(object sender, EventArgs e)
        => Race.FinishType = (FinishType)comboBox_FinishType.SelectedIndex;

    private void numericUpDown_StartVCoord_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        if (numericUpDown_StartVCoord.Value is null)
            numericUpDown_StartVCoord.Value = -1;

        Race.EventStartV = (int)numericUpDown_StartVCoord.Value;
    }

    private void numericUpDown_FinishVCoord_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        Race.EventGoalV = numericUpDown_FinishVCoord.Value ?? -1;
    }

    private void numericUpDown_FinishWidth_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        Race.EventGoalWidth = numericUpDown_FinishWidth.Value ?? -1;
    }

    private void numericUpDown_TimeToFinish_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        numericUpDown_TimeToFinish.Value ??= 0;
        Race.TimeToFinish = TimeSpan.FromMilliseconds((double)numericUpDown_TimeToFinish.Value);
    }

    private void numericUpDown_TimeToStart_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        if (numericUpDown_TimeToStart.Value is null)
            numericUpDown_TimeToStart.Value = 0;

        Race.TimeToStart = TimeSpan.FromMilliseconds((double)numericUpDown_TimeToStart.Value);
    }

    private void numericUpDown_LapsToFinish_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        numericUpDown_LapsToFinish.Value ??= 0;

        Race.RaceLimitLaps = (short)numericUpDown_LapsToFinish.Value;
    }

    private void numericUpDown_MinutesToFinish_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        if (numericUpDown_MinutesToFinish.Value is null)
            numericUpDown_MinutesToFinish.Value = 0;

        Race.RaceLimitMinute = (short)numericUpDown_MinutesToFinish.Value;
    }

    private void comboBox_GhostPresence_SelectedIndexChanged(object sender, EventArgs e)
        => Race.GhostPresenceType = (GhostPresenceType)comboBox_GhostPresence.SelectedIndex;

    private void comboBox_StartingType_SelectedIndexChanged(object sender, EventArgs e)
        => Race.StartType = (StartType)comboBox_StartingType.SelectedIndex - 1;

    private void numericUpDown_TireConsumptionMultiplier_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        if (numericUpDown_TireConsumptionMultiplier.Value is null)
            numericUpDown_TireConsumptionMultiplier.Value = 0;

        Race.ConsumeTireRate = (byte)numericUpDown_TireConsumptionMultiplier.Value;
    }

    private void numericUpDown_FuelConsumptionMultiplier_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        if (numericUpDown_FuelConsumptionMultiplier.Value is null)
            numericUpDown_FuelConsumptionMultiplier.Value = 0;

        Race.ConsumeFuelRate = (byte)numericUpDown_FuelConsumptionMultiplier.Value;
    }

    private void checkBox_EnableDamage_CheckedChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        comboBox_DamageBehavior.IsEnabled = checkBox_EnableDamage.IsChecked.Value;
        if (!checkBox_EnableDamage.IsChecked.Value)
        {
            comboBox_DamageBehavior.SelectedIndex = 0;
            Race.BehaviorDamage = BehaviorDamageType.WEAK;
        }
    }

    private void comboBox_GhostType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Race.GhostType = (GhostType)comboBox_GhostType.SelectedIndex;
        comboBox_GhostPresence.IsEnabled = Race.GhostType != GhostType.NONE;
        if (comboBox_GhostType.SelectedIndex == 0)
        {
            comboBox_GhostPresence.SelectedIndex = 0;
            Race.GhostPresenceType = GhostPresenceType.NORMAL;
        }
    }

    private void checkBox_ImmediateFinish_Checked(object sender, RoutedEventArgs e)
    {
        Race.ImmediateFinish = checkBox_ImmediateFinish.IsChecked.Value;
    }

    private void checkBox_EnablePits_Checked(object sender, RoutedEventArgs e)
    {
        Race.EnablePit = checkBox_EnablePits.IsChecked.Value;
    }

    private void checkBox_EnableDamage_CheckedChanged(object sender, RoutedEventArgs e)
    {
        Race.EnableDamage = checkBox_EnableDamage.IsChecked.Value;
    }

    private void checkBox_Endless_Checked(object sender, RoutedEventArgs e)
    {
        Race.Endless = checkBox_Endless.IsChecked.Value;
    }

    private void checkBox_DisableCollision_Checked(object sender, RoutedEventArgs e)
    {
        Race.DisableCollision = checkBox_DisableCollision.IsChecked.Value;
    }

    private void comboBox_GridSortType_SelectedIndexChanged(object sender, EventArgs e)
        => Race.GridSortType = (GridSortType)comboBox_GridSortType.SelectedIndex;

    private void comboBox_LightingMode_SelectedIndexChanged(object sender, EventArgs e)
        => Race.LightingMode = (LightingMode)comboBox_LightingMode.SelectedIndex;

    private void comboBox_PenaltyLevel_SelectedIndexChanged(object sender, EventArgs e)
        => Race.PenaltyLevel = (PenaltyLevelTypes)comboBox_PenaltyLevel.SelectedIndex - 1;

    private void comboBox_SlipstreamBehavior_SelectedIndexChanged(object sender, EventArgs e)
        => Race.SlipstreamBehavior = (BehaviorSlipStreamType)comboBox_SlipstreamBehavior.SelectedIndex;

    private void comboBox_DamageBehavior_SelectedIndexChanged(object sender, EventArgs e)
        => Race.BehaviorDamage = (BehaviorDamageType)comboBox_DamageBehavior.SelectedIndex;

    private void comboBox_RaceType_SelectedIndexChanged(object sender, EventArgs e)
        => Race.RaceType = (RaceType)comboBox_RaceType.SelectedIndex;

    private void comboBox_LowMuType_SelectedIndexChanged(object sender, EventArgs e)
        => Race.LowMuType = (LowMuType)comboBox_LowMuType.SelectedIndex;

    private void comboBox_LineGhostRecordType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Race.LineGhostRecordType = (LineGhostRecordType)comboBox_LineGhostRecordType.SelectedIndex;
        numericUpDown_MaxGhostLines.IsEnabled = Race.LineGhostRecordType != LineGhostRecordType.OFF;
        numericUpDown_MaxGhostLines.Value = numericUpDown_MaxGhostLines.IsEnabled ? (byte)1 : (byte)0;
    }

    private void numericUpDown_MaxGhostLines_ValueChanged(object sender, EventArgs e)
    {
        if (Race is null)
            return;

        Race.LineGhostPlayMax = numericUpDown_MaxGhostLines.Value.Value;
    }

    private void comboBox_Flagset_SelectedIndexChanged(object sender, EventArgs e)
    {
        Race.Flagset = (Flagset)comboBox_Flagset.SelectedIndex;
    }

    private void checkBox_DisableReplayRecord_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.DisableRecordingReplay = chk.IsChecked.Value;
    }

    private void checkBox_AcademyEvent_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.AcademyEvent = chk.IsChecked.Value;
    }

    private void checkBox_Accumulation_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.Accumulation = chk.IsChecked.Value;

        numericUpDown_TireConsumptionMultiplier.IsEnabled = Race.Accumulation;
        numericUpDown_FuelConsumptionMultiplier.IsEnabled = Race.Accumulation;
        if (!Race.Accumulation)
        {
            numericUpDown_TireConsumptionMultiplier.Value = 0;
            numericUpDown_FuelConsumptionMultiplier.Value = 0;
        }
    }

    private void checkBox_CoDriver_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.AllowCoDriver = chk.IsChecked.Value;
    }

    private void checkBox_AutostartPitout_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.AutostartPitout = chk.IsChecked.Value;
    }

    private void checkBox_PaceNote_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.DisableRecordingReplay = chk.IsChecked.Value;
    }

    private void checkBox_ImmediateFinish_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.ImmediateFinish = chk.IsChecked.Value;
    }

    private void checkBox_BoostFlag_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.BoostFlag = chk.IsChecked.Value;
    }

    private void checkBox_PenaltyNoReset_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.PenaltyNoReset = chk.IsChecked.Value;
    }

    private void checkBox_OnlineOn_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.OnlineOn = chk.IsChecked.Value;
    }

    private void checkBox_GhostEnabled_CheckedChanged(object sender, EventArgs e)
    {
        var chk = sender as CheckBox;
        Race.WithGhost = chk.IsChecked.Value;
    }

    private void btn_BoostTable_Click(object sender, RoutedEventArgs e)
    {
        var window = new BoostTableEditWindow(Race.BoostTables);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();
    }

    private void checkBox_BoostType_Checked(object sender, RoutedEventArgs e)
    {
        Race.BoostType = checkBox_BoostType.IsChecked.Value;
    }

    private void checkBox_BoostFlag_Checked(object sender, RoutedEventArgs e)
    {
        Race.BoostFlag = checkBox_BoostFlag.IsChecked.Value;
    }

    private void checkBox_DisableReplayRecord_Checked(object sender, RoutedEventArgs e)
    {
        Race.DisableRecordingReplay = checkBox_DisableReplayRecord.IsChecked.Value;
    }

    private void checkBox_AcademyEvent_Checked(object sender, RoutedEventArgs e)
    {
        Race.AcademyEvent = checkBox_AcademyEvent.IsChecked.Value;
    }

    private void checkBox_Accumulation_Checked(object sender, RoutedEventArgs e)
    {
        Race.Accumulation = checkBox_Accumulation.IsChecked.Value;
    }

    private void checkBox_CoDriver_Checked(object sender, RoutedEventArgs e)
    {
        Race.AllowCoDriver = checkBox_CoDriver.IsChecked.Value;
    }

    private void checkBox_AutostartPitout_Checked(object sender, RoutedEventArgs e)
    {
        Race.AutostartPitout = checkBox_AutostartPitout.IsChecked.Value;
    }

    private void checkBox_PaceNote_Checked(object sender, RoutedEventArgs e)
    {
        Race.PaceNote = checkBox_PaceNote.IsChecked.Value;
    }

    private void checkBox_PenaltyNoReset_Checked(object sender, RoutedEventArgs e)
    {
        Race.PenaltyNoReset = checkBox_PenaltyNoReset.IsChecked.Value;
    }

    private void checkBox_OnlineOn_Checked(object sender, RoutedEventArgs e)
    {
        Race.OnlineOn = checkBox_OnlineOn.IsChecked.Value;
    }

    private void checkBox_ReplaceAtCourseout_Checked(object sender, RoutedEventArgs e)
    {
        Race.ReplaceAtCourseOut = checkBox_ReplaceAtCourseout.IsChecked.Value;
    }

    private void cb_StartTime_Checked(object sender, RoutedEventArgs e)
    {
        date_Date.IsEnabled = cb_StartTime.IsChecked == true;
        if (!date_Date.IsEnabled)
            Race.Date = null;
    }

    private void comboBox_DecisiveWeather_SelectedIndexChanged(object sender, EventArgs e)
        => Race.DecisiveWeather = (DecisiveWeatherType)comboBox_DecisiveWeather.SelectedIndex;

    private void NewWeatherData_Click(object sender, EventArgs e)
    {
        if (Race.WeatherTotalSec <= 0)
        {
            System.Windows.MessageBox.Show("Weather Progress Length is not set, so there is no weather steps to set.", "Warning", MessageBoxButton.OK);
            return;
        }
        else if (Race.DecisiveWeather != DecisiveWeatherType.NONE)
        {
            System.Windows.MessageBox.Show("Decisive Weather is set to a fixed weather. Weather cannot change this way. Set it to 'None' if you want it to be variable and editable.",
                "Warning", MessageBoxButton.OK);
            return;
        }

        var window = new NewWeatherDataSettingsWindow(Race.NewWeatherData, TimeSpan.FromSeconds(Race.WeatherTotalSec));
        window.ShowDialog();
        iud_WeatherPointNum.Value = (byte)Race.NewWeatherData.Count;
        Race.WeatherPointNum = (byte)Race.NewWeatherData.Count;
    }

    private void date_Date_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.Date = date_Date.Value;
    }

    private void checkBox_KeepLoadGhost_Checked(object sender, RoutedEventArgs e)
    {
        Race.KeepLoadGhost = checkBox_KeepLoadGhost.IsChecked ?? false;
    }

    private void numericUpDown_BSpecVitality10_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.BSpecVitality10 = numericUpDown_BSpecVitality10.Value.Value;
    }

    private void checkBox_GoalTimeUseLapTotal_Checked(object sender, RoutedEventArgs e)
    {
        Race.GoalTimeUseLapTotal = checkBox_GoalTimeUseLapTotal.IsChecked ?? false;
    }

    private void numericUpDown_StartTimeOffset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.StartTimeOffset = numericUpDown_StartTimeOffset.Value.Value;
    }

    private void comboBox_StartSignalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Race.StartSignalType = (StartSignalType)comboBox_StartSignalType.SelectedIndex;
    }

    private void numericUpDown_ConsiderationType_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.ConsiderationType = numericUpDown_ConsiderationType.Value.Value;

    }

    private void numericUpDown_PitConstraint_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.PitConstraint = numericUpDown_PitConstraint.Value.Value;
    }

    private void checkBox_MandatoryTireChange_Checked(object sender, RoutedEventArgs e)
    {
        Race.NeedTireChange = checkBox_NeedTireChange.IsChecked ?? false;
    }

    private void numericUpDown_CourseOutPenaltyMargin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.CourseOutPenaltyMargin = numericUpDown_CourseOutPenaltyMargin.Value.Value;
    }

    private void numericUpDown_BehaviorFallback_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.BehaviorFallBack = numericUpDown_BehaviorFallback.Value.Value;
    }

    private void comboBox_AttackSeparateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Race.AttackSeparateType = (AttackSeparateType)comboBox_AttackSeparateType.SelectedIndex;
    }

    private void numericUpDown_MuRatio100_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.MuRatio100 = numericUpDown_MuRatio100.Value.Value;
    }

    private void numericUpDown_TemperatureTire_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.TemperatureTire = numericUpDown_TemperatureTire.Value.Value;
    }

    private void numericUpDown_TemperatureBrake_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.TemperatureBrake = numericUpDown_TemperatureBrake.Value.Value;
    }

    private void numericUpDown_TemperatureEngine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.TemperatureEngine = numericUpDown_TemperatureEngine.Value.Value;
    }

    private void iud_WeatherBaseCelsius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherBaseCelsius = iud_WeatherBaseCelsius.Value.Value;
    }

    private void iud_WeatherMinCelsius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherMinCelsius = iud_WeatherMinCelsius.Value.Value;
    }

    private void iud_WeatherMaxCelsius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherMaxCelsius = iud_WeatherMaxCelsius.Value.Value;
    }

    private void checkBox_WeatherNoPrecipitation_Checked(object sender, RoutedEventArgs e)
    {
        Race.WeatherNoPrecipitation = checkBox_WeatherNoPrecipitation.IsChecked.Value;
    }

    private void checkBox_WeatherNoWind_Checked(object sender, RoutedEventArgs e)
    {
        Race.WeatherNoWind = checkBox_WeatherNoWind.IsChecked.Value;
    }

    private void checkBox_RandomWeather_Checked(object sender, RoutedEventArgs e)
    {
        Race.WeatherRandom = checkBox_RandomWeather.IsChecked.Value;
    }

    private void iud_WeatherRandomSeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherRandomSeed = iud_WeatherRandomSeed.Value.Value;
    }

    private void checkBox_WeatherPrecRainOnly_Checked(object sender, RoutedEventArgs e)
    {
        Race.WeatherPrecRainOnly = checkBox_WeatherPrecRainOnly.IsChecked.Value;
    }

    private void iud_TimeProgressSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.TimeProgressSpeed = iud_TimeProgressSpeed.Value.Value;
    }

    private void iud_WeatherAccel10_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherAccel10 = iud_WeatherAccel10.Value.Value;
    }

    private void iud_WeatherAccelWaterRetention10_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherAccelWaterRetention10 = iud_WeatherAccelWaterRetention10.Value.Value;
    }

    private void iud_InitialRetention10_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.InitialRetention10_TrackWetness = iud_InitialRetention10.Value.Value;
    }

    private void iud_WeatherTotalSec_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherTotalSec = iud_WeatherTotalSec.Value.Value;
    }

    private void iud_WeatherPointNum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Race.WeatherPointNum = iud_WeatherPointNum.Value.Value;
    }

    public void Populate()
    {
        PopulateControls();
        PopulateParameters();
    }

    private void PopulateControls()
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

        var penalties = (PenaltyLevelTypes[])Enum.GetValues(typeof(PenaltyLevelTypes));
        for (int i = -1; i < penalties.Length - 1; i++)
        {
            var p = (PenaltyLevelTypes)i;
            string pName = p.Humanize();
            comboBox_PenaltyLevel.Items.Add(pName);
        }
        var slipstreams = (BehaviorSlipStreamType[])Enum.GetValues(typeof(BehaviorSlipStreamType));
        for (int i = 0; i < slipstreams.Length; i++)
        {
            var s = (BehaviorSlipStreamType)i;
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

        var startTypes = ((StartType[])Enum.GetValues(typeof(StartType)))
                                        .OrderBy(e => (int)e).ToArray();
        for (int i = 0; i < startTypes.Length; i++)
        {
            string sName = startTypes[i].Humanize();
            comboBox_StartingType.Items.Add(sName);
        }

        var raceTypes = (RaceType[])Enum.GetValues(typeof(RaceType));
        for (int i = 0; i < raceTypes.Length; i++)
        {
            var r = (RaceType)i;
            string rName = r.Humanize();
            comboBox_RaceType.Items.Add(rName);
        }

        var lowMuTypes = (LowMuType[])Enum.GetValues(typeof(LowMuType));
        for (int i = 0; i < lowMuTypes.Length; i++)
        {
            var r = (LowMuType)i;
            string rName = r.Humanize();
            comboBox_LowMuType.Items.Add(rName);
        }

        var weatherTypes = (DecisiveWeatherType[])Enum.GetValues(typeof(DecisiveWeatherType));
        for (int i = 0; i < weatherTypes.Length; i++)
        {
            var w = (DecisiveWeatherType)i;
            string wName = w.Humanize();
            comboBox_DecisiveWeather.Items.Add(wName);
        }

        var sepTypes = (AttackSeparateType[])Enum.GetValues(typeof(AttackSeparateType));
        for (int i = 0; i < sepTypes.Length; i++)
        {
            var w = (AttackSeparateType)i;
            string wName = w.Humanize();
            comboBox_AttackSeparateType.Items.Add(wName);
        }

        var startSignalTypes = (StartSignalType[])Enum.GetValues(typeof(StartSignalType));
        for (int i = 0; i < startSignalTypes.Length; i++)
        {
            var w = (StartSignalType)i;
            string wName = w.Humanize();
            comboBox_StartSignalType.Items.Add(wName);
        }
    }

    private void PopulateParameters()
    {
        if (Race is null)
            return;

        date_Date.Value = Race.Date;
        numericUpDown_EntryMax.Value = (byte)Race.EntryMax;
        numericUpDown_RacersMax.Value = (byte)Race.RacersMax;
        comboBox_CompleteType.SelectedIndex = (int)Race.CompleteType;
        comboBox_FinishType.SelectedIndex = (int)Race.FinishType;
        numericUpDown_StartVCoord.Value = Race.EventStartV;
        numericUpDown_FinishVCoord.Value = Race.EventGoalV;
        numericUpDown_FinishWidth.Value = Race.EventGoalWidth;
        numericUpDown_TimeToFinish.Value = (int)Race.TimeToFinish.TotalMilliseconds;
        numericUpDown_TimeToStart.Value = (int)Race.TimeToStart.TotalMilliseconds;
        numericUpDown_LapsToFinish.Value = Race.RaceLimitLaps;
        numericUpDown_MinutesToFinish.Value = Race.RaceLimitMinute;
        checkBox_ImmediateFinish.IsChecked = Race.ImmediateFinish;
        checkBox_EnableDamage.IsChecked = Race.EnableDamage;
        checkBox_EnablePits.IsChecked = Race.EnablePit;
        checkBox_Endless.IsChecked = Race.Endless;
        checkBox_DisableCollision.IsChecked = Race.DisableCollision;
        comboBox_DamageBehavior.SelectedIndex = (int)Race.BehaviorDamage;
        comboBox_GhostType.SelectedIndex = (int)Race.GhostType;
        comboBox_GhostPresence.SelectedIndex = (int)Race.GhostPresenceType;
        comboBox_StartingType.SelectedIndex = (int)Race.StartType + 1;
        numericUpDown_TireConsumptionMultiplier.Value = Race.ConsumeTireRate;
        numericUpDown_FuelConsumptionMultiplier.Value = Race.ConsumeFuelRate;

        comboBox_GridSortType.SelectedIndex = (int)Race.GridSortType;
        comboBox_LightingMode.SelectedIndex = (int)Race.LightingMode;
        comboBox_PenaltyLevel.SelectedIndex = (int)Race.PenaltyLevel + 1;
        comboBox_SlipstreamBehavior.SelectedIndex = (int)Race.SlipstreamBehavior;
        comboBox_LineGhostRecordType.SelectedIndex = (int)Race.LineGhostRecordType;
        numericUpDown_MaxGhostLines.Value = Race.LineGhostPlayMax;
        comboBox_Flagset.SelectedIndex = (int)Race.Flagset;
        comboBox_RaceType.SelectedIndex = (int)Race.RaceType;
        comboBox_LowMuType.SelectedIndex = (int)Race.LowMuType;
        numericUpDown_BSpecVitality10.Value = Race.BSpecVitality10;
        numericUpDown_StartTimeOffset.Value = Race.StartTimeOffset;
        comboBox_StartSignalType.SelectedIndex = (int)Race.StartSignalType;
        numericUpDown_ConsiderationType.Value = Race.ConsiderationType;
        numericUpDown_PitConstraint.Value = Race.PitConstraint;
        numericUpDown_CourseOutPenaltyMargin.Value = Race.CourseOutPenaltyMargin;
        numericUpDown_BehaviorFallback.Value = Race.BehaviorFallBack;
        comboBox_AttackSeparateType.SelectedIndex = (int)Race.AttackSeparateType;
        numericUpDown_MuRatio100.Value = Race.MuRatio100;
        numericUpDown_TemperatureBrake.Value = Race.TemperatureBrake;
        numericUpDown_TemperatureTire.Value = Race.TemperatureTire;
        numericUpDown_TemperatureEngine.Value = Race.TemperatureEngine;

        checkBox_DisableReplayRecord.IsChecked = Race.DisableRecordingReplay;
        checkBox_AcademyEvent.IsChecked = Race.AcademyEvent;
        checkBox_Accumulation.IsChecked = Race.Accumulation;
        checkBox_CoDriver.IsChecked = Race.AllowCoDriver;
        checkBox_AutostartPitout.IsChecked = Race.AutostartPitout;
        checkBox_PaceNote.IsChecked = Race.PaceNote;
        checkBox_PenaltyNoReset.IsChecked = Race.PenaltyNoReset;
        checkBox_OnlineOn.IsChecked = Race.OnlineOn;
        checkBox_ReplaceAtCourseout.IsChecked = Race.ReplaceAtCourseOut;
        checkBox_KeepLoadGhost.IsChecked = Race.KeepLoadGhost;
        checkBox_GoalTimeUseLapTotal.IsChecked = Race.GoalTimeUseLapTotal;
        checkBox_NeedTireChange.IsChecked = Race.NeedTireChange;

        iud_WeatherBaseCelsius.Value = Race.WeatherBaseCelsius;
        iud_WeatherMinCelsius.Value = Race.WeatherMinCelsius;
        iud_WeatherMaxCelsius.Value = Race.WeatherMaxCelsius;
        checkBox_WeatherNoPrecipitation.IsChecked = Race.WeatherNoPrecipitation;
        checkBox_WeatherNoWind.IsChecked = Race.WeatherNoWind;
        checkBox_RandomWeather.IsChecked = Race.WeatherRandom;
        iud_WeatherRandomSeed.Value = Race.WeatherRandomSeed;
        checkBox_WeatherPrecRainOnly.IsChecked = Race.WeatherPrecRainOnly;
        iud_TimeProgressSpeed.Value = Race.TimeProgressSpeed;
        iud_WeatherAccel10.Value = Race.WeatherAccel10;
        iud_WeatherAccelWaterRetention10.Value = Race.WeatherAccelWaterRetention10;
        iud_InitialRetention10.Value = Race.InitialRetention10_TrackWetness;
        iud_WeatherTotalSec.Value = Race.WeatherTotalSec;
        iud_WeatherPointNum.Value = Race.WeatherPointNum;

        comboBox_DecisiveWeather.SelectedIndex = (int)Race.DecisiveWeather;
        cb_StartTime.IsChecked = Race.Date != null;
    }

}
