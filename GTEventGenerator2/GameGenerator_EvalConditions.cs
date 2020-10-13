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

        public void PrePopulateEvalConditions()
        {
            PopulateOneTimeEvalConditionControls();
            cb_EvalType.SelectedIndex = (int)CurrentEvent.EvalConditions.ConditionType;
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