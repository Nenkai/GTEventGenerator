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

using Humanizer;

using PDTools.Enums;
namespace GTEventMaker
{
    /// <summary>
    /// Interaction logic for StarTableEditWindow.xaml
    /// </summary>
    public partial class StarTableEditWindow : Window
    {
        public bool Saved { get; set; }
        public List<FinishResult> Values { get; set; } = new List<FinishResult>();

        public StarTableEditWindow(List<FinishResult> values)
        {
            InitializeComponent();

            for (int i = 0; i < values.Count; i++)
                Values.Add(values[i]);

            PopulateComboBox(num_1);
            PopulateComboBox(num_2);
            PopulateComboBox(num_3);
            PopulateComboBox(num_4);
            PopulateComboBox(num_5);
            PopulateComboBox(num_6);
            PopulateComboBox(num_7);
            PopulateComboBox(num_8);
            PopulateComboBox(num_9);
            PopulateComboBox(num_10);
            PopulateComboBox(num_11);
            PopulateComboBox(num_12);
            PopulateComboBox(num_13);
            PopulateComboBox(num_14);
            PopulateComboBox(num_15);
            PopulateComboBox(num_16);

            num_1.SelectedIndex = (int)Values[0] + 1;
            num_2.SelectedIndex = (int)Values[1] + 1;
            num_3.SelectedIndex = (int)Values[2] + 1;
            num_4.SelectedIndex = (int)Values[3] + 1;
            num_5.SelectedIndex = (int)Values[4] + 1;
            num_6.SelectedIndex = (int)Values[5] + 1;
            num_7.SelectedIndex = (int)Values[6] + 1;
            num_8.SelectedIndex = (int)Values[7] + 1;
            num_9.SelectedIndex = (int)Values[8] + 1;
            num_10.SelectedIndex = (int)Values[9] + 1;
            num_11.SelectedIndex = (int)Values[10] + 1;
            num_12.SelectedIndex = (int)Values[11] + 1;
            num_13.SelectedIndex = (int)Values[12] + 1;
            num_14.SelectedIndex = (int)Values[13] + 1;
            num_15.SelectedIndex = (int)Values[14] + 1;
            num_16.SelectedIndex = (int)Values[15] + 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Values[0] = (FinishResult)num_1.SelectedIndex - 1;
            Values[1] = (FinishResult)num_2.SelectedIndex - 1;
            Values[2] = (FinishResult)num_3.SelectedIndex - 1;
            Values[3] = (FinishResult)num_4.SelectedIndex - 1;
            Values[4] = (FinishResult)num_5.SelectedIndex - 1;
            Values[5] = (FinishResult)num_6.SelectedIndex - 1;
            Values[6] = (FinishResult)num_7.SelectedIndex - 1;
            Values[7] = (FinishResult)num_8.SelectedIndex - 1;
            Values[8] = (FinishResult)num_9.SelectedIndex - 1;
            Values[9] = (FinishResult)num_10.SelectedIndex - 1;
            Values[10] = (FinishResult)num_11.SelectedIndex - 1;
            Values[11] = (FinishResult)num_12.SelectedIndex - 1;
            Values[12] = (FinishResult)num_13.SelectedIndex - 1;
            Values[13] = (FinishResult)num_14.SelectedIndex - 1;
            Values[14] = (FinishResult)num_15.SelectedIndex - 1;
            Values[15] = (FinishResult)num_16.SelectedIndex - 1;
            Saved = true;
            Close();
        }

        private void PopulateComboBox(ComboBox cb)
        {
            for (int i = -1; i < 38; i++)
            {
                var t = (FinishResult)i;
                string tName = t.Humanize();
                cb.Items.Add(tName);
            }
        }
    }
}
