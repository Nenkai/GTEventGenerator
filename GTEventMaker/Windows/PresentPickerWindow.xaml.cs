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

using Humanizer;

using PDTools.Enums.PS3;
using PDTools.Structures.PS3;
using PDTools.Structures.MGameParameter;

using GTEventMaker.Database;

namespace GTEventMaker
{
    /// <summary>
    /// Interaction logic for PresentPickerWindow.xaml
    /// </summary>
    public partial class PresentPickerWindow : Window
    {
        private GameDB _gameDB;
        private List<string> CarList { get; set; } = new List<string>();
        private List<PaintInfo> PaintList { get; set; } = new List<PaintInfo>();

        public EventPresent _present;

        public PresentPickerWindow(GameDB gameDB, EventPresent present)
        {
            _gameDB = gameDB;
            _present = present;

            InitializeComponent();

            lv_PaintList.ItemsSource = PaintList;
            lv_CarList.ItemsSource = CarList;
            Populate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void cb_Manufacturers_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateCarList();
        }

        private void btn_SelectCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_CarList.SelectedIndex == -1)
                return;

            _present.TypeID = GameItemType.SPECIAL;
            _present.CategoryID = GameItemCategory.PRESENTCAR_TICKET;
            _present.Argument1 = -1;
            _present.Argument2 = 0;
            _present.Argument3 = 0;
            _present.Argument4 = 0;
            _present.FName = _gameDB.GetCarLabelByActualName((string)lv_CarList.SelectedItem);

            Populate();
        }

        private void btn_SelectPresentCarParameter_Click(object sender, RoutedEventArgs e)
        {
            if (lv_CarList.SelectedIndex == -1)
                return;

            var tunedEntry = new EntryBase();
            var window = new EventEntryBaseEditWindow(tunedEntry);
            window.Owner = this;
            window.ShowDialog();

            /*
            CarLabelSelected = _gameDB.GetCarLabelByActualName((string)lv_CarList.SelectedItem);
            SelectedType = SelectionType.CarWithParts;
            TunedEntrySelected = tunedEntry;
            TunedEntrySelected.Car.CarLabel = CarLabelSelected;
            Close();
            */
        }

        private void btn_EditPresentCarParameter_Click(object sender, RoutedEventArgs e)
        {
            /*
            var window = new EventEntryBaseEditWindow();
            window.Owner = this;
            window.ShowDialog();
            */
        }

        private void btn_SelectPaint_Click(object sender, RoutedEventArgs e)
        {
            if (lv_PaintList.SelectedIndex == -1)
                return;

            _present.TypeID = GameItemType.DRIVER_ITEM;
            _present.CategoryID = GameItemCategory.PAINT_ITEM;
            _present.Argument1 = ((PaintInfo)lv_PaintList.SelectedItem).ID;
            _present.Argument2 = 0;
            _present.Argument3 = 0;
            _present.Argument4 = 0;
            _present.FName = string.Empty;

            Populate();
        }


        private void iud_Arg1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _present.Argument1 = iud_Arg1.Value.Value;
        }

        private void iud_Arg2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _present.Argument2 = iud_Arg1.Value.Value;
        }

        private void iud_Arg3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _present.Argument3 = iud_Arg1.Value.Value;
        }

        private void iud_Arg4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _present.Argument4 = iud_Arg1.Value.Value;
        }

        private void tb_FName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _present.FName = tb_FName.Text;
        }

        public void UpdateCarList()
        {
            CarList.Clear();

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
                CarList.Add(results.GetString(0));

            lv_CarList.ItemsSource = null;
            lv_CarList.ItemsSource = CarList;
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

        public void PopulateOneTimeControls()
        {
            if (cb_TypeID.Items.Count == 0)
            {
                var typeIds = (GameItemType[])Enum.GetValues(typeof(GameItemType));
                for (int i = 0; i < typeIds.Length; i++)
                    cb_TypeID.Items.Add(typeIds[i]);

                var categories = (GameItemCategory[])Enum.GetValues(typeof(GameItemCategory));
                for (int i = 0; i < categories.Length; i++)
                    cb_Category.Items.Add(categories[i].Humanize());

                foreach (var manufacturer in _gameDB.GetAllManufacturersSorted())
                    cb_Manufacturers.Items.Add(manufacturer);
                cb_Manufacturers.SelectedIndex = 0;
            }
        }

        public void Populate()
        {
            PopulateOneTimeControls();
            UpdateCarList();

            iud_Arg1.Value = _present.Argument1;
            iud_Arg2.Value = _present.Argument2;
            iud_Arg3.Value = _present.Argument3;
            iud_Arg4.Value = _present.Argument4;
            cb_TypeID.SelectedIndex = cb_TypeID.Items.IndexOf(_present.TypeID);
            cb_Category.SelectedIndex = cb_Category.Items.IndexOf(_present.CategoryID.Humanize());
            tb_FName.Text = _present.FName;
        }
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
