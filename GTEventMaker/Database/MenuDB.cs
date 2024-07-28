using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PDTools.Structures.MGameParameter;

namespace GTEventMaker.Database
{
    public class MenuDB
    {
        private SQLiteConnection _sConn;

        private string _conString;
        public MenuDB(string fileName)
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
            catch
            {
                return false;
            }
            return true;
        }

        public void CloseConnection()
        {
            _sConn.Close();
        }

        public string GetFolderNameByID(int id)
        {
            var res = ExecuteQuery($"SELECT Name FROM t_event_folder WHERE FolderID = '{id}'");
            res.Read();

            return res.GetString(0);
        }

        public int GetLastFolderID()
        {
            var res = ExecuteQuery("SELECT MAX(FolderID) FROM t_event_folder");
            res.Read();

            return res.GetInt32(0);
        }

        public int GetLastFolderLocalizeID()
        {
            var res = ExecuteQuery("SELECT MAX(LocalizeID) FROM t_event_folder_localize");
            res.Read();

            return res.GetInt32(0);
        }

        public void AddNewFolderID(int totalStars, int starsNeeded, string folderName, int type, int firstSafeTitleID, int firstSafeFolderID, int firstSafeSortOrderInCategory)
        {
            ExecuteQuery("INSERT INTO t_event_folder (NeedFolderID, Star, NeedStar, Name, TitleID, FolderID, Type, FolderOrder) " +
                            $"VALUES(0,{totalStars},{starsNeeded},'{folderName}',{firstSafeTitleID},{firstSafeFolderID},{type},{firstSafeSortOrderInCategory})");
        }

        public void AddNewFolderLocalization(int firstSafeTitleID, string desc)
        {
            ExecuteQuery("INSERT INTO t_event_folder_localize (LocalizeID, EL, FR, DE, JP, IT, HU, CZ, BP, GB, ES, RU, NL, PT, TW, TR, US, KR, MS, PL) " +
                            string.Format("VALUES({0},'{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}','{1}')",
                            firstSafeTitleID, desc));
        }

        public List<string> GetLocalizedLanguagesSorted()
        {
            var res = ExecuteQuery("SELECT LocaliseLanguage, SortOrder FROM LocaliseLanguages ORDER BY SortOrder");

            List<string> list = new List<string>();
            while (res.Read())
                list.Add(res.GetString(0));
            return list;
        }

        public SQLiteDataReader ExecuteQuery(string query)
        {
            SQLiteCommand cmd = _sConn.CreateCommand();
            cmd.CommandText = query;

            return cmd.ExecuteReader();
        }
    }
}
