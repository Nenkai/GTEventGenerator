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
    /// Interaction logic for BoostTableEditWindow.xaml
    /// </summary>
    public partial class BoostTableEditWindow : Window
    {
        private BoostTable[] _tables;
        public BoostTableEditWindow(BoostTable[] tables)
        {
            _tables = tables;
            this.DataContext = tables[0];
            InitializeComponent();
        }

        private void tabBoostTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabBoostTable.SelectedIndex != -1)
                this.DataContext = _tables[tabBoostTable.SelectedIndex];
        }
    }
}
