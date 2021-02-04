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

using GTEventGenerator.Entities;
using Humanizer;
namespace GTEventGenerator
{
    /// <summary>
    /// Interaction logic for EventEntryTuningWindow.xaml
    /// </summary>
    public partial class EventEntryTuningWindow : Window
    {
        private EventEntry _entry { get; set; }
        public EventEntryTuningWindow(EventEntry entry)
        {
            _entry = entry;
            this.DataContext = _entry;
            InitializeComponent();

            var tires = (TireType[])Enum.GetValues(typeof(TireType));
            for (int i = 0; i < tires.Length - 1; i++) // - 1 as the combo boxes have a default "none" entry
            {
                var tire = (TireType)i;
                string tireName = tire.Humanize();
                cb_EntryTireF.Items.Add(tireName);
                cb_EntryTireR.Items.Add(tireName);
            }

            var tuneStage = ((EngineNATuneState[])Enum.GetValues(typeof(EngineNATuneState)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < tuneStage.Length; i++)
            {
                string tName = tuneStage[i].Humanize();
                cb_EngineStage.Items.Add(tName);
            }

            var turbos = ((EngineTurboKit[])Enum.GetValues(typeof(EngineTurboKit)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < turbos.Length; i++)
            {
                string tName = turbos[i].Humanize();
                cb_Turbo.Items.Add(tName);
            }

            var computers = ((EngineComputer[])Enum.GetValues(typeof(EngineComputer)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < computers.Length; i++)
            {
                string tName = computers[i].Humanize();
                cb_Computer.Items.Add(tName);
            }

            var suspensions = ((Suspension[])Enum.GetValues(typeof(Suspension)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < suspensions.Length; i++)
            {
                string tName = suspensions[i].Humanize();
                cb_Suspension.Items.Add(tName);
            }

            var transmissions = ((Transmission[])Enum.GetValues(typeof(Transmission)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < transmissions.Length; i++)
            {
                string tName = transmissions[i].Humanize();
                cb_Transmission.Items.Add(tName);
            }

            var exhausts = ((Muffler[])Enum.GetValues(typeof(Muffler)))
                               .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < exhausts.Length; i++)
            {
                string tName = exhausts[i].Humanize();
                cb_Exhaust.Items.Add(tName);
            }

            cb_EngineStage.SelectedIndex = (int)_entry.EngineStage + 1;
            cb_Turbo.SelectedIndex = (int)_entry.TurboKit + 1;
            cb_Computer.SelectedIndex = (int)_entry.Computer + 1;
            cb_Suspension.SelectedIndex = (int)_entry.Suspension + 1;
            cb_Transmission.SelectedIndex = (int)_entry.Transmission + 1;
            cb_Exhaust.SelectedIndex = (int)_entry.Exhaust + 1;

            cb_EntryTireF.SelectedIndex = (int)_entry.TireFront + 1;
            cb_EntryTireR.SelectedIndex = (int)_entry.TireRear + 1;
        }

        private void cb_EntryTireF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entry.TireFront = (TireType)cb_EntryTireF.SelectedIndex - 1;
        }

        private void cb_EntryTireR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entry.TireRear = (TireType)cb_EntryTireR.SelectedIndex - 1;
        }

        private void cb_EngineStage_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.EngineStage = (EngineNATuneState)(cb_EngineStage.SelectedIndex - 1);

        private void cb_Turbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.TurboKit = (EngineTurboKit)(cb_Turbo.SelectedIndex - 1);

        private void cb_Computer_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Computer = (EngineComputer)(cb_Computer.SelectedIndex - 1);

        private void cb_Suspension_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Suspension = (Suspension)(cb_Suspension.SelectedIndex - 1);

        private void cb_Transmission_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Transmission = (Transmission)(cb_Transmission.SelectedIndex - 1);

        private void cb_Exhaust_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entry.Exhaust = (Muffler)(cb_Exhaust.SelectedIndex - 1);
    }
}
