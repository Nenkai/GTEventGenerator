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
        private List<RaceEntry> _orderedEntries { get; set; } = new List<RaceEntry>();
        private void comboBox_AIManifacturerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateAIEntriesCarList();

        public void PopulateEntries()
        {
            PopulateComboBoxesIfNeeded();

            _orderedEntries.Clear();
            listBox_AIEntries.Items.Clear();
            foreach (var entry in CurrentEvent.Entries.AIBases)
            {
                listBox_AIEntries.Items.Add($"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}");
                _orderedEntries.Add(entry);
            }

            foreach (var entry in CurrentEvent.Entries.AI)
            {
                listBox_AIEntries.Items.Add($"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}");
                _orderedEntries.Add(entry);
            }

            foreach (var entry in _orderedEntries)
                entry.ActualCarName = GameDatabase.GetCarNameByLabel(entry.CarLabel);

            if (_orderedEntries.Count != 0)
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

            sl_EntryCount.Value = CurrentEvent.Entries.EntryCount;
            sl_EntryPlayerPos.Maximum = CurrentEvent.Entries.EntryCount;
            sl_EntryPlayerPos.Value = CurrentEvent.Entries.PlayerPos;
            label_EntryCount.Content = CurrentEvent.Entries.EntryCount.ToString();
            label_PlayerPos.Content = $"#{CurrentEvent.Entries.PlayerPos}";
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

            bool isRandom = checkBox_AINotRandom.IsChecked != true;

            RaceEntry raceEntry = GenerateEntry();

            if (isRandom)
                CurrentEvent.Entries.AIBases.Add(raceEntry);
            else
                CurrentEvent.Entries.AI.Add(raceEntry);
            _orderedEntries.Add(raceEntry);

            listBox_AIEntries.Items.Add($"{raceEntry.DriverName} [{raceEntry.DriverRegion}]: {raceEntry.ActualCarName}");

            button_EditAI.IsEnabled = true;
            button_RemoveAI.IsEnabled = true;
        }

        private void button_EditAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AIEntries.SelectedIndex == -1)
                return;

            var entry = _orderedEntries[listBox_AIEntries.SelectedIndex];
            var entryEdit = new EventEntryEditWindow(entry);
            entryEdit.ShowDialog();

            ResortAIs();
        }

        private void button_SetCarAsPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_AIEntries.SelectedIndex == -1)
                return;

            var entry = new RaceEntry();
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

            button_EditPlayerEntry.IsEnabled = false;
            button_RemovePlayerEntry.IsEnabled = false;

            button_SetCarAsPlayer.IsEnabled = true;
        }

        private void button_RemoveAI_Clicked(object sender, RoutedEventArgs e)
        {
            if (listBox_AIEntries.SelectedIndex == -1)
                return;

            var entry = _orderedEntries[listBox_AIEntries.SelectedIndex];
            _orderedEntries.Remove(entry);
            CurrentEvent.Entries.AI.Remove(entry);
            CurrentEvent.Entries.AIBases.Remove(entry);
            listBox_AIEntries.Items.Remove(listBox_AIEntries.SelectedItem);

            if (listBox_AIEntries.Items.Count == 0)
            {
                button_EditAI.IsEnabled = false;
                button_RemoveAI.IsEnabled = false;
            }
        }

        private void button_EditPlayerEntry_Click(object sender, RoutedEventArgs e)
        {
            var entryEdit = new EventEntryEditWindow(CurrentEvent.Entries.Player);
            entryEdit.ShowDialog();
        }

        private void slider_EntryCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentEvent is null)
                return;

            if (CurrentEvent.Entries.PlayerPos > CurrentEvent.Entries.EntryCount)
            {
                CurrentEvent.Entries.PlayerPos = CurrentEvent.Entries.EntryCount;
                sl_EntryPlayerPos.Value = CurrentEvent.Entries.PlayerPos;
                label_PlayerPos.Content = $"#{CurrentEvent.Entries.PlayerPos}";
            }

            sl_EntryPlayerPos.Maximum = CurrentEvent.Entries.EntryCount;
            label_EntryCount.Content = CurrentEvent.Entries.EntryCount.ToString();
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

        private RaceEntry GenerateEntry()
        {
            var raceEntry = new RaceEntry();
            raceEntry.IsAI = true;

            string driverName = $"AI #{listBox_AIEntries.Items.Count + 1}";
            string driverRegion = "PDI";

            raceEntry.CarLabel = GameDatabase.GetCarLabelByActualName((string)listBox_AICarList.SelectedItem);
            raceEntry.ActualCarName = (string)listBox_AICarList.SelectedItem;

            if (checkBox_RandomDriverName.IsChecked == true)
            {
                var driverInfo = GenerateDriverInfo();
                var regionInfo = RegionUtil.GetRandomInitial(_random, driverInfo.initialType);

                driverName = $"{regionInfo.initial}. {driverInfo.driverName}";
                if (checkBox_GenerateRandomFlag.IsChecked == true)
                    driverRegion = regionInfo.country;
            }
            else
            {
                if (checkBox_GenerateRandomFlag.IsChecked == true)
                {
                    var driverInfo = GenerateDriverInfo();
                    var regionInfo = RegionUtil.GetRandomInitial(_random, driverInfo.initialType);
                    driverRegion = regionInfo.country;
                }
            }

            raceEntry.DriverName = driverName;
            raceEntry.DriverRegion = driverRegion;

            raceEntry.Roughness = numericUpDown_AIRoughness.Value.Value;
            raceEntry.BaseSkill = _random.Next(numericUpDown_BaseSkillMin.Value.Value, numericUpDown_BaseSkillMax.Value.Value + 1);
            raceEntry.AccelSkill = _random.Next(numericUpDown_AccelSkillMin.Value.Value, numericUpDown_AccelSkillMax.Value.Value + 1);
            raceEntry.BrakingSkill = _random.Next(numericUpDown_BrakeSkillMin.Value.Value, numericUpDown_AccelSkillMax.Value.Value + 1);
            raceEntry.CorneringSkill = _random.Next(numericUpDown_CornerSkillMin.Value.Value, numericUpDown_CornerSkillMax.Value.Value + 1);
            raceEntry.StartingSkill = _random.Next(numericUpDown_StartSkillMin.Value.Value, numericUpDown_StartSkillMax.Value.Value + 1);

            raceEntry.TireFront = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;
            raceEntry.TireRear = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;

            return raceEntry;
        }

        private void ResortAIs()
        {
            for (int i = 0; i < _orderedEntries.Count; i++)
            {
                var entry = _orderedEntries[i];
                listBox_AIEntries.Items[i] = $"{entry.DriverName} [{entry.DriverRegion}]: {entry.ActualCarName}";
            }
        }

        private (string driverName, int initialType) GenerateDriverInfo()
        {
            return GameDatabase.GetRandomDriverInfo();
        }
    }
}