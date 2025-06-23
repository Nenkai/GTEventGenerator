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
public partial class RankingView : UserControl
{
    public Ranking Ranking => DataContext as Ranking;

    public RankingView()
    {
        InitializeComponent();
    }

    public void PopulateOneTimeInfoControls()
    {
        if (cb_RankingType.Items.Count > 0)
            return; // Assume it was already all filled in

        var types = (RankingType[])Enum.GetValues(typeof(RankingType));
        for (int i = 0; i < types.Length; i++)
        {
            var comp = (RankingType)i;
            string compName = comp.Humanize();
            cb_RankingType.Items.Add(compName);
        }

        RegistrationType[] rtypes = (RegistrationType[])Enum.GetValues(typeof(RegistrationType));
        for (int i = 0; i < rtypes.Length; i++)
        {
            var comp = (RegistrationType)i;
            string compName = comp.Humanize();
            cb_RegistrationType.Items.Add(compName);
        }
    }


    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Populate();
    }

    private void cb_RankingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Ranking.Type = (RankingType)cb_RankingType.SelectedIndex;
    }

    private void cb_IsLocal_Checked(object sender, RoutedEventArgs e)
    {
        Ranking.IsLocal = cb_IsLocal.IsChecked.Value;
    }

    private void iud_ReplayRankLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Ranking.ReplayRankLimit = iud_ReplayRankLimit.Value.Value;
    }

    private void iud_DisplayRankLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Ranking.DisplayRankLimit = iud_DisplayRankLimit.Value.Value;
    }

    private void cb_RegistrationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Ranking.RegistrationType = (RegistrationType)cb_RegistrationType.SelectedIndex;
    }

    private void iud_BoardID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Ranking.BoardID = iud_BoardID.Value.Value;
    }

    private void iud_RegistrationID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Ranking.Registration = iud_RegistrationID.Value.Value;
    }

    private void dt_StartDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Ranking.BeginDate = dt_StartDate.Value;
    }

    private void dt_EndDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Ranking.EndDate = dt_EndDate.Value;
    }

    public void PopulateEventInfoTab()
    {
        cb_RankingType.SelectedIndex = (int)Ranking.Type;
        cb_IsLocal.IsChecked = Ranking.IsLocal;
        iud_ReplayRankLimit.Value = Ranking.ReplayRankLimit;
        iud_DisplayRankLimit.Value = Ranking.DisplayRankLimit;
        cb_RegistrationType.SelectedIndex = (int)Ranking.RegistrationType;
        iud_BoardID.Value = Ranking.BoardID;
        iud_RegistrationID.Value = Ranking.Registration;
        dt_StartDate.Value = Ranking.BeginDate;
        dt_EndDate.Value = Ranking.EndDate;
    }

    private void Populate()
    {
        PopulateOneTimeInfoControls();
        PopulateEventInfoTab();
    }
}
