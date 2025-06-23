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
using System.Windows.Navigation;
using System.IO;

using Microsoft.Win32;

using Humanizer;

using PDTools.Structures.MGameParameter;

using GTEventMaker.Database;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for TrackView.xaml
/// </summary>
public partial class TrackView : UserControl
{
    public Track Track => DataContext as Track;
    public GameDB GameDatabase { get; set; }
    public CustomCourse CustomCourse { get; set; }

    public TrackView()
    {
        GameDatabase = App.GameDatabase;
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Populate();
    }

    private void cb_IsOmodetoDifficulty_Checked(object sender, RoutedEventArgs e)
    {
        Track.IsOmodetoDifficulty = cb_IsOmodetoDifficulty.IsChecked.Value;
    }

    private void cb_IsOmodetoDifficulty_Unchecked(object sender, RoutedEventArgs e)
    {
        Track.IsOmodetoDifficulty = cb_IsOmodetoDifficulty.IsChecked.Value;
    }

    private void iud_CourseLayoutNumber_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Track.CourseLayoutNumber = iud_CourseLayoutNumber.Value.Value;
    }

    private void iud_MapOffsetWorldX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Track.MapOffsetWorldX = iud_MapOffsetWorldX.Value.Value;
    }

    private void iud_MapOffsetWorldY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Track.MapOffsetWorldY = iud_MapOffsetWorldY.Value.Value;
    }

    private void iud_MapScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Track.MapScale = iud_MapScale.Value.Value;
    }

    private void iud_GeneratedCourseID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Track.GeneratedCourseID = iud_GeneratedCourseID.Value.Value;
    }

    private void comboBox_CourseList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Track != null)
        {
            var label = GameDatabase.GetCourseLabelByIndex(comboBox_CourseList.SelectedIndex + 1);
            Track.CourseLabel = label;

            if (label.Equals("coursemaker"))
            {
                gb_GT6CustomTrack.IsEnabled = true;
            }
            else
            {
                gb_GT6CustomTrack.IsEnabled = false;
                CustomCourse = null;
                PopulateCourses();
            }

            UpdateCourseLogos();
        }
    }

    private void Button_SelectCustomTrack_Clicked(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog();
        open.ShowDialog();

        if (!string.IsNullOrEmpty(open.FileName))
        {
            var ext = Path.GetExtension(open.FileName);

            CustomCourse customCourse;
            if (ext.Equals(".b64"))
            {
                customCourse = CustomCourse.FromBase64File(open.FileName);
            }
            else if (ext.Equals(".ted"))
            {
                customCourse = CustomCourse.FromTED(open.FileName);
            }
            else
                return;


            CustomCourse = customCourse;
            PopulateCourses();
        }
    }

    public void Populate()
    {
        PopulateOneTimeCourseControls();
        PopulateCourses();
    }

    public void PopulateOneTimeCourseControls()
    {
        if (Track is null)
            return;

        if (comboBox_CourseList.Items.Count == 0)
        {
            foreach (var courseName in GameDatabase.GetAllCourseNamesSorted())
                comboBox_CourseList.Items.Add(courseName);
        }
    }

    public void PopulateCourses()
    {
        if (Track is null)
            return;

        comboBox_CourseList.SelectedIndex = GameDatabase.GetCourseIndexByLabel(Track.CourseLabel) - 1;

        iud_CourseLayoutNumber.Value = Track.CourseLayoutNumber;
        iud_MapOffsetWorldX.Value = Track.MapOffsetWorldX;
        iud_MapOffsetWorldY.Value = Track.MapOffsetWorldY;
        iud_MapScale.Value = Track.MapScale;
        iud_GeneratedCourseID.Value = Track.GeneratedCourseID;
        cb_IsOmodetoDifficulty.IsChecked = Track.IsOmodetoDifficulty;

        gb_GT6CustomTrack.IsEnabled = Track.CourseLabel.Equals("coursemaker");
        if (Track.CourseLabel.Equals("coursemaker") && CustomCourse != null)
        {
            label_CustomScenery.Content = CustomCourse.Scenery.Humanize();
            label_CustomRoadWidth.Content = CustomCourse.RoadWidth;
            label_CustomCorners.Content = CustomCourse.CornerCount;
            label_CustomBeginPosition.Content = CustomCourse.StartLine;
            label_CustomEndPosition.Content = CustomCourse.FinishLine;
            label_ElevationDifference.Content = CustomCourse.ElevationDifference;
            label_CustomScenery.Content = CustomCourse.Scenery.Humanize();
            label_Created.Content = CustomCourse.Time.ToString();
            label_HomeStraightLength.Content = CustomCourse.HomeStraightLength;
            label_CustomIsCircuit.Content = CustomCourse.IsCircuit.ToString();

            UpdateCustomCourseLogo();
        }
        else
        {
            label_CustomScenery.Content = "N/A";
            label_CustomRoadWidth.Content = "N/A";
            label_CustomCorners.Content = "N/A";
            label_CustomBeginPosition.Content = "N/A";
            label_CustomEndPosition.Content = "N/A";
            label_ElevationDifference.Content = "N/A";
            label_CustomScenery.Content = "N/A";
            label_Created.Content = "N/A";
            label_HomeStraightLength.Content = "N/A";
            label_CustomIsCircuit.Content = "N/A";
            image_CustomCourseLogo.Source = null;
            lb_SelectCourseMakerTrack.Visibility = Visibility.Visible;
        }


        UpdateCourseLogos();
    }

    public void UpdateCourseLogos()
    {
        if (comboBox_CourseList.SelectedIndex != -1)
        {
            if (Track.CourseLabel == "coursemaker" || Track.CourseLabel.StartsWith("rail") || Track.CourseLabel.StartsWith("scenery"))
            {
                SetNoLogo();
                SetNoMiniMap();
                return;
            }

            var logoName = GameDatabase.GetCourseLogoByLabel(Track.CourseLabel);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseLogos", logoName + ".png");
            if (File.Exists(path))
            {
                image_CourseLogo.Source = new BitmapImage(new Uri(path));
                image_CourseLogoNoPreview.Visibility = Visibility.Hidden;
            }
            else
                SetNoLogo();

            var mapName = GameDatabase.GetCourseMapByLabel(Track.CourseLabel);
            var mappath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseMaps", mapName + ".png");
            if (File.Exists(mappath))
            {
                image_CourseMap.Source = new BitmapImage(new Uri(mappath));
                image_CourseMapNoPreview.Visibility = Visibility.Hidden;
            }
            else
                SetNoMiniMap();
        }
    }

    public void UpdateCustomCourseLogo()
    {
        if (CustomCourse != null)
        {
            string path;
            switch (CustomCourse.Scenery)
            {
                case SceneryType.Andalusia:
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseLogos", "t_scenery_andalusia.png"); break;
                case SceneryType.Eifel:
                case SceneryType.Eifel_Flat:
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseLogos", "t_scenery_eifel.png"); break;
                case SceneryType.Death_Valley:
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseLogos", "t_scenery_deathvalley.png"); break;
                default:
                    path = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(path))
                image_CustomCourseLogo.Source = new BitmapImage(new Uri(path));
            else
                image_CustomCourseLogo.Source = null;

            lb_SelectCourseMakerTrack.Visibility = Visibility.Hidden;
        }
        else
        {
            image_CustomCourseLogo.Source = null;
            lb_SelectCourseMakerTrack.Visibility = Visibility.Visible;
        }
    }

    public void SetNoLogo()
    {
        image_CourseLogo.Source = null;
        image_CourseLogoNoPreview.Visibility = Visibility.Visible;
    }

    public void SetNoMiniMap()
    {
        image_CourseMap.Source = null;
        image_CourseMapNoPreview.Visibility = Visibility.Visible;
    }
}
