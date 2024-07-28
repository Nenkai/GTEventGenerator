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
using System.Collections.ObjectModel;

using PDTools.Structures.MGameParameter;
using System.Diagnostics;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for ArcadeStyleView.xaml
/// </summary>
public partial class ArcadeStyleView : UserControl
{
    public ArcadeStyleSetting ArcadeStyle => DataContext as ArcadeStyleSetting;

    public ArcadeStyleView()
    {
        InitializeComponent();
    }

    public ObservableCollection<string> SectionExtendSecondStrings = new ObservableCollection<string>();
    public ObservableCollection<string> SpeedTrapStrings = new ObservableCollection<string>();

    private bool process = true;

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

    private void cb_EnableJumpBonus_Checked(object sender, RoutedEventArgs e)
    {
        ArcadeStyle.EnableJumpBonus = cb_EnableJumpBonus.IsChecked.Value;
    }

    private void cb_EnableSpeedTrap_Checked(object sender, RoutedEventArgs e)
    {
        ArcadeStyle.EnableSpeedTrap = cb_EnableSpeedTrap.IsChecked.Value;
    }

    private void iud_OvertakeSeconds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.OvertakeSeconds = iud_OvertakeSeconds.Value.Value;
    }

    private void iud_StartSeconds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.StartSeconds = iud_StartSeconds.Value.Value;
    }

    private void iud_DefaultExtendSeconds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.DefaultExtendSeconds = iud_DefaultExtendSeconds.Value.Value;
    }

    private void iud_LimitSeconds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.LimitSeconds = iud_LimitSeconds.Value.Value;
    }

    private void iud_AppearStepV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.AppearStepV = iud_AppearStepV.Value.Value;
    }

    private void iud_DisappearStepV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.DisappearStepV = iud_DisappearStepV.Value.Value;
    }

    private void iud_AffordTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.AffordTime = iud_AffordTime.Value.Value;
    }

    private void iud_OvertakeScore_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.OvertakeScore = iud_OvertakeScore.Value.Value;
    }

    private void iud_SpeedTrapScore_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.SpeedTrapScore = iud_SpeedTrapScore.Value.Value;
    }

    private void iud_JumpBonusScore_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.JumpBonusScore = iud_JumpBonusScore.Value.Value;
    }

    private void iud_StartupStepV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.StartupStepV = iud_StartupStepV.Value.Value;
    }

    private void iud_StartupOffsetV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.StartupOffsetV = iud_StartupOffsetV.Value.Value;
    }

    private void iud_InitialVelocityL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.InitialVelocityL = iud_InitialVelocityL.Value.Value;
    }

    private void iud_InitialVelocityH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.InitialVelocityH = iud_InitialVelocityH.Value.Value;
    }

    private void iud_LevelUpStep_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ArcadeStyle.LevelUpStep = iud_LevelUpStep.Value.Value;
    }

    private void iud_ExtraSecondPerSection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (lb_SectionExtendSecond.SelectedIndex == -1)
            return;

        int oldIndex = lb_SectionExtendSecond.SelectedIndex;

        process = false;
        SectionExtendSecondStrings[lb_SectionExtendSecond.SelectedIndex] = GetSectionExtendSecondString(lb_SectionExtendSecond.SelectedIndex, iud_ExtraSecondPerSection.Value.Value);
        lb_SectionExtendSecond.SelectedIndex = oldIndex;
        process = true;

        ArcadeStyle.SectionExtendSeconds[lb_SectionExtendSecond.SelectedIndex] = iud_ExtraSecondPerSection.Value.Value;
    }

    private void iud_SpeedTrap_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (lb_SpeedTrap.SelectedIndex == -1)
            return;

        int oldIndex = lb_SpeedTrap.SelectedIndex;

        process = false;
        SpeedTrapStrings[lb_SpeedTrap.SelectedIndex] = GetSpeedTrapString(lb_SpeedTrap.SelectedIndex, iud_SpeedTrap.Value.Value);
        lb_SpeedTrap.SelectedIndex = oldIndex;
        process = true;

        ArcadeStyle.SpeedTraps[lb_SpeedTrap.SelectedIndex] = iud_SpeedTrap.Value.Value;
    }

    public void Refresh()
    {
        SectionExtendSecondStrings.Clear();
        SpeedTrapStrings.Clear();

        cb_EnableJumpBonus.IsChecked = ArcadeStyle.EnableJumpBonus;
        cb_EnableSpeedTrap.IsChecked = ArcadeStyle.EnableSpeedTrap;
        iud_OvertakeSeconds.Value = ArcadeStyle.OvertakeSeconds;
        iud_StartSeconds.Value = ArcadeStyle.StartSeconds;
        iud_DefaultExtendSeconds.Value = ArcadeStyle.DefaultExtendSeconds;
        iud_LimitSeconds.Value = ArcadeStyle.LimitSeconds;
        iud_AppearStepV.Value = ArcadeStyle.AppearStepV;
        iud_DisappearStepV.Value = ArcadeStyle.DisappearStepV;
        iud_AffordTime.Value = ArcadeStyle.AffordTime;
        iud_OvertakeScore.Value = ArcadeStyle.OvertakeScore;
        iud_SpeedTrapScore.Value = ArcadeStyle.SpeedTrapScore;
        iud_JumpBonusScore.Value = ArcadeStyle.JumpBonusScore;
        iud_StartupStepV.Value = ArcadeStyle.StartupStepV;
        iud_StartupOffsetV.Value = ArcadeStyle.StartupOffsetV;
        iud_InitialVelocityL.Value = ArcadeStyle.InitialVelocityL;
        iud_InitialVelocityH.Value = ArcadeStyle.InitialVelocityH;
        iud_LevelUpStep.Value = ArcadeStyle.LevelUpStep;

        for (var i = 0; i < ArcadeStyle.SectionExtendSeconds.Length; i++)
            SectionExtendSecondStrings.Add(GetSectionExtendSecondString(i, ArcadeStyle.SectionExtendSeconds[i]));

        for (var i = 0; i < ArcadeStyle.SpeedTraps.Length; i++)
            SpeedTrapStrings.Add(GetSpeedTrapString(i, ArcadeStyle.SpeedTraps[i]));

        lb_SectionExtendSecond.ItemsSource = SectionExtendSecondStrings;
        lb_SpeedTrap.ItemsSource = SpeedTrapStrings;
    }

    private string GetSectionExtendSecondString(int index, sbyte value)
    {
        if (value != -1)
            return $"Section #{index + 1}: +{value} second(s)";
        else
            return $"Section #{index + 1}: -1 (disabled)";
    }

    private string GetSpeedTrapString(int index, uint value)
    {
        if (value != 0)
            return $"Speed Trap #{index + 1}: VCoord {value}";
        else
            return $"Speed Trap #{index + 1}: 0 (none)";
    }

    private void lb_SectionExtendSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_SectionExtendSecond.SelectedIndex == -1 || !process)
            return;

        iud_ExtraSecondPerSection.Value = ArcadeStyle.SectionExtendSeconds[lb_SectionExtendSecond.SelectedIndex];
    }

    private void lb_SpeedTrap_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_SectionExtendSecond.SelectedIndex == -1 || process)
            return;

        iud_SpeedTrap.Value = ArcadeStyle.SpeedTraps[lb_SectionExtendSecond.SelectedIndex];
    }
}
