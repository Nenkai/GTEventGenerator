using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

using PDTools.Enums;
using PDTools.Structures.MGameParameter;
using PDTools.Utils;

namespace GTEventMaker.Views;

/// <summary>
/// Interaction logic for Information.xaml
/// </summary>
public partial class InformationView : UserControl
{
    public Information Information { get => this.DataContext as Information; } 

    public InformationView()
    {
        InitializeComponent();
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Refresh();
    }

    private void txtEventName_TextChanged(object sender, RoutedEventArgs e)
    {
        if (Information is null)
            return;

        Language country = (Language)cb_InfoLanguage.SelectedItem;
        Information.Title.SetText(country, txt_EventTitle.Text);
    }

    private void txt_Description_TextChanged(object sender, TextChangedEventArgs e)
    {
        Language country = (Language)cb_InfoLanguage.SelectedItem;
        Information.Description.SetText(country, txt_Description.Text);
    }

    private void txt_OneLineTitle_TextChanged(object sender, TextChangedEventArgs e)
    {
        Language country = (Language)cb_InfoLanguage.SelectedItem;
        Information.OneLineTitle.SetText(country, txt_OneLineTitle.Text);
    }

    private void txt_AdvancedNotice_TextChanged(object sender, TextChangedEventArgs e)
    {
        Language country = (Language)cb_InfoLanguage.SelectedItem;
        Information.AdvancedNotice.SetText(country, txt_AdvancedNotice.Text);
    }

    private void txt_RegistrationNotice_TextChanged(object sender, TextChangedEventArgs e)
    {
        Language country = (Language)cb_InfoLanguage.SelectedItem;
        Information.RegistrationNotice.SetText(country, txt_RegistrationNotice.Text);
    }


    private void btn_TitleApplyAllLocales_Click(object sender, RoutedEventArgs e)
    {
        foreach (var kv in Information.Title.Texts)
            Information.Title.SetText(kv.Key, txt_EventTitle.Text);
    }

    private void btn_OneLineTitleApplyAllLocales_Click(object sender, RoutedEventArgs e)
    {
        foreach (var kv in Information.OneLineTitle.Texts)
            Information.OneLineTitle.SetText(kv.Key, txt_OneLineTitle.Text);
    }

    private void btn_DescriptionApplyAllLocales_Click(object sender, RoutedEventArgs e)
    {
        foreach (var kv in Information.Description.Texts)
            Information.Description.SetText(kv.Key, txt_Description.Text);
    }

    private void btn_AdvancedNoticeApplyAllLocales_Click(object sender, RoutedEventArgs e)
    {
        foreach (var kv in Information.AdvancedNotice.Texts)
            Information.AdvancedNotice.SetText(kv.Key, txt_AdvancedNotice.Text);
    }

    private void btn_RegistrationNoticeApplyAllLocales_Click(object sender, RoutedEventArgs e)
    {
        foreach (var kv in Information.RegistrationNotice.Texts)
            Information.RegistrationNotice.SetText(kv.Key, txt_AdvancedNotice.Text);
    }

    private void cb_InfoLanguage_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (cb_InfoLanguage.SelectedIndex == -1)
            return;

        Refresh();
    }

    private void txt_LogoImagePath_TextChanged(object sender, TextChangedEventArgs e)
    {
        Information.LogoImagePath = txt_LogoImagePath.Text;
    }

    private void iud_LogoImageLayout_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Information.LogoImageLayout = iud_LogoImageLayout.Value ?? 0;
    }

    private void txt_LogoOtherInfo_TextChanged(object sender, TextChangedEventArgs e)
    {
        Information.LogoOtherInfo = txt_LogoImagePath.Text;
    }

    private void txt_FlierImagePath_TextChanged(object sender, TextChangedEventArgs e)
    {
        Information.FlierImagePath = txt_FlierImagePath.Text;

    }

    private void txt_FlierOtherInfo_TextChanged(object sender, TextChangedEventArgs e)
    {
        Information.FlierOtherInfo = txt_FlierOtherInfo.Text;
    }

    private void txt_RaceLabel_TextChanged(object sender, TextChangedEventArgs e)
    {
        Information.RaceLabel = txt_RaceLabel.Text;
    }

    private void iud_RaceInfoMinute_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Information.RaceInfoMinute = iud_RaceInfoMinute.Value ?? 0;
    }


    private void PopulateControls()
    {
        if (!cb_InfoLanguage.HasItems)
        {
            foreach (Language locale in Enum.GetValues(typeof(Language)))
            {
                if (locale == PDTools.Enums.Language.MAX)
                    break;

                cb_InfoLanguage.Items.Add(locale);
            }

            cb_InfoLanguage.SelectedIndex = (int)PDTools.Enums.Language.GB;
        }
    }

    public void Refresh()
    {
        PopulateControls();
        if (Information is null)
            return;

        Language country = (Language)cb_InfoLanguage.SelectedItem;
        txt_EventTitle.Text = Information.Title.Texts[country];
        txt_Description.Text = Information.Description.Texts[country];
        txt_OneLineTitle.Text = Information.OneLineTitle.Texts[country];
        txt_AdvancedNotice.Text = Information.AdvancedNotice.Texts[country];
        txt_RegistrationNotice.Text = Information.RegistrationNotice.Texts[country];

        txt_LogoImagePath.Text = Information.LogoImagePath;
        iud_LogoImageLayout.Value = Information.LogoImageLayout;
        txt_LogoOtherInfo.Text = Information.LogoOtherInfo;
        txt_FlierImagePath.Text = Information.FlierImagePath;
        txt_FlierOtherInfo.Text = Information.FlierOtherInfo;
        txt_RaceLabel.Text = Information.RaceLabel;
        iud_RaceInfoMinute.Value = Information.RaceInfoMinute;
    }
}
