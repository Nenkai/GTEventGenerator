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

using PDTools.Enums;
using PDTools.Structures.MGameParameter;
using PDTools.Structures;

using GTEventMaker.Database;
using Humanizer;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for EntrySetView.xaml
/// </summary>
public partial class EntrySetView : UserControl
{
    public EntrySet EntrySet => this.DataContext as EntrySet;
    public GameDB GameDatabase { get; set; }

    public EntrySetView()
    {
        GameDatabase = App.GameDatabase;
        InitializeComponent();
    }

    public ObservableCollection<EntryWrapper> FixedEntries { get; set; } = new();
    public ObservableCollection<EntryBaseWrapper> GeneratedEntries { get; set; } = new();

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

    private void comboBox_AIManifacturerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateAIEntriesCarList();

    private void numericUpDown_BaseSkillMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.AISkill = numericUpDown_BaseSkillMax.Value.Value;
    }

    private void numericUpDown_CornerSkillMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.AISkillCornering = numericUpDown_CornerSkillMax.Value.Value;
    }

    private void numericUpDown_BrakeSkillMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.AISkillBraking = numericUpDown_BrakeSkillMax.Value.Value;
    }

    private void numericUpDown_AccelSkillMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.AISkillAccelerating = numericUpDown_AccelSkillMax.Value.Value;
    }

    private void numericUpDown_StartSkillMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.AISkillStarting = numericUpDown_StartSkillMax.Value.Value;
    }

    private void numericUpDown_AIRoughnessMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.AIRoughness = numericUpDown_AIRoughnessMax.Value.Value;
    }

    private void button_GenerateEntryBase_Clicked(object sender, RoutedEventArgs e)
    {
        if (listBox_AICarList.SelectedIndex == -1)
        {
            MessageBox.Show("Select a car first.", "No car selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        EntryBase raceEntry = GenerateEntryBase();
        EntrySet.EntryGenerate.EntryBaseArray.Add(raceEntry);

        var carName = GameDatabase.GetCarNameByLabel(raceEntry.Car.CarLabel);
        GeneratedEntries.Add(new EntryBaseWrapper() { EntryBase = raceEntry, Name = $"{raceEntry.DriverName} ({raceEntry.DriverRegion}) - {carName}" });

        grp_GeneratedEntryPool.IsEnabled = true;
    }

    private void button_GenerateFixedEntry_Clicked(object sender, RoutedEventArgs e)
    {
        if (listBox_AICarList.SelectedIndex == -1)
        {
            MessageBox.Show("Select a car first.", "No car selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Entry raceEntry = GenerateEntry();

        var carName = GameDatabase.GetCarNameByLabel(raceEntry.Car.CarLabel);
        FixedEntries.Add(new EntryWrapper() { Entry = raceEntry, Name = $"{raceEntry.DriverName} ({raceEntry.DriverRegion}) - {carName}" });
        EntrySet.Entries.Add(raceEntry);

        button_EditFixedEntry.IsEnabled = true;
        button_RemoveFixedEntry.IsEnabled = true;

        UpdateEntryControlVisibility();
    }

    private void button_EditAI_Clicked(object sender, RoutedEventArgs e)
    {
        if (listBox_EntryBases.SelectedIndex == -1)
            return;

        var entry = GeneratedEntries[listBox_EntryBases.SelectedIndex];
        var entryEdit = new EventEntryBaseEditWindow(entry.EntryBase);
        entryEdit.Owner = Window.GetWindow(this);
        entryEdit.ShowDialog();
    }

    private void button_EditFixedEntry_Clicked(object sender, RoutedEventArgs e)
    {
        if (listBox_FixedEntries.SelectedIndex == -1)
            return;

        var entry = FixedEntries[listBox_FixedEntries.SelectedIndex];
        var entryEdit = new EventEntryEditWindow(entry.Entry, GameDatabase.GetCarColorNumByLabel(entry.Entry.Car.CarLabel));
        entryEdit.Owner = Window.GetWindow(this);
        entryEdit.ShowDialog();
    }

    private void button_RemoveAI_Clicked(object sender, RoutedEventArgs e)
    {
        if (listBox_EntryBases.SelectedIndex == -1)
            return;

        // Build a list of entries to remove
        List<EntryBaseWrapper> toRemove = new List<EntryBaseWrapper>();
        foreach (var selected in listBox_EntryBases.SelectedItems)
        {
            int index = listBox_EntryBases.Items.IndexOf(selected);
            toRemove.Add(GeneratedEntries[index]);
        }

        foreach (EntryBaseWrapper entry in toRemove)
        {
            GeneratedEntries.Remove(entry);
            EntrySet.EntryGenerate.EntryBaseArray.Remove(entry.EntryBase);
        }

        int count = listBox_EntryBases.SelectedItems.Count;
        for (int i = count - 1; i >= 0; i--)
            listBox_EntryBases.Items.Remove(listBox_EntryBases.SelectedItems[i]);

        if (listBox_EntryBases.Items.Count == 0)
        {
            button_EditEntryBase.IsEnabled = false;
            button_RemoveEntryBase.IsEnabled = false;
        }
    }

    private void button_RemoveFixedAI_Clicked(object sender, RoutedEventArgs e)
    {
        if (listBox_FixedEntries.SelectedIndex == -1)
            return;

        // Build a list of entries to remove
        List<EntryWrapper> toRemove = new List<EntryWrapper>();
        foreach (var selected in listBox_FixedEntries.SelectedItems)
        {
            int index = listBox_FixedEntries.Items.IndexOf(selected);
            toRemove.Add(FixedEntries[index]);
        }

        foreach (EntryWrapper entry in toRemove)
        {
            FixedEntries.Remove(entry);
            EntrySet.Entries.Remove(entry.Entry);
        }

        int count = listBox_FixedEntries.SelectedItems.Count;
        for (int i = count - 1; i >= 0; i--)
            listBox_FixedEntries.Items.Remove(listBox_FixedEntries.SelectedItems[i]);

        if (listBox_FixedEntries.Items.Count == 0)
        {
            button_EditFixedEntry.IsEnabled = false;
            button_RemoveFixedEntry.IsEnabled = false;
        }

        UpdateEntryControlVisibility();
    }

    private void slider_EntryCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (EntrySet is null)
            return;

        EntrySet.EntryGenerate.EntryNum = (int)sl_EntryCount.Value;
        label_EntryCount.Content = EntrySet.EntryGenerate.EntryNum.ToString();
    }

    private void sl_EntryPlayerPos_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (EntrySet is null)
            return;

        EntrySet.EntryGenerate.PlayerPos = (int)sl_EntryPlayerPos.Value;
        label_PlayerPos.Content = $"#{EntrySet.EntryGenerate.PlayerPos}";
    }

    private void comboBox_entryGenerateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        EntrySet.EntryGenerate.GenerateType = (EntryGenerateType)(sender as ComboBox).SelectedIndex;

        bool canAddBaseEntries = EntrySet.EntryGenerate.GenerateType == EntryGenerateType.ENTRY_BASE_ORDER ||
                                EntrySet.EntryGenerate.GenerateType == EntryGenerateType.ENTRY_BASE_SHUFFLE;


        bool canAddToCarList = EntrySet.EntryGenerate.GenerateType == EntryGenerateType.SHUFFLE ||
                                EntrySet.EntryGenerate.GenerateType == EntryGenerateType.ORDER;

        grp_CarList.IsEnabled = canAddToCarList;

        button_EditEntryBase.IsEnabled = canAddBaseEntries;
        button_RemoveEntryBase.IsEnabled = canAddBaseEntries;
        button_GenerateEntryBase.IsEnabled = canAddBaseEntries;
        grp_GeneratedEntryPool.IsEnabled = canAddBaseEntries;
    }

    private void comboBox_EntrySortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        EntrySet.EntryGenerate.EnemySortType = (EnemySortType)(sender as ComboBox).SelectedIndex;
    }

    private void numericUpDown_GapForRollingDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        EntrySet.EntryGenerate.GapForStartRollingDistance = numericUpDown_GapForRollingDistance.Value.Value;
    }

    private void btn_RemoveFromCarList_Click(object sender, RoutedEventArgs e)
    {
        if (lb_CarList.SelectedIndex == -1)
            return;

        int count = lb_CarList.SelectedItems.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            string item = (string)lb_CarList.SelectedItems[i];
            MCarThin car = EntrySet.EntryGenerate.Cars.Find(e => e.CarLabel == item);

            lb_CarList.Items.Remove(item);
            EntrySet.EntryGenerate.Cars.Remove(car);
        }
    }

    private void btn_AddToCarList_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(tb_CarListTextBox.Text))
            return;

        if (EntrySet.EntryGenerate.Cars.Find(e => e.CarLabel == tb_CarListTextBox.Text) != null)
            return;

        lb_CarList.Items.Add(tb_CarListTextBox.Text);
        EntrySet.EntryGenerate.Cars.Add(new MCarThin(tb_CarListTextBox.Text));
    }

    // !-- End of generated methods

    public void UpdateEntryControlVisibility()
    {
        sl_EntryCount.Value = EntrySet.EntryGenerate.EntryNum;
        sl_EntryPlayerPos.Maximum = 16;
        sl_EntryPlayerPos.Value = EntrySet.EntryGenerate.PlayerPos;
        label_EntryCount.Content = EntrySet.EntryGenerate.EntryNum.ToString();
        label_PlayerPos.Content = $"#{EntrySet.EntryGenerate.PlayerPos}";
    }

    public void Refresh()
    {
        PopulateControls();

        if (EntrySet is null)
            return;

        sl_EntryCount.Value = EntrySet.EntryGenerate.EntryNum;
        sl_EntryPlayerPos.Value = EntrySet.EntryGenerate.PlayerPos;
        label_EntryCount.Content = EntrySet.EntryGenerate.EntryNum.ToString();
        label_PlayerPos.Content = $"#{EntrySet.EntryGenerate.PlayerPos}";

        comboBox_entryGenerateType.SelectedIndex = (int)EntrySet.EntryGenerate.GenerateType;
        comboBox_EntrySortType.SelectedIndex = (int)EntrySet.EntryGenerate.EnemySortType;

        numericUpDown_AIRoughnessMax.Value = EntrySet.EntryGenerate.AIRoughness;
        numericUpDown_BaseSkillMax.Value = EntrySet.EntryGenerate.AISkill;
        numericUpDown_CornerSkillMax.Value = EntrySet.EntryGenerate.AISkillCornering;
        numericUpDown_BrakeSkillMax.Value = EntrySet.EntryGenerate.AISkillBraking;
        numericUpDown_AccelSkillMax.Value = EntrySet.EntryGenerate.AISkillAccelerating;
        numericUpDown_StartSkillMax.Value = EntrySet.EntryGenerate.AISkillStarting;
        numericUpDown_AIRoughnessMax.Value = EntrySet.EntryGenerate.AIRoughness;
        numericUpDown_GapForRollingDistance.Value = EntrySet.EntryGenerate.GapForStartRollingDistance;

        GeneratedEntries.Clear();
        FixedEntries.Clear();

        foreach (EntryBase entry in EntrySet.EntryGenerate.EntryBaseArray)
        {
            var carName = GameDatabase.GetCarNameByLabel(entry.Car.CarLabel);
            GeneratedEntries.Add(new EntryBaseWrapper() { EntryBase = entry, Name = $"{entry.DriverName} ({entry.DriverRegion}) - {carName}" });
        }

        foreach (Entry entry in EntrySet.Entries)
        {
            string carLabel = entry.Car.CarLabel;
            if (string.IsNullOrEmpty(carLabel)) // r180 GT6
                carLabel = entry.EntryBase.Car.CarLabel;

            var carName = GameDatabase.GetCarNameByLabel(carLabel);
            FixedEntries.Add(new EntryWrapper() { Entry = entry, Name = $"{entry.DriverName} ({entry.DriverRegion}) - {carName}" });
        }

        foreach (MCarThin car in EntrySet.EntryGenerate.Cars)
        {
            lb_CarList.Items.Add(car.CarLabel);
        }

        if (EntrySet.Entries.Count > 0)
        {
            button_EditFixedEntry.IsEnabled = true;
            button_RemoveFixedEntry.IsEnabled = true;
        }

        if (EntrySet.EntryGenerate.EntryBaseArray.Count > 0)
        {
            grp_GeneratedEntryPool.IsEnabled = true;
        }

        UpdateEntryControlVisibility();
    }

    private void PopulateControls()
    {
        listBox_EntryBases.ItemsSource = GeneratedEntries;
        listBox_FixedEntries.ItemsSource = FixedEntries;

        if (comboBox_EntryGenTyreComp.Items.Count == 1)
        {
            var tires = (TireType[])Enum.GetValues(typeof(TireType));
            for (int i = 0; i < tires.Length - 1; i++) // -1 as the combo boxes have a default "none" entry
            {
                var tire = (TireType)i;
                string tireName = tire.Humanize();
                comboBox_EntryGenTyreComp.Items.Add(tireName);
            }

            comboBox_EntryGenTyreComp.SelectedIndex = 0;
        }

        if (comboBox_entryGenerateType.Items.Count == 0)
        {
            foreach (var type in (EntryGenerateType[])Enum.GetValues(typeof(EntryGenerateType)))
                comboBox_entryGenerateType.Items.Add(type.Humanize());
        }

        if (comboBox_EntrySortType.Items.Count == 0)
        {
            foreach (var type in (EnemySortType[])Enum.GetValues(typeof(EnemySortType)))
                comboBox_EntrySortType.Items.Add(type.Humanize());
        }

        if (comboBox_AIManifacturerList.Items.Count == 0)
        {
            foreach (var manufacturer in GameDatabase.GetAllManufacturersSorted())
                comboBox_AIManifacturerList.Items.Add(manufacturer);

            comboBox_AIManifacturerList.SelectedIndex = 0;
            UpdateAIEntriesCarList();
        }
    }

    private Entry GenerateEntry()
    {
        Entry raceEntry = new Entry();
        string driverName = $"Entry #{listBox_EntryBases.Items.Count + 1}";
        string driverRegion = "PDI";

        raceEntry.Car.CarLabel = GameDatabase.GetCarLabelByActualName((string)listBox_AICarList.SelectedItem);

        if (checkBox_RandomDriverName.IsChecked == true)
        {
            var driverInfo = GameDatabase.GetRandomDriverInfo();
            var regionInfo = RegionUtil.GetRandomInitial(App.Random, driverInfo.InitialType);

            driverName = $"{regionInfo.initial}. {driverInfo.DriverName}";
            if (checkBox_GenerateRandomFlag.IsChecked == true)
                driverRegion = regionInfo.country;
        }
        else
        {
            if (checkBox_GenerateRandomFlag.IsChecked == true)
            {
                var driverInfo = GameDatabase.GetRandomDriverInfo();
                var regionInfo = RegionUtil.GetRandomInitial(App.Random, driverInfo.InitialType);
                driverRegion = regionInfo.country;
            }
        }

        if (checkBox_RandomCarColor.IsEnabled == true)
            raceEntry.Car.Paint = (short)App.Random.Next(GameDatabase.GetCarColorNumByLabel(raceEntry.Car.CarLabel));

        raceEntry.DriverName = driverName;
        raceEntry.DriverRegion = driverRegion;

        if (numericUpDown_AIRoughnessMin.Value > numericUpDown_AIRoughnessMax.Value)
            numericUpDown_AIRoughnessMax.Value = numericUpDown_AIRoughnessMax.Value;

        if (numericUpDown_AIRoughnessMin.Value > numericUpDown_AIRoughnessMax.Value && numericUpDown_AIRoughnessMax.Value != -1)
            raceEntry.AIRoughness = (sbyte)App.Random.Next(numericUpDown_AIRoughnessMin.Value.Value, numericUpDown_AIRoughnessMax.Value.Value + 1);

        if (numericUpDown_AccelSkillMin.Value > numericUpDown_AccelSkillMax.Value && numericUpDown_AccelSkillMax.Value != -1)
            raceEntry.AISkillAccelerating = (sbyte)App.Random.Next(numericUpDown_AccelSkillMin.Value.Value, numericUpDown_AccelSkillMax.Value.Value + 1);

        if (numericUpDown_BrakeSkillMin.Value > numericUpDown_BrakeSkillMax.Value && numericUpDown_BrakeSkillMax.Value != -1)
            raceEntry.AISkillBraking = (short)App.Random.Next(numericUpDown_BrakeSkillMin.Value.Value, numericUpDown_BrakeSkillMax.Value.Value + 1);

        if (numericUpDown_CornerSkillMin.Value > numericUpDown_CornerSkillMax.Value && numericUpDown_CornerSkillMax.Value != -1)
            raceEntry.AISkillCornering = (short)App.Random.Next(numericUpDown_CornerSkillMin.Value.Value, numericUpDown_CornerSkillMax.Value.Value + 1);

        if (numericUpDown_StartSkillMin.Value > numericUpDown_StartSkillMax.Value && numericUpDown_StartSkillMax.Value != -1)
            raceEntry.AISkillStarting = (sbyte)App.Random.Next(numericUpDown_StartSkillMin.Value.Value, numericUpDown_StartSkillMax.Value.Value + 1);

        return raceEntry;
    }

    private EntryBase GenerateEntryBase()
    {
        var raceEntry = new EntryBase();
        string driverName = $"Entry #{listBox_EntryBases.Items.Count + 1}";
        string driverRegion = "PDI";

        raceEntry.Car.CarLabel = GameDatabase.GetCarLabelByActualName((string)listBox_AICarList.SelectedItem);

        if (checkBox_RandomDriverName.IsChecked == true)
        {
            var driverInfo = GameDatabase.GetRandomDriverInfo();
            var regionInfo = RegionUtil.GetRandomInitial(App.Random, driverInfo.InitialType);

            driverName = $"{regionInfo.initial}. {driverInfo.DriverName}";
            if (checkBox_GenerateRandomFlag.IsChecked == true)
                driverRegion = regionInfo.country;
        }
        else
        {
            if (checkBox_GenerateRandomFlag.IsChecked == true)
            {
                var driverInfo = GameDatabase.GetRandomDriverInfo();
                var regionInfo = RegionUtil.GetRandomInitial(App.Random, driverInfo.InitialType);
                driverRegion = regionInfo.country;
            }
        }

        if (checkBox_RandomCarColor.IsEnabled == true)
            raceEntry.Car.Paint = (short)App.Random.Next(GameDatabase.GetCarColorNumByLabel(raceEntry.Car.CarLabel));

        raceEntry.DriverName = driverName;
        raceEntry.DriverRegion = driverRegion;

        if (numericUpDown_AIRoughnessMin.Value > numericUpDown_AIRoughnessMax.Value && numericUpDown_AIRoughnessMax.Value != -1)
            raceEntry.AIRoughness = (sbyte)App.Random.Next(numericUpDown_AIRoughnessMin.Value.Value, numericUpDown_AIRoughnessMax.Value.Value + 1);

        if (numericUpDown_AccelSkillMin.Value > numericUpDown_AccelSkillMax.Value && numericUpDown_AccelSkillMax.Value != -1)
            raceEntry.AIAcceleratingSkill = (sbyte)App.Random.Next(numericUpDown_AccelSkillMin.Value.Value, numericUpDown_AccelSkillMax.Value.Value + 1);

        if (numericUpDown_BrakeSkillMin.Value > numericUpDown_BrakeSkillMax.Value && numericUpDown_BrakeSkillMax.Value != -1)
            raceEntry.AIBrakingSkill = (short)App.Random.Next(numericUpDown_BrakeSkillMin.Value.Value, numericUpDown_BrakeSkillMax.Value.Value + 1);

        if (numericUpDown_CornerSkillMin.Value > numericUpDown_CornerSkillMax.Value && numericUpDown_CornerSkillMax.Value != -1)
            raceEntry.AICorneringSkill = (short)App.Random.Next(numericUpDown_CornerSkillMin.Value.Value, numericUpDown_CornerSkillMax.Value.Value + 1);

        if (numericUpDown_StartSkillMin.Value > numericUpDown_StartSkillMax.Value && numericUpDown_StartSkillMax.Value != -1)
            raceEntry.AIStartingSkill = (sbyte)App.Random.Next(numericUpDown_StartSkillMin.Value.Value, numericUpDown_StartSkillMax.Value.Value + 1);

        raceEntry.TireFront = (TireType)comboBox_EntryGenTyreComp.SelectedIndex - 1;
        raceEntry.TireRear = (TireType)comboBox_EntryGenTyreComp.SelectedIndex - 1;

        return raceEntry;
    }

    private void UpdateAIEntriesCarList()
    {
        listBox_AICarList.Items.Clear();

        var results = GameDatabase.ExecuteQuery(
            "SELECT " +
                "V.VehicleName " +
            "FROM Vehicles V " +
            "INNER JOIN Manufacturers M " +
                "ON M.ManufacturerID = V.VehicleManufacturerID " +
            "WHERE " +
                $"M.ManufacturerName = '{comboBox_AIManifacturerList.SelectedItem.ToString()}' " +
            "ORDER BY VehicleName ");

        while (results.Read())
            listBox_AICarList.Items.Add(results.GetString(0));
    }
}

public class EntryBaseWrapper
{
    public EntryBase EntryBase { get; set; }
    public string Name { get; set; }
}

public class EntryWrapper
{
    public Entry Entry { get; set; }
    public string Name { get; set; }
}
