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

using System.Windows.Controls;
using GTEventGenerator.Entities;
using GTEventGenerator.Utils;

using Humanizer;
namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        private void cb_EvalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentEvent != null)
                CurrentEvent.EvalConditions.ConditionType = (EvalConditionType)cb_EvalType.SelectedIndex;

            bool isNone = CurrentEvent.EvalConditions.ConditionType == EvalConditionType.NONE;
            iud_EvalGold.IsEnabled = !isNone;
            iud_EvalSilver.IsEnabled = !isNone;
            iud_EvalBronze.IsEnabled = !isNone;

            if (isNone)
            {
                CurrentEvent.EvalConditions.Bronze = 0;
                CurrentEvent.EvalConditions.Silver = 0;
                CurrentEvent.EvalConditions.Gold = 0;
            }
        }

        private void OffTrack_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.COURSE_OUT;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.COURSE_OUT;
        }

        private void CarCollision_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.HIT_CAR;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.HIT_CAR;
        }

        private void HardCarCollision_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.HIT_CAR_HARD;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.HIT_CAR_HARD;
        }

        private void Barrier_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.HIT_WALL;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.HIT_WALL;
        }

        private void HardBarrierCollision_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.PYLON;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.PYLON;
        }

        private void ObstacleCollision_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.PYLON;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.PYLON;
        }

        private void WrongWay_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.WRONGWAY;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.WRONGWAY;
        }

        private void InstantWrongWay_Checked(object sender, EventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
                CurrentEvent.FailConditions.FailConditions |= FailCondition.WRONGWAY_LOOSE;
            else
                CurrentEvent.FailConditions.FailConditions &= ~FailCondition.WRONGWAY_LOOSE;
        }

        public void PrePopulateEvalConditions()
        {
            PopulateOneTimeEvalConditionControls();
            cb_EvalType.SelectedIndex = (int)CurrentEvent.EvalConditions.ConditionType;

            chk_OffTrack.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.COURSE_OUT);
            chk_CarCollision.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.HIT_CAR);
            chk_HardCarCollision.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.HIT_CAR_HARD);
            chk_BarrierCollision.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.HIT_WALL);
            chk_HardBarrierCollision.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.HIT_WALL_HARD);
            chk_ObstacleCollision.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.PYLON);
            chk_WrongWay.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.WRONGWAY);
            chk_InstantWrongWay.IsChecked = CurrentEvent.FailConditions.FailConditions.HasFlag(FailCondition.WRONGWAY_LOOSE);

        }

        public void PopulateOneTimeEvalConditionControls()
        {
            if (cb_EvalType.Items.Count == 0)
            {
                var types = (EvalConditionType[])Enum.GetValues(typeof(EvalConditionType));
                for (int i = 0; i < types.Length; i++)
                {
                    var t = (EvalConditionType)i;
                    string tName = t.Humanize();
                    cb_EvalType.Items.Add(tName);
                }
            }
        }

    }
}