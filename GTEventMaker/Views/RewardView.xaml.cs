using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;

using PDTools.Structures.MGameParameter;
using PDTools.Enums;
using PDTools.Enums.PS3;

using Humanizer;

using GTEventMaker.Database;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for RewardView.xaml
/// </summary>
public partial class RewardView : UserControl
{
    public Reward Reward => DataContext as Reward;
    public GameDB GameDatabase { get; set; }

    public RewardView()
    {
        GameDatabase = App.GameDatabase;

        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Populate();
    }

    private void cb_RewardParticipationType_SelectionChanged(object sender, RoutedEventArgs e)
        => Reward.EntryPresentType = (RewardEntryPresentType)cb_RewardParticipationType.SelectedIndex;

    private void cb_RewardPlacementType_SelectionChanged(object sender, RoutedEventArgs e)
        => Reward.PresentType = (RewardPresentType)cb_RewardPlacementType.SelectedIndex;

    private void btnCreditRewards_Click(object sender, RoutedEventArgs e)
    {
        var list = new List<int>(16);
        for (var i = 0; i < 16; i++)
            list.Add(0);

        CollectionsMarshal.AsSpan(Reward.PrizeTable).CopyTo(CollectionsMarshal.AsSpan(list));
        var dlg = new CreditXPEditWindow(list);
        dlg.Owner = Window.GetWindow(this);
        dlg.ShowDialog();

        if (dlg.Saved)
            Reward.PrizeTable = dlg.Values;
    }

    private void btnXPRewards_Click(object sender, RoutedEventArgs e)
    {
        var list = new List<int>(16);
        for (var i = 0; i < 16; i++)
            list.Add(0);

        CollectionsMarshal.AsSpan(Reward.PrizeTable).CopyTo(CollectionsMarshal.AsSpan(list));
        var dlg = new CreditXPEditWindow(list);
        dlg.Owner = Window.GetWindow(this);
        dlg.ShowDialog();

        if (dlg.Saved)
            Reward.PointTable = dlg.Values;
    }

    private void btn_StarRewards_Click(object sender, RoutedEventArgs e)
    {
        var list = new List<FinishResult>(16);
        for (var i = 0; i < 16; i++)
            list.Add(FinishResult.NONE);

        CollectionsMarshal.AsSpan(Reward.StarTable).CopyTo(CollectionsMarshal.AsSpan(list));
        var dlg = new StarTableEditWindow(list);
        dlg.Owner = Window.GetWindow(this);
        dlg.ShowDialog();

        if (dlg.Saved)
            Reward.StarTable = dlg.Values;
    }

    private void btn_Present1_Click(object sender, RoutedEventArgs e)
    {
        if (Reward.Present.Count == 0)
            Reward.Present.Add(new EventPresent());

        var window = new PresentPickerWindow(App.GameDatabase, Reward.Present[0]);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();

        btn_Present1.Content = PresentToString(Reward.Present.Count >= 1 ? Reward.Present[0] : null);
    }

    private void btn_Present2_Click(object sender, RoutedEventArgs e)
    {
        while (Reward.Present.Count < 2)
            Reward.Present.Add(new EventPresent());

        var window = new PresentPickerWindow(App.GameDatabase, Reward.Present[1]);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();

        btn_Present2.Content = PresentToString(Reward.Present.Count >= 2 ? Reward.Present[1] : null);
    }

    private void btn_Present3_Click(object sender, RoutedEventArgs e)
    {
        while (Reward.Present.Count < 3)
            Reward.Present.Add(new EventPresent());

        var window = new PresentPickerWindow(App.GameDatabase, Reward.Present[2]);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();

        btn_Present3.Content = PresentToString(Reward.Present.Count >= 3 ? Reward.Present[2] : null);
    }

    private void btn_EntryPresent1_Click(object sender, RoutedEventArgs e)
    {
        if (Reward.EntryPresent.Count == 0)
            Reward.EntryPresent.Add(new EventPresent());

        var window = new PresentPickerWindow(App.GameDatabase, Reward.EntryPresent[0]);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();

        btn_Present1.Content = PresentToString(Reward.EntryPresent.Count >= 1 ? Reward.EntryPresent[0] : null);
    }

    private void btn_EntryPresent2_Click(object sender, RoutedEventArgs e)
    {
        while (Reward.EntryPresent.Count < 2)
            Reward.EntryPresent.Add(new EventPresent());

        var window = new PresentPickerWindow(App.GameDatabase, Reward.EntryPresent[1]);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();

        btn_Present2.Content = PresentToString(Reward.EntryPresent.Count >= 2 ? Reward.EntryPresent[1] : null);
    }

    private void btn_EntryPresent3_Click(object sender, RoutedEventArgs e)
    {
        while (Reward.EntryPresent.Count < 3)
            Reward.EntryPresent.Add(new EventPresent());

        var window = new PresentPickerWindow(App.GameDatabase, Reward.EntryPresent[2]);
        window.Owner = Window.GetWindow(this);
        window.ShowDialog();

        btn_Present3.Content = PresentToString(Reward.EntryPresent.Count >= 3 ? Reward.EntryPresent[2] : null);
    }

    public string PresentToString(EventPresent present)
    {
        if (present is null)
            return "No Present Selected";

        return $"{present.TypeID} / {present.CategoryID}";
    }

    public void PopulateOneTimeRewardControls()
    {
        if (cb_RewardPlacementType.Items.Count == 0)
        {
            var types = (RewardPresentType[])Enum.GetValues(typeof(RewardPresentType));
            for (int i = 0; i < types.Length; i++)
            {
                var t = (RewardPresentType)i;
                string tName = t.Humanize();
                cb_RewardPlacementType.Items.Add(tName);
            }

            var pTypes = (RewardEntryPresentType[])Enum.GetValues(typeof(RewardEntryPresentType));
            for (int i = 0; i < pTypes.Length; i++)
            {
                var t = (RewardEntryPresentType)i;
                string tName = t.Humanize();
                cb_RewardParticipationType.Items.Add(tName);
            }
        }
    }

    public void Populate()
    {
        PopulateOneTimeRewardControls();
        cb_RewardPlacementType.SelectedIndex = (int)Reward.PresentType;
        cb_RewardParticipationType.SelectedIndex = (int)Reward.EntryPresentType;

        btn_Present1.Content = PresentToString(Reward.Present.Count >= 1 ? Reward.Present[0] : null);
        btn_Present2.Content = PresentToString(Reward.Present.Count >= 2 ? Reward.Present[1] : null);
        btn_Present3.Content = PresentToString(Reward.Present.Count >= 3 ? Reward.Present[2] : null);

        btn_EntryPresent1.Content = PresentToString(Reward.EntryPresent.Count >= 1 ? Reward.EntryPresent[0] : null);
        btn_EntryPresent2.Content = PresentToString(Reward.EntryPresent.Count >= 2 ? Reward.EntryPresent[1] : null);
        btn_EntryPresent3.Content = PresentToString(Reward.EntryPresent.Count >= 3 ? Reward.EntryPresent[2] : null);
    }
}
