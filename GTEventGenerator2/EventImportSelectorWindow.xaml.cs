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

namespace GTEventGenerator
{
    /// <summary>
    /// Interaction logic for EventImportSelectorWindow.xaml
    /// </summary>
    public partial class EventImportSelectorWindow : Window
    {
        public Event SelectedEvent { get; set; }
        private GameParameter _gp;
        public EventImportSelectorWindow(GameParameter gp)
        {
            InitializeComponent();
            _gp = gp;

            lb_Events.ItemsSource = gp.Events.Select(e => e.Name);
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Events.SelectedIndex != -1)
                SelectedEvent = _gp.Events[lb_Events.SelectedIndex];

            Close();
        }
    }
}
