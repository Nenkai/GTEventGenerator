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
using Xceed.Wpf.Toolkit;
using GTEventGenerator.Entities;
using GTEventGenerator.Utils;
using Humanizer;

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        #region Car Restrictions
        private void btn_AddAllowedCar_Click(object sender, EventArgs e)
        {
            var selectedItems = lb_CarSelection.SelectedItems;
            if (selectedItems.Count == 0)
                return;

            if (lb_CarSelection.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    string name = GameDatabase.GetCarLabelByActualName((string)selectedItems[i]);

                    lb_AllowedCars.Items.Add(selectedItems[i]);
                    lb_CarSelection.Items.Remove(selectedItems[i]);

                    CurrentEvent.Regulations.AllowedVehicles.Add(name);
                    
                }
            }

            //lb_AllowedCars.Sorted = true;
        }

        private void btn_AddBannedCar_Click(object sender, EventArgs e)
        {
            var selectedItems = lb_CarSelection.SelectedItems;

            if (lb_CarSelection.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    string name = GameDatabase.GetCarLabelByActualName((string)selectedItems[i]);

                    lb_BannedCars.Items.Add(selectedItems[i]);
                    lb_CarSelection.Items.Remove(selectedItems[i]);

                    CurrentEvent.Regulations.RestrictedVehicles.Add(name);
                }
            }

            // lb_BannedCars.Sorted = true;
        }

        private void btn_RemoveAllowedCar_Click(object sender, EventArgs e)
        {
            var selectedItems = lb_AllowedCars.SelectedItems;
            if (lb_AllowedCars.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    string name = GameDatabase.GetCarLabelByActualName((string)selectedItems[i]);
                    lb_AllowedCars.Items.Remove(selectedItems[i]);
                    CurrentEvent.Regulations.AllowedVehicles.Remove(name);
                }

                UpdateCarsLists();
            }
        }

        private void btn_RemoveBannedCar_Click(object sender, EventArgs e)
        {
            var selectedItems = lb_BannedCars.SelectedItems;

            if (lb_BannedCars.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    string name = GameDatabase.GetCarLabelByActualName((string)selectedItems[i]);
                    lb_BannedCars.Items.Remove(selectedItems[i]);
                    CurrentEvent.Regulations.RestrictedVehicles.Remove(name);
                }

                UpdateCarsLists();
            }
        }
        #endregion

        #region Manufacturers
        private void cbo_Manufacturers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCarsLists();
        }

        private void btn_ManufacturersAdd_Click(object sender, EventArgs e)
        {
            var selectedItems = lbManufacturers.SelectedItems;

            if (lbManufacturers.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    string name = GameDatabase.GetManufacturerLabelByName((string)selectedItems[i]);

                    lb_AllowedManufacturers.Items.Add(selectedItems[i]);
                    lbManufacturers.Items.Remove(selectedItems[i]);

                    CurrentEvent.Regulations.AllowedManufacturers.Add(name);
                }
            }

        }

        private void btn_RemoveManufacturer_Click(object sender, EventArgs e)
        {
            var selectedItems = lb_AllowedManufacturers.SelectedItems;

            if (lb_AllowedManufacturers.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    string name = GameDatabase.GetManufacturerLabelByName((string)selectedItems[i]);
                    lb_AllowedManufacturers.Items.Remove(selectedItems[i]);

                    CurrentEvent.Regulations.AllowedManufacturers.Remove(name);
                }
            }

            UpdateManufacturerList();
        }

        #endregion

        #region Aspirations
        private void chkNA_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxNA = sender as CheckBox;
            const AspirationBits na = AspirationBits.NA;
            if (chkboxNA.IsChecked.Value)
                CurrentEvent.Regulations.AspirationNeeded |= na;
            else
                CurrentEvent.Regulations.AspirationNeeded &= ~na;
        }

        private void chkT_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxTurbo = sender as CheckBox;
            const AspirationBits turbo = AspirationBits.Turbo;
            if (chkboxTurbo.IsChecked.Value)
                CurrentEvent.Regulations.AspirationNeeded |= turbo;
            else
                CurrentEvent.Regulations.AspirationNeeded &= ~turbo;
        }

        private void chkSC_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxSC = sender as CheckBox;
            const AspirationBits sc = AspirationBits.Supercharger;
            if (chkboxSC.IsChecked.Value)
                CurrentEvent.Regulations.AspirationNeeded |= sc;
            else
                CurrentEvent.Regulations.AspirationNeeded &= ~sc;
        }
        #endregion

        #region Drivetrains
        private void chkFR_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxFR = sender as CheckBox;
            const DrivetrainBits fr = DrivetrainBits.FR;
            if (chkboxFR.IsChecked.Value)
                CurrentEvent.Regulations.DrivetrainNeeded |= fr;
            else
                CurrentEvent.Regulations.DrivetrainNeeded &= ~fr;
        }

        private void chkFF_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxFF = sender as CheckBox;
            const DrivetrainBits ff = DrivetrainBits.FF;
            if (chkboxFF.IsChecked.Value)
                CurrentEvent.Regulations.DrivetrainNeeded |= ff;
            else
                CurrentEvent.Regulations.DrivetrainNeeded &= ~ff;
        }

        private void chkAWD_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxAWD = sender as CheckBox;
            const DrivetrainBits awd = DrivetrainBits.AWD;
            if (chkboxAWD.IsChecked.Value)
                CurrentEvent.Regulations.DrivetrainNeeded |= awd;
            else
                CurrentEvent.Regulations.DrivetrainNeeded &= ~awd;
        }

        private void chkMR_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxMR = sender as CheckBox;
            const DrivetrainBits mr = DrivetrainBits.MR;
            if (chkboxMR.IsChecked.Value)
                CurrentEvent.Regulations.DrivetrainNeeded |= mr;
            else
                CurrentEvent.Regulations.DrivetrainNeeded &= ~mr;
        }

        private void chkRR_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkboxRR = sender as CheckBox;
            const DrivetrainBits rr = DrivetrainBits.RR;
            if (chkboxRR.IsChecked.Value)
                CurrentEvent.Regulations.DrivetrainNeeded |= rr;
            else
                CurrentEvent.Regulations.DrivetrainNeeded &= ~rr;
        }
        #endregion

        #region Tire Restrictions
        private void cbo_MinTireCompoundF_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox minCbF = sender as ComboBox;
            CurrentEvent.Regulations.TireCompoundMinFront = (TireType)(minCbF.SelectedIndex - 1);
        }

        private void cbo_MaxTireCompoundF_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox maxCbF = sender as ComboBox;
            CurrentEvent.Regulations.TireCompoundMaxFront = (TireType)(maxCbF.SelectedIndex - 1);
        }

        private void cbo_MinTireCompoundR_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox minCbR = sender as ComboBox;
            CurrentEvent.Regulations.TireCompoundMinRear = (TireType)(minCbR.SelectedIndex - 1);
        }

        private void cbo_MaxTireCompoundR_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox maxCbR = sender as ComboBox;
            CurrentEvent.Regulations.TireCompoundMaxRear = (TireType)(maxCbR.SelectedIndex - 1);
        }
        #endregion

        private void cb_CarCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_CarCountries.SelectedIndex <= 0)
                return;

            lb_AllowedCountries.Items.Add(cb_CarCountries.SelectedItem);

            CurrentEvent.Regulations.AllowedCountries.Add(
                EventRegulations.CountryDefinitions.FirstOrDefault(c => c.Value == (string)cb_CarCountries.SelectedItem).Key);

            cb_CarCountries.Items.Remove(cb_CarCountries.SelectedItem);
            cb_CarCountries.SelectedIndex = 0;
        }

        private void btn_RemoveCarCountry_Click(object sender, RoutedEventArgs e)
        {
            if (lb_AllowedCountries.SelectedIndex == -1)
                return;

            CurrentEvent.Regulations.AllowedCountries.Remove(
                EventRegulations.CountryDefinitions.FirstOrDefault(c => c.Value == (string)lb_AllowedCountries.SelectedItem).Key);

            UpdateCountriesAndCategoriesRegulations();
        }

        private void cb_CarCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_CarCategories.SelectedIndex <= 0)
                return;

            lb_AllowedCategories.Items.Add(cb_CarCategories.SelectedItem);

            CurrentEvent.Regulations.AllowedCategories.Add((CarCategoryRestriction)cb_CarCategories.SelectedIndex - 1);

            cb_CarCategories.Items.Remove(cb_CarCategories.SelectedItem);
            cb_CarCategories.SelectedIndex = 0;
        }

        private void btn_RemoveCarCategory_Click(object sender, RoutedEventArgs e)
        {
            if (lb_AllowedCategories.SelectedIndex == -1)
                return;

            var type = lb_AllowedCategories.SelectedItem.ToString().DehumanizeTo<CarCategoryRestriction>();
            CurrentEvent.Regulations.AllowedCategories.Remove(type);

            UpdateCountriesAndCategoriesRegulations();
        }

        public void PopulateFirstTimeRegulationControls()
        {
            if (cbo_Manufacturers.Items.Count == 0)
            {
                foreach (var manufacturer in GameDatabase.GetAllManufacturersSorted())
                    cbo_Manufacturers.Items.Add(manufacturer);

                cbo_Manufacturers.SelectedIndex = 0;
            }

            if (cbo_MaxTireCompoundF.Items.Count == 1) // Load them if empty
            {
                var tires = (TireType[])Enum.GetValues(typeof(TireType));
                for (int i = 0; i < tires.Length - 1; i++) // - 1 as the combo boxes have a default "none" entry
                {
                    var tire = (TireType)i;
                    string tireName = tire.Humanize();
                    cbo_MaxTireCompoundF.Items.Add(tireName);
                    cbo_MaxTireCompoundR.Items.Add(tireName);
                    cbo_MinTireCompoundF.Items.Add(tireName);
                    cbo_MinTireCompoundR.Items.Add(tireName);
                }
            }

        }

        // ------ Non-generated functions ------
        public void PrePopulateRegulations()
        {
            lb_AllowedCars.Items.Clear();
            lb_BannedCars.Items.Clear();
            lb_AllowedManufacturers.Items.Clear();

            PopulateFirstTimeRegulationControls();
            UpdateCountriesAndCategoriesRegulations();

            if (CurrentEvent.Regulations.AllowedVehicles.Count > 0)
            {
                foreach (string vehicle in CurrentEvent.Regulations.AllowedVehicles)
                {
                    string carName = GameDatabase.GetCarNameByLabel(vehicle);
                    if (!lb_AllowedCars.Items.Contains(carName))
                        lb_AllowedCars.Items.Add(carName);

                }
                UpdateCarsLists();
            }

            if (CurrentEvent.Regulations.RestrictedVehicles.Count > 0)
            {
                foreach (string vehicle in CurrentEvent.Regulations.RestrictedVehicles)
                {
                    string carName = GameDatabase.GetCarNameByLabel(vehicle);
                    if (!lb_BannedCars.Items.Contains(carName))
                        lb_BannedCars.Items.Add(carName);

                }
                UpdateCarsLists();
            }

            foreach (string manufacturer in CurrentEvent.Regulations.AllowedManufacturers)
            {
                results = GameDatabase.ExecuteQuery($"SELECT ManufacturerName FROM Manufacturers WHERE ManufacturerInternalName = '{manufacturer}' ORDER BY ManufacturerName");
                while (results.Read())
                {
                    if (!lb_AllowedManufacturers.Items.Contains(results.GetString(0)))
                        lb_AllowedManufacturers.Items.Add(results.GetString(0));
                }
            }

            UpdateManufacturerList();
            
            PopulateAspirations();
            PopulateDrivetrains();
            PopulateTireCompounds();

            CurrentEvent.Regulations.NeedsPopulating = false;
        }

        public void UpdateCountriesAndCategoriesRegulations()
        {
            lb_AllowedCountries.Items.Clear();
            lb_AllowedCategories.Items.Clear();

            for (int i = cb_CarCountries.Items.Count - 1; i > 0; i--)
                cb_CarCountries.Items.Remove(cb_CarCountries.Items[i]);

            for (int i = cb_CarCategories.Items.Count - 1; i > 0; i--)
                cb_CarCategories.Items.Remove(cb_CarCategories.Items[i]);

            foreach (var country in EventRegulations.CountryDefinitions)
                cb_CarCountries.Items.Add(country.Value);

            if (CurrentEvent.Regulations.AllowedCountries.Count > 0)
            {
                foreach (string country in CurrentEvent.Regulations.AllowedCountries)
                {
                    string fullName = EventRegulations.CountryDefinitions[country];
                    if (!lb_AllowedCountries.Items.Contains(fullName))
                        lb_AllowedCountries.Items.Add(fullName);
                    cb_CarCountries.Items.Remove(fullName);
                }
            }

            foreach (var cat in (CarCategoryRestriction[])Enum.GetValues(typeof(CarCategoryRestriction)))
                cb_CarCategories.Items.Add(cat.Humanize());

            if (CurrentEvent.Regulations.AllowedCategories.Count > 0)
            {
                foreach (CarCategoryRestriction cat in CurrentEvent.Regulations.AllowedCategories)
                {
                    string fullName = cat.Humanize();
                    if (!lb_AllowedCategories.Items.Contains(fullName))
                        lb_AllowedCategories.Items.Add(fullName);
                    cb_CarCategories.Items.Remove(fullName);
                }
            }
        }

        private void UpdateCarsLists()
        {
            lb_CarSelection.Items.Clear();

            results = GameDatabase.ExecuteQuery(
                "SELECT " +
                    "V.VehicleName " +
                "FROM Vehicles V " +
                "INNER JOIN Manufacturers M " +
                    "ON M.ManufacturerID = V.VehicleManufacturerID " +
                "WHERE " +
                    $"M.ManufacturerName = '{cbo_Manufacturers.SelectedItem.ToString()}' " +
                "ORDER BY VehicleName ");

            while (results.Read())
            {
                var car = results.GetString(0);
                // Only re-load the vehicle into the source list if it isn't in either
                if (!lb_AllowedCars.Items.Contains(car) && !lb_BannedCars.Items.Contains(car))
                    lb_CarSelection.Items.Add(car);
            }
        }

        private void UpdateManufacturerList()
        {
            lbManufacturers.Items.Clear();

            foreach (var manufacturer in GameDatabase.GetAllManufacturersSorted())
            {
                // Only re-load the manufacturer into the source list if it isn't in the allowed list
                if (!lb_AllowedManufacturers.Items.Contains(manufacturer))
                    lbManufacturers.Items.Add(manufacturer);
            }
        }

        private void PopulateAspirations()
        {
            AspirationBits mask = CurrentEvent.Regulations.AspirationNeeded;
            if (mask == AspirationBits.None)
            {
                chkT.IsChecked = false;
                chkSC.IsChecked = false;
                chkNA.IsChecked = false;
            }
            else
            {
                chkT.IsChecked = mask.HasFlag(AspirationBits.Turbo);
                chkSC.IsChecked = mask.HasFlag(AspirationBits.Supercharger);
                chkNA.IsChecked = mask.HasFlag(AspirationBits.NA);
            }
        }
            
        public void PopulateDrivetrains()
        {
            DrivetrainBits mask = CurrentEvent.Regulations.DrivetrainNeeded;
            if (mask == DrivetrainBits.None)
            {
                chkFF.IsChecked = false;
                chkFR.IsChecked = false;
                chkMR.IsChecked = false;
                chkAWD.IsChecked = false;
                chkRR.IsChecked = false;
            }
            else
            {
                chkFF.IsChecked = mask.HasFlag(DrivetrainBits.FF);
                chkFR.IsChecked = mask.HasFlag(DrivetrainBits.FR);
                chkMR.IsChecked = mask.HasFlag(DrivetrainBits.MR);
                chkAWD.IsChecked = mask.HasFlag(DrivetrainBits.AWD);
                chkRR.IsChecked = mask.HasFlag(DrivetrainBits.RR);
            }
        }

        private void PopulateTireCompounds()
        {
            cbo_MinTireCompoundF.SelectedIndex = (int)CurrentEvent.Regulations.TireCompoundMinFront + 1;
            cbo_MaxTireCompoundF.SelectedIndex = (int)CurrentEvent.Regulations.TireCompoundMaxFront + 1;
            cbo_MinTireCompoundR.SelectedIndex = (int)CurrentEvent.Regulations.TireCompoundMinRear + 1;
            cbo_MaxTireCompoundR.SelectedIndex = (int)CurrentEvent.Regulations.TireCompoundMaxRear + 1;
        }
    }
}