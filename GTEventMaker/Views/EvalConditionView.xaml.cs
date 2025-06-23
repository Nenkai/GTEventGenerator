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

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for EvalConditionView.xaml
/// </summary>
public partial class EvalConditionView : UserControl
{
    public EvalCondition EvalConditions => DataContext as EvalCondition;

    public EvalConditionView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }


    private void cb_EvalType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (EvalConditions != null)
            EvalConditions.ConditionType = (EvalConditionType)cb_EvalType.SelectedIndex;

        bool isNone = EvalConditions.ConditionType == EvalConditionType.NONE;
        iud_EvalGold.IsEnabled = !isNone;
        iud_EvalSilver.IsEnabled = !isNone;
        iud_EvalBronze.IsEnabled = !isNone;

        if (isNone)
        {
            EvalConditions.Bronze = 0;
            EvalConditions.Silver = 0;
            EvalConditions.Gold = 0;
        }
    }


    private void iud_EvalGold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EvalConditions.Gold = iud_EvalGold.Value.Value;
    }

    private void iud_EvalSilver_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EvalConditions.Silver = iud_EvalSilver.Value.Value;
    }

    private void iud_EvalBronze_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EvalConditions.Bronze = iud_EvalBronze.Value.Value;
    }

    private void tb_GhostDataPath_TextChanged(object sender, TextChangedEventArgs e)
    {
        EvalConditions.GhostDataPath = tb_GhostDataPath.Text;
    }

    public void Refresh()
    {
        PopulateControls();
        if (EvalConditions is null)
            return;

        cb_EvalType.SelectedIndex = (int)EvalConditions.ConditionType;
        iud_EvalGold.Value = EvalConditions.Gold;
        iud_EvalSilver.Value = EvalConditions.Silver;
        iud_EvalBronze.Value = EvalConditions.Bronze;
        tb_GhostDataPath.Text = EvalConditions.GhostDataPath;
    }

    private void PopulateControls()
    {
        if (cb_EvalType.Items.Count == 0)
        {
            var types = (EvalConditionType[])Enum.GetValues(typeof(EvalConditionType));
            for (int i = 0; i < types.Length; i++)
            {
                var t = (EvalConditionType)i;
                string tName = t.Humanize();
                cb_EvalType.Items.Add(tName);
            }
        }
    }
}
