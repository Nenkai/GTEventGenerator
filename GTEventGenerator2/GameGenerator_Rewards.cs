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
    }
}
