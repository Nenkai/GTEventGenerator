using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;

using Microsoft.Win32;
using Humanizer;

using GTEventGenerator.Entities;
using GTEventGenerator.Utils;


namespace GTEventGenerator
{
    public partial class GameGeneratorWindow : Window
    {
        private Random _random = new Random();
        private System.Drawing.Image _eventImage;
        private GameDB GameDatabase;
        private MenuDB MenuDB;
        private SQLiteDataReader results;
        public GameParameter GameParameter { get; set; }
        public Event CurrentEvent { get; set; }
        int lastEventFolderId = 1000, lastEventRaceId = 9900000, eventFolderId = 1000, eventRaceId = 9900000;
        bool menuDBValid = false, eventHasNoStars = false, validationErrors = false;
        string selectedPath = "";

        private bool _processEventSwitch = true;
        public List<string> EventNames { get; set; }
        public GameGeneratorWindow()
        {
            InitializeComponent();
            GameParameter = new GameParameter();
            this.DataContext = new Event();
            EventNames = new List<string>();
            lstRaces.ItemsSource = EventNames;
            cb_QuickEventPicker.ItemsSource = EventNames;
        }

        private void btnAddRace_Click(object sender, EventArgs e)
        {
            Event evnt = new Event();
            this.DataContext = evnt;

            // Assign default values
            evnt.Id = GameParameter.Events.Count + 1;
            evnt.Name = $"{GameParameter.EventList.Title}: Event {evnt.Id}";

            evnt.Rewards.Stars = 3;

            _processEventSwitch = false;

            EventNames.Add($"{evnt.Id} - {evnt.Name}");
            UpdateEventListing();

            GameParameter.Events.Add(evnt);

            SelectEvent(evnt.Id - 1);
            rdoStarsThree.IsChecked = true;

            ToggleEventControls(true);

            btnRemoveRace.IsEnabled = GameParameter.Events.Count <= 64;

            _processEventSwitch = true;
        }

        private void GameGenerator_Load(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "eventid.dat")))
            {
                // Check what the last eventid written was
                using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "eventid.dat")))
                {
                    if (int.TryParse(sr.ReadLine(), out eventRaceId))
                    {
                        // Increment after reading
                        eventRaceId++;
                    }
                }
            }

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "folderid.dat")))
            {
                // Check what the last eventid written was
                using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "folderid.dat")))
                {
                    if (int.TryParse(sr.ReadLine(), out eventRaceId))
                    {
                        // Increment after reading
                        eventFolderId++;
                    }
                }
            }

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "data.db");
            if (!File.Exists(dbPath))
            {
                MessageBox.Show("Required database file for the generator is missing (data/data.db), exiting.", "Database file missing");
                this.Close();
                return;
            }

            GameDatabase = new GameDB(Path.Combine(Directory.GetCurrentDirectory(), "Data", "data.db"));
            if (!GameDatabase.CreateConnection())
            {
                MessageBox.Show("Could not connect to local database (data.db).");
                Environment.Exit(0);
            }

            txtGameParamName.Text = GameParameter.EventList.Title;
            txtGameParamDesc.Text = GameParameter.EventList.Description;

            tabEvent.SelectionChanged += new SelectionChangedEventHandler(tabEvent_Selecting);

            var categories = GameDatabase.GetFolderCategoriesSorted();
            foreach (var category in categories)
            {
                GameParameterEventList.EventCategories.Add(new EventCategory(category.CategoryName, category.CategoryType));
                cboEventCategory.Items.Add(category.CategoryName);
            }

            cboEventCategory.SelectedIndex = 0;

            var langs = GameDatabase.GetLocalizedLanguagesSorted();

            foreach (var lang in langs)
                GameParameterEventList.LocaliseLanguages.Add(lang);

            foreach (var i in (GameMode[])Enum.GetValues(typeof(GameMode)))
                cb_gameModes.Items.Add(i.Humanize());
            cb_gameModes.SelectedIndex = 0;
        }

        private void tabEvent_Selecting(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                if (tabControl.SelectedIndex > 2 && CurrentEvent is null)
                {
                    MessageBox.Show("Create an Event to edit any event first.", "Event missing");
                    e.Handled = false;
                    return;
                }

                PopulateSelectedTab();
            }
        }

        private void lstEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRaces.SelectedIndex != -1 && _processEventSwitch)
                OnNewEventSelected(lstRaces.SelectedIndex);
        }

        private void cb_QuickEventPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_QuickEventPicker.SelectedIndex != -1 && _processEventSwitch)
                OnNewEventSelected(cb_QuickEventPicker.SelectedIndex);
        }

        private void txtEventName_TextChanged(object sender, EventArgs e)
        {
            if (CurrentEvent is null)
                return;

            CurrentEvent.Name = txtEventName.Text;
            CurrentEvent.Information.SetTitle(CurrentEvent.Name);

            int currentEventIndex = GameParameter.Events.IndexOf(CurrentEvent);
            EventNames[currentEventIndex] = $"{CurrentEvent.Id} - {CurrentEvent.Name}";

            UpdateEventListing();
        }

        private void btnRemoveRace_Click(object sender, EventArgs e)
        {
            if (lstRaces.SelectedIndex == -1)
            { 
                MessageBox.Show("No event selected.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult deletionResult = MessageBox.Show($"Are you sure you wish to delete the event \"{CurrentEvent.Name}\"?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deletionResult == MessageBoxResult.Yes)
            {
                //raceIdCounter = CurrentEvent.Id;
                GameParameter.Events.Remove(GameParameter.Events.Find(x => x.Id == CurrentEvent.Id));

                CurrentEvent = GameParameter.Events.Count > 0 ? GameParameter.Events.Last() : null;
                ReloadEventLists(GameParameter.Events.Count - 1);
                UpdateEventListing();

                if (lstRaces.Items.Count == 0)
                {
                    btnRemoveRace.IsEnabled = false;
                    ToggleEventControls(false);
                }
                else
                    PopulateEventDetails();
            }
        }

        private void btnOpenMenuDB_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog();

            openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Filter = "menudb.dat|menudb.dat";
            openFile.Title = "Open menudb.dat";
            openFile.ShowDialog();

            if (openFile.FileName.Contains("menudb.dat"))
                CheckMenuDB(openFile.FileName);
        }

        private void btnEventGenerate_Click(object sender, EventArgs e)
        {
            if (GameParameter.Events == null || GameParameter.Events.Count == 0)
                MessageBox.Show("Cannot generate a folder with no event. Please add at least one race to this event and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                GenerateGameParameter();
        }

        private void newEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxResult newOverwrite = MessageBox.Show("This will delete the event you are currently editing. Would you like to save your event now?", "New Event", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);

            if (newOverwrite == MessageBoxResult.Yes)
                btnEventGenerate_Click(sender, e);
            else if (newOverwrite == MessageBoxResult.No)
            {
                GameParameter = new GameParameter();
                CurrentEvent = null;

                GameParameter.EventList.Title = "New Event";
                GameParameter.EventList.Description = "Event Description";

                GameParameter.FolderId = eventFolderId;
                GameParameter.EventList.FileNameID = "";

                txtGameParamName.Text = GameParameter.EventList.Title;
                txtGameParamDesc.Text = GameParameter.EventList.Description;

                txtEventName.Text = "";

                ReloadEventLists();
                UpdateEventListing();
            }
        }

        private void importEventToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult importOverwrite = MessageBox.Show("This will overwrite the folder you are currently editing. Would you like to save your folder now?",
                "Import Folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);

            if (importOverwrite == MessageBoxResult.Yes)
                btnEventGenerate_Click(sender, e);
            else if (importOverwrite == MessageBoxResult.Cancel)
                return;

            var openFile = new OpenFileDialog();
            openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Filter = "Folder XML Files (i.e sundaycup.xml) (*.xml)|*.xml";
            openFile.Title = "Import Folder";
            openFile.ShowDialog();

            if (openFile.FileName.Contains(".xml"))
            {
                try
                {
                    GameParameter = ImportFolder(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not import folder\nError: {ex.Message}",
                        "Import failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                OnNewEventSelected(0);
                ReloadEventLists();
                UpdateEventListing();

                if (GameParameter.Events != null && GameParameter.Events.Count > 0)
                    ToggleEventControls(true);
                else
                    ToggleEventControls(false);

                RefreshControls();
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void exportEventToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (GameParameter.Events == null || GameParameter.Events.Count == 0)
            {
                MessageBox.Show("Cannot generate a folder with no events. Please add at least one event to this folder and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saveFile = new System.Windows.Forms.FolderBrowserDialog();

            saveFile.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            saveFile.ShowDialog();
            if (string.IsNullOrEmpty(saveFile.SelectedPath))
                return;

            selectedPath = saveFile.SelectedPath;

            GenerateGameParameter();

        }

        private void importEventListToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult importOverwrite = MessageBox.Show("This will overwrite the folder you are currently editing. Would you like to save your folder now?",
                "Import Folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);

            if (importOverwrite == MessageBoxResult.Yes)
                btnEventGenerate_Click(sender, e);
            else if (importOverwrite == MessageBoxResult.Cancel)
                return;

            var openFile = new OpenFileDialog();
            openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Filter = "Event List XML Files (r/l*.xml) (*.xml)|*.xml";
            openFile.Title = "Import Events";
            openFile.ShowDialog();

            if (openFile.FileName.Contains(".xml"))
            {
                GameParameter = ImportFromEventList(openFile.FileName);
                OnNewEventSelected(0);
                ReloadEventLists();
                UpdateEventListing();

                if (GameParameter.Events != null && GameParameter.Events.Count > 0)
                    ToggleEventControls(true);
                else
                    ToggleEventControls(false);

                RefreshControls();
            }
        }

        private void cboEventCategory_SelectedIndexChanged(object sender, RoutedEventArgs e)
            => GameParameter.EventList.Category = GameParameterEventList.EventCategories.Find(x => x.name == cboEventCategory.SelectedItem.ToString());

        private void txtEventsName_TextChanged(object sender, EventArgs e)
        {
            if (GameParameter.Events != null)
            {
                foreach (Event evnt in GameParameter.Events)
                {
                    if (!string.IsNullOrEmpty(evnt.Name))
                    {
                        if (evnt.Name.Contains(GameParameter.EventList.Title))
                            evnt.Name = evnt.Name.Replace(GameParameter.EventList.Title, txtGameParamName.Text);
                    }
                }

                ReloadEventLists();

                GameParameter.EventList.Title = txtGameParamName.Text;
            }
        }

        private void txtEventsDesc_TextChanged(object sender, EventArgs e)
        {
            GameParameter.EventList.Description = txtGameParamDesc.Text;
        }

        private void iud_StarsNeeded_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (GameParameter != null)
                GameParameter.EventList.StarsNeeded = ((Xceed.Wpf.Toolkit.IntegerUpDown)sender).Value.Value;
        }

        private void rdoStarsOne_CheckedChanged(object sender, EventArgs e)
            => CurrentEvent.Rewards.Stars = rdoStarsOne.IsChecked.Value ? 1 : 0;

        private void rdoStarsThree_CheckedChanged(object sender, EventArgs e)
            => CurrentEvent.Rewards.Stars = rdoStarsThree.IsChecked.Value ? 3 : 0;

        private void cb_gameModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentEvent is null)
                return;

            CurrentEvent.GameMode = (GameMode)(sender as ComboBox).SelectedIndex;
        }

        private void chkIsChampionship_CheckedChanged(object sender, EventArgs e)
        {
            btnChampionshipRewards.IsEnabled = chkIsChampionship.IsChecked.Value;
            GameParameter.EventList.IsChampionship = chkIsChampionship.IsChecked.Value;
        }

        private void btnPickImage_Click(object sender, EventArgs e)
        {
            var openImage = new OpenFileDialog();

            openImage.InitialDirectory = Directory.GetCurrentDirectory();
            openImage.Filter = "All files|*.*|BMP Images|*.bmp|JPEG Images|*.jpg|PNG Images|*.png";
            openImage.Title = "Open Image";
            openImage.ShowDialog();

            if (!openImage.FileName.ToLower().Contains(".bmp") && !openImage.FileName.ToLower().Contains(".jpg") && !openImage.FileName.ToLower().Contains(".png"))
            {
                MessageBox.Show("Input file was not a supported image format. Please input a BMP, JPG, or PNG image and try again.", "Open Image", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            _eventImage = System.Drawing.Image.FromFile(openImage.FileName);

            using (Graphics graphic = Graphics.FromImage(new Bitmap(_eventImage)))
            {
                _eventImage = MiscUtils.ResizeImage(_eventImage, new System.Drawing.Size(432, 244));
                pctImagePreview.Source = new BitmapImage(new Uri(openImage.FileName));
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "texconv.exe")))
            {
                MessageBox.Show("TexConv not found. Please download TexConv from https://github.com/microsoft/DirectXTex/releases and place it in the program folder.",
                    "Save Image", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "TXS3Converter.exe")))
            {
                MessageBox.Show("TexConv not found. Please download TexConv from https://github.com/microsoft/DirectXTex/releases and place it in the program folder.",
                    "Save Image", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string imageFileName = Regex.Replace(GameParameter.EventList.Title.Replace(" ", "").Replace(".", ""), "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToLower();
            string imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), imageFileName+".img");

            _eventImage.Save(imageFilePath, ImageFormat.Png);

            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "TXS3Converter.exe");
            p.StartInfo.Arguments = $"{imageFilePath} --DXT3";
            p.StartInfo.CreateNoWindow = true;
            p.WaitForExit();

            string newPath = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "output", "piece", "gt6", "event_flyer")).FullName;

            string oldPath = Path.Combine(Directory.GetCurrentDirectory(), imageFilePath + ".img");
            string finalPath = Path.Combine(newPath, imageFileName + ".img");
            File.Move(oldPath, finalPath);

        }

        // ------ Non-generated functions ------

        public void UpdateEventListing()
        {
            // Why does refresh unselect my items??? Ugly workaround
            int index = lstRaces.SelectedIndex;
            lstRaces.Items.Refresh();
            cb_QuickEventPicker.Items.Refresh();
            _processEventSwitch = false;
            lstRaces.SelectedIndex = index;
            cb_QuickEventPicker.SelectedIndex = index;
            _processEventSwitch = true;
        }

        /// <summary>
        /// When a new event is selected.
        /// </summary>
        /// <param name="eventIndex"></param>
        public void OnNewEventSelected(int eventIndex)
        {
            _processEventSwitch = false;
            SelectEvent(eventIndex);
            _processEventSwitch = true;

            this.DataContext = CurrentEvent;

            CurrentEvent.MarkUnpopulated();
            PopulateSelectedTab();
            PopulateEventDetails();
        }

        /// <summary>
        /// Swaps to the specified event index and updates the event list & quick switcher.
        /// </summary>
        /// <param name="index"></param>
        public void SelectEvent(int index)
        {
            if (index != -1)
                CurrentEvent = GameParameter.Events[index];

            cb_QuickEventPicker.SelectedIndex = index;
            lstRaces.SelectedIndex = index;
        }

        private void PopulateEventDetails()
        {
            cb_gameModes.SelectedIndex = (int)CurrentEvent.GameMode;
            txtEventName.Text = CurrentEvent.Name;
        }

        private void ReloadEventLists(int selectedIndex = 0, bool isQuickPick = false)
        {
            _processEventSwitch = false;
            EventNames.Clear();
            // Rebuild list from 1 e.g. if 1, 2, 3, delete 2, 3 becomes 2
            int eventIdCounter = 1;
            for (int i = 0; i < GameParameter.Events.Count; i++)
            {
                Event evnt = GameParameter.Events[i];
                evnt.Id = eventIdCounter++;
                EventNames.Add($"{evnt.Id} - {evnt.Name}");
            }


            if (GameParameter.Events.Count == 0)
            {
                lstRaces.SelectedIndex = -1;
                cb_QuickEventPicker.SelectedIndex = -1;
                ToggleEventControls(false);
            }
            else
            {
                lstRaces.SelectedIndex = selectedIndex;
                cb_QuickEventPicker.SelectedIndex = selectedIndex;
            }
            _processEventSwitch = true;
        }

        void ToggleEventControls(bool isEnabled)
        {
            for (int i1 = 2; i1 < tabEvent.Items.Count; i1++)
            {
                var i = (TabItem)tabEvent.Items[i1];
                i.IsEnabled = isEnabled;
            }

            txtEventName.IsEnabled = isEnabled;
            btnCreditRewards.IsEnabled = isEnabled;
            rdoStarsOne.IsEnabled = isEnabled;
            rdoStarsThree.IsEnabled = isEnabled;
            cb_gameModes.IsEnabled = isEnabled;

            checkBox_SeasonalEvent.IsEnabled = isEnabled;
            cb_QuickEventPicker.IsEnabled = isEnabled;
        }

        void CheckMenuDB(string file)
        {
            MenuDB = new MenuDB(file);

            try
            {
                if (MenuDB.CreateConnection())
                {
                    // Check if the table we need exists
                    if (MenuDB.GetFolderNameByID(23).Equals("sundaycup"))
                        MessageBox.Show("Menudb.dat valid!", "Menudb.dat", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Could not verify menudb.dat, please try again.", "Menudb.dat", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("No MenuDB connection was established, please try again.", "Menudb.dat", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Could not open MenuDB.dat: {e.Message}", "Menudb.dat error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        void GenerateGameParameter()
        {
            GameParameter.EventList.Stars = 0;

            foreach (Event @event in GameParameter.Events)
                eventHasNoStars = @event.Rewards.Stars == 0;

            if (!eventHasNoStars && !validationErrors)
            {
                if (selectedPath == "")
                {
                    if (menuDBValid)
                    {
                        SerializeGameParameter(true);
                    }
                    else
                    {
                        MessageBox.Show("Menudb.dat could not be verified. This event cannot be automatically generated. " +
                            "Please either provide a valid menudb.dat or export your file to XML from the File menu.", "Event Generation Information", MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                }
                else
                {
                    SerializeGameParameter(false);
                }
            }
            else
            {
                if (eventHasNoStars)
                    MessageBox.Show("One or more races has no stars assigned to it. Please pick a number of stars and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (validationErrors)
                {
                    MessageBox.Show("One or more text fields is blank. Please populate the highlighted fields and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    validationErrors = false;
                }
            }
        }

        public void SerializeGameParameter(bool shouldEditDB)
        {
            GameParameter.EventList.FileName = Regex.Replace(GameParameter.EventList.Title.Replace(" ", "").Replace(".", ""), "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToLower();

            lastEventFolderId = eventFolderId;

            if (menuDBValid && shouldEditDB)
            {
                int firstSafeFolderID = 9999, firstSafeSortOrderInCategory = 9999, firstSafeTitleID = 9999;

                int lastID = MenuDB.GetLastFolderID();
               
                    // If we can read the menudb then we can increment based on what's already in it to avoid a clash
                    firstSafeFolderID = lastID + 1;

                    // If not already input - probably from saved file
                    if (string.IsNullOrEmpty(GameParameter.EventList.FileNameID))
                        GameParameter.EventList.FileNameID = firstSafeFolderID.ToString();

                    if (GameParameter.FolderId == -1)
                        GameParameter.FolderId = firstSafeFolderID;
                

                firstSafeTitleID = MenuDB.GetLastFolderLocalizeID() + 1;

                results = MenuDB.ExecuteQuery(string.Format("SELECT COALESCE(MAX(FolderOrder), 0) FROM t_event_folder WHERE Type = {0}", GameParameter.EventList.Category.typeID));
                while (results.Read())
                    firstSafeSortOrderInCategory = results.GetInt32(0) + 1;

                MenuDB.AddNewFolderID(GameParameter, firstSafeTitleID, firstSafeFolderID, firstSafeSortOrderInCategory);

                // To give it a shorter name and escape any 's
                string s = GameParameter.EventList.Title.Replace("'", "''");

                MenuDB.AddNewFolderLocalization(firstSafeTitleID, s);
                MenuDB.CloseConnection();
            }
            else
            {
                GameParameter.EventList.FileNameID = lastEventFolderId.ToString();
            }

            if (selectedPath == "")
                selectedPath = Path.Combine(Directory.GetCurrentDirectory(), "output", "game_parameter", "gt6", "event");

            string mainParamFile = Path.Combine(selectedPath);
            GameParameter.EventList.WriteToXML(GameParameter, mainParamFile, ref lastEventRaceId, ref eventRaceId, cboEventCategory.SelectedItem.ToString());

            string filePath = Path.Combine(Path.GetDirectoryName(selectedPath), $"r{GameParameter.EventList.FileNameID}.xml");

            /*
            // Assume new event's races are in the same path as the event itself, as it should be for GT
            if (GameParameter.EventList.FileNameID != null && GameParameter.EventList.FileNameID != "xxx" && File.Exists(filePath))
                ParseEventRaces(GameParameter, filePath);
            else
                MessageBox.Show("No races found for the imported event. This event will have no races until some are added.", "Import Event", MessageBoxButton.OK, MessageBoxImage.Information);
                */

            using (var xml = XmlWriter.Create(Path.Combine(selectedPath, $"r{GameParameter.EventList.FileNameID}.xml"), new XmlWriterSettings() { Indent = true }))
            {
                xml.WriteStartElement("xml");
                GameParameter.WriteToXml(xml);
                xml.WriteEndElement();
            }

            using (var sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "eventid.dat")))
                sw.WriteLine(lastEventRaceId);

            eventFolderId++;

            using (var sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "folderid.dat")))
                sw.WriteLine(lastEventFolderId);

            GameParameter.EventList.Stars = 0;

            MessageBox.Show($"Event and races successfully written to {selectedPath}\\{GameParameter.EventList.FileName}.xml and {selectedPath}\\r{GameParameter.EventList.FileNameID}.xml!", 
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public GameParameter ImportFolder(string filePath)
        {
            GameParameter gp = new GameParameter();

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            gp.EventList.ParseEventList(doc);

            string dir = Path.GetDirectoryName(filePath);
            XmlDocument eventDoc = new XmlDocument();
            eventDoc.Load(Path.Combine(dir, $"r{gp.EventList.FileNameID}.xml"));
            gp.ParseEventsFromFile(eventDoc);

            return gp;
        }

        public GameParameter ImportFromEventList(string filePath)
        {
            XmlDocument eventDoc = new XmlDocument();
            GameParameter gp = new GameParameter();
            eventDoc.Load(filePath);
            gp.ParseEventsFromFile(eventDoc);
            return gp;
        }

        public void ParseEventRaces(GameParameter gameParam, string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            gameParam.ParseEventsFromFile(doc);
        }

        public void RefreshControls()
        {
            txtGameParamName.Text = GameParameter.EventList.Title;
            txtGameParamDesc.Text = GameParameter.EventList.Description;
            iud_StarsNeeded.Value = GameParameter.EventList.StarsNeeded;

            if (GameParameter.Events != null)
                ReloadEventLists(isQuickPick: false);
            else
                ToggleEventControls(false);
        }

        public void PopulateSelectedTab()
        {
            var current = tabEvent.SelectedItem as TabItem;
            if (current.Name.Equals("tabEventRegulation"))
            {
                if (CurrentEvent.Regulations.NeedsPopulating)
                    PrePopulateRegulations();
            }
            else if (current.Name.Equals("tabEventConstraints"))
            {
                if (CurrentEvent.Constraints.NeedsPopulating)
                    PopulateConstraints();
            }
            else if (current.Name.Equals("tabEventParams"))
            {
                if (CurrentEvent.RaceParameters.NeedsPopulating)
                    PopulateParameters();
            }
            else if (current.Name.Equals("tabEntries"))
            {
                PopulateEntries();
            }
            else if (current.Name.Equals("tabEventCourse"))
            {
                PrePopulateCourses();
            }
            else if (current.Name.Equals("tabEvalConditions"))
            {
                PrePopulateEvalConditions();
            }
        }
    }
}
