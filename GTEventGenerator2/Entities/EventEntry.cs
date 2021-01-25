using System;
using System.Xml;
using System.ComponentModel;

using GTEventGenerator.Entities;
using GTEventGenerator.Database;
using PDTools.Utils;

namespace GTEventGenerator
{
    public class EventEntry
    {
        public bool IsAI { get; set; }
        public bool IsPresentEntry { get; set; }

        public string DriverName { get; set; } = "Unnamed";
        public string DriverRegion { get; set; } = "PDI";

        #region Skill Properties
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

        private short _brakingSkill = 80;
        public short BrakingSkill
        {
            get => _brakingSkill;
            set
            {
                if (value <= 200 && value >= 0)
                    _brakingSkill = value;
            }
        }

        private short _corneringSKill = 80;
        public short CorneringSkill
        {
            get => _corneringSKill;
            set
            {
                if (value <= 200 && value >= 0)
                    _corneringSKill = value;
            }
        }

        private sbyte _accelSkill = 80;
        public sbyte AccelSkill
        {
            get => _accelSkill;
            set
            {
                if (value <= 100 && value >= 0)
                    _accelSkill = value;
            }
        }

        private sbyte _startSkill = 80;
        public sbyte StartingSkill
        {
            get => _startSkill;
            set
            {
                if (value <= 100 && value >= 0)
                    _startSkill = value;
            }
        }

        private sbyte _roughness = -1;
        public sbyte Roughness
        {
            get => _roughness;
            set
            {
                if (value <= 10 && value >= -1)
                    _roughness = value;
            }
        }
        #endregion

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

        private int _initialVelocity = -1;
        public int InitialVelocity
        {
            get => _initialVelocity;
            set
            {
                if (value <= 1000 && value >= -1)
                    _initialVelocity = value;
            }
        }

        private float _initialVCoord;
        public float InitialVCoord
        {
            get => _initialVCoord;
            set
            {
                if (value <= 99999 && value >= -1)
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

        public byte BallastWeight { get; set; }
        public sbyte BallastPosition { get; set; } = -1;
        public sbyte DownforceRear { get; set; } = -1;
        public sbyte DownforceFront { get; set; } = -1;

        public short BodyPaintID { get; set; } = -1;
        public short WheelPaintID { get; set; } = -1;
        public int WheelID { get; set; } = -1;
        public int WheelInchUp { get; set; } = -1;
        public int AeroKit { get; set; } = -1;
        public int FlatFloor { get; set; } = -1;
        public int AeroOther { get; set; } = -1;

        #endregion

        public string CarLabel { get; set; }
        public string ActualCarName { get; set; }
        public StartType StartType { get; set; } = StartType.NONE;

        public byte RaceClassID { get; set; }

        public short ColorIndex { get; set; }
        public TireType TireFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireRear { get; set; } = TireType.NONE_SPECIFIED;

        public EngineNATuneState EngineStage { get; set; } = EngineNATuneState.NONE;
        public EngineTurboKit TurboKit { get; set; } = EngineTurboKit.NONE;
        public EngineComputer Computer { get; set; } = EngineComputer.NONE;
        public Muffler Exhaust { get; set; } = Muffler.UNSPECIFIED;
        public Suspension Suspension { get; set; } = Suspension.UNSPECIFIED;
        public Transmission Transmission { get; set; } = Transmission.UNSPECIFIED;

        public void WriteToXml(XmlWriter xml, bool isFixed)
        {
            if (isFixed)
                xml.WriteStartElement("entry");
            else
                xml.WriteStartElement("entry_base");

            if (!string.IsNullOrEmpty(CarLabel))
            {
                xml.WriteStartElement("car");
                xml.WriteAttributeString("color", ColorIndex.ToString());
                xml.WriteAttributeString("label", CarLabel);
                xml.WriteEndElement();
            }

            if (!IsPresentEntry)
            {
                if (isFixed)
                {
                    xml.WriteElementFloat("initial_position", InitialVCoord);
                    xml.WriteElementInt("initial_velocity", InitialVelocity);
                    xml.WriteElementInt("delay", Delay);
                    if (StartType != StartType.NONE)
                        xml.WriteElementValue("start_type", StartType.ToString());
                }

                xml.WriteElementValue("driver_name", IsAI ? DriverName : "Player");
                xml.WriteElementInt("player_no", IsAI ? -1 : 0);

                if (IsAI)
                    xml.WriteElementValue("driver_region", DriverRegion);

                xml.WriteElementInt("race_class_id", RaceClassID);

                if (IsAI)
                {
                    xml.WriteElementInt("ai_skill", BaseSkill);
                    xml.WriteElementInt("ai_skill_accelerating", AccelSkill);
                    xml.WriteElementInt("ai_skill_breaking", BrakingSkill);
                    xml.WriteElementInt("ai_skill_cornering", CorneringSkill);
                    xml.WriteElementInt("ai_skill_starting", StartingSkill);
                    xml.WriteElementInt("ai_roughness", Roughness);
                }
            }

            // Fixed entries can have a child entry_base.
            if (isFixed)
                xml.WriteStartElement("entry_base");

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

            if (isFixed)
                xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void WriteEntryBaseToBuffer(ref BitStream bs, GameDB db)
        {
            bs.WriteUInt32(0xE6_EB_45_F8);
            bs.WriteUInt32(1_06);

            // Write car (carthin)
            int carCode = !string.IsNullOrEmpty(CarLabel) ? db.GetCarCodeByLabel(CarLabel) : 0;
            bs.WriteInt32(carCode);
            bs.WriteInt16(ColorIndex);
            bs.WriteInt16(0);
            bs.WriteInt32(0);

            bs.WriteNullStringAligned4(DriverName);
            bs.WriteNullStringAligned4(DriverRegion);
            bs.WriteByte(RaceClassID);
            bs.WriteByte(0); // Proxy Driver Model

            // boost_rate count, ignore with empty list
            bs.WriteInt32(0);

            bs.WriteInt16(BrakingSkill);
            bs.WriteInt16(CorneringSkill);
            bs.WriteSByte(AccelSkill);
            bs.WriteSByte(StartingSkill);
            bs.WriteSByte(Roughness);

            bs.WriteSByte((sbyte)EngineStage);
            bs.WriteSByte((sbyte)TurboKit);
            bs.WriteSByte((sbyte)Computer);
            bs.WriteSByte((sbyte)Exhaust);
            bs.WriteSByte((sbyte)Suspension);
            bs.WriteInt16((sbyte)WheelID);
            bs.WriteInt16((sbyte)WheelPaintID);
            bs.WriteInt16((sbyte)WheelInchUp);
            bs.WriteSByte((sbyte)TireFront);
            bs.WriteSByte((sbyte)TireRear);

            // Aero stuff
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);

            bs.WriteByte((byte)PowerLimiter);
            bs.WriteSByte(DownforceFront);
            bs.WriteSByte(DownforceRear);
            bs.WriteInt16(BodyPaintID);
            bs.WriteInt16(-1); // Unk
            bs.WriteInt16(-1); // decken_number

            bs.WriteInt16(-1); // head/body codes
            bs.WriteInt16(-1);
            bs.WriteInt16(-1);
            bs.WriteInt16(-1);

            bs.WriteByte(0); // ai_reaction
            bs.WriteByte(BallastWeight);
            bs.WriteSByte(BallastPosition);
            bs.WriteSByte(-1); // Decken Type
            bs.WriteSByte(-1); // Decken Custom ID
            bs.WriteSByte(-1); // Decken Custom Type
        }
    }

    public enum EngineNATuneState // PARTS_NATUNE
    {
        [Description("Default")]
        NONE = -1,

        [Description("Stage 1")]
        LEVEL1 = 1,

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
        NONE = -1,

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
        NONE = -1,

        [Description("Sports Computer")]
        LEVEL1 = 1,
        LEVEL2,
    }

    public enum Muffler // PARTS_MUFFLER
    {
        [Description("Unspecified")]
        UNSPECIFIED = -1,

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
        [Description("Unspecified")]
        UNSPECIFIED = -1,

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
        [Description("Unspecified")]
        UNSPECIFIED = -1,

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
