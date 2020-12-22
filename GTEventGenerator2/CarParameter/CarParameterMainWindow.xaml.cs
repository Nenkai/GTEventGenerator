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
using System.IO;
using Microsoft.Win32;

using GTEventGenerator.PDUtils;
using GTEventGenerator.Database;

namespace GTEventGenerator.CarParameter
{
    /// <summary>
    /// Interaction logic for CarParameterMainWindow.xaml
    /// </summary>
    public partial class CarParameterMainWindow : Window
    {
        public GameDB GameDb { get; set; }
        public MCarParameter Parameter { get; set; }
        public CarParameterMainWindow(GameDB gameDB)
        {
            GameDb = gameDB;
            InitializeComponent();
        }

        public void btn_PurchasedParts_Click(object sender, RoutedEventArgs e)
        {
            var purchaseWindow = new PurchasedPartsWindow(Parameter);
            purchaseWindow.ShowDialog();
        }

        public void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Title = "Pick from MCarParameter blob";
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    Parameter = MCarParameter.ImportFromBlob(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", $"Could not parse car parameter: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                btn_PurchasedParts.IsEnabled = true;
                lbl_CarName.Content = Parameter.Settings.CarCode.ToString();

            }
        }
    }
}
