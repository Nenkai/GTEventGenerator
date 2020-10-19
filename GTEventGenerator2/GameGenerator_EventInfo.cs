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
using System.Windows;
using System.Windows.Controls;
using System.Xml;

using GTEventGenerator.Entities;
using GTEventGenerator.Utils;

using Humanizer;
namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        private void txt_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentEvent.Information.SetDescription(txt_Description.Text);
        }

        private void txt_OneLineTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentEvent.Information.SetOneLineTitle(txt_OneLineTitle.Text);
        }


        public void PopulateOneTimeInfoControls()
        {
            if (comboBox_CompleteType.Items.Count > 0)
                return; // Assume it was already all filled in

            var types = (RankingDisplayType[])Enum.GetValues(typeof(RankingDisplayType));
            for (int i = 0; i < types.Length; i++)
            {
                var comp = (RankingDisplayType)i;
                string compName = comp.Humanize();
                cb_RankingDisplayType.Items.Add(compName);
            }
        }

        public void PopulateEventInfoTab()
        {
            PopulateOneTimeInfoControls();

            if (!CurrentEvent.Information.NeedsPopulating)
                return;

            txt_EventTitle.Text = CurrentEvent.Information.Titles["GB"];
            txt_Description.Text = CurrentEvent.Information.Descriptions["GB"];
            txt_OneLineTitle.Text = CurrentEvent.Information.OneLineTitles["GB"];

            cb_RankingDisplayType.SelectedIndex = (int)CurrentEvent.RankingDisplayType;
            CurrentEvent.Information.NeedsPopulating = false;
        }
    }
}
