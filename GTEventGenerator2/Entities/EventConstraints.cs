using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

namespace GTEventGenerator.Entities
{
    public class EventConstraints
    {
        public bool NeedsPopulating { get; set; } = true;

        public bool ABSConstrained { get; set; }
        private bool? _absEnabled;
        public bool? ABSEnabled
        {
            get => ABSConstrained ? _absEnabled : null;
            set
            {
                _absEnabled = value;
                ABSConstrained = _absEnabled.HasValue;
            }
        }

        public bool ActiveSteeringConstrained { get; set; }
        private bool? _activeSteeringEnabled;
        public bool? ActiveSteeringEnabled
        {
            get => ActiveSteeringConstrained ? _activeSteeringEnabled : null;
            set
            {
                _activeSteeringEnabled = value;
                ActiveSteeringConstrained = _activeSteeringEnabled.HasValue;
            }
        }

        public bool ASMConstrained { get; set; }
        private bool? _asmEnabled;
        public bool? ASMEnabled
        {
            get => ASMConstrained ? _asmEnabled : null;
            set
            {
                _asmEnabled = value;
                ASMConstrained = _asmEnabled.HasValue;
            }
        }

        public bool SkidRecoveryForceConstrained { get; set; }
        private bool? _skidRecoveryForceEnabled;
        public bool? SkidRecoveryForceEnabled
        {
            get => SkidRecoveryForceConstrained ? _skidRecoveryForceEnabled : null;
            set
            {
                _skidRecoveryForceEnabled = value;
                SkidRecoveryForceConstrained = _skidRecoveryForceEnabled.HasValue;
            }
        }

        public bool DriftType { get; set; }

        public bool DrivingLineConstrained { get; set; }
        private bool? _drivingLineEnabled;
        public bool? DrivingLineEnabled
        {
            get => DrivingLineConstrained ? _drivingLineEnabled : null;
            set
            {
                _drivingLineEnabled = value;
                DrivingLineConstrained = _drivingLineEnabled.HasValue;
            }
        }

        public int EnemyTire { get; set; }
        public TireType FrontTireLimit { get; set; } = TireType.NONE_SPECIFIED;
        public TireType RearTireLimit { get; set; } = TireType.NONE_SPECIFIED;
        public TireType NeededFrontTire { get; set; } = TireType.NONE_SPECIFIED;
        public TireType NeededRearTire { get; set; } = TireType.NONE_SPECIFIED;
        public TireType SuggestedRearTire { get; set; } = TireType.NONE_SPECIFIED;
        public TireType SuggestedFrontTire { get; set; } = TireType.NONE_SPECIFIED;

        public bool TCSConstrained { get; set; }
        private bool? _tcsEnabled;
        public bool? TCSEnabled
        {
            get => TCSConstrained ? _tcsEnabled : null;
            set
            {
                _tcsEnabled = value;
                TCSConstrained = _tcsEnabled.HasValue;
            }
        }

        public bool TransmissionConstrained { get; set; }
        private bool? _transmissionEnabled;
        public bool? TransmissionEnabled
        {
            get => TransmissionConstrained ? _transmissionEnabled : null;
            set
            {
                _transmissionEnabled = value;
                TransmissionConstrained = _transmissionEnabled.HasValue;
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("constraint");
            xml.WriteElementBoolOrNull("abs", ABSEnabled);
            xml.WriteElementBoolOrNull("active_steering", ActiveSteeringEnabled);
            xml.WriteElementBoolOrNull("asm", ASMEnabled);
            xml.WriteElementInt("drift_type", -1);
            xml.WriteElementBoolOrNull("driving_line", DrivingLineEnabled);
            xml.WriteElementInt("enemy_tire", -1);
            xml.WriteElementEnumInt("limit_tire_f", FrontTireLimit);
            xml.WriteElementEnumInt("limit_tire_r", RearTireLimit);
            xml.WriteElementEnumInt("need_tire_f", NeededFrontTire);
            xml.WriteElementEnumInt("need_tire_r", NeededRearTire);
            xml.WriteElementBoolOrNull("simulation", !SkidRecoveryForceEnabled);
            xml.WriteElementEnumInt("suggest_tire_f", SuggestedFrontTire);
            xml.WriteElementEnumInt("suggest_tire_r", SuggestedRearTire);
            xml.WriteElementBoolOrNull("tcs", TCSEnabled);
            xml.WriteElementBoolOrNull("transmission", TransmissionEnabled);
            xml.WriteEndElement();
        }

        public void ParseRaceConstraints(XmlNode node)
        {
            foreach (XmlNode constraintNode in node.ChildNodes)
            {
                switch (constraintNode.Name)
                {
                    case "abs":
                        ABSEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "active_steering":
                        ActiveSteeringEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "asm":
                        ASMEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "drift_type":
                        DriftType = constraintNode.ReadValueBool();
                        break;
                    case "driving_line":
                        DrivingLineEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "enemy_tire":
                        EnemyTire = constraintNode.ReadValueInt();
                        break;
                    case "limit_tire_f":
                        FrontTireLimit = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "limit_tire_r":
                        RearTireLimit = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "need_tire_f":
                        NeededFrontTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "need_tire_r":
                        NeededRearTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "suggest_tire_f":
                        SuggestedFrontTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "suggest_tire_r":
                        SuggestedRearTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "tcs":
                        TCSEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "transmission":
                        TransmissionEnabled = constraintNode.ReadValueBoolNull();
                        break;

                }
            }
        }
    }

    public enum TireType
    {
        [Description("No restrictions")]
        NONE_SPECIFIED = -1,

        [Description("Comfort - Hard")]
        COMFORT_HARD,

        [Description("Comfort - Medium")]
        COMFORT_MEDIUM,

        [Description("Comfort - Soft")]
        COMFORT_SOFT,

        [Description("Sports - Hard")]
        SPORTS_HARD,

        [Description("Sports - Medium")]
        SPORTS_MEDIUM,

        [Description("Sports - Soft")]
        SPORTS_SOFT,

        [Description("Sports - Super Soft")]
        SPORTS_SUPER_SOFT,

        [Description("Racing - Hard")]
        RACING_HARD,

        [Description("Racing - Medium")]
        RACING_MEDIUM,

        [Description("Racing - Soft")]
        RACING_SOFT,

        [Description("Racing - Super Soft")]
        RACING_SUPER_SOFT,

        [Description("Racing - Rain Intermediate")]
        RACING_RAIN_INTERMEDIATE,

        [Description("Racing - Heavy Wet")]
        RACING_HEAVY_WET,

        [Description("Dirt Tyres")]
        DIRT,

        [Description("Spiked Snow Tyres")]
        SPIKED_SNOW,
    }
}
