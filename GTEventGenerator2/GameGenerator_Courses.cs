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

namespace GTEventGenerator
{
    public partial class GameGeneratorWindow
    {
        private void comboBox_CourseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentEvent != null)
                CurrentEvent.Course.CourseLabel = GameDatabase.GetCourseLabelByIndex(comboBox_CourseList.SelectedIndex + 1);
        }

        public void PrePopulateCourses()
        {
            PopulateOneTimeCourseControls();
            comboBox_CourseList.SelectedIndex = GameDatabase.GetCourseIndexByLabel(CurrentEvent.Course.CourseLabel) - 1;
        }

        public void PopulateOneTimeCourseControls()
        {
            if (comboBox_CourseList.Items.Count == 0)
            {
                foreach (var courseName in GameDatabase.GetAllCourseNamesSorted())
                    comboBox_CourseList.Items.Add(courseName);
            }
        }

    }
}