using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Windows.Input;
using System.ComponentModel;

using Microsoft.Win32;

using Humanizer;
using DiscordRPC;

using PDTools.Enums;
using PDTools.Structures.MGameParameter;
using PDTools.Utils;

using GTEventMaker.Database;
using GTEventMaker.Views;

using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace GTEventMaker;

public partial class GameMakerWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public const string Version = "3.0.0";

    private UserControl _selectedView;
    public UserControl SelectedView
    {
        get { return _selectedView; }
        set
        {
            _selectedView = value;
            // Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged(nameof(SelectedView));
        }
    }

    private GameDB _gameDb;
    private MenuDB MenuDB;

    public EventListFolder Folder { get; set; } = new EventListFolder();
    public GameParameter GameParameter { get; set; }
    public Event CurrentEvent { get; set; }

    public LocalSettings Settings { get; set; }
    public DiscordRpcClient Client { get; private set; }

    public const int BaseEventID = 9900000;
    public const int BaseFolderID = 1000;

    public static RoutedCommand EventSwitchUpCommand = new RoutedCommand();
    public static RoutedCommand EventSwitchDownCommand = new RoutedCommand();

    private bool _processEventSwitch = true;
    private bool _loading = true;

    public GameMakerWindow()
    {
        _gameDb = App.GameDatabase;

        InitializeComponent();

        ToolTipService.ShowDurationProperty.OverrideMetadata(
            typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

        GameParameter = new GameParameter();
        GameParameter.FolderId = BaseFolderID;
        Settings = new LocalSettings();

        Title = $"GT Event Maker - {Version}";
    }

    ~GameMakerWindow()
    {
        if (Client != null && !Client.IsDisposed)
            Client.Dispose();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (GameParameter?.Events?.Count != 0)
        {
            if (MessageBox.Show("You are currently editing a folder. Are you sure that you would like to exit?",
                "Exiting", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        Settings.Save(".settings");
        if (Client != null && !Client.IsDisposed)
            Client.Dispose();
    }

    private void GameGenerator_Load(object sender, EventArgs e)
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "data.db");
        if (!File.Exists(dbPath))
        {
            MessageBox.Show("Required database file for the generator is missing (data/data.db), exiting.", "Database file missing");
            this.Close();
            return;
        }

        // Load Settings
        if (File.Exists(".settings"))
            Settings.ReadFromFile(".settings");
        else
            Settings.CreateDefault();

        // Discord Presence 
        Client = new DiscordRpcClient("784198457220005899");
        if (Settings.HasEnabledSetting("Discord_Presence_Enabled"))
        {
            Client.Initialize();
            UpdateDiscordPresence();
            DiscordRichPresenceMenuItem.IsChecked = true;
        }
        minimizeXMLToolStripMenuItem.IsChecked = Settings.HasEnabledSetting("Minify_XML");
        //exportCacheToolStripMenuItem.IsChecked = Settings.HasEnabledSetting("Create_FGP");

       
        EventSwitchUpCommand.InputGestures.Add(new KeyGesture(Key.Up, ModifierKeys.Control));
        EventSwitchDownCommand.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Control));
        CommandBindings.Add(new CommandBinding(EventSwitchUpCommand, EventSwitchUpCommand_Executed));
        CommandBindings.Add(new CommandBinding(EventSwitchDownCommand, EventSwitchDownCommand_Executed));
        _loading = false;

    }


    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 1 || !files[0].EndsWith(".xml"))
                return;

            // Quickly check the file, not the most efficient
            var txt = File.ReadAllText(files[0]);
            if (txt.Contains("<event_list>"))
            {
                if (MessageBox.Show("Load event folder? Current progress will be lost.", "New Event Folder", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    HandleImportFolder(files[0]);
            }
            else if (txt.Contains("<GameParameter version="))
            {
                if (MessageBox.Show("Load event list? Current progress will be lost.", "New Event List", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    HandleNewEventList(files[0]);
            }
            else
            {
                MessageBox.Show("Not a recognized folder or event list", "Could not load XML file",
                   MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    #region Menu Bar
    private void mi_exportEventToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events == null || GameParameter.Events.Count == 0)
        {
            MessageBox.Show("Cannot generate a folder with no events. Please add at least one event to this folder and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var saveFile = new SaveFileDialog();
        saveFile.CheckPathExists = true;
        saveFile.FileName = "folder.xml";
        saveFile.Filter = "XML File|*.xml";

        if (saveFile.ShowDialog() == true)
        {
            if (string.IsNullOrEmpty(saveFile.FileName))
                return;

            string path = saveFile.FileName;
            string folder = Path.GetDirectoryName(path);
            string gameParameterFile = $"r{GameParameter.FolderId:D3}.xml";
            if (VerifyAndGenerateGameParameter(Path.Combine(folder, gameParameterFile), GameParameter, out List<ValidationInfo> infos))
            {
                Folder.EventIDList.Clear();
                foreach (Event @event in GameParameter.Events)
                    Folder.EventIDList.Add(@event.EventID);

                SerializeEventList(saveFile.FileName, Folder);
                MessageBox.Show($"Event list & Game parameter successfully written to {path} and {gameParameterFile}.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            if (infos.Count > 0)
            {
                var window = new ErrorListWindow(infos);
                window.Owner = this;
                window.Show();
            }
        }
    }

    private void mi_ExportGameParameterToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events == null || GameParameter.Events.Count == 0)
        {
            MessageBox.Show("Cannot generate a folder with no events. Please add at least one event to this folder and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string gameParameterFile = $"r{GameParameter.FolderId:D3}.xml";
        var saveFile = new SaveFileDialog();
        saveFile.CheckPathExists = true;
        saveFile.FileName = gameParameterFile;
        saveFile.Filter = "XML File|*.xml";

        if (saveFile.ShowDialog() == true)
        {
            if (string.IsNullOrEmpty(saveFile.FileName))
                return;

            if (VerifyAndGenerateGameParameter(saveFile.FileName, GameParameter, out List<ValidationInfo> infos))
            {
                MessageBox.Show($"Game parameter successfully written to {gameParameterFile}.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (infos.Count > 0)
            {
                var window = new ErrorListWindow(infos);
                window.Owner = this;
                window.Show();
            }
        }
    }

    private void mi_exportSeasonalGPListToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events == null || GameParameter.Events.Count == 0)
        {
            MessageBox.Show("Cannot generate a folder with no events. Please add at least one event to this folder and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var saveFile = new SaveFileDialog();
        saveFile.FileName = $"event_{GameParameter.FolderId}.xml";
        saveFile.CheckPathExists = true;
        saveFile.Filter = "Seasonal Game Parameter List XML|*.xml";

        List<GameParameter> list = new List<GameParameter>();
        foreach (var @event in GameParameter.Events)
        {
            var gp = new GameParameter();
            gp.FolderId = GameParameter.FolderId;
            gp.Arcade = GameParameter.Arcade;
            gp.Championship = GameParameter.Championship;
            gp.Events.Add(@event);
            list.Add(gp);
        }

        if (saveFile.ShowDialog() == true)
        {
            if (string.IsNullOrEmpty(saveFile.FileName))
                return;

            string path = saveFile.FileName;
            if (VerifyAndGenerateGameParameterList(path, list, out List<ValidationInfo> infos))
            {
                MessageBox.Show($"Game parameter successfully saved to {path}.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (infos.Count > 0)
            {
                var window = new ErrorListWindow(infos);
                window.Owner = this;
                window.Show();
            }
        }
    }

    private void mi_importEventListToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events.Any())
        {
            MessageBoxResult result = MessageBox.Show("This will overwrite the folder you are currently editing. Continue?",
                "Import Folder", MessageBoxButton.YesNo, MessageBoxImage.Information);

             if (result == MessageBoxResult.No)
                return;
        }

        var openFile = new OpenFileDialog();
        openFile.InitialDirectory = Directory.GetCurrentDirectory();
        openFile.Filter = "Event List XML Files (r/l*.xml) (*.xml)|*.xml";
        openFile.Title = "Import Events";
        openFile.ShowDialog();

        if (openFile.FileName.Contains(".xml"))
            HandleNewEventList(openFile.FileName);
    }

    private void mi_importSeasonalGPListToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events.Any())
        {
            MessageBoxResult result = MessageBox.Show("This will overwrite the folder you are currently editing. Continue?",
                "Import Folder", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.No)
                return;
        }

        var openFile = new OpenFileDialog();
        openFile.InitialDirectory = Directory.GetCurrentDirectory();
        openFile.Filter = "Seasonal Game Parameter XML Files (event_*.xml)|*.xml";
        openFile.Title = "Import Events";
        openFile.ShowDialog();

        if (openFile.FileName.Contains(".xml"))
            HandleSeasonalGameParameterList(openFile.FileName);
    }

    private void mi_importCacheToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events.Any())
        {
            MessageBoxResult result = MessageBox.Show("This will overwrite the folder you are currently editing. Continue?",
                "Import Folder", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.No)
                return;
        }

        var openFile = new OpenFileDialog();
        openFile.InitialDirectory = Directory.GetCurrentDirectory();
        openFile.Title = "Import Events";
        openFile.ShowDialog();

        if (!string.IsNullOrEmpty(openFile.FileName))
            ;//new GameParameter().ReadFromCache(openFile.FileName);
    }

    private void mi_newEventToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("This will overwrite the folder you are currently editing. Continue?",
                "Import Folder", MessageBoxButton.YesNo, MessageBoxImage.Information);

        if (result == MessageBoxResult.No)
            return;

        GameParameter = new GameParameter();
        GameParameter.FolderId = BaseFolderID;
        Folder = new EventListFolder();

        ClearQuickPicker();

        SelectedView = new FolderView(GameParameter, Folder);
        ReselectEvent(GameParameter.Events.Count - 1);
    }

    private void mi_importEventToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (GameParameter.Events.Any())
        {
            MessageBoxResult result = MessageBox.Show("This will overwrite the folder you are currently editing. Continue?",
                "Import Folder", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.No)
                return;
        }

        var openFile = new OpenFileDialog();
        openFile.InitialDirectory = Directory.GetCurrentDirectory();
        openFile.Filter = "Folder XML Files (i.e sundaycup.xml) (*.xml)|*.xml";
        openFile.Title = "Import Folder";
        openFile.ShowDialog();

        if (openFile.FileName.Contains(".xml"))
            HandleImportFolder(openFile.FileName);
    }

    private void mi_exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void mi_importEventFromFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var openFile = new OpenFileDialog();
        openFile.InitialDirectory = Directory.GetCurrentDirectory();
        openFile.Filter = "Folder XML Files (i.e sundaycup.xml) (*.xml)|*.xml";
        openFile.Title = "Import Events";
        openFile.ShowDialog();

        if (openFile.FileName.Contains(".xml"))
        {
            GameParameter gp;
            try
            {
                gp = ImportFolder(openFile.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not read event folder: {ex.Message}", "Error",
                   MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!gp.Events.Any())
            {
                MessageBox.Show($"No events found in provided folder.", "Information",
                   MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selector = new EventImportSelectorWindow(gp);
            selector.ShowDialog();

            if (selector.SelectedEvent != null)
            {
                GameParameter.Events.Add(selector.SelectedEvent);
            }
        }
    }

    private void mi_decryptTedMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var openFile = new OpenFileDialog();
        openFile.Filter = "Course Maker File (*.ted)|*.ted";
        openFile.Title = "Import Course Maker File to decrypt";
        if (openFile.ShowDialog() == true)
        {
            if (!CustomCourse.Decrypt(openFile.FileName))
            {
                MessageBox.Show("Could not decrypt TED file - File is already decrypted or not a valid TED Custom track.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("File successfully decrypted.", "Completed",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    private void mi_encryptTedMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var openFile = new OpenFileDialog();
        openFile.Filter = "Course Maker File (*.ted)|*.ted";
        openFile.Title = "Import Course Maker File to encrypt";
        if (openFile.ShowDialog() == true)
        {
            if (!CustomCourse.Encrypt(openFile.FileName))
            {
                MessageBox.Show("Could not encrypt TED file - File is already encrypted or not a valid TED Custom track.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("File successfully encrypted.", "Completed",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public void mi_DiscordRichPresenceMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Settings.SetSettingValue("Discord_Presence_Enabled", DiscordRichPresenceMenuItem.IsChecked);
        if (DiscordRichPresenceMenuItem.IsChecked)
        {
            if (!Client.IsInitialized)
                Client.Initialize();
            UpdateDiscordPresence();
        }
        else
        {
            if (Client.IsInitialized)
                Client.Deinitialize();
        }
    }

    private void mi_minimizeXMLToolStripMenuItem_Checked(object sender, RoutedEventArgs e)
        => Settings.SetSettingValue("Minify_XML", minimizeXMLToolStripMenuItem.IsChecked);

    private void mi_exportCacheToolStripMenuItem_Checked(object sender, RoutedEventArgs e)
    {
        if (_loading)
            return;

        /*
        if (exportCacheToolStripMenuItem.IsChecked)
        {
            var res = MessageBox.Show("This option makes the generator export a cache file and .fgp file for GT6 1.22 ONLY.\n" +
                "These are used in GT6 to drastically speed up loading large folders (up to 5 times with 60 events).\n" +
                "This may not work 100%, so use and test this accordingly when you are fully done with your event.\n\n" +
                "Once exporting, drag the cache file (the one without any file extension) to the game_parameter/gp_cache folder. The other files goes in the event folder as usual.\n\n" +
                "Enable this option?", "Information",
                   MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (res != MessageBoxResult.Yes)
                exportCacheToolStripMenuItem.IsChecked = false;
        }

        Settings.SetSettingValue("Create_FGP", exportCacheToolStripMenuItem.IsChecked);
        */
    }

    private void mi_randomizeAINamesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        foreach (Entry entry in CurrentEvent.EntrySet.Entries)
        {
            var driverInfo = _gameDb.GetRandomDriverInfo();
            var regionInfo = RegionUtil.GetRandomInitial(App.Random, driverInfo.InitialType);

            entry.DriverName = $"{regionInfo.initial}. {driverInfo.DriverName}";
            entry.DriverRegion = regionInfo.country;
        }

        foreach (EntryBase entryBase in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
        {
            var driverInfo = _gameDb.GetRandomDriverInfo();
            var regionInfo = RegionUtil.GetRandomInitial(App.Random, driverInfo.InitialType);

            entryBase.DriverName = $"{regionInfo.initial}. {driverInfo.DriverName}";
            entryBase.DriverRegion = regionInfo.country;
        }

        RefreshView();
    }

    private bool _alreadyRefreshingView = false;

    private void mi_randomizeAIRoughnessMenuItem_Click(object sender, RoutedEventArgs e)
    {
        foreach (var entry in CurrentEvent.EntrySet.Entries)
            entry.AIRoughness = (sbyte)App.Random.Next(0, 10 + 1);

        foreach (var entry in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
            entry.AIRoughness = (sbyte)App.Random.Next(0, 10 + 1);

        if (!_alreadyRefreshingView)
            RefreshView();
    }

    private void mi_randomizeAICornerSkillMenuItem_Click(object sender, RoutedEventArgs e)
    {
        foreach (var entry in CurrentEvent.EntrySet.Entries)
            entry.AISkillCornering = (sbyte)App.Random.Next(80, 105 + 1);

        foreach (var entry in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
            entry.AICorneringSkill = (sbyte)App.Random.Next(80, 105 + 1);

        if (!_alreadyRefreshingView)
            RefreshView();
    }

    private void mi_randomizeAIBrakingSkillMenuItem_Click(object sender, RoutedEventArgs e)
    {
        foreach (var entry in CurrentEvent.EntrySet.Entries)
            entry.AISkillBraking = (sbyte)App.Random.Next(80, 105 + 1);

        foreach (var entry in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
            entry.AIBrakingSkill = (sbyte)App.Random.Next(80, 105 + 1);

        if (!_alreadyRefreshingView)
            RefreshView();
    }

    private void mi_randomizeAIAccelSkillMenuItem_Click(object sender, RoutedEventArgs e)
    {
        foreach (var entry in CurrentEvent.EntrySet.Entries)
            entry.AISkillAccelerating = (sbyte)App.Random.Next(80, 105 + 1);

        foreach (var entry in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
            entry.AIAcceleratingSkill = (sbyte)App.Random.Next(80, 105 + 1);

        if (!_alreadyRefreshingView)
            RefreshView();
    }

    private void mi_randomizeAIStartSkillMenuItem_Click(object sender, RoutedEventArgs e)
    {
        foreach (var entry in CurrentEvent.EntrySet.Entries)
            entry.AISkillStarting = (sbyte)App.Random.Next(80, 105 + 1);

        foreach (var entry in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
            entry.AIStartingSkill = (sbyte)App.Random.Next(80, 105 + 1);

        if (!_alreadyRefreshingView)
            RefreshView();
    }

    private void mi_randomizeAISkillsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("This will regenerate ALL AI skills for the current event. Continue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Information)
            == MessageBoxResult.Yes)
        {
            _alreadyRefreshingView = true;

            mi_randomizeAIRoughnessMenuItem_Click(sender, e);
            mi_randomizeAICornerSkillMenuItem_Click(sender, e);
            mi_randomizeAIBrakingSkillMenuItem_Click(sender, e);
            mi_randomizeAIAccelSkillMenuItem_Click(sender, e);
            mi_randomizeAIStartSkillMenuItem_Click(sender, e);

            _alreadyRefreshingView = false;

            RefreshView();
        }
    }

    private void mi_randomizeAITireCompoundToolStripMenuItem_Click(object sender, RoutedEventArgs e)
    {
        /*
        foreach (var entry in CurrentEvent.EntrySet.Entries)
        {
            entry.TireFront = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;
            entry.TireRear = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;
        }

        foreach (var entry in CurrentEvent.EntrySet.EntryGenerate.EntryBaseArray)
        {
            entry.TireFront = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;
            entry.TireRear = (TireType)comboBox_AIGenTyreComp.SelectedIndex - 1;
        }
        */
    }

    private void mi_SingleRaceSample_Click(object sender, RoutedEventArgs e)
    {
        CurrentEvent.GameMode = GameMode.EVENT_RACE; // SINGLE_RACE
        CurrentEvent.PlayStyle.PlayType = PlayType.RACE;
        CurrentEvent.RaceParameter.RaceType = RaceType.COMPETITION;
        CurrentEvent.RaceParameter.StartType = StartType.GRID;
        CurrentEvent.RaceParameter.CompleteType = CompleteType.BYLAPS;
        CurrentEvent.RaceParameter.TimeToStart = TimeSpan.FromSeconds(6);
        CurrentEvent.RaceParameter.TimeToFinish = TimeSpan.FromSeconds(30);
        CurrentEvent.RaceParameter.EnablePit = false;
        CurrentEvent.RaceParameter.FinishType = FinishType.TARGET;
        CurrentEvent.RaceParameter.LineGhostPlayMax = 0;
        CurrentEvent.RaceParameter.LineGhostRecordType = LineGhostRecordType.OFF;
        CurrentEvent.RaceParameter.GhostType = GhostType.NONE;
        CurrentEvent.RaceParameter.PenaltyLevel = PenaltyLevelTypes.OFF;
        CurrentEvent.RaceParameter.EnableDamage = true;
        CurrentEvent.RaceParameter.BehaviorDamage = BehaviorDamageType.WEAK;

        RefreshView();
    }

    private void mi_FreeRunSample_Click(object sender, RoutedEventArgs e)
    {
        CurrentEvent.GameMode = GameMode.FREE_RUN;
        CurrentEvent.PlayStyle.PlayType = PlayType.RACE;
        CurrentEvent.RaceParameter.RaceType = RaceType.TIMEATTACK;
        CurrentEvent.RaceParameter.StartType = StartType.ATTACK;
        CurrentEvent.RaceParameter.CompleteType = CompleteType.NONE;
        CurrentEvent.RaceParameter.FinishType = FinishType.FASTEST;
        CurrentEvent.RaceParameter.LineGhostPlayMax = 0;
        CurrentEvent.RaceParameter.LineGhostRecordType = LineGhostRecordType.OFF;
        CurrentEvent.RaceParameter.GhostType = GhostType.ONELAP;
        CurrentEvent.RaceParameter.PenaltyLevel = PenaltyLevelTypes.OFF;
        CurrentEvent.RaceParameter.EnableDamage = true;
        CurrentEvent.RaceParameter.BehaviorDamage = BehaviorDamageType.WEAK;

        RefreshView();
    }

    private void mi_LicenseSample_Click(object sender, RoutedEventArgs e)
    {
        CurrentEvent.GameMode = GameMode.LICENSE;
        CurrentEvent.EventType = PDTools.Enums.EventType.RACE;
        CurrentEvent.RaceParameter.RaceType = RaceType.COMPETITION;
        CurrentEvent.RaceParameter.StartType = StartType.STANDING;
        CurrentEvent.RaceParameter.CompleteType = CompleteType.NONE;
        CurrentEvent.RaceParameter.FinishType = FinishType.FASTEST;
        CurrentEvent.RaceParameter.RaceLimitLaps = 0;
        CurrentEvent.RaceParameter.LineGhostPlayMax = 10;
        CurrentEvent.RaceParameter.LineGhostRecordType = LineGhostRecordType.ONE;
        CurrentEvent.RaceParameter.PenaltyLevel = PenaltyLevelTypes.OFF;
        CurrentEvent.RaceParameter.EnableDamage = false;
        CurrentEvent.RaceParameter.BehaviorDamage = BehaviorDamageType.WEAK;

        CurrentEvent.EvalCondition.ConditionType = EvalConditionType.TIME;
        CurrentEvent.EvalCondition.Gold = 59000;
        CurrentEvent.EvalCondition.Silver = 59000;
        CurrentEvent.EvalCondition.Bronze = 59000;

        RefreshView();
    }

    private void mi_TimeAttackSample_Click(object sender, RoutedEventArgs e)
    {
        CurrentEvent.GameMode = GameMode.TIME_ATTACK;
        CurrentEvent.PlayStyle.PlayType = PlayType.RACE;
        CurrentEvent.RaceParameter.RaceType = RaceType.TIMEATTACK;
        CurrentEvent.RaceParameter.StartType = StartType.ATTACK;
        CurrentEvent.RaceParameter.CompleteType = CompleteType.NONE;
        CurrentEvent.RaceParameter.RaceLimitLaps = 1;
        CurrentEvent.RaceParameter.LineGhostPlayMax = 10;
        CurrentEvent.RaceParameter.LineGhostRecordType = LineGhostRecordType.ONE;
        CurrentEvent.RaceParameter.FinishType = FinishType.NONE;
        CurrentEvent.RaceParameter.GhostType = GhostType.ONELAP;
        CurrentEvent.RaceParameter.PenaltyLevel = PenaltyLevelTypes.OFF;
        CurrentEvent.RaceParameter.EnableDamage = false;
        CurrentEvent.RaceParameter.BehaviorDamage = BehaviorDamageType.WEAK;

        RefreshView();
    }

    private void mi_SeasonalTimeAttackSample_Click(object sender, RoutedEventArgs e)
    {
        mi_TimeAttackSample_Click(sender, e);
        CurrentEvent.GameMode = GameMode.ONLINE_TIME_ATTACK;
        CurrentEvent.BeginDate = new DateTime(1999, 04, 01);
        CurrentEvent.EndDate = new DateTime(2999, 04, 01);
        CurrentEvent.Ranking.BeginDate = new DateTime(1999, 04, 01);
        CurrentEvent.Ranking.EndDate = new DateTime(2999, 04, 01);
        CurrentEvent.RaceParameter.EnableDamage = true;

        RefreshView();
    }

    private void mi_DriftAttackSample_Click(object sender, RoutedEventArgs e)
    {
        CurrentEvent.GameMode = GameMode.DRIFT_ATTACK;
        CurrentEvent.PlayStyle.PlayType = PlayType.RACE;
        CurrentEvent.RaceParameter.RaceType = RaceType.DRIFTATTACK;
        CurrentEvent.RaceParameter.StartType = StartType.COURSEINFO;
        CurrentEvent.RaceParameter.CompleteType = CompleteType.NONE;
        CurrentEvent.RaceParameter.LineGhostPlayMax = 10;
        CurrentEvent.RaceParameter.TimeToStart = TimeSpan.FromSeconds(1.9);
        CurrentEvent.RaceParameter.TimeToFinish = TimeSpan.FromSeconds(2);
        CurrentEvent.RaceParameter.LineGhostRecordType = LineGhostRecordType.OFF;
        CurrentEvent.RaceParameter.GhostType = GhostType.NONE;
        CurrentEvent.RaceParameter.PenaltyLevel = PenaltyLevelTypes.OFF;
        CurrentEvent.RaceParameter.EnableDamage = true;
        CurrentEvent.RaceParameter.BehaviorDamage = BehaviorDamageType.WEAK;

        RefreshView();
    }

    private void mi_SeasonalDriftAttackSample_Click(object sender, RoutedEventArgs e)
    {
        mi_DriftAttackSample_Click(sender, e);
        CurrentEvent.GameMode = GameMode.ONLINE_DRIFT_ATTACK;
    }

    private void mi_ArcadeStyleSample_Click(object sender, RoutedEventArgs e)
    {
        CurrentEvent.GameMode = GameMode.ARCADE_STYLE_RACE;

        CurrentEvent.PlayStyle.PlayType = PlayType.RACE;
        CurrentEvent.RaceParameter.RaceType = RaceType.TIMEATTACK;
        CurrentEvent.RaceParameter.StartType = StartType.STANDING_CENTER;
        CurrentEvent.RaceParameter.CompleteType = CompleteType.BYLAPS;
        CurrentEvent.RaceParameter.RaceLimitLaps = 1;
        CurrentEvent.RaceParameter.FinishType = FinishType.TARGET;
        CurrentEvent.RaceParameter.GhostType = GhostType.NONE;
        CurrentEvent.RaceParameter.PenaltyLevel = PenaltyLevelTypes.OFF;
        CurrentEvent.RaceParameter.EnableDamage = false;
        CurrentEvent.RaceParameter.BehaviorDamage = BehaviorDamageType.WEAK;

        RefreshView();
    }

    #endregion

    #region Commands
    public void EventSwitchUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (!GameParameter.Events.Any())
            return;

        if (cb_QuickEventPicker.SelectedIndex > 0)
            ReselectEvent(cb_QuickEventPicker.SelectedIndex - 1);
    }

    public void EventSwitchDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (!GameParameter.Events.Any())
            return;

        if (cb_QuickEventPicker.SelectedIndex < cb_QuickEventPicker.Items.Count - 1)
            ReselectEvent(cb_QuickEventPicker.SelectedIndex + 1);
    }
    #endregion

    #region Event Select
    private void cb_QuickEventPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cb_QuickEventPicker.SelectedIndex == -1)
            return;

        if (!_processEventSwitch)
            return;

        ReselectEvent(cb_QuickEventPicker.SelectedIndex);
    }

    #endregion

    #region Non Generated
    // ------ Non-generated functions ------

    public void ToggleContextualWindowControls(bool isEnabled)
    {
        for (int i1 = 2; i1 < lb_ViewList.Items.Count; i1++)
        {
            ListBoxItem i = (ListBoxItem)lb_ViewList.Items[i1];
            if (string.IsNullOrEmpty(i.Name))
                continue; // Skip all non well defined entries

            i.IsEnabled = isEnabled;
        }

        cb_QuickEventPicker.IsEnabled = isEnabled;

        menuRegenerate.IsEnabled = isEnabled;
        menuSample.IsEnabled = isEnabled;
        importEventFromFolderToolStripMenuItem.IsEnabled = isEnabled;
    }

    public void CheckMenuDB(string file)
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

    public bool ValidateGameParameter(GameParameter gameParameter, out List<ValidationInfo> infos)
    {
        infos = new List<ValidationInfo>();
        if (gameParameter.Events.DistinctBy(e => e.EventID).Count() != gameParameter.Events.Count)
        {
            infos.Add(new ValidationInfo(Severity.Error, "More than one event shares the same Event Id"));
        }

        for (int i = 0; i < gameParameter.Events.Count; i++)
        {
            var @event = gameParameter.Events[i];
            if (@event.EntrySet.EntryGenerate.GenerateType == EntryGenerateType.NONE)
            {
                // Game will allocate player if it hasn't been set
                int numEntries = @event.EntrySet.Entries.Count == 0 ? 1 : @event.EntrySet.Entries.Count;
                if (numEntries > @event.RaceParameter.RacersMax)
                {
                    infos.Add(new ValidationInfo(Severity.Error, "More race entries than Racers Num allows"));
                }

                if (numEntries > @event.RaceParameter.EntryMax)
                {
                    infos.Add(new ValidationInfo(Severity.Error, "More race entries than Entry Max allows"));
                }


            }
            else if (@event.EntrySet.EntryGenerate.GenerateType == EntryGenerateType.ONE_MAKE)
            {
                if (@event.EntrySet.EntryGenerate.EntryNum == 0)
                {
                    infos.Add(new ValidationInfo(Severity.Warning, "Entry generation set to one-make but entries to generate is set to 0"));
                }
            }
        }
        return infos.Any(e => e.Severity == Severity.Error);
    }

    /// <summary>
    /// Verifies a game parameter and serializes it if possible.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="infos"></param>
    /// <returns></returns>
    public bool VerifyAndGenerateGameParameter(string fileName, GameParameter gameParameter, out List<ValidationInfo> infos)
    {
        bool validationError = ValidateGameParameter(GameParameter, out infos);
        if (validationError)
        {
            return false;
        }
        else
        {
            SerializeGameParameter(fileName, gameParameter);
        }

        return true;
    }

    /// <summary>
    /// Verifies a game parameter list and serializes it if possible.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="infos"></param>
    /// <returns></returns>
    public bool VerifyAndGenerateGameParameterList(string path, List<GameParameter> gameParameters, out List<ValidationInfo> infos)
    {
        infos = new List<ValidationInfo>();
        foreach (var @event in gameParameters)
        {
            bool validationError = ValidateGameParameter(@event, out infos);
            if (validationError)
            {
                return false;
            }
        }

        SerializeGameParameterList(path, gameParameters);
        return true;
    }

    /// <summary>
    /// Serializes a game parameter to a XML file.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="gameParameter"></param>
    public void SerializeGameParameter(string fileName, GameParameter gameParameter)
    {
        bool minify = Settings.HasEnabledSetting("Minify_XML");

        using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = !minify, IndentChars = "  ", OmitXmlDeclaration = true }))
        {
            writer.WriteStartElement("xml");
            gameParameter.WriteToXmlSorted(writer);
            writer.WriteEndElement();
        }            

        /* Original GT6 Events cache the files using an MD5.
        * The FGP file contains the file name of the cache file in gp_cache
        * It does not have to be MD5'd at all, so we ignore doing it */
        /*
        if (Settings.HasEnabledSetting("Create_FGP")) 
        {
            byte[] file = File.ReadAllBytes(eventXmlPath);

            string cacheFileName;
            using (var md5 = MD5.Create())
            {
                byte[] file = File.ReadAllBytes(eventXmlPath);
                md5Str = BitConverter.ToString(md5.ComputeHash(file)).Replace("-", "").ToLower();
                File.WriteAllText(eventXmlPath + ".fgp", md5Str);
            }

            File.WriteAllText(eventXmlPath + ".fgp", folderName);

            string pathDir = Path.Combine(path, folderName);
            File.WriteAllBytes(pathDir, GameParameter.Serialize(GameDatabase));
        }
        */
    }

    /// <summary>
    /// Serializes an event list to a XML file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="eventList"></param>
    public void SerializeEventList(string path, EventListFolder eventList)
    {
        bool minify = Settings.HasEnabledSetting("Minify_XML");

        using (var writer = XmlWriter.Create(path, new XmlWriterSettings() { Indent = !minify, IndentChars = "  " }))
        {
            eventList.WriteToXML(writer);
        }
    }

    /// <summary>
    /// Serializes a game parameter list to a XML file.
    /// </summary>
    /// <param name="xmlPath"></param>
    /// <param name="gameParameters"></param>
    public void SerializeGameParameterList(string xmlPath, List<GameParameter> gameParameters)
    {
        bool minify = Settings.HasEnabledSetting("Minify_XML");

        using (var writer = XmlWriter.Create(xmlPath, new XmlWriterSettings() { Indent = !minify, IndentChars = "  ", OmitXmlDeclaration = true }))
        {
            writer.WriteStartElement("xml");
            foreach (var gp in gameParameters)
                gp.WriteToXmlSorted(writer);
            writer.WriteEndElement();
        }
    }

    public GameParameter ImportFolder(string filePath)
    {
        var gp = new GameParameter();

        var doc = new XmlDocument();
        doc.Load(filePath);

        XmlNode eventListNode = doc["event_list"] ?? throw new XmlException($"Not a valid event folder xml (event_list node missing).");
        XmlNode eventNode = eventListNode["event"] ?? throw new XmlException($"Not a valid event folder xml (event_list->event node missing).");

        Folder = new EventListFolder();
        Folder.ParseEventText(eventNode);
        string dir = Path.GetDirectoryName(filePath);

        string eventListFile = Path.Combine(dir, $"r{Folder.Id:000}.xml");
        if (!File.Exists(eventListFile))
            throw new FileNotFoundException($"Could not find file {eventListFile} referenced by the provided folder.");

        var settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        using (var reader = XmlReader.Create(eventListFile, settings))
        {
            var eventDoc = new XmlDocument();
            eventDoc.Load(reader);

            gp.ParseFromXmlNode(eventDoc["xml"]["GameParameter"]);
        }

        return gp;
    }

    /// <summary>
    /// Opens and reads a game parameter from the specified XML file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public GameParameter ReadGameParameterXMLFile(string filePath)
    {
        XmlDocument eventDoc = new XmlDocument();
        var settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        using (var reader = XmlReader.Create(filePath, settings))
        {
            var gp = new GameParameter();
            eventDoc.Load(reader);

            gp.ParseFromXmlNode(eventDoc["GameParameter"]); 
            return gp;
        }
    }

    /// <summary>
    /// Opens and reads all the game parameter nodes in the specified XML file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public List<GameParameter> ReadGameParameterListXML(string filePath)
    {
        List<GameParameter> gameParameters = new List<GameParameter>();

        XmlDocument eventDoc = new XmlDocument();
        eventDoc.Load(filePath);

        var root = eventDoc["xml"];
        foreach (XmlNode gpNode in root.SelectNodes("GameParameter"))
        {
            var gp = new GameParameter();
            gp.ParseFromXmlNode(gpNode);
            gameParameters.Add(gp);
        }

        return gameParameters;
    }

    private void MenuItem_About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Credits: " +
            "- Nenkai#9075 - Creator\n" +
            "- TheAdmiester - Co-Creator/Made the original tool\n" +
            "- Everyone who tested this tool and uses it - thank you!", "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void UpdateDiscordPresence()
    {
        if (Settings.HasEnabledSetting("Discord_Presence_Enabled") && Client?.IsInitialized == true)
        {
            var presence = new RichPresence();
            if (CurrentEvent is null)
            {
                presence.Details = "No Folder.";
            }
            else
            {
                presence.Details = $"{Folder.Title[PDTools.Enums.Language.GB]}";
                presence.State = $"Event:";
                presence.Timestamps = Timestamps.Now;
            }

            presence.Assets = new Assets()
            {
                LargeImageText = "Gran Turismo 5/6 Event Generator",
                LargeImageKey = "icon",
            };

            Client.SetPresence(presence);
        }
    }

    public void HandleImportFolder(string fileName)
    {
        try
        {
            GameParameter = ImportFolder(fileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not import folder\nError: {ex.Message}",
                "Import failed", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        PopulateQuickPickerWithNewEventList();
        SelectedView = new FolderView(GameParameter, Folder);
        ReselectEvent(GameParameter.Events.Count - 1);
    }

    private void PopulateQuickPickerWithNewEventList()
    {
        ClearQuickPicker();
        for (int i = 0; i < GameParameter.Events.Count; i++)
        {
            Event @event = GameParameter.Events[i];
            AddQuickPickerEntry($"{@event.EventID} - {@event.Information.Title.Texts[PDTools.Enums.Language.GB]}");
        }
    }

    private void lb_ViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RefreshView();
    }

    private void RefreshView()
    {
        ListBoxItem item = lb_ViewList.SelectedItem as ListBoxItem;
        UserControl view = item.Name switch
        {
            nameof(lbi_FolderInfo) => new FolderView(GameParameter, Folder),
            nameof(lbi_Events) => new EventsView(GameParameter),
            nameof(lbi_Information) => new InformationView() { DataContext = CurrentEvent.Information },
            nameof(lbi_PlayStyle) => new PlayStyleView() { DataContext = CurrentEvent.PlayStyle },
            nameof(lbi_RaceParameters) => new RaceParameterView { DataContext = CurrentEvent.RaceParameter },
            nameof(lbi_Regulations) => new RegulationView { DataContext = CurrentEvent.Regulation },
            nameof(lbi_Entries) => new EntrySetView { DataContext = CurrentEvent.EntrySet },
            nameof(lbi_Constraints) => new ConstraintView { DataContext = CurrentEvent.Constraint },
            nameof(lbi_Course) => new TrackView { DataContext = CurrentEvent.Track },
            nameof(lbi_Rewards) => new RewardView() { DataContext = CurrentEvent.Reward },
            nameof(lbi_EvalConditions) => new EvalConditionView { DataContext = CurrentEvent.EvalCondition },
            nameof(lbi_FailConditions) => new FailConditionView { DataContext = CurrentEvent.FailureCondition },
            nameof(lbi_LicenseConditions) => new LicenseConditionView { DataContext = CurrentEvent.LicenseCondition },
            nameof(lbi_ArcadeStyleSettings) => new ArcadeStyleView { DataContext = CurrentEvent.ArcadeSetting },
            nameof(lbi_Replay) => new ReplayView() { DataContext = CurrentEvent.Replay },
            nameof(lbi_Ranking) => new RankingView() { DataContext = CurrentEvent.Ranking },
            _ => throw new NotImplementedException("View not implemented"),
        };

        view.Name = view.GetType().Name;
        SelectedView = view;

        if (SelectedView is EventsView eventView)
        {
            eventView.GameParameter = GameParameter;

            int idx = GameParameter.Events.IndexOf(CurrentEvent);
            eventView.DataContext = CurrentEvent;
            eventView.SelectEventAndPopulate(idx);
            eventView.ToggleEventControls(true);
            eventView.PopulateEventControls();
        }
        else if (SelectedView is FolderView folderView)
        {
            folderView.Folder = Folder;
            folderView.GameParameter = GameParameter;
            folderView.Populate();
        }
    }

    public void SetQuickPickerIndex(int index)
    {
        _processEventSwitch = false;
        cb_QuickEventPicker.SelectedIndex = index;
        _processEventSwitch = true;
    }

    public void AddQuickPickerEntry(string str)
    {
        _processEventSwitch = false;
        cb_QuickEventPicker.Items.Add(str);
        _processEventSwitch = true;
    }

    public void RenameQuickPickerEntry(int index, string name)
    {
        _processEventSwitch = false;
        cb_QuickEventPicker.Items[index] = name;
        _processEventSwitch = true;
    }

    public void RemoveQuickPickerEntry(int index)
    {
        _processEventSwitch = false;
        cb_QuickEventPicker.Items.RemoveAt(index);
        _processEventSwitch = true;
    }

    public void ClearQuickPicker()
    {
        _processEventSwitch = false;
        cb_QuickEventPicker.Items.Clear();
        _processEventSwitch = true;
    }

    private void HandleNewEventList(string fileName)
    {
        GameParameter = ReadGameParameterXMLFile(fileName);
        Folder = new EventListFolder();
        Folder.Id = (int)GameParameter.FolderId;

        PopulateQuickPickerWithNewEventList();

        SelectedView = new FolderView(GameParameter, Folder);

        if (GameParameter.Events.Count > 0)
            ReselectEvent(0);
    }

    private void HandleSeasonalGameParameterList(string fileName)
    {
        var list = ReadGameParameterListXML(fileName);
        GameParameter = list[0];

        // Merge game parameters into one event each
        for (int i = 1; i < list.Count; i++)
        {
            GameParameter gp = list[i];

            if (gp.Events.Any())
                GameParameter.Events.Add(gp.Events[0]);
        }

        Folder = new EventListFolder();
        Folder.Id = (int)GameParameter.FolderId;

        PopulateQuickPickerWithNewEventList();

        SelectedView = new FolderView(GameParameter, Folder);

        if (GameParameter.Events.Count > 0)
            ReselectEvent(0);
    }

    private void ReselectEvent(int index)
    {
        if (index == -1)
            CurrentEvent = null;
        else
            CurrentEvent = GameParameter.Events[index];

        SetQuickPickerIndex(index);

        ToggleContextualWindowControls(GameParameter.Events.Count > 0);

        if (SelectedView is EventsView eventView)
        {
            eventView.GameParameter = GameParameter;
            eventView.DataContext = index != -1 ? GameParameter.Events[index] : null;
            eventView.SelectEventAndPopulate(index);
            eventView.ToggleEventControls(true);
            eventView.PopulateEventControls();
        }
        else if (SelectedView is FolderView folderView)
        {
            folderView.Folder = Folder;
            folderView.GameParameter = GameParameter;
            folderView.Populate();
        }
        else
        {
            SelectedView.DataContext = SelectedView.Name switch
            {
                nameof(InformationView) => CurrentEvent.Information,
                nameof(PlayStyleView) => CurrentEvent.PlayStyle,
                nameof(RaceParameterView) => CurrentEvent.RaceParameter,
                nameof(RegulationView) => CurrentEvent.Regulation,
                nameof(EntrySetView) => CurrentEvent.EntrySet,
                nameof(ConstraintView) => CurrentEvent.Constraint,
                nameof(TrackView) => CurrentEvent.Track,
                nameof(RewardView) => DataContext = CurrentEvent.Reward,
                nameof(EvalConditionView) => CurrentEvent.EvalCondition,
                nameof(FailConditionView) => CurrentEvent.FailureCondition,
                nameof(LicenseConditionView) => CurrentEvent.LicenseCondition,
                nameof(ArcadeStyleView) => CurrentEvent.ArcadeSetting,
                nameof(ReplayView) => CurrentEvent.Replay,
                nameof(RankingView) => CurrentEvent.Ranking,
                _ => throw new NotImplementedException("View not implemented"),
            };
        }

        UpdateDiscordPresence();
    }
    #endregion
}
