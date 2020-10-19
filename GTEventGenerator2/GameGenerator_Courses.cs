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
            }


            UpdateCourseLogos();
        }

        public void UpdateCourseLogos()
        {
            if (comboBox_CourseList.SelectedIndex != -1)
            {
                if (CurrentEvent.Course.CourseLabel == "coursemaker" || CurrentEvent.Course.CourseLabel.StartsWith("rail") || CurrentEvent.Course.CourseLabel.StartsWith("scenery"))
                {
                    image_CourseLogo.Source = null;
                    return;
                }

                var logoName = GameDatabase.GetCourseLogoByLabel(CurrentEvent.Course.CourseLabel);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CourseLogos", logoName + ".png");
                if (File.Exists(path))
                    image_CourseLogo.Source = new BitmapImage(new Uri(path));
                else
                    image_CourseLogo.Source = null;
            }
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