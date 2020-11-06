using System;
using System.Xml;
using System.ComponentModel;

using GTEventGenerator.Entities;

namespace GTEventGenerator
{
    public class RaceEntry
    {
        public bool IsAI { get; set; } = false;

        public string DriverName { get; set; } = "Unnamed";
        public string DriverRegion { get; set; } = "PDI";

        private int _baseSkill = 80;
        public int BaseSkill
        {
            get => _baseSkill; 
            set
            {
                if (value <= 200 && value >= 0)
                    _baseSkill = value;
            }
        }

        private int _brakingSkill = 80;
        public int BrakingSkill
        {
            get => _brakingSkill;
            set
            {
                if (value <= 200 && value >= 0)
                    _brakingSkill = value;
            }
        }

        private int _corneringSKill = 80;
        public int CorneringSkill
        {
            get => _corneringSKill;
            set
            {
                if (value <= 200 && value >= 0)
                    _corneringSKill = value;
            }
        }

        private int _accelSkill = 80;
        public int AccelSkill
        {
            get => _accelSkill;
            set
            {
                if (value <= 200 && value >= 0)
                    _accelSkill = value;
            }
        }

        private int _startSkill = 80;
        public int StartingSkill
        {
            get => _startSkill;
            set
            {
                if (value <= 200 && value >= 0)
                    _startSkill = value;
            }
        }

        private int _roughness = -1;
        public int Roughness
        {
            get => _roughness;
            set
            {
                if (value <= 10 && value >= -1)
                    _roughness = value;
            }
        }

        private int _delay;
        public int Delay
        {
            get => _delay;
            set
            {
                if (value <= 3_600_000 && value >= -1)
                    _delay = value;
            }
        }
        public int raceBucket { get; set; }

        private int _initialVelocity = -1;
        public int InitialVelocity
        {
            get => _initialVelocity;
            set
            {
                if (value <= 200 && value >= -1)
                    _initialVelocity = value;
            }
        }

        private float _initialVCoord;
        public float InitialVCoord
        {
            get => _initialVCoord;
            set
            {
                if (value <= 200)
                    _initialVCoord = value;
            }
        }

        #region Car Setting Related Properties
        private int _maxGearSpeed;
        public int MaxGearSpeed
        {
            get => _maxGearSpeed;
            set
            {
                if (value < 0)
                    value = 0;
                _maxGearSpeed = value;
            }
        }

        private float _powerLimiter;
        public float PowerLimiter
        {
            get => _powerLimiter;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;
                _powerLimiter = (float)Math.Round(value, 1);
            }
        }

        private int _ballastWeight;
        public int BallastWeight
        {
            get => _ballastWeight;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1000)
                    value = 1000;
                _ballastWeight = value;
            }
        }

        private int _ballastPosition;
        public int BallastPosition
        {
            get => _ballastPosition;
            set
            {
                if (value < -1000)
                    value = 0;
                else if (value > 1000)
                    value = 1000;
                _ballastPosition = value;
            }
        }

        private int _downforceRear;
        public int DownforceRear
        {
            get => _downforceRear;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1000)
                    value = 1000;
                _downforceRear = value;
            }
        }

        private int _downforceFront;
        public int DownforceFront
        {
            get => _downforceFront;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1000)
                    value = 1000;
                _downforceFront = value;
            }
        }

        public int BodyPaintID { get; set; }
        public int WheelPaintID { get; set; }
        public int WheelID { get; set; }
        public int WheelInchUp { get; set; }
        public int AeroKit { get; set; }
        public int FlatFloor { get; set; }
        public int AeroOther { get; set; }

        #endregion
        public string CarLabel { get; set; }
        public string ActualCarName { get; set; }

        public int ColorIndex { get; set; }
        public TireType TireFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireRear { get; set; } = TireType.NONE_SPECIFIED;

        public EngineNATuneState EngineStage { get; set; }
        public EngineTurboKit TurboKit { get; set; }
        public EngineComputer Computer { get; set; }
        public Muffler Exhaust { get; set; }
        public Suspension Suspension { get; set; }
        public Transmission Transmission { get; set; }

        public RaceEntry()
        {
            this.Delay = 0;
            this.raceBucket = 0;
            this.InitialVelocity = 0;
            this.InitialVCoord = 0;
        }

        public void WriteToXml(XmlWriter xml, bool isFixed)
        {
            if (isFixed)
                xml.WriteStartElement("entry");
            else
                xml.WriteStartElement("entry_base");

            xml.WriteElementFloat("initial_position", InitialVCoord);
            xml.WriteElementInt("initial_velocity", InitialVelocity);

            xml.WriteElementValue("driver_name", IsAI ? DriverName : "Player");
            xml.WriteElementInt("player_no", IsAI ? -1 : 0);

            if (IsAI)
                xml.WriteElementValue("driver_region", DriverRegion);

            xml.WriteElementInt("delay", Delay);
            xml.WriteStartElement("car");
            xml.WriteAttributeString("color", ColorIndex.ToString());
            xml.WriteAttributeString("label", CarLabel);
            xml.WriteEndElement();
            xml.WriteElementInt("race_class_id", 0);

            if (IsAI)
            {
                xml.WriteElementInt("ai_skill", BaseSkill);
                xml.WriteElementInt("ai_skill_accelerating", AccelSkill);
                xml.WriteElementInt("ai_skill_breaking", BrakingSkill);
                xml.WriteElementInt("ai_skill_cornering", CorneringSkill);
                xml.WriteElementInt("ai_skill_starting", StartingSkill);
                xml.WriteElementInt("ai_roughness", Roughness);
            }

            if (EngineStage != EngineNATuneState.NONE)
                xml.WriteElementValue("engine_na_tune_stage", EngineStage.ToString());
            if (TurboKit != EngineTurboKit.NONE)
                xml.WriteElementValue("engine_turbo_kit", TurboKit.ToString());
            if (Computer != EngineComputer.NONE)
                xml.WriteElementValue("engine_computer", Computer.ToString());
            if (Exhaust != Muffler.NONE)
                xml.WriteElementValue("muffler", Exhaust.ToString());
            if (Suspension != Suspension.NORMAL)
                xml.WriteElementValue("suspension", Suspension.ToString());
            if (Transmission != Transmission.NORMAL)
                xml.WriteElementValue("transmission", Transmission.ToString());

            if (PowerLimiter != 0)
                xml.WriteElementInt("power_limiter", (int)(PowerLimiter * 10));

            if (MaxGearSpeed != 0)
                xml.WriteElementInt("gear_max_speed", MaxGearSpeed);

            if (BodyPaintID != 0)
                xml.WriteElementInt("paint_id", BodyPaintID);

            if (WheelID != 0)
                xml.WriteElementInt("wheel", WheelID);

            if (WheelPaintID != 0)
                xml.WriteElementInt("wheel_color", WheelPaintID);

            if (WheelInchUp != 0)
                xml.WriteElementInt("wheel_inch_up", WheelInchUp);

            if (BallastWeight != 0)
                xml.WriteElementInt("ballast_weight", BallastWeight);

            if (BallastPosition != 0)
                xml.WriteElementInt("ballast_position", BallastPosition);

            if (DownforceFront != 0)
                xml.WriteElementInt("downforce_f", DownforceFront);

            if (DownforceRear != 0)
                xml.WriteElementInt("downforce_r", DownforceRear);

            if (AeroKit != 0)
                xml.WriteElementInt("aero_1", AeroKit);
            if (FlatFloor != 0)
                xml.WriteElementInt("aero_2", FlatFloor);
            if (AeroOther != 0)
                xml.WriteElementInt("aero_3", AeroOther);

            if (TireFront != TireType.NONE_SPECIFIED)
                xml.WriteElementValue("tire_f", TireFront.ToString());
            if (TireRear != TireType.NONE_SPECIFIED)
                xml.WriteElementValue("tire_r", TireRear.ToString());

            xml.WriteEndElement();
        }
    }

    public enum EngineNATuneState // PARTS_NATUNE
    {
        [Description("Default")]
        NONE,

        [Description("Stage 1")]
        LEVEL1,

        [Description("Stage 2")]
        LEVEL2,

        [Description("Stage 3")]
        LEVEL3,

        [Description("Stage 4 (Normally Unavailable)")]
        LEVEL4,

        [Description("Stage 5 (Normally Unavailable)")]
        LEVEL5,
    }

    public enum EngineTurboKit // PARTS_TURBINEKIT
    {
        [Description("Default")]
        NONE,

        [Description("NO (?)")]
        NO,

        [Description("Low RPM Range Turbo Kit")]
        LEVEL1,

        [Description("Mid RPM Range Turbo Kit")]
        LEVEL2,

        [Description("High RPM Range Turbo Kit")]
        LEVEL3,

        [Description("Super RPM Range Turbo Kit (Normally Unavailable)")]
        LEVEL4,

        [Description("Ultra RPM Range Turbo Kit (Normally Unavailable)")]
        LEVEL5,
    }

    public enum EngineComputer // PARTS_COMPUTER
    {
        [Description("Default")]
        NONE,

        [Description("Sports Computer")]
        LEVEL1,
        LEVEL2,
    }

    public enum Muffler // PARTS_MUFFLER
    {
        [Description("Default")]
        NONE,

        [Description("Sports Exhaust")]
        SPORTS,

        [Description("Semi-Racing Exhaust")]
        SEMIRACING,

        [Description("Racing Exhaust")]
        RACING,
    }

    public enum Suspension // PARTS_SUSPENSION
    {
        [Description("Default")]
        NORMAL,

        [Description("Racing Suspension: Soft")]
        SPORTS1,

        [Description("Racing Suspension: Hard")]
        SPORTS2,

        [Description("Suspension: Rally")]
        SPORTS3,

        [Description("Height-Adjustable, Fully Customisable Suspension")]
        RACING,

        [Description("Full Active (?)")]
        FULL_ACTIVE,
    }

    public enum Transmission // PARTS_GEAR
    {
        [Description("Default")]
        NORMAL,

        [Description("Five-Speed Transmission")]
        CLOSE,

        [Description("Six-Speed Transmission")]
        SUPER_CLOSE,

        [Description("Fully Customisable Transmission")]
        VARIABLE,
    }
}
