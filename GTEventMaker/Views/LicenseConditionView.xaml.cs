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
/// Interaction logic for LicenseConditionView.xaml
/// </summary>
public partial class LicenseConditionView : UserControl
{
    public LicenseCondition LicenseCondition => DataContext as LicenseCondition;
    public LicenseConditionData Data { get; set; } = new LicenseConditionData();

    public LicenseConditionView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Populate();
    }

    private void btn_Check_Click(object sender, RoutedEventArgs e)
    {
        ContextMenu cm = new ContextMenu();
        cm.PlacementTarget = sender as Button;
        cm.IsOpen = true;

        foreach (LicenseCheckType i in Enum.GetValues<LicenseCheckType>())
        {
            var menuItem = new MenuItem();
            menuItem.Header = i.Humanize();
            menuItem.Click += mi_Check_Click;

            cm.Items.Add(menuItem);
        }
    }

    void mi_Check_Click(object sender, RoutedEventArgs e)
    {
        MenuItem mi = sender as MenuItem;
        btn_Check.Content = mi.Header;
        Data.CheckType = (mi.Header as string).DehumanizeTo<LicenseCheckType>();

        CheckCanAdd();
    }

    private void btn_Condition_Click(object sender, RoutedEventArgs e)
    {
        ContextMenu cm = new ContextMenu();
        cm.PlacementTarget = sender as Button;
        cm.IsOpen = true;

        foreach (LicenseConditionType i in Enum.GetValues<LicenseConditionType>())
        {
            var menuItem = new MenuItem();
            menuItem.Header = i.Humanize();
            menuItem.Click += mi_Condition_Click;

            cm.Items.Add(menuItem);
        }
    }

    void mi_Condition_Click(object sender, RoutedEventArgs e)
    {
        MenuItem mi = sender as MenuItem;
        btn_Condition.Content = mi.Header;
        Data.Condition = (mi.Header as string).DehumanizeTo<LicenseConditionType>();

        CheckCanAdd();
    }

    private void btn_Connection_Click(object sender, RoutedEventArgs e)
    {
        ContextMenu cm = new ContextMenu();
        cm.PlacementTarget = sender as Button;
        cm.IsOpen = true;

        foreach (LicenseConnectionType i in Enum.GetValues<LicenseConnectionType>())
        {
            var menuItem = new MenuItem();
            menuItem.Header = i.Humanize();
            menuItem.Click += mi_Connection_Click;

            cm.Items.Add(menuItem);
        }
    }

    void mi_Connection_Click(object sender, RoutedEventArgs e)
    {
        MenuItem mi = sender as MenuItem;
        btn_Connection.Content = mi.Header;
        Data.Connection = (mi.Header as string).DehumanizeTo<LicenseConnectionType>();

        CheckCanAdd();
    }

    private void btn_ResultType_Click(object sender, RoutedEventArgs e)
    {
        ContextMenu cm = new ContextMenu();
        cm.PlacementTarget = sender as Button;
        cm.IsOpen = true;

        foreach (LicenseResultType i in Enum.GetValues<LicenseResultType>())
        {
            var menuItem = new MenuItem();
            menuItem.Header = i.Humanize();
            menuItem.Click += mi_ResultType_Click;

            cm.Items.Add(menuItem);
        }
    }

    void mi_ResultType_Click(object sender, RoutedEventArgs e)
    {
        MenuItem mi = sender as MenuItem;
        btn_ResultType.Content = mi.Header;
        Data.ResultType = (mi.Header as string).DehumanizeTo<LicenseResultType>();

        CheckCanAdd();
    }


    private void iud_IntValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Data.IntValue = iud_IntValue.Value.Value;
    }

    private void iud_UIntValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Data.UIntValue = iud_UIntValue.Value.Value;
    }

    private void iud_FloatValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Data.FloatValue = iud_FloatValue.Value.Value;
    }

    private void btn_AddFinishCondition_Click(object sender, RoutedEventArgs e)
    {
        lb_FinishConditions.Items.Add(Data);
        LicenseCondition.FinishCondition.Add(Data);
    }

    private void btn_AddSuccessCondition_Click(object sender, RoutedEventArgs e)
    {
        lb_SuccessConditions.Items.Add(Data);
        LicenseCondition.SuccessCondition.Add(Data);
    }

    private void btn_AddFailureCondition_Click(object sender, RoutedEventArgs e)
    {
        lb_FailureConditions.Items.Add(Data);
        LicenseCondition.FailureCondition.Add(Data);
    }

    private void cb_DisplayMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LicenseCondition.DisplayMode = (LicenseDisplayModeType)cb_DisplayMode.SelectedIndex;
    }

    private void cb_UseBasicFinish_Checked(object sender, RoutedEventArgs e)
    {
        LicenseCondition.UseBasicFinish = cb_UseBasicFinish.IsChecked.Value;
    }

    private void cb_StopOnFinish_Checked(object sender, RoutedEventArgs e)
    {
        LicenseCondition.StopOnFinish = cb_StopOnFinish.IsChecked.Value;
    }


    private void btn_AddGadgetName_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(tb_GadgetName.Text))
            return;

        lb_GadgetNames.Items.Add(tb_GadgetName.Text);
        LicenseCondition.GadgetNames.Add(tb_GadgetName.Text);
    }

    private void btn_RemoveGadgetName_Click(object sender, RoutedEventArgs e)
    {
        if (lb_GadgetNames.SelectedIndex == -1)
            return;

        LicenseCondition.GadgetNames.RemoveAt(lb_GadgetNames.SelectedIndex);
        lb_GadgetNames.Items.Remove(lb_GadgetNames.SelectedItem);
    }

    private void lb_FinishConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_FinishConditions.SelectedIndex == -1)
            return;

        LicenseConditionData data = lb_FinishConditions.SelectedItem as LicenseConditionData;
        PopulateData(data);
        CheckCanAdd();
    }

    private void lb_SuccessConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_SuccessConditions.SelectedIndex == -1)
            return;

        LicenseConditionData data = lb_SuccessConditions.SelectedItem as LicenseConditionData;
        PopulateData(data);
        CheckCanAdd();
    }

    private void lb_FailureConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_FailureConditions.SelectedIndex == -1)
            return;

        LicenseConditionData data = lb_FailureConditions.SelectedItem as LicenseConditionData;
        PopulateData(data);
        CheckCanAdd();
    }

    private void btn_RemoveFinishCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_FinishConditions.SelectedIndex == -1)
            return;

        LicenseCondition.FinishCondition.RemoveAt(lb_FinishConditions.SelectedIndex);
        lb_FinishConditions.Items.Remove(lb_FinishConditions.Items[lb_FinishConditions.SelectedIndex]);
    }

    private void btn_RemoveSuccessCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_SuccessConditions.SelectedIndex == -1)
            return;

        LicenseCondition.SuccessCondition.RemoveAt(lb_SuccessConditions.SelectedIndex);
        lb_SuccessConditions.Items.Remove(lb_SuccessConditions.Items[lb_SuccessConditions.SelectedIndex]);
    }

    private void btn_RemoveFailureCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_FailureConditions.SelectedIndex == -1)
            return;

        LicenseCondition.FailureCondition.RemoveAt(lb_FailureConditions.SelectedIndex);
        lb_FailureConditions.Items.Remove(lb_FailureConditions.Items[lb_FailureConditions.SelectedIndex]);
    }

    private void btn_UpdateFinishCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_FinishConditions.SelectedIndex == -1)
            return;

        LicenseCondition.FinishCondition[lb_FinishConditions.SelectedIndex] = Data;
        lb_FinishConditions.Items[lb_FinishConditions.SelectedIndex] = Data;
        lb_FinishConditions.Items.Refresh();
    }

    private void btn_UpdateSuccessCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_SuccessConditions.SelectedIndex == -1)
            return;

        LicenseCondition.SuccessCondition[lb_SuccessConditions.SelectedIndex] = Data;
        lb_SuccessConditions.Items[lb_SuccessConditions.SelectedIndex] = Data;
        lb_SuccessConditions.Items.Refresh();
    }

    private void btn_UpdateFailureCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_FailureConditions.SelectedIndex == -1)
            return;

        LicenseCondition.FailureCondition[lb_FailureConditions.SelectedIndex] = Data;
        lb_FailureConditions.Items[lb_FailureConditions.SelectedIndex] = Data;
        lb_FailureConditions.Items.Refresh();
    }

    public void Populate()
    {
        PopulateOneTimeControls();

        lb_FailureConditions.Items.Clear();
        lb_SuccessConditions.Items.Clear();
        lb_FinishConditions.Items.Clear();

        cb_DisplayMode.SelectedIndex = (int)LicenseCondition.DisplayMode;

        foreach (string gadgetName in LicenseCondition.GadgetNames)
            lb_GadgetNames.Items.Add(gadgetName);

        foreach (LicenseConditionData data in LicenseCondition.FailureCondition)
            lb_FailureConditions.Items.Add(data);

        foreach (LicenseConditionData data in LicenseCondition.SuccessCondition)
            lb_SuccessConditions.Items.Add(data);

        foreach (LicenseConditionData data in LicenseCondition.FinishCondition)
            lb_FinishConditions.Items.Add(data);

        cb_UseBasicFinish.IsChecked = LicenseCondition.UseBasicFinish;
        cb_StopOnFinish.IsChecked = LicenseCondition.StopOnFinish;
    }

    public void PopulateOneTimeControls()
    {
        if (cb_DisplayMode.Items.Count == 0)
        {
            foreach (LicenseDisplayModeType i in Enum.GetValues<LicenseDisplayModeType>())
                cb_DisplayMode.Items.Add(i);
        }
    }

    private void PopulateData(LicenseConditionData data)
    {
        Data = data;

        btn_Check.Content = data.CheckType.Humanize();
        btn_Condition.Content = data.Condition.Humanize();
        btn_Connection.Content = data.Connection.Humanize();
        btn_ResultType.Content = data.ResultType.Humanize();

        iud_FloatValue.Value = data.FloatValue;
        iud_IntValue.Value = data.IntValue;
        iud_UIntValue.Value = data.UIntValue;
    }

    public void CheckCanAdd()
    {
        bool isInvalid = (string)btn_Check.Content == "Check" || (string)btn_Condition.Content == "Condition" || (string)btn_Connection.Content == "Connection";

        btn_AddFailureCondition.IsEnabled = !isInvalid;
        btn_AddFinishCondition.IsEnabled = !isInvalid;
        btn_AddSuccessCondition.IsEnabled = !isInvalid;
    }
}
