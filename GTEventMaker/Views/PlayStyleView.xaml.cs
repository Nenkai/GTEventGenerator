using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PDTools.Enums;
using PDTools.Structures.MGameParameter;

using Humanizer;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for PlayStyleView.xaml
/// </summary>
public partial class PlayStyleView : UserControl
{
    public PlayStyle PlayStyle => DataContext as PlayStyle;

    public PlayStyleView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

    public void Refresh()
    {
        PopulateControls();
        if (PlayStyle is null)
            return;

        iud_WindowNumber.Value = PlayStyle.WindowNum;
        iud_TimeLimit.Value = PlayStyle.TimeLimit;
        iud_LeaveLimit.Value = PlayStyle.LeaveLimit;
        cb_NoQuickMenu.IsChecked = PlayStyle.NoQuickMenu;
        cb_NoInstantReplay.IsChecked = PlayStyle.NoInstantReplay;
        cb_ReplayRecordingEnable.IsChecked = PlayStyle.ReplayRecordEnable;
        cb_RentCarSettingEnable.IsChecked = PlayStyle.RentCarSettingEnable;
        cb_PlayType.SelectedIndex = (int)PlayStyle.PlayType;
        cb_BSpecType.SelectedIndex = (int)PlayStyle.BSpecType;
    }

    private void PopulateControls()
    {
        if (PlayStyle is null)
            return;

        if (cb_BSpecType.Items.Count == 0)
        {
            foreach (BSpecType type in Enum.GetValues<BSpecType>())
            {
                string s = type.Humanize();
                cb_BSpecType.Items.Add(s);
            }
        }

        if (cb_PlayType.Items.Count == 0)
        {
            foreach (PlayType type in Enum.GetValues<PlayType>())
            {
                string s = type.Humanize();
                cb_PlayType.Items.Add(s);
            }
        }
    }

    private void iud_WindowNumber_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        PlayStyle.WindowNum = iud_WindowNumber.Value.Value;
    }

    private void iud_TimeLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        PlayStyle.TimeLimit = iud_TimeLimit.Value.Value;
    }

    private void iud_LeaveLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        PlayStyle.LeaveLimit = iud_LeaveLimit.Value.Value;
    }

    private void cb_BSpecType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PlayStyle.BSpecType = (BSpecType)cb_BSpecType.SelectedIndex;
    }

    private void cb_PlayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PlayStyle.PlayType = (PlayType)cb_PlayType.SelectedIndex;
    }

    private void cb_NoQuickMenu_Checked(object sender, RoutedEventArgs e)
    {
        PlayStyle.NoQuickMenu = cb_NoQuickMenu.IsChecked.Value;
    }

    private void cb_NoInstantReplay_Checked(object sender, RoutedEventArgs e)
    {
        PlayStyle.NoInstantReplay = cb_NoInstantReplay.IsChecked.Value;
    }

    private void cb_ReplayRecordingEnable_Checked(object sender, RoutedEventArgs e)
    {
        PlayStyle.ReplayRecordEnable = cb_ReplayRecordingEnable.IsChecked.Value;
    }

    private void cb_RentCarSettingEnable_Checked(object sender, RoutedEventArgs e)
    {
        PlayStyle.RentCarSettingEnable = cb_RentCarSettingEnable.IsChecked.Value;
    }
}
