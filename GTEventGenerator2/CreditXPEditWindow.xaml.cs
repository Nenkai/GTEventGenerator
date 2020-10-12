﻿using System;
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

namespace GTEventGenerator
{
    /// <summary>
    /// Interaction logic for CreditXPEditWindow.xaml
    /// </summary>
    public partial class CreditXPEditWindow : Window
    {
        public bool Saved { get; set; }
        public int[] Values { get; set; } = new int[16];
        public CreditXPEditWindow(int[] values)
        {
            InitializeComponent();

            for (int i = 0; i < values.Length; i++)
                Values[i] = values[i];

            num_1.Value = Values[0];
            num_2.Value = Values[1];
            num_3.Value = Values[2];
            num_4.Value = Values[3];
            num_5.Value = Values[4];
            num_6.Value = Values[5];
            num_7.Value = Values[6];
            num_8.Value = Values[7];
            num_9.Value = Values[8];
            num_10.Value = Values[9];
            num_11.Value = Values[10];
            num_12.Value = Values[11];
            num_13.Value = Values[12];
            num_14.Value = Values[13];
            num_15.Value = Values[14];
            num_16.Value = Values[15];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Values[0] = num_1.Value.Value;
            Values[1] = num_2.Value.Value;
            Values[2] = num_3.Value.Value;
            Values[3] = num_4.Value.Value;
            Values[4] = num_5.Value.Value;
            Values[5] = num_6.Value.Value;
            Values[6] = num_7.Value.Value;
            Values[7] = num_8.Value.Value;
            Values[8] = num_9.Value.Value;
            Values[9] = num_10.Value.Value;
            Values[10] = num_11.Value.Value;
            Values[11] = num_12.Value.Value;
            Values[12] = num_13.Value.Value;
            Values[13] = num_14.Value.Value;
            Values[14] = num_15.Value.Value;
            Values[15] = num_16.Value.Value;
            Saved = true;
        }
    }
}
