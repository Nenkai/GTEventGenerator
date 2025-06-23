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
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;

using PDTools.Enums;
using PDTools.Structures.MGameParameter;

using Humanizer;

namespace GTEventMaker
{
    /// <summary>
    /// Interaction logic for EventEntryEditWindow.xaml
    /// </summary>
    public partial class EventEntryEditWindow : Window
    {
        private Entry _entry { get; set; }
        private int _maxColors;

        public EventEntryEditWindow(Entry entry, int maxColors)
        {
            _entry = entry;
            this.DataContext = _entry;
            InitializeComponent();

            _maxColors = maxColors;

            var startTypes = ((StartType[])Enum.GetValues(typeof(StartType)))
                                                .OrderBy(e => (int)e).ToArray();

            for (int i = 0; i < startTypes.Length; i++)
            {
                string sName = startTypes[i].Humanize();
                comboBox_StartingType.Items.Add(sName);
            }

            comboBox_StartingType.SelectedIndex = (int)_entry.StartType + 1;

            iud_CorneringSkill.Value = _entry.AISkillCornering;
            iud_BrakingSkill.Value = _entry.AISkillBraking;
            iud_AccelSkill.Value = _entry.AISkillAccelerating;
            iud_StartingSkill.Value = _entry.AISkillStarting;
            iud_Roughness.Value = _entry.AIRoughness;
            iud_InitialVelocity.Value = _entry.InitialVelocity;
            iud_InitialVCoord.Value = _entry.InitialPosition;
            iud_Delay.Value = _entry.Delay;

            tb_DriverName.Text = _entry.DriverName;
            tb_DriverCountry.Text = _entry.DriverRegion;
            iud_RaceClassID.Value = _entry.RaceClassID;
            iud_ProxyDriverModel.Value = _entry.ProxyDriverModel;

            iud_CarColorIndex.Value = _entry.Car.Paint;
        }

        private void iud_CarColorIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (iud_CarColorIndex.Value >= _maxColors)
                iud_CarColorIndex.Value = _maxColors - 1;
            else if (iud_CarColorIndex.Value < 0)
                iud_CarColorIndex.Value = 0;

            _entry.Car.Paint = (short)iud_CarColorIndex.Value;
        }

        private void comboBox_StartingType_SelectedIndexChanged(object sender, EventArgs e)
            => _entry.StartType = (StartType)comboBox_StartingType.SelectedIndex - 1;

        private void btn_CarSettings_Click(object sender, RoutedEventArgs e)
        {
            //var tuningWindow = new EventEntryTuningWindow(_entry);
            //tuningWindow.ShowDialog();
        }

        private void iud_CorneringSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.AISkillCornering = iud_CorneringSkill.Value.Value;
        }

        private void iud_BrakingSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.AISkillBraking = iud_BrakingSkill.Value.Value;
        }

        private void iud_AccelSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.AISkillAccelerating = iud_AccelSkill.Value.Value;
        }

        private void iud_StartingSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.AISkillStarting = iud_StartingSkill.Value.Value;
        }

        private void iud_Roughness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.AIRoughness = iud_Roughness.Value.Value;
        }

        private void tb_DriverName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _entry.DriverName = tb_DriverName.Text;
        }

        private void tb_DriverCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _entry.DriverRegion = tb_DriverCountry.Text;
        }

        private void iud_InitialVCoord_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.InitialPosition = iud_InitialVCoord.Value.Value;
        }

        private void iud_InitialVelocity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.InitialVelocity = iud_InitialVelocity.Value.Value;
        }

        private void iud_Delay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.Delay = iud_Delay.Value.Value;
        }

        private void iud_RaceClassID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.RaceClassID = iud_RaceClassID.Value.Value;
        }

        private void iud_ProxyDriverModel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entry.ProxyDriverModel = iud_ProxyDriverModel.Value.Value;
        }

        private void btn_EntryBaseEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EventEntryBaseEditWindow(_entry.EntryBase);
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }
    }
}
