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

using Humanizer;

using PDTools.Structures.MGameParameter;
using PDTools.Enums;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for ConstraintView.xaml
/// </summary>
public partial class ConstraintView : UserControl
{
    public Constraint Constraints => DataContext as Constraint;

    public ConstraintView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

    #region Tire Constraints
    private void comboBox_ConstrainedTiresMinF_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        if (cb.SelectedIndex == -1)
            return;

        Constraints.NeedTireFront = (TireType)(cb.SelectedIndex - 1);
    }

    private void comboBox_ConstrainedTiresMinR_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        if (cb.SelectedIndex == -1)
            return;

        Constraints.NeedTireRear = (TireType)(cb.SelectedIndex - 1);
    }

    private void comboBox_ConstrainedTiresMaxF_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        if (cb.SelectedIndex == -1)
            return;

        Constraints.LimitTireFront = (TireType)(cb.SelectedIndex - 1);
    }

    private void comboBox_ConstrainedTiresMaxR_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        if (cb.SelectedIndex == -1)
            return;

        Constraints.LimitTireRear = (TireType)(cb.SelectedIndex - 1);
    }

    private void comboBox_ConstrainedTiresSuggestF_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        if (cb.SelectedIndex == -1)
            return;

        Constraints.SuggestTireFront = (TireType)(cb.SelectedIndex - 1);
    }

    private void comboBox_ConstrainedTiresSuggestR_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        if (cb.SelectedIndex == -1)
            return;

        Constraints.SuggestTireRear = (TireType)(cb.SelectedIndex - 1);
    }

    private void iud_Transmission_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.Transmission = iud_Transmission.Value.Value;
    }

    private void iud_SkidRecoveryForce_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.Simulation_SkidRecoveryForce = iud_SkidRecoveryForce.Value.Value;
    }

    private void iud_ASM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.ASM = iud_ASM.Value.Value;

    }

    private void iud_ActiveSteering_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.ActiveSteering = iud_ActiveSteering.Value.Value;

    }

    private void iud_ABS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.ABS = iud_ABS.Value.Value;

    }

    private void iud_InCarView_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.InCarView = iud_InCarView.Value.Value;
    }

    private void iud_TCS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.TCS = iud_TCS.Value.Value;
    }


    private void iud_PowerLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.PowerRestrictorLimit = iud_PowerLimit.Value.Value;
    }

    private void iud_DrivingLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.DrivingLine = iud_DrivingLine.Value.Value;
    }

    private void iud_SuggestedGear_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Constraints.SuggestedGear = iud_SuggestedGear.Value.Value;
    }
    #endregion

    public void Refresh()
    {
        PopulateControls();
        PopulateConstraints();
    }

    public void PopulateConstraints()
    {
        if (Constraints is null)
            return;

        comboBox_ConstrainedTiresMaxF.SelectedIndex = (int)Constraints.LimitTireFront + 1;
        comboBox_ConstrainedTiresMaxR.SelectedIndex = (int)Constraints.LimitTireRear + 1;
        comboBox_ConstrainedTiresMinF.SelectedIndex = (int)Constraints.NeedTireFront + 1;
        comboBox_ConstrainedTiresMinR.SelectedIndex = (int)Constraints.NeedTireRear + 1;
        comboBox_ConstrainedTiresSuggestF.SelectedIndex = (int)Constraints.SuggestTireFront + 1;
        comboBox_ConstrainedTiresSuggestR.SelectedIndex = (int)Constraints.SuggestTireRear + 1;

        iud_PowerLimit.Value = Constraints.PowerRestrictorLimit;
        iud_Transmission.Value = Constraints.Transmission;
        iud_SkidRecoveryForce.Value = Constraints.Simulation_SkidRecoveryForce;
        iud_ASM.Value = Constraints.ASM;
        iud_ActiveSteering.Value = Constraints.ActiveSteering;
        iud_ABS.Value = Constraints.ABS;
        iud_InCarView.Value = Constraints.InCarView;
        iud_TCS.Value = Constraints.TCS;
        iud_DrivingLine.Value = Constraints.DrivingLine;
        iud_SuggestedGear.Value = Constraints.SuggestedGear;

    }

    private void PopulateControls()
    {
        if (comboBox_ConstrainedTiresMinF.Items.Count == 1)
        {
            var tires = (TireType[])Enum.GetValues(typeof(TireType));
            for (int i = 0; i < tires.Length - 1; i++) // -1 as the combo boxes have a default "none" entry
            {
                var tire = (TireType)i;
                string tireName = tire.Humanize();
                comboBox_ConstrainedTiresMinF.Items.Add(tireName);
                comboBox_ConstrainedTiresMaxR.Items.Add(tireName);
                comboBox_ConstrainedTiresMinR.Items.Add(tireName);
                comboBox_ConstrainedTiresMaxF.Items.Add(tireName);
                comboBox_ConstrainedTiresSuggestF.Items.Add(tireName);
                comboBox_ConstrainedTiresSuggestR.Items.Add(tireName);
            }
        }
    }
}
