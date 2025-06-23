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
using System.Windows.Shapes;

using PDTools.Enums.PS3;
using PDTools.Enums;
using PDTools.Structures.MGameParameter;

using Humanizer;
namespace GTEventMaker
{
    /// <summary>
    /// Interaction logic for EventEntryBaseEditWindow.xaml
    /// </summary>
    public partial class EventEntryBaseEditWindow : Window
    {
        private EntryBase _entryBase { get; set; }
        public EventEntryBaseEditWindow(EntryBase entry)
        {
            _entryBase = entry;

            InitializeComponent();

            var tires = (PARTS_TIRE[])Enum.GetValues(typeof(PARTS_TIRE));
            for (int i = 0; i < tires.Length - 1; i++) // - 1 as the combo boxes have a default "none" entry
            {
                var tire = (PARTS_TIRE)i;
                string tireName = tire.Humanize();
                cb_EntryTireF.Items.Add(tireName);
                cb_EntryTireR.Items.Add(tireName);
            }

            var tuneStage = ((PARTS_NATUNE[])Enum.GetValues(typeof(PARTS_NATUNE)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < tuneStage.Length; i++)
            {
                string tName = tuneStage[i].Humanize();
                cb_EngineStage.Items.Add(tName);
            }

            var turbos = ((PARTS_TURBINEKIT[])Enum.GetValues(typeof(PARTS_TURBINEKIT)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < turbos.Length; i++)
            {
                string tName = turbos[i].Humanize();
                cb_Turbo.Items.Add(tName);
            }

            var computers = ((PARTS_COMPUTER[])Enum.GetValues(typeof(PARTS_COMPUTER)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < computers.Length; i++)
            {
                string tName = computers[i].Humanize();
                cb_Computer.Items.Add(tName);
            }

            var suspensions = ((PARTS_SUSPENSION[])Enum.GetValues(typeof(PARTS_SUSPENSION)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < suspensions.Length; i++)
            {
                string tName = suspensions[i].Humanize();
                cb_Suspension.Items.Add(tName);
            }

            var transmissions = ((PARTS_GEAR[])Enum.GetValues(typeof(PARTS_GEAR)))
                                .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < transmissions.Length; i++)
            {
                string tName = transmissions[i].Humanize();
                cb_Transmission.Items.Add(tName);
            }

            var exhausts = ((PARTS_MUFFLER[])Enum.GetValues(typeof(PARTS_MUFFLER)))
                               .OrderBy(e => (int)e).ToArray();
            for (int i = 0; i < exhausts.Length; i++)
            {
                string tName = exhausts[i].Humanize();
                cb_Exhaust.Items.Add(tName);
            }

            cb_EngineStage.SelectedIndex = (int)_entryBase.EngineNaTuneStage + 1;
            cb_Turbo.SelectedIndex = (int)_entryBase.EngineTurboKit + 1;
            cb_Computer.SelectedIndex = (int)_entryBase.EngineComputer + 1;
            cb_Suspension.SelectedIndex = (int)_entryBase.Suspension + 1;
            cb_Transmission.SelectedIndex = (int)_entryBase.Transmission + 1;
            cb_Exhaust.SelectedIndex = (int)_entryBase.Muffler + 1;

            cb_EntryTireF.SelectedIndex = (int)_entryBase.TireFront + 1;
            cb_EntryTireR.SelectedIndex = (int)_entryBase.TireRear + 1;

            iud_CorneringSkill.Value = _entryBase.AICorneringSkill;
            iud_BrakingSkill.Value = _entryBase.AIBrakingSkill;
            iud_AccelSkill.Value = _entryBase.AIAcceleratingSkill;
            iud_StartingSkill.Value = _entryBase.AIStartingSkill;
            iud_Roughness.Value = _entryBase.AIRoughness;
            iud_Reaction.Value = _entryBase.AIReaction;
            tb_DriverName.Text = _entryBase.DriverName;
            tb_DriverCountry.Text = _entryBase.DriverRegion;

            iud_GearMaxSpeed.Value = _entryBase.GearMaxSpeed;
            iud_DownforceFront.Value = _entryBase.DownforceFront;
            iud_BallastWeight.Value = _entryBase.BallastWeight;
            iud_BodyPaintID.Value = _entryBase.BodyColorCode;
            iud_WheelID.Value = _entryBase.WheelID;
            iud_Aero1.Value = _entryBase.Aero1_AeroKit;
            iud_Aero2.Value = _entryBase.Aero2_FlatFloor;
            iud_Aero3.Value = _entryBase.Aero3_AeroOther;
            iud_PowerLimiter.Value = _entryBase.PowerLimiter;
            iud_DownforceRear.Value = _entryBase.DownforceRear;
            slider_BallastPosition.Value = _entryBase.BallastPosition;
            iud_WheelPaintID.Value = _entryBase.WheelColor;
            iud_WheelInch.Value = _entryBase.WheelInchUp;
            iud_StickerNumber.Value = _entryBase.DeckenNumber;
            iud_RaceClassID.Value = _entryBase.RaceClassID;
            iud_ProxyDriverModel.Value = _entryBase.ProxyDriverModel;
        }

        private void cb_EntryTireF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entryBase.TireFront = (TireType)cb_EntryTireF.SelectedIndex - 1;
        }

        private void cb_EntryTireR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entryBase.TireRear = (TireType)cb_EntryTireR.SelectedIndex - 1;
        }

        private void cb_EngineStage_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entryBase.EngineNaTuneStage = (PARTS_NATUNE)(cb_EngineStage.SelectedIndex - 1);

        private void cb_Turbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entryBase.EngineTurboKit = (PARTS_TURBINEKIT)(cb_Turbo.SelectedIndex - 1);

        private void cb_Computer_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entryBase.EngineComputer = (PARTS_COMPUTER)(cb_Computer.SelectedIndex - 1);

        private void cb_Suspension_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entryBase.Suspension = (PARTS_SUSPENSION)(cb_Suspension.SelectedIndex - 1);

        private void cb_Transmission_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entryBase.Transmission = (PARTS_GEAR)(cb_Transmission.SelectedIndex - 1);

        private void cb_Exhaust_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _entryBase.Muffler = (PARTS_MUFFLER)(cb_Exhaust.SelectedIndex - 1);

        private void iud_CorneringSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.AICorneringSkill = iud_CorneringSkill.Value.Value;
        }

        private void iud_BrakingSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.AIBrakingSkill = iud_BrakingSkill.Value.Value;
        }

        private void iud_AccelSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.AIAcceleratingSkill = iud_AccelSkill.Value.Value;
        }

        private void iud_StartingSkill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.AIStartingSkill = iud_StartingSkill.Value.Value;
        }

        private void iud_Roughness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.AIRoughness = iud_Roughness.Value.Value;
        }

        private void tb_DriverCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _entryBase.DriverRegion = tb_DriverCountry.Text;
        }

        private void tb_DriverName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _entryBase.DriverName = tb_DriverName.Text;
        }

        private void iud_GearMaxSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.GearMaxSpeed = iud_GearMaxSpeed.Value.Value;
        }

        private void iud_DownforceFront_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.DownforceFront = iud_DownforceFront.Value.Value;
        }

        private void iud_BallastWeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.BallastWeight = iud_BallastWeight.Value.Value;
        }

        private void iud_BodyPaintID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.BodyColorCode = iud_BodyPaintID.Value.Value;
        }

        private void iud_WheelID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.WheelID = iud_WheelID.Value.Value;
        }

        private void iud_Aero1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.Aero1_AeroKit = iud_Aero1.Value.Value;
        }

        private void iud_Aero2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.Aero2_FlatFloor = iud_Aero2.Value.Value;
        }

        private void iud_Aero3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.Aero3_AeroOther = iud_Aero3.Value.Value;
        }

        private void iud_PowerLimiter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.PowerLimiter = iud_PowerLimiter.Value.Value;
        }

        private void iud_DownforceRear_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.DownforceRear = iud_DownforceRear.Value.Value;
        }

        private void slider_BallastPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _entryBase.BallastPosition = (sbyte)slider_BallastPosition.Value;
        }

        private void iud_WheelPaintID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.WheelColor = iud_WheelPaintID.Value.Value;
        }

        private void iud_WheelInch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.WheelInchUp = iud_WheelInch.Value.Value;
        }

        private void iud_StickerNumber_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.DeckenNumber = iud_StickerNumber.Value.Value;
        }

        private void iud_RaceClassID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.RaceClassID = iud_RaceClassID.Value.Value;
        }

        private void iud_ProxyDriverModel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.ProxyDriverModel = iud_ProxyDriverModel.Value.Value;
        }

        private void iud_Reaction_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _entryBase.AIReaction = iud_Reaction.Value.Value;
        }
    }
}
