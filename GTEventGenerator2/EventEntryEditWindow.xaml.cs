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

using GTEventGenerator.Entities;
using Humanizer;

namespace GTEventGenerator
{
    /// <summary>
    /// Interaction logic for EventEntryEditWindow.xaml
    /// </summary>
    public partial class EventEntryEditWindow : Window
    {
        private RaceEntry _entry { get; set; }
        private int _maxColors;
        public EventEntryEditWindow(RaceEntry entry, int maxColors)
        {
            _entry = entry;
            this.DataContext = _entry;
            InitializeComponent();

            _maxColors = maxColors;
            if (!entry.IsAI)
            {
                iud_AccelSkill.IsEnabled = false;
                iud_BaseSkill.IsEnabled = false;
                iud_CorneringSkill.IsEnabled = false;
                iud_BrakingSkill.IsEnabled = false;
                iud_Roughness.IsEnabled = false;
                iud_StartingSkill.IsEnabled = false;

                tb_DriverName.IsEnabled = false;
                tb_DriverCountry.IsEnabled = false;
            }

            var tires = (TireType[])Enum.GetValues(typeof(TireType));
            for (int i = 0; i < tires.Length - 1; i++) // - 1 as the combo boxes have a default "none" entry
            {
                var tire = (TireType)i;
                string tireName = tire.Humanize();
                cb_EntryTireF.Items.Add(tireName);
                cb_EntryTireR.Items.Add(tireName);
            }

            var tuneStage = (EngineNATuneState[])Enum.GetValues(typeof(EngineNATuneState));
            for (int i = 0; i < tuneStage.Length; i++)
            {
                var t = (EngineNATuneState)i;
                string tName = t.Humanize();
                cb_EngineStage.Items.Add(tName);
            }

            var turbos = (EngineTurboKit[])Enum.GetValues(typeof(EngineTurboKit));
            for (int i = 0; i < turbos.Length; i++)
            {
                var t = (EngineTurboKit)i;
                string tName = t.Humanize();
                cb_Turbo.Items.Add(tName);
            }

            var computers = (EngineComputer[])Enum.GetValues(typeof(EngineComputer));
            for (int i = 0; i < computers.Length; i++)
            {
                var t = (EngineComputer)i;
                string tName = t.Humanize();
                cb_Computer.Items.Add(tName);
            }

            var suspensions = (Suspension[])Enum.GetValues(typeof(Suspension));
            for (int i = 0; i < suspensions.Length; i++)
            {
                var t = (Suspension)i;
                string tName = t.Humanize();
                cb_Suspension.Items.Add(tName);
            }

            var transmissions = (Transmission[])Enum.GetValues(typeof(Transmission));
            for (int i = 0; i < transmissions.Length; i++)
            {
                var t = (Transmission)i;
                string tName = t.Humanize();
                cb_Transmission.Items.Add(tName);
            }

            var exhausts = (Muffler[])Enum.GetValues(typeof(Muffler));
            for (int i = 0; i < exhausts.Length; i++)
            {
                var t = (Muffler)i;
                string tName = t.Humanize();
                cb_Exhaust.Items.Add(tName);
            }

            cb_EngineStage.SelectedIndex = (int)_entry.EngineStage;
            cb_Turbo.SelectedIndex = (int)_entry.TurboKit;
            cb_Computer.SelectedIndex = (int)_entry.Computer;
            cb_Suspension.SelectedIndex = (int)_entry.Suspension;
            cb_Transmission.SelectedIndex = (int)_entry.Transmission;
            cb_Exhaust.SelectedIndex = (int)_entry.Exhaust;

            cb_EntryTireF.SelectedIndex = (int)_entry.TireFront + 1;
            cb_EntryTireR.SelectedIndex = (int)_entry.TireRear + 1;

            iud_CarColorIndex.Value = entry.ColorIndex;
        }

        private void cb_EntryTireF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entry.TireFront = (TireType)cb_EntryTireF.SelectedIndex - 1;
        }

        private void cb_EntryTireR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entry.TireRear = (TireType)cb_EntryTireR.SelectedIndex - 1;
        }

        private void iud_CarColorIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (iud_CarColorIndex.Value >= _maxColors)
                iud_CarColorIndex.Value = _maxColors - 1;
            else if (iud_CarColorIndex.Value < 0)
                iud_CarColorIndex.Value = 0;

            _entry.ColorIndex = (int)iud_CarColorIndex.Value;
        }

        private void cb_EngineStage_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.EngineStage = (EngineNATuneState)cb_EngineStage.SelectedIndex;

        private void cb_Turbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.TurboKit = (EngineTurboKit)cb_Turbo.SelectedIndex;

        private void cb_Computer_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Computer = (EngineComputer)cb_Computer.SelectedIndex;

        private void cb_Suspension_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Suspension = (Suspension)cb_Suspension.SelectedIndex;

        private void cb_Transmission_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Transmission = (Transmission)cb_Transmission.SelectedIndex;

        private void cb_Exhaust_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Exhaust = (Muffler)cb_Exhaust.SelectedIndex;
    }
}
