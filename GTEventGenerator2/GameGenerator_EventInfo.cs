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
        private void txtEventName_TextChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentEvent is null || !_processEventSwitch)
                return;

            // We use GB as the event gen display name
            if (EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex] == "GB")
            {
                CurrentEvent.Name = txt_EventTitle.Text;
                _processEventSwitch = false;
                txt_EventTitle.Text = CurrentEvent.Name;
                _processEventSwitch = true;
            }

            CurrentEvent.Information.SetTitle(EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex], txt_EventTitle.Text);

            int currentEventIndex = GameParameter.Events.IndexOf(CurrentEvent);

            if (currentEventIndex < EventNames.Count)
                EventNames[currentEventIndex] = $"{CurrentEvent.Index} - {CurrentEvent.Name}";

            ReloadEventLists();
            UpdateEventListing();

            _processEventSwitch = false;
            cb_QuickEventPicker.SelectedIndex = currentEventIndex;
            _processEventSwitch = true;
        }

        private void txt_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentEvent.Information.SetDescription(EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex], txt_Description.Text);
        }

        private void txt_OneLineTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentEvent.Information.SetOneLineTitle(EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex], txt_OneLineTitle.Text);
        }

        private void btn_TitleApplyAllLocales_Click(object sender, RoutedEventArgs e)
        {
            foreach (var code in EventInformation.LocaleCodes)
                CurrentEvent.Information.SetTitle(code, txt_EventTitle.Text);

            // Update everything as the GB one has been changed
            if (EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex] != "GB")
            {
                CurrentEvent.Name = txt_EventTitle.Text;
                int currentEventIndex = GameParameter.Events.IndexOf(CurrentEvent);

                if (currentEventIndex < EventNames.Count)
                    EventNames[currentEventIndex] = $"{CurrentEvent.Index} - {CurrentEvent.Name}";

                ReloadEventLists();
                UpdateEventListing();

                _processEventSwitch = false;
                cb_QuickEventPicker.SelectedIndex = currentEventIndex;
                _processEventSwitch = true;
            }
        }

        private void btn_OneLineTitleApplyAllLocales_Click(object sender, RoutedEventArgs e)
        {
            foreach (var code in EventInformation.LocaleCodes)
                CurrentEvent.Information.SetOneLineTitle(code, txt_OneLineTitle.Text);
        }

        private void btn_DescriptionApplyAllLocales_Click(object sender, RoutedEventArgs e)
        {
            foreach (var code in EventInformation.LocaleCodes)
                CurrentEvent.Information.SetDescription(code, txt_Description.Text);
        }

        private void cb_InfoLanguage_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cb_InfoLanguage.SelectedIndex == -1)
                return;
            UpdateLocalizationInfo();
        }

        public void PopulateOneTimeInfoControls()
        {
            if (cb_RankingDisplayType.Items.Count > 0)
                return; // Assume it was already all filled in

            var types = (RankingDisplayType[])Enum.GetValues(typeof(RankingDisplayType));
            for (int i = 0; i < types.Length; i++)
            {
                var comp = (RankingDisplayType)i;
                string compName = comp.Humanize();
                cb_RankingDisplayType.Items.Add(compName);
            }

            if (!cb_InfoLanguage.HasItems)
            {
                foreach (var locale in EventInformation.Locales.Values)
                    cb_InfoLanguage.Items.Add(locale);
            }

            cb_InfoLanguage.SelectedIndex = 2; // Bri'ish
        }

        

        public void PopulateEventInfoTab()
        {
            PopulateOneTimeInfoControls();

            if (!CurrentEvent.Information.NeedsPopulating)
                return;

            UpdateLocalizationInfo();

            cb_RankingDisplayType.SelectedIndex = (int)CurrentEvent.RankingDisplayType;
            CurrentEvent.Information.NeedsPopulating = false;
        }

        public void UpdateLocalizationInfo()
        {
            txt_EventTitle.Text = CurrentEvent.Information.Titles[EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex]];
            txt_Description.Text = CurrentEvent.Information.Descriptions[EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex]];
            txt_OneLineTitle.Text = CurrentEvent.Information.OneLineTitles[EventInformation.LocaleCodes[cb_InfoLanguage.SelectedIndex]];
        }
    }
}
