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

using GTEventGenerator.Database;
namespace GTEventGenerator
{
    /// <summary>
    /// Interaction logic for PresentPickerWindow.xaml
    /// </summary>
    public partial class PresentPickerWindow : Window
    {
        private GameDB _gameDB;
        private List<PaintInfo> PaintList { get; set; } = new List<PaintInfo>();

        public SelectionType SelectedType { get; set; }
        public string CarLabelSelected { get; set; }
        public int PaintIDSelected { get; set; }

        public PresentPickerWindow(GameDB gameDB)
        {
            _gameDB = gameDB;
            InitializeComponent();

            lv_PaintList.ItemsSource = PaintList;
            Populate();
        }

        public void cb_Manufacturers_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateCarList();
        }

        public void PrePopulateManufacturers()
        {
            foreach (var manufacturer in _gameDB.GetAllManufacturersSorted())
                cb_Manufacturers.Items.Add(manufacturer);
            cb_Manufacturers.SelectedIndex = 0;
        }

        public void Populate()
        {
            PrePopulateManufacturers();
            UpdateCarList();
        }

        private void btn_SelectCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_CarList.SelectedIndex == -1)
                return;

            CarLabelSelected = _gameDB.GetCarLabelByActualName((string)lv_CarList.SelectedItem);
            SelectedType = SelectionType.Car;
            Close();
        }

        private void btn_SelectPaint_Click(object sender, RoutedEventArgs e)
        {
            if (lv_PaintList.SelectedIndex == -1)
                return;

            PaintIDSelected = PaintList[lv_PaintList.SelectedIndex].ID;
            SelectedType = SelectionType.Paint;
            Close();
        }

        public void UpdateCarList()
        {
            lv_CarList.Items.Clear();

            var results = _gameDB.ExecuteQuery(
                "SELECT " +
                    "V.VehicleName " +
                "FROM Vehicles V " +
                "INNER JOIN Manufacturers M " +
                    "ON M.ManufacturerID = V.VehicleManufacturerID " +
                "WHERE " +
                    $"M.ManufacturerName = '{cb_Manufacturers.SelectedItem.ToString()}' " +
                "ORDER BY VehicleName ");

            while (results.Read())
                lv_CarList.Items.Add(results.GetString(0));
        }

        private void btn_PaintSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_PaintSearcher.Text) || tb_PaintSearcher.Text.Length < 3)
                return;

            PaintList.Clear();
            foreach (var paint in _gameDB.SearchPaintsByName(tb_PaintSearcher.Text))
            {
                var colorBytes = BitConverter.GetBytes(paint.color);

                var paintC = new PaintInfo(paint.id, Color.FromRgb(colorBytes[0], colorBytes[1], colorBytes[2]), paint.name);
                PaintList.Add(paintC);
            }

            lv_PaintList.ItemsSource = null;
            lv_PaintList.ItemsSource = PaintList;
        }


        public enum SelectionType
        {
            None,
            Car,
            Paint,
            Suit
        }

        public class PaintInfo
        {
            public int ID { get; set; }
            public SolidColorBrush Color { get; set; }
            public string PaintName { get; set; }

            public PaintInfo(int id, Color color, string paintName)
            {
                ID = id;
                Color = new SolidColorBrush(color);
                PaintName = paintName;
            }
        }
    }
}
