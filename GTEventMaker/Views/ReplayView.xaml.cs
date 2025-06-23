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

using PDTools.Enums;
using PDTools.Structures.MGameParameter;
using Humanizer;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for RankingView.xaml
/// </summary>
public partial class ReplayView : UserControl
{
    public Replay Replay => DataContext as Replay;

    public ReplayView()
    {
        InitializeComponent();
    }

    public void PopulateOneTimeInfoControls()
    {
        if (cb_ReplayRecordingQuality.Items.Count > 0)
            return; // Assume it was already all filled in

        var types = (ReplayRecordingQuality[])Enum.GetValues(typeof(ReplayRecordingQuality));
        for (int i = 0; i < types.Length; i++)
        {
            var comp = (ReplayRecordingQuality)i;
            string compName = comp.Humanize();
            cb_ReplayRecordingQuality.Items.Add(compName);
        }
    }


    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Populate();
    }

    public void PopulateEventInfoTab()
    {
        tb_LocalPath.Text = Replay.LocalPath;
        tb_Url.Text = Replay.Url;
        tb_DemoDataPath.Text = Replay.DemoDataPath;
        cb_ReplayRecordingQuality.SelectedIndex = (int)Replay.ReplayRecordingQuality;
        cb_AutoSave.IsChecked = Replay.AutoSave;
    }

    private void Populate()
    {
        PopulateOneTimeInfoControls();
        PopulateEventInfoTab();
    }

    private void tb_LocalPath_TextChanged(object sender, TextChangedEventArgs e)
    {
        Replay.LocalPath = tb_LocalPath.Text;
    }

    private void tb_Url_TextChanged(object sender, TextChangedEventArgs e)
    {
        Replay.Url = tb_Url.Text;
    }

    private void cb_ReplayRecordingQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Replay.ReplayRecordingQuality = (ReplayRecordingQuality)cb_ReplayRecordingQuality.SelectedIndex;
    }

    private void cb_AutoSave_Checked(object sender, RoutedEventArgs e)
    {
        Replay.AutoSave = cb_AutoSave.IsChecked.Value;
    }

    private void tb_DemoDataPath_TextChanged(object sender, TextChangedEventArgs e)
    {
        Replay.DemoDataPath = tb_DemoDataPath.Text;
    }
}
