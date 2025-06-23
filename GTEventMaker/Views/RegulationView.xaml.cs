using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

using PDTools.Structures;
using PDTools.Structures.MGameParameter;
using PDTools.Enums;

using GTEventMaker.Database;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for RegulationView.xaml
/// </summary>
public partial class RegulationView : UserControl
{
    public Regulation Regulations => this.DataContext as Regulation;

    public GameDB GameDatabase { get; set; }

    public RegulationView()
    {
        GameDatabase = App.GameDatabase;
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

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
                string name = GameDatabase.GetCarLabelByActualName((string)selectedItems[i]) ?? (string)selectedItems[i];

                lb_AllowedCars.Items.Add(selectedItems[i]);
                lb_CarSelection.Items.Remove(selectedItems[i]);

                Regulations.Cars.Add(new MCarThin(name));

            }
        }
    }

    private void btn_AddBannedCar_Click(object sender, EventArgs e)
    {
        var selectedItems = lb_CarSelection.SelectedItems;

        if (lb_CarSelection.SelectedIndex != -1)
        {
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                string name = GameDatabase.GetCarLabelByActualName((string)selectedItems[i]) ?? (string)selectedItems[i];

                lb_BannedCars.Items.Add(selectedItems[i]);
                lb_CarSelection.Items.Remove(selectedItems[i]);

                Regulations.BanCars.Add(new MCarThin(name));
            }
        }
    }

    private void btn_RemoveAllowedCar_Click(object sender, EventArgs e)
    {
        var selectedItems = lb_AllowedCars.SelectedItems;
        if (lb_AllowedCars.SelectedIndex != -1)
        {
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                string item = (string)selectedItems[i];
                string label = GameDatabase.GetCarLabelByActualName(item) ?? item;

                lb_AllowedCars.Items.Remove(selectedItems[i]);
                lb_CarSelection.Items.Add(item);

                Regulations.Cars.Remove(Regulations.BanCars.Find(e => e.CarLabel == label));
            }

            UpdateCarsLists();
        }
    }

    private void btn_RemoveBannedCar_Click(object sender, EventArgs e)
    {
        var selectedItems = lb_BannedCars.SelectedItems;
        if (selectedItems.Count == 0)
            return;

        if (lb_BannedCars.SelectedIndex != -1)
        {
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                string item = (string)selectedItems[i];
                string label = GameDatabase.GetCarLabelByActualName(item) ?? item;
                lb_BannedCars.Items.Remove(item);
                lb_CarSelection.Items.Add(item);

                Regulations.BanCars.Remove(Regulations.BanCars.Find(e => e.CarLabel == label));
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
                string item = (string)selectedItems[i];

                lb_AllowedManufacturers.Items.Add(item);
                lbManufacturers.Items.Remove(item);

                var label = GameDatabase.GetManufacturerLabelByName(item);
                Regulations.Tuners.Add(Enum.Parse<Tuner>(label));
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
                string item = (string)selectedItems[i];

                lb_AllowedManufacturers.Items.Remove(item);
                lbManufacturers.Items.Add(item);

                var label = GameDatabase.GetManufacturerLabelByName(item);
                Tuner tuner = Enum.Parse<Tuner>(label);
                Regulations.Tuners.Remove(tuner);
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
        {
            if ((int)Regulations.NeedAspiration == -1)
                Regulations.NeedAspiration = 0;
            Regulations.NeedAspiration |= na;
        }
        else
        {
            Regulations.NeedAspiration &= ~na;
            if (Regulations.NeedAspiration == 0)
                Regulations.NeedAspiration = (AspirationBits)(-1);
        }
    }

    private void chkT_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxTurbo = sender as CheckBox;
        const AspirationBits turbo = AspirationBits.Turbo;
        if (chkboxTurbo.IsChecked.Value)
        {
            if ((int)Regulations.NeedAspiration == -1)
                Regulations.NeedAspiration = 0;
            Regulations.NeedAspiration |= turbo;
        }
        else
        {
            Regulations.NeedAspiration &= ~turbo;
            if (Regulations.NeedAspiration == 0)
                Regulations.NeedAspiration = (AspirationBits)(-1);
        }
    }

    private void chkSC_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxSC = sender as CheckBox;
        const AspirationBits sc = AspirationBits.Supercharger;
        if (chkboxSC.IsChecked.Value)
        {
            if ((int)Regulations.NeedAspiration == -1)
                Regulations.NeedAspiration = 0;
            Regulations.NeedAspiration |= sc;
        }
        else
        {
            Regulations.NeedAspiration &= ~sc;
            if (Regulations.NeedAspiration == 0)
                Regulations.NeedAspiration = (AspirationBits)(-1);
        }
    }
    #endregion

    #region Drivetrains
    private void chkFR_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxFR = sender as CheckBox;
        const DrivetrainBits fr = DrivetrainBits.FR;
        if (chkboxFR.IsChecked.Value)
        {
            if ((int)Regulations.NeedDrivetrain == -1)
                Regulations.NeedDrivetrain = 0;

            Regulations.NeedDrivetrain |= fr;
        }
        else
        { 
            Regulations.NeedDrivetrain &= ~fr;
            if (Regulations.NeedDrivetrain == 0)
                Regulations.NeedDrivetrain = (DrivetrainBits)(-1);
        }
    }

    private void chkFF_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxFF = sender as CheckBox;
        const DrivetrainBits ff = DrivetrainBits.FF;
        if (chkboxFF.IsChecked.Value)
        {
            if ((int)Regulations.NeedDrivetrain == -1)
                Regulations.NeedDrivetrain = 0;
            Regulations.NeedDrivetrain |= ff;
        }
        else
        {
            Regulations.NeedDrivetrain &= ~ff;
            if (Regulations.NeedDrivetrain == 0)
                Regulations.NeedDrivetrain = (DrivetrainBits)(-1);
        }
    }

    private void chkAWD_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxAWD = sender as CheckBox;
        const DrivetrainBits awd = DrivetrainBits.AWD;
        if (chkboxAWD.IsChecked.Value)
        {
            if ((int)Regulations.NeedDrivetrain == -1)
                Regulations.NeedDrivetrain = 0;
            Regulations.NeedDrivetrain |= awd;
        }
        else
        {
            Regulations.NeedDrivetrain &= ~awd;
            if (Regulations.NeedDrivetrain == 0)
                Regulations.NeedDrivetrain = (DrivetrainBits)(-1);
        }
    }

    private void chkMR_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxMR = sender as CheckBox;
        const DrivetrainBits mr = DrivetrainBits.MR;
        if (chkboxMR.IsChecked.Value)
        {
            if ((int)Regulations.NeedDrivetrain == -1)
                Regulations.NeedDrivetrain = 0;
            Regulations.NeedDrivetrain |= mr;
        }
        else
        {
            Regulations.NeedDrivetrain &= ~mr;
            if (Regulations.NeedDrivetrain == 0)
                Regulations.NeedDrivetrain = (DrivetrainBits)(-1);
        }
    }

    private void chkRR_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkboxRR = sender as CheckBox;
        const DrivetrainBits rr = DrivetrainBits.RR;
        if (chkboxRR.IsChecked.Value)
        {
            if ((int)Regulations.NeedDrivetrain == -1)
                Regulations.NeedDrivetrain = 0;
            Regulations.NeedDrivetrain |= rr;
        }
        else
        {
            Regulations.NeedDrivetrain &= ~rr;
            if (Regulations.NeedDrivetrain == 0)
                Regulations.NeedDrivetrain = (DrivetrainBits)(-1);
        }
    }
    #endregion

    #region Tire Restrictions
    private void cbo_MinTireCompoundF_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox minCbF = sender as ComboBox;
        Regulations.NeedTireFront = (TireType)(minCbF.SelectedIndex - 1);
    }

    private void cbo_MaxTireCompoundF_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox maxCbF = sender as ComboBox;
        Regulations.LimitTireFront = (TireType)(maxCbF.SelectedIndex - 1);
    }

    private void cbo_MinTireCompoundR_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox minCbR = sender as ComboBox;
        Regulations.NeedTireRear = (TireType)(minCbR.SelectedIndex - 1);
    }

    private void cbo_MaxTireCompoundR_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox maxCbR = sender as ComboBox;
        Regulations.LimitTireRear = (TireType)(maxCbR.SelectedIndex - 1);
    }
    #endregion

    private void cb_CarCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cb_CarCountries.SelectedIndex <= 0)
            return;

        Country country = ((string)cb_CarCountries.SelectedItem).DehumanizeTo<Country>();
        Regulations.Countries.Add(country);
        lb_AllowedCountries.Items.Add(cb_CarCountries.SelectedItem);

        cb_CarCountries.Items.Remove(cb_CarCountries.SelectedItem);
        cb_CarCountries.SelectedIndex = 0;
    }

    private void btn_RemoveCarCountry_Click(object sender, RoutedEventArgs e)
    {
        if (lb_AllowedCountries.SelectedIndex == -1)
            return;

        Country country = ((string)lb_AllowedCountries.SelectedItem).DehumanizeTo<Country>();
        Regulations.Countries.Remove(country);
        UpdateCountriesAndCategoriesRegulations();
    }

    private void cb_CarCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cb_CarCategories.SelectedIndex <= 0)
            return;

        CarCategoryRestriction category = ((string)cb_CarCategories.SelectedItem).DehumanizeTo<CarCategoryRestriction>();
        lb_AllowedCategories.Items.Add(cb_CarCategories.SelectedItem);
        Regulations.CarCategories.Add(category);

        cb_CarCategories.Items.Remove(cb_CarCategories.SelectedItem);
        cb_CarCategories.SelectedIndex = 0;
    }

    private void btn_RemoveCarCategory_Click(object sender, RoutedEventArgs e)
    {
        if (lb_AllowedCategories.SelectedIndex == -1)
            return;

        var type = lb_AllowedCategories.SelectedItem.ToString().DehumanizeTo<CarCategoryRestriction>();
        Regulations.CarCategories.Remove(type);

        UpdateCountriesAndCategoriesRegulations();
    }

    private void numericUpDown_PPMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NeedPP = numericUpDown_NeedPP.Value.Value;
    }

    private void numericUpDown_PPMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.LimitPP = numericUpDown_LimitPP.Value.Value;
    }

    private void numericUpDown_TorqueMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NeedTorque = numericUpDown_NeedTorque.Value.Value;
    }

    private void numericUpDown_TorqueMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.LimitTorque = numericUpDown_LimitTorque.Value.Value;
    }

    private void numericUpDown_PowerMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NeedPower = numericUpDown_NeedPower.Value.Value;
    }

    private void numericUpDown_PowerMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.LimitPower = numericUpDown_LimitPower.Value.Value;
    }

    private void numericUpDown_WeightMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NeedWeight = numericUpDown_NeedWeight.Value.Value;
    }

    private void numericUpDown_WeightMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.LimitWeight = numericUpDown_LimitWeight.Value.Value;
    }

    private void numericUpDown_CarYearMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NeedYear = numericUpDown_NeedYear.Value.Value;
    }

    private void numericUpDown_CarYearMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.LimitYear = numericUpDown_LimitYear.Value.Value;
    }

    private void numericUpDown_NeedLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NeedLength = numericUpDown_NeedLength.Value.Value;

    }

    private void numericUpDown_LimitLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.LimitLength = numericUpDown_LimitLength.Value.Value;
    }

    private void numericUpDown_Nitrous_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Regulations.NOS = numericUpDown_Nitrous.Value.Value;
    }

    // ------ Non-generated functions ------

    public void Refresh()
    {
        PopulateControls();
        PopulateRegulations();
    }

    public void PopulateRegulations()
    {
        if (Regulations is null)
            return;

        lb_AllowedCars.Items.Clear();
        lb_BannedCars.Items.Clear();
        lb_AllowedManufacturers.Items.Clear();

        UpdateCountriesAndCategoriesRegulations();

        if (Regulations.Cars.Count > 0)
        {
            foreach (MCarThin vehicle in Regulations.Cars)
            {
                string carName = GameDatabase.GetCarNameByLabel(vehicle.CarLabel) ?? vehicle.CarLabel;
                if (!lb_AllowedCars.Items.Contains(carName))
                    lb_AllowedCars.Items.Add(carName);

            }
            UpdateCarsLists();
        }

        if (Regulations.BanCars.Count > 0)
        {
            foreach (MCarThin vehicle in Regulations.BanCars)
            {
                string carName = GameDatabase.GetCarNameByLabel(vehicle.CarLabel) ?? vehicle.CarLabel;
                if (!lb_BannedCars.Items.Contains(carName))
                    lb_BannedCars.Items.Add(carName);

            }
            UpdateCarsLists();
        }

        foreach (Tuner manufacturer in Regulations.Tuners)
        {
            var results = GameDatabase.ExecuteQuery($"SELECT ManufacturerName FROM Manufacturers WHERE ManufacturerInternalName = '{manufacturer}' ORDER BY ManufacturerName");
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
        PopulateGeneralRegulations();
    }

    public void UpdateCountriesAndCategoriesRegulations()
    {
        if (Regulations is null)
            return;

        lb_AllowedCountries.Items.Clear();
        lb_AllowedCategories.Items.Clear();

        for (int i = cb_CarCountries.Items.Count - 1; i > 0; i--)
            cb_CarCountries.Items.Remove(cb_CarCountries.Items[i]);

        for (int i = cb_CarCategories.Items.Count - 1; i > 0; i--)
            cb_CarCategories.Items.Remove(cb_CarCategories.Items[i]);

        foreach (Country country in Regulations.Countries)
            cb_CarCountries.Items.Add(country);

        if (Regulations.Countries.Count > 0)
        {
            foreach (Country country in Regulations.Countries)
            {
                if (!lb_AllowedCountries.Items.Contains(country))
                    lb_AllowedCountries.Items.Add(country);
                cb_CarCountries.Items.Remove(country);
            }
        }

        foreach (Country cat in (Country[])Enum.GetValues(typeof(Country)))
        {
            if (cat == Country.NumOfCountries)
                continue;

            cb_CarCountries.Items.Add(cat.Humanize());
        }

        foreach (var cat in (CarCategoryRestriction[])Enum.GetValues(typeof(CarCategoryRestriction)))
            cb_CarCategories.Items.Add(cat.Humanize());

        if (Regulations.CarCategories.Count > 0)
        {
            foreach (CarCategoryRestriction cat in Regulations.CarCategories)
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

        var results = App.GameDatabase.ExecuteQuery(
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
        AspirationBits mask = Regulations.NeedAspiration;
        if (mask == AspirationBits.NONE_SPECIFIED)
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
        DrivetrainBits mask = Regulations.NeedDrivetrain;
        if (mask == DrivetrainBits.NONE_SPECIFIED)
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
        cbo_LimitTireF.SelectedIndex = (int)Regulations.LimitTireFront + 1;
        cbo_NeedTireF.SelectedIndex = (int)Regulations.NeedTireFront + 1;
        cbo_LimitTireR.SelectedIndex = (int)Regulations.LimitTireRear + 1;
        cbo_NeedTireR.SelectedIndex = (int)Regulations.NeedTireRear + 1;
    }

    private void PopulateGeneralRegulations()
    {
        numericUpDown_NeedPP.Value = Regulations.NeedPP;
        numericUpDown_LimitPP.Value = Regulations.LimitPP;
        numericUpDown_NeedTorque.Value = Regulations.NeedTorque;
        numericUpDown_LimitTorque.Value = Regulations.LimitTorque;
        numericUpDown_NeedPower.Value = Regulations.NeedPower;
        numericUpDown_LimitPower.Value = Regulations.LimitPower;
        numericUpDown_NeedWeight.Value = Regulations.NeedWeight;
        numericUpDown_LimitWeight.Value = Regulations.LimitWeight;
        numericUpDown_NeedYear.Value = Regulations.NeedYear;
        numericUpDown_LimitYear.Value = Regulations.LimitYear;
        numericUpDown_NeedLength.Value = Regulations.NeedLength;
        numericUpDown_LimitLength.Value = Regulations.LimitLength;
        numericUpDown_Nitrous.Value = Regulations.NOS;
    }

    private void PopulateControls()
    {
        if (GameDatabase is null)
            return;

        if (cbo_Manufacturers.Items.Count == 0)
        {
            foreach (var manufacturer in GameDatabase.GetAllManufacturersSorted())
                cbo_Manufacturers.Items.Add(manufacturer);

            cbo_Manufacturers.SelectedIndex = 0;
        }

        if (cbo_LimitTireF.Items.Count == 1) // Load them if empty
        {
            var tires = (TireType[])Enum.GetValues(typeof(TireType));
            for (int i = 0; i < tires.Length - 1; i++) // - 1 as the combo boxes have a default "none" entry
            {
                var tire = (TireType)i;
                string tireName = tire.Humanize();
                cbo_LimitTireF.Items.Add(tireName);
                cbo_LimitTireR.Items.Add(tireName);
                cbo_NeedTireF.Items.Add(tireName);
                cbo_NeedTireR.Items.Add(tireName);
            }
        }
    }
}
