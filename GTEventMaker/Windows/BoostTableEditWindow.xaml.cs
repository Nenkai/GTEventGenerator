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

using PDTools.Structures.MGameParameter;

namespace GTEventMaker
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
            tabBoostTable.SelectedIndex = 0;
        }

        public void Populate()
        {
            iud_FrontRate1.Value = _tables[tabBoostTable.SelectedIndex].FrontRate1;
            iud_FrontRate2.Value = _tables[tabBoostTable.SelectedIndex].FrontRate2;
            iud_FrontDistance1.Value = _tables[tabBoostTable.SelectedIndex].FrontDistance1;
            iud_FrontDistance2.Value = _tables[tabBoostTable.SelectedIndex].FrontDistance2;
            iud_RearRate1.Value = _tables[tabBoostTable.SelectedIndex].RearRate1;
            iud_RearRate2.Value = _tables[tabBoostTable.SelectedIndex].RearRate2;
            iud_RearDistance1.Value = _tables[tabBoostTable.SelectedIndex].RearDistance1;
            iud_RearDistance2.Value = _tables[tabBoostTable.SelectedIndex].RearDistance2;
            iud_ReferenceRank.Value = _tables[tabBoostTable.SelectedIndex].TargetPosition;
            iud_RaceProgress.Value = _tables[tabBoostTable.SelectedIndex].RaceProgress;
        }

        private void tabBoostTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabBoostTable.SelectedIndex != -1)
                this.DataContext = _tables[tabBoostTable.SelectedIndex];

            Populate();
        }


        private void iud_FrontInitialRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].FrontRate1 = iud_FrontRate1.Value.Value;
        }

        private void iud_FrontStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].FrontDistance1 = iud_FrontDistance1.Value.Value;

        }

        private void iud_FrontMaximumRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].FrontRate2 = iud_FrontRate2.Value.Value;

        }

        private void iud_FrontLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].FrontDistance2 = iud_FrontDistance2.Value.Value;
        }

        private void iud_RearInitialRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].RearRate1 = iud_RearRate1.Value.Value;
        }

        private void iud_RearStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].RearDistance1 = iud_RearDistance1.Value.Value;
        }

        private void iud_RearMaximumRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].RearRate2 = iud_RearRate2.Value.Value;
        }

        private void iud_RearLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].RearDistance2 = iud_RearDistance2.Value.Value;
        }

        private void iud_ReferenceRank_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].TargetPosition = iud_ReferenceRank.Value.Value;
        }

        private void iud_RaceProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _tables[tabBoostTable.SelectedIndex].RaceProgress = iud_RaceProgress.Value.Value;
        }
    }
}
