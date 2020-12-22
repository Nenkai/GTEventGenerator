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
        private EventEntry _entry { get; set; }
        private int _maxColors;
        public EventEntryEditWindow(EventEntry entry, int maxColors)
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

            iud_CarColorIndex.Value = entry.ColorIndex;
        }

        private void iud_CarColorIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (iud_CarColorIndex.Value >= _maxColors)
                iud_CarColorIndex.Value = _maxColors - 1;
            else if (iud_CarColorIndex.Value < 0)
                iud_CarColorIndex.Value = 0;

            _entry.ColorIndex = (int)iud_CarColorIndex.Value;
        }

        private void btn_CarSettings_Click(object sender, RoutedEventArgs e)
        {
            var tuningWindow = new EventEntryTuningWindow(_entry);
            tuningWindow.ShowDialog();
        }
    }
}
