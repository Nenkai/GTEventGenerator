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

using Microsoft.Win32;
using GTEventGenerator.Entities;
using Humanizer;

namespace GTEventGenerator
{
    /// <summary>
    /// Interaction logic for NewWeatherDataSettingsWindow.xaml
    /// </summary>
    public partial class NewWeatherDataSettingsWindow : Window
    {
        public WeatherData CurrentPoint { get; set; }
        public List<WeatherData> Points { get; set; }
        public TimeSpan TimeProgressionLength { get; set; }

        public NewWeatherDataSettingsWindow(List<WeatherData> data, TimeSpan timeProgressionLength)
        {
            InitializeComponent();
            this.DataContext = CurrentPoint;
            Points = data;
            lb_WeatherParamList.ItemsSource = Points;
            CurrentPoint = data.FirstOrDefault();
            TimeProgressionLength = timeProgressionLength;
            lbl_TotalTimeProgression.Content = "Total Weather Progression is set to last for " +
                $"{timeProgressionLength.Humanize(3, maxUnit: Humanizer.Localisation.TimeUnit.Hour, minUnit: Humanizer.Localisation.TimeUnit.Second)}";
        }

        private void btn_AddNew_Click(object sender, RoutedEventArgs e)
        {
            var point = new WeatherData();
            if (Points.Any())
            {
                point.TimeRate = Points.Last().TimeRate + 10;
                if (point.TimeRate > 100)
                    point.TimeRate = 100;
            }
            Points.Add(point);

            lb_WeatherParamList.Items.Refresh();
            if (Points.Count == 1)
                CurrentPoint = Points[0];
            SetPointDescription();
        }

        private void lb_WeatherParamList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_WeatherParamList.SelectedIndex == -1)
                return;

            CurrentPoint = Points[lb_WeatherParamList.SelectedIndex];
            this.DataContext = CurrentPoint;
            gb_Params.IsEnabled = true;
            SetPointDescription();
        }

        private bool _process = true;
        private void pointTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_process)
                return;

            SetPointDescription();
            lb_WeatherParamList.Items.Refresh();
        }

        private void low_ValueChanged(object sender, RoutedEventArgs e)
        {
            lb_WeatherParamList.Items.Refresh();
        }

        private void high_ValueChanged(object sender, RoutedEventArgs e)
        {
            lb_WeatherParamList.Items.Refresh();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Points.Count == 0 || lb_WeatherParamList.SelectedIndex == -1)
                return;

            Points.Remove(CurrentPoint);
            if (Points.Count > 0)
            {
                lb_WeatherParamList.SelectedIndex = Points.Count - 1;
            }
            else
            {
                gb_Params.IsEnabled = false;
                CurrentPoint = null;
            }

            lb_WeatherParamList.Items.Refresh();
        }

        public void SetPointDescription()
        {
            if (CurrentPoint.TimeRate == 0)
            {
                lbl_CurrentPointDesc.Text = "Weather at Event Start";
            }
            else if (CurrentPoint.TimeRate == 100)
            {
                lbl_CurrentPointDesc.Text = "Weather at the end of progression";
            }
            else
            {
                var time = new TimeSpan(TimeProgressionLength.Ticks / 100 * CurrentPoint.TimeRate);
                lbl_CurrentPointDesc.Text = $"Step to occur at {time.Humanize(3, maxUnit: Humanizer.Localisation.TimeUnit.Hour, minUnit: Humanizer.Localisation.TimeUnit.Second)}";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < Points.IndexOf(CurrentPoint); i++)
            {
                if (Points[i].TimeRate >= iud_TimeRate.Value)
                {
                    MessageBox.Show($"Step #{i+1} is One step is under or equal to one of the previous steps.", "Step Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
