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
using System.Linq;

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
        private EventEntry _entry { get; set; }
        private int _maxColors;
        public EventEntryEditWindow(EventEntry entry, int maxColors)
        {
            _entry = entry;
            this.DataContext = _entry;
            InitializeComponent();

            _maxColors = maxColors;
            grp_AIParameters.IsEnabled = entry.IsAI;

            iud_CarColorIndex.Value = entry.ColorIndex;

            var startTypes = ((StartType[])Enum.GetValues(typeof(StartType)))
                                                .OrderBy(e => (int)e).ToArray();

            for (int i = 0; i < startTypes.Length; i++)
            {
                string sName = startTypes[i].Humanize();
                comboBox_StartingType.Items.Add(sName);
            }

            comboBox_StartingType.SelectedIndex = (int)_entry.StartType + 1;
        }

        private void iud_CarColorIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (iud_CarColorIndex.Value >= _maxColors)
                iud_CarColorIndex.Value = _maxColors - 1;
            else if (iud_CarColorIndex.Value < 0)
                iud_CarColorIndex.Value = 0;

            _entry.ColorIndex = (short)iud_CarColorIndex.Value;
        }

        private void comboBox_StartingType_SelectedIndexChanged(object sender, EventArgs e)
            => _entry.StartType = (StartType)comboBox_StartingType.SelectedIndex - 1;

        private void btn_CarSettings_Click(object sender, RoutedEventArgs e)
        {
            var tuningWindow = new EventEntryTuningWindow(_entry);
            tuningWindow.ShowDialog();
        }
    }
}
