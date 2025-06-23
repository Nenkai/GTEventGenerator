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

using PDTools.Structures.MGameParameter;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for FailConditionView.xaml
/// </summary>
public partial class FailConditionView : UserControl
{
    public FailureCondition FailConditions => DataContext as FailureCondition;

    public FailConditionView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

    private void btn_AddFailCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_AvailableFailConditions.SelectedIndex == -1)
            return;

        for (var i = lb_AvailableFailConditions.SelectedItems.Count - 1; i >= 0; i--)
        {
            FailCondition condition = (FailCondition)lb_AvailableFailConditions.SelectedItems[i];

            lb_AvailableFailConditions.Items.Remove(condition);
            lb_CurrentFailConditions.Items.Add(condition);

            FailConditions.FailConditions.Add(condition);
        }
    }

    private void btn_RemoveFailCondition_Click(object sender, RoutedEventArgs e)
    {
        if (lb_CurrentFailConditions.SelectedIndex == -1)
            return;

        for (var i = lb_CurrentFailConditions.SelectedItems.Count - 1; i >= 0; i--)
        {
            FailCondition condition = (FailCondition)lb_CurrentFailConditions.SelectedItems[i];

            lb_AvailableFailConditions.Items.Add(condition);
            lb_CurrentFailConditions.Items.Remove(condition);

            FailConditions.FailConditions.Remove(condition);
        }
    }

    private void cb_NoFailureAtResult_Checked(object sender, RoutedEventArgs e)
    {
        FailConditions.NoFailureAtResult = cb_NoFailureAtResult.IsChecked.Value;
    }

    public void Refresh()
    {
        if (FailConditions is null)
            return;

        lb_CurrentFailConditions.Items.Clear();
        lb_AvailableFailConditions.Items.Clear();

        foreach (FailCondition cond in Enum.GetValues<FailCondition>())
        {
            if (cond == FailCondition.NONE || cond == FailCondition.MAX)
                continue;

            if (FailConditions.FailConditions.Contains(cond))
                lb_CurrentFailConditions.Items.Add(cond);
            else
                lb_AvailableFailConditions.Items.Add(cond);
        }

        cb_NoFailureAtResult.IsChecked = FailConditions.NoFailureAtResult;
    }
}
