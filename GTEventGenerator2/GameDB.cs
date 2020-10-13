using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GTEventGenerator
{
    public class GameDB
    {
        private SQLiteConnection _sConn;

        private string _conString;
        public GameDB(string fileName)
        {
            _conString = $"Data Source={fileName};Version=3;New=False;Compress=True;";
        }

        public bool CreateConnection()
        {
            _sConn = new SQLiteConnection(_conString);

            try
            {
                _sConn.Open();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public string GetCarNameByLabel(string label)
        {
            var res = ExecuteQuery($"SELECT VehicleName FROM Vehicles WHERE VehicleInternalName = \"{label}\"");
            res.Read();

            return res.GetString(0);
        }

        public string GetManufacturerLabelByName(string name)
        {
            var res = ExecuteQuery($"SELECT ManufacturerInternalName FROM Manufacturers WHERE ManufacturerName = \"{name}\"");
            res.Read();

            return res.GetString(0);
        }

        public List<string> GetLocalizedLanguagesSorted()
        {
            var res = ExecuteQuery("SELECT LocaliseLanguage, SortOrder FROM LocaliseLanguages ORDER BY SortOrder");

            List<string> list = new List<string>();
            while (res.Read())
                list.Add(res.GetString(0));
            return list;
        }

        public List<string> GetAllManufacturersSorted()
        {
            var res = ExecuteQuery("SELECT ManufacturerName FROM Manufacturers ORDER BY ManufacturerName");

            List<string> list = new List<string>();
            while (res.Read())
                list.Add(res.GetString(0));
            return list;
        }

        public List<string> GetAllCourseNamesSorted()
        {
            var res = ExecuteQuery("SELECT * FROM Courses ORDER BY CourseName");

            List<string> list = new List<string>();
            while (res.Read())
                list.Add(res.GetString(0));
            return list;
        }

        public List<(string CategoryName, int CategoryType)> GetFolderCategoriesSorted()
        {
            var res = ExecuteQuery("SELECT Name, Type FROM EventCategories ORDER BY Type, Name");

            var list = new List<(string, int)>();
            while (res.Read())
                list.Add((res.GetString(0), res.GetInt32(1)));
            return list;
        }

        public string GetCourseLabelByIndex(int index)
        {
            var res = ExecuteQuery($"SELECT CourseInternalName FROM Courses WHERE CourseIndex = {index}");
            res.Read();

            return res.GetString(0);
        }

        public int GetCourseIndexByLabel(string label)
        {
            var res = ExecuteQuery($"SELECT CourseIndex FROM Courses WHERE CourseInternalName = '{label}'");
            res.Read();

            return res.GetInt32(0);
        }

        public string GetCarLabelByActualName(string name)
        {
            var res = ExecuteQuery($"SELECT VehicleInternalName FROM Vehicles WHERE VehicleName = '{name}'");
            res.Read();

            return res.GetString(0);
        }

        public (string DriverName, int InitialType) GetRandomDriverInfo()
        {
            var res = ExecuteQuery("SELECT * FROM DriverNames ORDER BY RANDOM() LIMIT 1");
            res.Read();

            return (res.GetString(0), res.GetInt32(1));
        }

        public SQLiteDataReader ExecuteQuery(string query)
        {
            SQLiteCommand cmd = _sConn.CreateCommand();
            cmd.CommandText = query;

            return cmd.ExecuteReader();
        }
    }
}
