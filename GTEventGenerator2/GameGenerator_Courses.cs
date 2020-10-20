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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

using GTEventGenerator.Entities;
using GTEventGenerator.Utils;

using Humanizer;

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        private void comboBox_CourseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentEvent != null)
            {
                var label = GameDatabase.GetCourseLabelByIndex(comboBox_CourseList.SelectedIndex + 1);
                CurrentEvent.Course.CourseLabel = label;

                if (label.Equals("coursemaker"))
                {
                    Button_SelectCustomTrack.IsEnabled = true;
                }
                else
                {
                    Button_SelectCustomTrack.IsEnabled = false;
                    CurrentEvent.Course.CustomCourse = null;
                    PrePopulateCourses();
                }

                UpdateCourseLogos();
            }
        }

        public void PrePopulateCourses()
        {
            PopulateOneTimeCourseControls();
            comboBox_CourseList.SelectedIndex = GameDatabase.GetCourseIndexByLabel(CurrentEvent.Course.CourseLabel) - 1;

            Button_SelectCustomTrack.IsEnabled = CurrentEvent.Course.CourseLabel.Equals("coursemaker") && CurrentEvent.Course.CustomCourse != null;
            if (CurrentEvent.Course.CourseLabel.Equals("coursemaker") && CurrentEvent.Course.CustomCourse != null)
            {
                label_CustomScenery.Content = CurrentEvent.Course.CustomCourse.Scenery.Humanize();
                label_CustomRoadWidth.Content = CurrentEvent.Course.CustomCourse.RoadWidth;
                label_CustomCorners.Content = CurrentEvent.Course.CustomCourse.CornerCount;
                label_CustomBeginPosition.Content = CurrentEvent.Course.CustomCourse.StartLine;
                label_CustomEndPosition.Content = CurrentEvent.Course.CustomCourse.FinishLine;
                label_ElevationDifference.Content = CurrentEvent.Course.CustomCourse.ElevationDifference;
                label_CustomScenery.Content = CurrentEvent.Course.CustomCourse.Scenery.Humanize();
                label_Created.Content = CurrentEvent.Course.CustomCourse.Time.ToString();
                label_HomeStraightLength.Content = CurrentEvent.Course.CustomCourse.HomeStraightLength;
                label_CustomIsCircuit.Content = CurrentEvent.Course.CustomCourse.IsCircuit.ToString();

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
            }


            UpdateCourseLogos();
            CurrentEvent.Course.NeedsPopulating = false;
        }

        public void UpdateCourseLogos()
        {
            if (comboBox_CourseList.SelectedIndex != -1)
            {
                if (CurrentEvent.Course.CourseLabel == "coursemaker" || CurrentEvent.Course.CourseLabel.StartsWith("rail") || CurrentEvent.Course.CourseLabel.StartsWith("scenery"))
                {
                    SetNoLogo();
                    SetNoMiniMap();
                    return;
                }

                var logoName = GameDatabase.GetCourseLogoByLabel(CurrentEvent.Course.CourseLabel);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseLogos", logoName + ".png");
                if (File.Exists(path))
                {
                    image_CourseLogo.Source = new BitmapImage(new Uri(path));
                    image_CourseLogoNoPreview.Visibility = Visibility.Hidden;
                }
                else
                    SetNoLogo();

                var mapName = GameDatabase.GetCourseMapByLabel(CurrentEvent.Course.CourseLabel);
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
            if (CurrentEvent.Course.CustomCourse != null)
            {
                string path;
                switch (CurrentEvent.Course.CustomCourse.Scenery)
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
            }
            else
                image_CustomCourseLogo.Source = null;
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

        public void PopulateOneTimeCourseControls()
        {
            if (comboBox_CourseList.Items.Count == 0)
            {
                foreach (var courseName in GameDatabase.GetAllCourseNamesSorted())
                    comboBox_CourseList.Items.Add(courseName);
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


                CurrentEvent.Course.CustomCourse = customCourse;
                PrePopulateCourses();

            }
        }

    }
}