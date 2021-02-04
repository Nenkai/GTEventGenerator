using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

using GTEventGenerator.Entities;
using GTEventGenerator.Utils;
using Humanizer;

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        #region Tire Constraints
        private void comboBox_ConstrainedTiresMinF_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1)
                return;

            CurrentEvent.Constraints.NeededFrontTire = (TireType)(cb.SelectedIndex - 1);
        }

        private void comboBox_ConstrainedTiresMinR_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1)
                return;

            CurrentEvent.Constraints.NeededRearTire = (TireType)(cb.SelectedIndex - 1);
        }

        private void comboBox_ConstrainedTiresMaxF_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1)
                return;

            CurrentEvent.Constraints.FrontTireLimit = (TireType)(cb.SelectedIndex - 1);
        }

        private void comboBox_ConstrainedTiresMaxR_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1)
                return;

            CurrentEvent.Constraints.RearTireLimit = (TireType)(cb.SelectedIndex - 1);
        }

        private void comboBox_ConstrainedTiresSuggestF_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1)
                return;

            CurrentEvent.Constraints.SuggestedFrontTire = (TireType)(cb.SelectedIndex - 1);
        }

        private void comboBox_ConstrainedTiresSuggestR_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1)
                return;

            CurrentEvent.Constraints.SuggestedRearTire = (TireType)(cb.SelectedIndex - 1);
        }
        #endregion

        public void sud_PowerLimit_ValueChanged(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Constraints.PowerLimit = sud_PowerLimit.Value.Value;
        }

        public void PopulateConstraints()
        {
            if (comboBox_ConstrainedTiresMinF.Items.Count == 1)
            {
                var tires = (TireType[])Enum.GetValues(typeof(TireType));
                for (int i = 0; i < tires.Length - 1; i++) // -1 as the combo boxes have a default "none" entry
                {
                    var tire = (TireType)i;
                    string tireName = tire.Humanize();
                    comboBox_ConstrainedTiresMinF.Items.Add(tireName);
                    comboBox_ConstrainedTiresMaxR.Items.Add(tireName);
                    comboBox_ConstrainedTiresMinR.Items.Add(tireName);
                    comboBox_ConstrainedTiresMaxF.Items.Add(tireName);
                    comboBox_ConstrainedTiresSuggestF.Items.Add(tireName);
                    comboBox_ConstrainedTiresSuggestR.Items.Add(tireName);
                }
            }

            comboBox_ConstrainedTiresMinF.SelectedIndex = (int)CurrentEvent.Constraints.NeededFrontTire+1;
            comboBox_ConstrainedTiresMinR.SelectedIndex = (int)CurrentEvent.Constraints.NeededRearTire+1;
            comboBox_ConstrainedTiresMaxR.SelectedIndex = (int)CurrentEvent.Constraints.RearTireLimit+1;
            comboBox_ConstrainedTiresMaxF.SelectedIndex = (int)CurrentEvent.Constraints.FrontTireLimit+1;
            comboBox_ConstrainedTiresSuggestF.SelectedIndex = (int)CurrentEvent.Constraints.SuggestedFrontTire+1;
            comboBox_ConstrainedTiresSuggestR.SelectedIndex = (int)CurrentEvent.Constraints.SuggestedRearTire+1;

            sud_PowerLimit.Value = CurrentEvent.Constraints.PowerLimit;
            CurrentEvent.Constraints.NeedsPopulating = false;
        }
    }
}