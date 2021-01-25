using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

using GTEventGenerator.Entities;
using GTEventGenerator.Utils;

using Humanizer;
namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        private List<EventEntry> _fixedEntries { get; set; } = new List<EventEntry>();
        private List<EventEntry> _AIPoolEntries { get; set; } = new List<EventEntry>();

        private void comboBox_AIManifacturerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateAIEntriesCarList();

        public void PopulateEntries()
        {
            sl_EntryCount.Maximum = CurrentEvent.GetFreeCarSlotsLeft();
            if (CurrentEvent.Entries.AIsToPickFromPool > CurrentEvent.RaceParameters.RacersMax - 1)
                CurrentEvent.Entries.AIsToPickFromPool = CurrentEvent.RaceParameters.RacersMax - 1;
            sl_EntryCount.Value = CurrentEvent.Entries.AIsToPickFromPool;
            sl_EntryPlayerPos.Maximum = CurrentEvent.RaceParameters.RacersMax;

            if (CurrentEvent.Entries.PlayerPos > CurrentEvent.RaceParameters.RacersMax)
                CurrentEvent.Entries.PlayerPos = CurrentEvent.RaceParameters.RacersMax;
            sl_EntryPlayerPos.Value = CurrentEvent.Entries.PlayerPos;
            label_EntryCount.Content = CurrentEvent.Entries.AIsToPickFromPool.ToString();
            label_PlayerPos.Content = $"#{CurrentEvent.Entries.PlayerPos}";

            if (!CurrentEvent.Entries.NeedsPopulating)
                return;

            PopulateComboBoxesIfNeeded();

            _AIPoolEntries.Clear();
            _fixedEntries.Clear();

            listBox_AIEntries.Items.Clear();
            listBox_AIFixedEntries.Items.Clear();

            foreach (var entry in CurrentEvent.Entries.AIBases)
            {
                if (string.IsNullOrEmpty(entry.ActualCarName))
                    entry.ActualCarName = GameDatabase.GetCarNameByLabel(entry.CarLabel);
                listBox_AIEntries.Items.Add($"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}");
                _AIPoolEntries.Add(entry);
            }

            foreach (var entry in CurrentEvent.Entries.AI)
            {
                if (string.IsNullOrEmpty(entry.ActualCarName))
                    entry.ActualCarName = GameDatabase.GetCarNameByLabel(entry.CarLabel);
                listBox_AIFixedEntries.Items.Add($"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}");
                _fixedEntries.Add(entry);
            }

            if (_fixedEntries.Count != 0)
            {
                button_EditFixedAI.IsEnabled = true;
                button_RemoveFixedAI.IsEnabled = true;
            }

            if (_AIPoolEntries.Count != 0)
            {
                button_EditAI.IsEnabled = true;
                button_RemoveAI.IsEnabled = true;
            }

            if (CurrentEvent.Entries.Player != null)
            {
                CurrentEvent.Entries.Player.ActualCarName = GameDatabase.GetCarNameByLabel(CurrentEvent.Entries.Player.CarLabel);

                label_PlayerRentedCar.Content = $"Player has rented car: {CurrentEvent.Entries.Player.ActualCarName}";

                button_SetCarAsPlayer.IsEnabled = false;

                button_EditPlayerEntry.IsEnabled = true;
                button_RemovePlayerEntry.IsEnabled = true;
            }
            else
            {
                label_PlayerRentedCar.Content = "No custom player entry set - Player will not use a rented Car";

                button_SetCarAsPlayer.IsEnabled = true;

                button_EditPlayerEntry.IsEnabled = false;
                button_RemovePlayerEntry.IsEnabled = false;
            }

            UpdateEntryControls();
            CurrentEvent.Entries.NeedsPopulating = false;
        }

        private void PopulateComboBoxesIfNeeded()
        {
            if (comboBox_AIManifacturerList.Items.Count == 0)
            {
                foreach (var manufacturer in GameDatabase.GetAllManufacturersSorted())
                    comboBox_AIManifacturerList.Items.Add(manufacturer);

                comboBox_AIManifacturerList.SelectedIndex = 0;
                UpdateAIEntriesCarList();
            }

            if (comboBox_AIGenTyreComp.Items.Count == 1)
            {
                var tires = (TireType[])Enum.GetValues(typeof(TireType));
                for (int i = 0; i < tires.Length - 1; i++) // -1 as the combo boxes have a default "none" entry
                {
                    var tire = (TireType)i;
                    string tireName = tire.Humanize();
                    comboBox_AIGenTyreComp.Items.Add(tireName);
                }

                comboBox_AIGenTyreComp.SelectedIndex = 0;
            }

            if (comboBox_entryGenerateType.Items.Count == 0)
            {
                var types = (EntryGenerateType[])Enum.GetValues(typeof(EntryGenerateType));
                for (int i = 0; i < types.Length; i++)
                {
                    var type = (EntryGenerateType)i;
                    string typeName = type.Humanize();
                    comboBox_entryGenerateType.Items.Add(typeName);
                }
            }
            comboBox_entryGenerateType.SelectedIndex = (int)CurrentEvent.Entries.AIEntryGenerateType;

            if (comboBox_EntrySortType.Items.Count == 0)
            {
                var types = (EnemySortType[])Enum.GetValues(typeof(EnemySortType));
                for (int i = 0; i < types.Length; i++)
                {
                    var type = (EnemySortType)i;
                    string typeName = type.Humanize();
                    comboBox_EntrySortType.Items.Add(typeName);
                }
            }
            comboBox_EntrySortType.SelectedIndex = (int)CurrentEvent.Entries.AISortType;
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

        private void button_GenerateAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AICarList.SelectedIndex == -1)
            {
                MessageBox.Show("Select a car first.", "No car selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EventEntry raceEntry = GenerateEntry();

            _AIPoolEntries.Add(raceEntry);
            CurrentEvent.Entries.AIBases.Add(raceEntry);
            listBox_AIEntries.Items.Add($"{raceEntry.DriverName} [{raceEntry.DriverRegion}]: {raceEntry.ActualCarName}");

            button_EditAI.IsEnabled = true;
            button_RemoveAI.IsEnabled = true;
        }

        private void button_GenerateFixedAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AICarList.SelectedIndex == -1)
            {
                MessageBox.Show("Select a car first.", "No car selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentEvent.GetFreeCarSlotsLeft() <= 0)
            {
                MessageBox.Show($"Fixed entry count exceeds the amount of allowed 'Max Cars' specified ({_fixedEntries.Count} fixed entries + Player >= {CurrentEvent.RaceParameters.RacersMax}).", "No car selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EventEntry raceEntry = GenerateEntry();

            _fixedEntries.Add(raceEntry);
            CurrentEvent.Entries.AI.Add(raceEntry);
            listBox_AIFixedEntries.Items.Add($"{raceEntry.DriverName} [{raceEntry.DriverRegion}]: {raceEntry.ActualCarName}");

            button_EditFixedAI.IsEnabled = true;
            button_RemoveFixedAI.IsEnabled = true;

            UpdateEntryControls();
        }

        private void button_EditAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AIEntries.SelectedIndex == -1)
                return;

            var entry = _AIPoolEntries[listBox_AIEntries.SelectedIndex];
            var entryEdit = new EventEntryEditWindow(entry, GameDatabase.GetCarColorNumByLabel(entry.CarLabel));
            entryEdit.ShowDialog();

            ResortAIs();
        }

        private void button_EditFixedAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AIFixedEntries.SelectedIndex == -1)
                return;

            var entry = _fixedEntries[listBox_AIFixedEntries.SelectedIndex];
            var entryEdit = new EventEntryEditWindow(entry, GameDatabase.GetCarColorNumByLabel(entry.CarLabel));
            entryEdit.ShowDialog();

            ResortFixedAIs();
        }

        private void button_SetCarAsPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_AICarList.SelectedIndex == -1)
                return;

            var entry = new EventEntry();
            entry.IsAI = false;

            entry.CarLabel = GameDatabase.GetCarLabelByActualName((string)listBox_AICarList.SelectedItem);
            CurrentEvent.Entries.Player = entry;

            button_SetCarAsPlayer.IsEnabled = false;

            button_EditPlayerEntry.IsEnabled = true;
            button_RemovePlayerEntry.IsEnabled = true;

            label_PlayerRentedCar.Content = $"Player has rented car: {(string)listBox_AICarList.SelectedItem}";
        }

        public void button_RemovePlayerEntry_Click(object sender, RoutedEventArgs e)
        {
            label_PlayerRentedCar.Content = "No custom player entry set - Player will not use a rented Car";
            CurrentEvent.Entries.Player = null;

            button_EditPlayerEntry.IsEnabled = false;
            button_RemovePlayerEntry.IsEnabled = false;

            button_SetCarAsPlayer.IsEnabled = true;
        }

        private void button_RemoveAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AIEntries.SelectedIndex == -1)
                return;

            // Build a list of entries to remove
            List<EventEntry> toRemove = new List<EventEntry>();
            foreach (var selected in listBox_AIEntries.SelectedItems)
            {
                int index = listBox_AIEntries.Items.IndexOf(selected);
                toRemove.Add(_AIPoolEntries[index]);
            }

            foreach (EventEntry entry in toRemove)
            {
                _AIPoolEntries.Remove(entry);
                CurrentEvent.Entries.AIBases.Remove(entry);
            }

            int count = listBox_AIEntries.SelectedItems.Count;
            for (int i = count - 1; i >= 0; i--)
                listBox_AIEntries.Items.Remove(listBox_AIEntries.SelectedItems[i]);

            if (listBox_AIEntries.Items.Count == 0)
            {
                button_EditAI.IsEnabled = false;
                button_RemoveAI.IsEnabled = false;
            }
        }

        private void button_EditPlayerEntry_Click(object sender, RoutedEventArgs e)
        {
            var entryEdit = new EventEntryEditWindow(CurrentEvent.Entries.Player, GameDatabase.GetCarColorNumByLabel(CurrentEvent.Entries.Player.CarLabel));
            entryEdit.ShowDialog();
        }

        private void button_RemoveFixedAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AIFixedEntries.SelectedIndex == -1)
                return;

            // Build a list of entries to remove
            List<EventEntry> toRemove = new List<EventEntry>();
            foreach (var selected in listBox_AIFixedEntries.SelectedItems)
            {
                int index = listBox_AIFixedEntries.Items.IndexOf(selected);
                toRemove.Add(_fixedEntries[index]);
            }

            foreach (EventEntry entry in toRemove)
            {
                _fixedEntries.Remove(entry);
                CurrentEvent.Entries.AI.Remove(entry);
            }

            int count = listBox_AIFixedEntries.SelectedItems.Count;
            for (int i = count - 1; i >= 0; i--)
                listBox_AIFixedEntries.Items.Remove(listBox_AIFixedEntries.SelectedItems[i]);

            if (listBox_AIFixedEntries.Items.Count == 0)
            {
                button_EditFixedAI.IsEnabled = false;
                button_RemoveFixedAI.IsEnabled = false;
            }

            UpdateEntryControls();
        }

        private void slider_EntryCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentEvent is null)
                return;

            label_EntryCount.Content = CurrentEvent.Entries.AIsToPickFromPool.ToString();
        }

        private void sl_EntryPlayerPos_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentEvent is null)
                return;

            label_PlayerPos.Content = $"#{CurrentEvent.Entries.PlayerPos}";
        }

        private void comboBox_entryGenerateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEvent.Entries.AIEntryGenerateType = (EntryGenerateType)(sender as ComboBox).SelectedIndex;
        }

        private void comboBox_EntrySortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEvent.Entries.AISortType = (EnemySortType)(sender as ComboBox).SelectedIndex;
        }

        public void UpdateEntryControls()
        {
            int slotsLeft = CurrentEvent.GetFreeCarSlotsLeft();
            sl_EntryCount.Maximum = slotsLeft;

            if (CurrentEvent.Entries.AIsToPickFromPool > slotsLeft)
                CurrentEvent.Entries.AIsToPickFromPool = slotsLeft;

            sl_EntryCount.Value = CurrentEvent.Entries.AIsToPickFromPool;
            sl_EntryPlayerPos.Maximum = CurrentEvent.RaceParameters.RacersMax;
            sl_EntryPlayerPos.Value = CurrentEvent.Entries.PlayerPos;
            label_EntryCount.Content = CurrentEvent.Entries.AIsToPickFromPool.ToString();
            label_PlayerPos.Content = $"#{CurrentEvent.Entries.PlayerPos}";
        }

        private EventEntry GenerateEntry()
        {
            var raceEntry = new EventEntry();
            raceEntry.IsAI = true;

            string driverName = $"AI #{listBox_AIEntries.Items.Count + 1}";
            string driverRegion = "PDI";

            raceEntry.CarLabel = GameDatabase.GetCarLabelByActualName((string)listBox_AICarList.SelectedItem);
            raceEntry.ActualCarName = (string)listBox_AICarList.SelectedItem;

            if (checkBox_RandomDriverName.IsChecked == true)
            {
                var driverInfo = GameDatabase.GetRandomDriverInfo();
                var regionInfo = RegionUtil.GetRandomInitial(_random, driverInfo.InitialType);

                driverName = $"{regionInfo.initial}. {driverInfo.DriverName}";
                if (checkBox_GenerateRandomFlag.IsChecked == true)
                    driverRegion = regionInfo.country;
            }
            else
            {
                if (checkBox_GenerateRandomFlag.IsChecked == true)
                {
                    var driverInfo = GameDatabase.GetRandomDriverInfo();
                    var regionInfo = RegionUtil.GetRandomInitial(_random, driverInfo.InitialType);
                    driverRegion = regionInfo.country;
                }
            }

            if (checkBox_RandomCarColor.IsEnabled == true)
                raceEntry.ColorIndex = (short)_random.Next(GameDatabase.GetCarColorNumByLabel(raceEntry.CarLabel));

            raceEntry.DriverName = driverName;
            raceEntry.DriverRegion = driverRegion;

            if (numericUpDown_AIRoughnessMin.Value > numericUpDown_AIRoughnessMax.Value)
                numericUpDown_AIRoughnessMax.Value = numericUpDown_AIRoughnessMax.Value;

            raceEntry.Roughness = (sbyte)_random.Next(numericUpDown_AIRoughnessMin.Value.Value, numericUpDown_AIRoughnessMax.Value.Value + 1);

            if (numericUpDown_BaseSkillMin.Value > numericUpDown_BaseSkillMax.Value)
                numericUpDown_BaseSkillMax.Value = numericUpDown_BaseSkillMin.Value;

            raceEntry.BaseSkill = _random.Next(numericUpDown_BaseSkillMin.Value.Value, numericUpDown_BaseSkillMax.Value.Value + 1);

            if (numericUpDown_AccelSkillMin.Value > numericUpDown_AccelSkillMax.Value)
                numericUpDown_AccelSkillMax.Value = numericUpDown_AccelSkillMin.Value;

            raceEntry.AccelSkill = (sbyte)_random.Next(numericUpDown_AccelSkillMin.Value.Value, numericUpDown_AccelSkillMax.Value.Value + 1);

            if (numericUpDown_BrakeSkillMin.Value > numericUpDown_BrakeSkillMax.Value)
                numericUpDown_BrakeSkillMax.Value = numericUpDown_BrakeSkillMin.Value;

            raceEntry.BrakingSkill = (short)_random.Next(numericUpDown_BrakeSkillMin.Value.Value, numericUpDown_BrakeSkillMax.Value.Value + 1);

            if (numericUpDown_CornerSkillMin.Value > numericUpDown_CornerSkillMax.Value)
                numericUpDown_CornerSkillMax.Value = numericUpDown_CornerSkillMin.Value;

            raceEntry.CorneringSkill = (short)_random.Next(numericUpDown_CornerSkillMin.Value.Value, numericUpDown_CornerSkillMax.Value.Value + 1);

            if (numericUpDown_StartSkillMin.Value > numericUpDown_StartSkillMax.Value)
                numericUpDown_StartSkillMax.Value = numericUpDown_StartSkillMin.Value;
            raceEntry.StartingSkill = (sbyte)_random.Next(numericUpDown_StartSkillMin.Value.Value, numericUpDown_StartSkillMax.Value.Value + 1);


            raceEntry.TireFront = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;
            raceEntry.TireRear = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;

            return raceEntry;
        }

        private void ResortAIs()
        {
            for (int i = 0; i < _AIPoolEntries.Count; i++)
            {
                var entry = _AIPoolEntries[i];
                listBox_AIEntries.Items[i] = $"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}";
            }
        }

        private void ResortFixedAIs()
        {
            for (int i = 0; i < _fixedEntries.Count; i++)
            {
                var entry = _fixedEntries[i];
                listBox_AIFixedEntries.Items[i] = $"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}";
            }
        }

    }
}