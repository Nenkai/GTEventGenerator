using GTEventMaker.Database;
using GTEventMaker.Utils;

using Humanizer;

using PDTools.Enums;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using PDTools.Structures.MGameParameter;
using System.Runtime.Versioning;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for EventsView.xaml
/// </summary>
public partial class EventsView : UserControl
{
    public GameParameter GameParameter { get; set; }
    public Event CurrentEvent => DataContext as Event;

    private bool _processEventSwitch = true;
    private System.Drawing.Image _eventImage;

    public EventsView(GameParameter gp)
    {
        InitializeComponent();

        GameParameter = gp;

        lb_Events.ItemsSource = GameParameter.Events;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var i in (GameMode[])Enum.GetValues(typeof(GameMode)))
            cb_gameModes.Items.Add(i.Humanize());
        cb_gameModes.SelectedIndex = 0;

        foreach (var i in (EventType[])Enum.GetValues(typeof(EventType)))
            cb_EventType.Items.Add(i.Humanize());
        cb_EventType.SelectedIndex = 0;
    }

    private void lb_Events_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lb_Events.SelectedIndex != -1 && _processEventSwitch)
            SelectEventAndPopulate(lb_Events.SelectedIndex);
    }

    private void btn_AddEvent_Click(object sender, EventArgs e)
    {
        Event evnt = new Event();

        foreach (Language code in Enum.GetValues<Language>())
        {
            if (code == PDTools.Enums.Language.SYSTEM || code == PDTools.Enums.Language.SYSTEM)
                continue;

            evnt.Information.Title.Texts[code] = $"Event #{GameParameter.Events.Count + 1}";
        }

        evnt.EventID = GameParameter.Events.Count == 0 ? 1000 : GameParameter.Events.Last().EventID + 1;

        GameParameter.Events.Add(evnt);
        lb_Events.Items.Refresh();

        var mainWindow = Application.Current.MainWindow as GameMakerWindow;
        mainWindow.AddQuickPickerEntry($"{evnt.EventID} - {evnt.Information.Title.Texts[PDTools.Enums.Language.GB]}");

        SelectEventAndPopulate(GameParameter.Events.Count - 1);
    }

    private void btn_RemoveRace_Click(object sender, EventArgs e)
    {
        if (lb_Events.SelectedIndex == -1)
        {
            MessageBox.Show("No event selected.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        MessageBoxResult deletionResult = MessageBox.Show($"Are you sure you wish to delete the event?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (deletionResult == MessageBoxResult.Yes)
        {
            var mainWindow = Application.Current.MainWindow as GameMakerWindow;
            mainWindow.RemoveQuickPickerEntry(lb_Events.SelectedIndex);

            GameParameter.Events.Remove(CurrentEvent);
            lb_Events.Items.Refresh();

            SelectEventAndPopulate(GameParameter.Events.Count - 1);
        }
    }

    private void btn_CopyRace_Click(object sender, EventArgs e)
    {
        if (lb_Events.SelectedIndex == -1)
        {
            MessageBox.Show("No event selected.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var @event = new Event();
        CurrentEvent.CopyTo(@event);
        @event.EventID = GameParameter.Events.Max(e => e.EventID) + 1;
        GameParameter.Events.Add(@event);

        var mainWindow = Application.Current.MainWindow as GameMakerWindow;
        mainWindow.AddQuickPickerEntry($"{@event.EventID} - {@event.Information.Title.Texts[PDTools.Enums.Language.GB]}");

        lb_Events.Items.Refresh();
        SelectEventAndPopulate(GameParameter.Events.Count - 1);
    }

    [SupportedOSPlatform("windows")]
    private void btn_PickImage_Click(object sender, EventArgs e)
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

        try
        {
            _eventImage = System.Drawing.Image.FromFile(openImage.FileName);
        }
        catch
        {
            MessageBox.Show("Could not load the image.", "Image Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (_eventImage.Width > 1920 || _eventImage.Height > 1080)
        {
            MessageBox.Show("Image file is too big in size. Recommended: 432x244.", "Image Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        using (Graphics graphic = Graphics.FromImage(new Bitmap(_eventImage)))
        {
            _eventImage = ImageUtils.ResizeImage(_eventImage, new System.Drawing.Size(432, 244));
            //pctImagePreview.Source = new BitmapImage(new Uri(openImage.FileName));
        }
    }

    private void btn_SaveImage_Click(object sender, EventArgs e)
    {
        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (!File.Exists(System.IO.Path.Combine(path, "texconv.exe")))
        {
            MessageBox.Show("TexConv not found. Please download TexConv from https://github.com/microsoft/DirectXTex/releases and place it in the program folder.",
                "Save Image", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        else if (!File.Exists(System.IO.Path.Combine(path, "TXS3Converter.exe")))
        {
            MessageBox.Show("TexConv not found. Please download TexConv from https://github.com/microsoft/DirectXTex/releases and place it in the program folder.",
                "Save Image", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        
        if (_eventImage is null)
        {
            MessageBox.Show("No image selected, pick one first.", "No image chosen", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        

        /*
        string imageFileName = $"{GameParameter.FolderFileName}_{CurrentEvent.Index.ToString("00")}";
        string imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), imageFileName + ".png");

        _eventImage.Save(imageFilePath, ImageFormat.Png);

        Process p = new Process();
        p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "TXS3Converter.exe");
        p.StartInfo.Arguments = $"{Path.GetFileName(imageFilePath)} --DXT3";
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();

        string newPath = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "output", "piece", "gt6", "event_flyer")).FullName;

        string imgOutput = Path.Combine(Directory.GetCurrentDirectory(), $"{imageFileName}.img");
        string finalPath = Path.Combine(newPath, imageFileName + ".img");

        try
        {
            File.Move(imgOutput, finalPath);
        } 
        catch (Exception ex)
        {
            MessageBox.Show("Could not convert file.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), imageFileName + ".png"));

        MessageBox.Show($"Imaged saved as: \n'{finalPath}'.\n\n When packing, move the entire \"piece\" folder to your mod folder.", "Image saved", MessageBoxButton.OK, MessageBoxImage.Information);
        */
    }

    private void iud_EventID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (CurrentEvent is null) return;
        CurrentEvent.EventID = iud_EventID.Value.Value;

        if (lb_Events.SelectedIndex != -1)
        {
            var window = Application.Current.MainWindow as GameMakerWindow;
            window.RenameQuickPickerEntry(lb_Events.SelectedIndex, $"{CurrentEvent.EventID} - {CurrentEvent.Information.Title.Texts[PDTools.Enums.Language.GB]}");
            window.SetQuickPickerIndex(lb_Events.SelectedIndex);
        }
    }

    private void cb_gameModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CurrentEvent is null) return;

        CurrentEvent.GameMode = (GameMode)(sender as ComboBox).SelectedIndex;
    }

    private void checkBox_SeasonalEvent_Checked(object sender, RoutedEventArgs e)
    {
        CurrentEvent.IsSeasonalEvent = checkBox_SeasonalEvent.IsChecked.Value;
    }

    private void cb_EventType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CurrentEvent is null) return;
        CurrentEvent.EventType = (EventType)cb_EventType.SelectedIndex;
    }

    private void cb_Inheritance_Checked(object sender, RoutedEventArgs e)
    {
        CurrentEvent.Inheritance = cb_Inheritance.IsChecked.Value;
    }

    private void dt_EndDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        CurrentEvent.BeginDate = dt_StartDate.Value;
    }

    private void dt_StartDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        CurrentEvent.EndDate = dt_EndDate.Value;
    }

    public void PopulateEventControls()
    {
        if (CurrentEvent is null)
            return;

        iud_EventID.Value = CurrentEvent.EventID;
        cb_gameModes.SelectedIndex = (int)CurrentEvent.GameMode;
        cb_EventType.SelectedIndex = (int)CurrentEvent.EventType;
        checkBox_SeasonalEvent.IsChecked = CurrentEvent.IsSeasonalEvent;
        dt_StartDate.Value = CurrentEvent.BeginDate;
        dt_EndDate.Value = CurrentEvent.EndDate;
        cb_Inheritance.IsChecked = CurrentEvent.Inheritance;
    }

    /// <summary>
    /// Selects a displayed event from the list, but does not populate the controls.
    /// </summary>
    /// <param name="index"></param>
    public void SelectEvent(int index)
    {
        _processEventSwitch = false;
        lb_Events.SelectedIndex = index;
        _processEventSwitch = true;
    }

    /// <summary>
    /// Swaps to the specified event index and updates the event list & main window's quick switcher.
    /// </summary>
    /// <param name="index"></param>
    public void SelectEventAndPopulate(int index)
    {
        var window = Application.Current.MainWindow as GameMakerWindow;
        if (index != -1)
        {
            this.DataContext = GameParameter.Events[index];
            window.CurrentEvent = GameParameter.Events[index];
        }

        lb_Events.ItemsSource = GameParameter.Events;
        lb_Events.Items.Refresh();
        SelectEvent(index);

        window.SetQuickPickerIndex(index);
        window.ToggleContextualWindowControls(GameParameter.Events.Count > 0);
        ToggleEventControls(true);
        PopulateEventControls();

        window.UpdateDiscordPresence();
    }

    public void ToggleEventControls(bool isEnabled)
    {
        grp_EventInfo.IsEnabled = isEnabled;
        btnAddRace.IsEnabled = GameParameter?.Events.Count < 100;
        btnRemoveRace.IsEnabled = GameParameter?.Events?.Any() == true;
        btnCopyRace.IsEnabled = GameParameter?.Events?.Any() == true;
    }
}
