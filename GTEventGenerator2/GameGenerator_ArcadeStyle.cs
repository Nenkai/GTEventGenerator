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

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        public void PopulateArcadeStyle()
        {
            lb_ArcadeSections.ItemsSource = CurrentEvent.ArcadeStyleSettings.Sections;

            iud_ArcadeSectionPos.Value = (ushort)CurrentEvent.ArcadeStyleSettings.Sections[0].CourseV;
            iud_ArcadeSectionExtraSec.Value = CurrentEvent.ArcadeStyleSettings.Sections[0].SectionExtendSeconds;
        }

        public void lb_ArcadeSections_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (lb_ArcadeSections.SelectedIndex == -1)
                return;

            iud_ArcadeSectionPos.Value = (ushort)CurrentEvent.ArcadeStyleSettings.Sections[lb_ArcadeSections.SelectedIndex].CourseV;
            iud_ArcadeSectionExtraSec.Value = CurrentEvent.ArcadeStyleSettings.Sections[lb_ArcadeSections.SelectedIndex].SectionExtendSeconds;
        }

        public void iud_ArcadeSectionPos_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (lb_ArcadeSections.SelectedIndex == -1)
                return;

            CurrentEvent.ArcadeStyleSettings.Sections[lb_ArcadeSections.SelectedIndex].CourseV = (ushort)iud_ArcadeSectionPos.Value;
            lb_ArcadeSections.Items.Refresh();
        }

        private void iud_ArcadeSectionExtraSec_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (lb_ArcadeSections.SelectedIndex == -1)
                return;

            CurrentEvent.ArcadeStyleSettings.Sections[lb_ArcadeSections.SelectedIndex].SectionExtendSeconds = iud_ArcadeSectionExtraSec.Value.Value;
            lb_ArcadeSections.Items.Refresh();
        }
    }
}
