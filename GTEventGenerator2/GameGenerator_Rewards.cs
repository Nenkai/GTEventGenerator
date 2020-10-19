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

using GTEventGenerator.Entities;
using GTEventGenerator.Utils;

using Humanizer;

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        private void btnCreditRewards_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CreditXPEditWindow(CurrentEvent.Rewards.MoneyPrizes);
            dlg.ShowDialog();

            if (dlg.Saved)
                CurrentEvent.Rewards.MoneyPrizes = dlg.Values;
        }

        private void btnXPRewards_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CreditXPEditWindow(CurrentEvent.Rewards.PointTable);
            dlg.ShowDialog();

            if (dlg.Saved)
                CurrentEvent.Rewards.PointTable = dlg.Values;
        }

        public void btn_PresentOne_Clicked(object sender, RoutedEventArgs e)
        {
            var dlg = new PresentPickerWindow(GameDatabase);
            dlg.ShowDialog();

            if (dlg.SelectedType != PresentPickerWindow.SelectionType.None)
            {
                EventPresent present = GetPresentReturned(dlg);
                CurrentEvent.Rewards.SetRewardPresent(0, present);
                btn_PresentOne.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[0]);
            }
            btn_RemovePresentOne.IsEnabled = CurrentEvent.Rewards.RewardPresents[0] != null;
        }

        public void btn_PresentTwo_Clicked(object sender, RoutedEventArgs e)
        {
            var dlg = new PresentPickerWindow(GameDatabase);
            dlg.ShowDialog();

            if (dlg.SelectedType != PresentPickerWindow.SelectionType.None)
            {
                EventPresent present = GetPresentReturned(dlg);
                CurrentEvent.Rewards.SetRewardPresent(1, present);
                btn_PresentTwo.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[1]);
            }
            btn_RemovePresentTwo.IsEnabled = CurrentEvent.Rewards.RewardPresents[1] != null;
        }

        public void btn_PresentThree_Clicked(object sender, RoutedEventArgs e)
        {
            var dlg = new PresentPickerWindow(GameDatabase);
            dlg.ShowDialog();

            if (dlg.SelectedType != PresentPickerWindow.SelectionType.None)
            {
                EventPresent present = GetPresentReturned(dlg);
                CurrentEvent.Rewards.SetRewardPresent(2, present);
                btn_PresentThree.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[2]);
            }
            btn_RemovePresentThree.IsEnabled = CurrentEvent.Rewards.RewardPresents[2] != null;
        }

        public void btn_PresentParticipationOne_Clicked(object sender, RoutedEventArgs e)
        {
            var dlg = new PresentPickerWindow(GameDatabase);
            dlg.ShowDialog();

            if (dlg.SelectedType != PresentPickerWindow.SelectionType.None)
            {
                EventPresent present = GetPresentReturned(dlg);
                CurrentEvent.Rewards.SetParticipatePresent(0, present);
                btn_PresentParticipationOne.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[0]);
            }
            btn_RemovePresentParticipationOne.IsEnabled = CurrentEvent.Rewards.ParticipatePresents[0] != null;
        }

        public void btn_PresentParticipationTwo_Clicked(object sender, RoutedEventArgs e)
        {
            var dlg = new PresentPickerWindow(GameDatabase);
            dlg.ShowDialog();

            if (dlg.SelectedType != PresentPickerWindow.SelectionType.None)
            {
                EventPresent present = GetPresentReturned(dlg);
                CurrentEvent.Rewards.SetParticipatePresent(1, present);
                btn_RemovePresentParticipationTwo.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[1]);
            }
            btn_RemovePresentParticipationTwo.IsEnabled = CurrentEvent.Rewards.ParticipatePresents[1] != null;
        }

        public void btn_PresentParticipationThree_Clicked(object sender, RoutedEventArgs e)
        {
            var dlg = new PresentPickerWindow(GameDatabase);
            dlg.ShowDialog();

            if (dlg.SelectedType != PresentPickerWindow.SelectionType.None)
            {
                EventPresent present = GetPresentReturned(dlg);
                CurrentEvent.Rewards.SetParticipatePresent(2, present);
                btn_PresentParticipationThree.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[2]);
            }
            btn_RemovePresentParticipationThree.IsEnabled = CurrentEvent.Rewards.ParticipatePresents[2] != null;
        }

        public void btn_RemovePresentOne_Clicked(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Rewards.RewardPresents[0] = null;
            btn_RemovePresentOne.IsEnabled = false;
            btn_PresentOne.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[0]);
        }

        public void btn_RemovePresentTwo_Clicked(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Rewards.RewardPresents[1] = null;
            btn_RemovePresentTwo.IsEnabled = false;
            btn_PresentTwo.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[1]);
        }

        public void btn_RemovePresentThree_Clicked(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Rewards.RewardPresents[2] = null;
            btn_RemovePresentThree.IsEnabled = false;
            btn_PresentThree.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[2]);
        }

        public void btn_RemovePresentParticipationOne_Clicked(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Rewards.ParticipatePresents[0] = null;
            btn_RemovePresentParticipationOne.IsEnabled = false;
            btn_PresentParticipationOne.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[0]);
        }

        public void btn_RemovePresentParticipationTwo_Clicked(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Rewards.ParticipatePresents[1] = null;
            btn_RemovePresentParticipationTwo.IsEnabled = false;
            btn_PresentParticipationTwo.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[1]);
        }

        public void btn_RemovePresentParticipationThree_Clicked(object sender, RoutedEventArgs e)
        {
            CurrentEvent.Rewards.ParticipatePresents[2] = null;
            btn_RemovePresentParticipationThree.IsEnabled = false;
            btn_PresentParticipationThree.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[2]);
        }

        public void PopulateOneTimeRewardControls()
        {
            if (cb_RewardPlacementType.Items.Count == 0)
            {
                var types = (PresentOrderType[])Enum.GetValues(typeof(PresentOrderType));
                for (int i = 0; i < types.Length; i++)
                {
                    var t = (PresentOrderType)i;
                    string tName = t.Humanize();
                    cb_RewardPlacementType.Items.Add(tName);
                }

                var pTypes = (ParticipationPresentType[])Enum.GetValues(typeof(ParticipationPresentType));
                for (int i = 0; i < pTypes.Length; i++)
                {
                    var t = (ParticipationPresentType)i;
                    string tName = t.Humanize();
                    cb_RewardParticipationType.Items.Add(tName);
                }
            }
        }

        public void PopulateRewards()
        {
            PopulateOneTimeRewardControls();

            btn_PresentOne.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[0]);
            btn_PresentTwo.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[1]);
            btn_PresentThree.Content = PresentToString(CurrentEvent.Rewards.RewardPresents[2]);

            btn_PresentParticipationOne.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[0]);
            btn_PresentParticipationTwo.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[1]);
            btn_PresentParticipationThree.Content = PresentToString(CurrentEvent.Rewards.ParticipatePresents[2]);

            btn_RemovePresentOne.IsEnabled = CurrentEvent.Rewards.RewardPresents[0] != null;
            btn_RemovePresentTwo.IsEnabled = CurrentEvent.Rewards.RewardPresents[1] != null;
            btn_RemovePresentThree.IsEnabled = CurrentEvent.Rewards.RewardPresents[2] != null;

            btn_RemovePresentParticipationOne.IsEnabled = CurrentEvent.Rewards.ParticipatePresents[0] != null;
            btn_RemovePresentParticipationTwo.IsEnabled = CurrentEvent.Rewards.ParticipatePresents[1] != null;
            btn_RemovePresentParticipationThree.IsEnabled = CurrentEvent.Rewards.ParticipatePresents[2] != null;

            cb_RewardPlacementType.SelectedIndex = (int)CurrentEvent.Rewards.PresentType;
            cb_RewardParticipationType.SelectedIndex = (int)CurrentEvent.Rewards.ParticipationPresentType;

            CurrentEvent.Rewards.NeedsPopulating = false;
        }

        public string PresentToString(EventPresent present)
        {
            if (present is null)
                return "No Present Selected";

            if (present.PresentType == PresentType.CAR)
                return $"Car: {GameDatabase.GetCarNameByLabel(present.CarLabel)}";
            else if (present.PresentType == PresentType.PAINT)
                return $"Paint: {GameDatabase.GetPaintNameByID(present.PaintID)}";
            else
                return "No Present Selected";
        }

        private EventPresent GetPresentReturned(PresentPickerWindow window)
        {
            EventPresent present;
            switch (window.SelectedType)
            {
                case PresentPickerWindow.SelectionType.Car:
                    present = EventPresent.FromCar(window.CarLabelSelected);
                    break;
                case PresentPickerWindow.SelectionType.Paint:
                    present = EventPresent.FromPaint(window.PaintIDSelected);
                    break;
                /*
                case PresentPickerWindow.SelectionType.Paint:
                    present = EventPresent.FromCar(dlg.PaintIDSelected);
                    break;
                */
                default:
                    present = null; 
                    break;
            }

            return present;
        }
    }
}
