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
using System.Runtime.InteropServices;

using PDTools.Structures.MGameParameter;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for FolderView.xaml
/// </summary>
public partial class FolderView : UserControl
{
    public GameParameter GameParameter { get; set; }
    public EventListFolder Folder { get; set; }

    public List<EventCategory> KnownCategories { get; set; } = new();
    public record EventCategory(string Name, int TypeID);

    public FolderView(GameParameter gp, EventListFolder folder)
    {
        InitializeComponent();

        GameParameter = gp;
        Folder = folder;
    }

    private void chkIsChampionship_CheckedChanged(object sender, EventArgs e)
    {
        btnChampionshipRewards.IsEnabled = chkIsChampionship.IsChecked.Value;
        GameParameter.Championship = chkIsChampionship.IsChecked.Value;

    }

    private void iud_FolderID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (GameParameter is null)
            return;

        var iud = ((Xceed.Wpf.Toolkit.ULongUpDown)sender);
        if (iud.Value is null)
            iud.Value = (ulong)e.OldValue;

        GameParameter.FolderId = iud.Value.Value;
    }

    public void btn_ChampionshipRewards_Click(object sender, RoutedEventArgs e)
    {
        var list = new List<int>(16);
        for (var i = 0; i < 16; i++)
            list.Add(0);

        CollectionsMarshal.AsSpan(GameParameter.SeriesRewards.PrizeTable).CopyTo(CollectionsMarshal.AsSpan(list));

        var dlg = new CreditXPEditWindow(list);
        dlg.Owner = Window.GetWindow(this);
        dlg.ShowDialog();

        if (dlg.Saved)
            GameParameter.SeriesRewards.PrizeTable = dlg.Values;
    }

    private void iud_FolderType_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Folder.Type = iud_FolderType.Value.Value;
    }

    public void Populate()
    {
        iud_FolderID.Value = GameParameter.FolderId;
        chkIsChampionship.IsChecked = GameParameter.Championship;

        iud_FolderType.Value = Folder.Type;
    }
}
