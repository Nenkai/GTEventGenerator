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
        public EventEntryEditWindow(RaceEntry entry)
        {
            _entry = entry;
            this.DataContext = _entry;
            InitializeComponent();

            if (!entry.IsAI)
            {
                iud_AccelSkill.IsEnabled = false;
                iud_BaseSkill.IsEnabled = false;
                iud_CorneringSkill.IsEnabled = false;
                iud_BrakingSkill.IsEnabled = false;

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
    }
}
