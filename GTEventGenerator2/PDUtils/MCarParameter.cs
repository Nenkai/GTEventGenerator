using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using GTEventGenerator.PDUtils.Enums;

namespace GTEventGenerator.PDUtils
{
    public class MCarParameter
    {
        public static Dictionary<CarPartsType, int> PartsTableToPurchaseBit = new Dictionary<CarPartsType, int>
        {
            { CarPartsType.BRAKE, 1 },
            { CarPartsType.BRAKE_CONTROLLER, 6 },
            { CarPartsType.SUSPENSION, 8 },
            { CarPartsType.ASCC, 14 },
            { CarPartsType.TCSC, 16 },
            { CarPartsType.LIGHT_WEIGHT, 18 },
            { CarPartsType.DRIVETRAIN, 27 },
            { CarPartsType.GEAR, 30 },
            { CarPartsType.ENGINE, 0 },
            { CarPartsType.NATUNE, 34 },
            { CarPartsType.TURBINEKIT, 40 },
            { CarPartsType.DISPLACEMENT, 46 },
            { CarPartsType.COMPUTER, 50 },
            { CarPartsType.INTERCOOLER, 53 },
            { CarPartsType.MUFFLER, 58 },
            { CarPartsType.CLUTCH, 62 },
            { CarPartsType.FLYWHEEL, 66 },
            { CarPartsType.PROPELLERSHAFT, 70 },
            { CarPartsType.LSD, 72 },
            { CarPartsType.FRONT_TIRE, 79 },
            { CarPartsType.REAR_TIRE, 94 },
            { CarPartsType.SUPERCHARGER, 109 },
            { CarPartsType.INTAKE_MANIFOLD, 111 },
            { CarPartsType.EXHAUST_MANIFOLD, 113 },
            { CarPartsType.CATALYST, 115 },
            { CarPartsType.AIR_CLEANER, 118 },
            { CarPartsType.BOOST_CONTROLLER, 121 },
            { CarPartsType.INDEP_THROTTLE, 123 },
            { CarPartsType.LIGHT_WEIGHT_WINDOW, 125 },
            { CarPartsType.BONNET, 127 },
            { CarPartsType.AERO, 130 },
            { CarPartsType.FLAT_FLOOR, 134 },
            { CarPartsType.FREEDOM, 136 },
            { CarPartsType.WING, 140 },
            { CarPartsType.STIFFNESS, 145 },
            { CarPartsType.NOS, 147 },
        };

        public const int Version = 109;

        private byte unk;
        MCarCondition Condition { get; set; } = new MCarCondition();

        public PDIDATETIME32 ObtainDate { get; set; }
        public short WinCount { get; set; }
        private short unk1;
        private short unk2;
        private short unk3;
        private uint _empty_;
        private ushort _empty2_;
        private short unk4;
        public ushort DealerColor;

        public MCarSettings Settings { get; set; } = new MCarSettings();

        public bool IsHavingParts(CarPartsType table, int partIndex)
        {
            int bit = PartsTableToPurchaseBit[table] + partIndex;
            return Settings.GetPurchasedPartFromBitIndex(bit);
        }

        public void SetOwnParts(CarPartsType table, int partIndex)
        {
            if (table == CarPartsType.FRONT_TIRE || table == CarPartsType.REAR_TIRE)
            {
                int ftBit = PartsTableToPurchaseBit[CarPartsType.FRONT_TIRE] + partIndex;
                if (ftBit != 0)
                    Settings.SetPurchasedPartFromBitIndex(ftBit);

                // Trick to also let it apply to rear tires
                table = CarPartsType.REAR_TIRE;
            }

            int bit = PartsTableToPurchaseBit[table] + partIndex;
            if (bit != 0)
                Settings.SetPurchasedPartFromBitIndex(bit);
        }

        // Custom
        public void RemovePurchasedParts(CarPartsType table, int partIndex)
        {
            if (table == CarPartsType.FRONT_TIRE || table == CarPartsType.REAR_TIRE)
            {
                int ftBit = PartsTableToPurchaseBit[CarPartsType.FRONT_TIRE] + partIndex;
                if (ftBit != 0)
                    Settings.RemovePurchasedPartFromBitIndex(ftBit);

                // Trick to also let it apply to rear tires
                table = CarPartsType.REAR_TIRE;
            }

            int bit = PartsTableToPurchaseBit[table] + partIndex;
            if (bit != 0)
                Settings.RemovePurchasedPartFromBitIndex(bit);
        }

        // Custom
        public void TogglePurchasedPart(CarPartsType table, int partIndex, bool purchased)
        {
            if (purchased)
                SetOwnParts(table, partIndex);
            else
                RemovePurchasedParts(table, partIndex);
        }

        public static MCarParameter ImportFromBlob(string fileName)
        {
            var car = new MCarParameter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                int version = bs.ReadInt32();
                if (version != Version)
                    throw new InvalidDataException("File is not an expected MCarParameter blob.");
                car.unk = bs.Read1Byte();
                bs.Position += 3;
                car.Condition.ParseCondition(bs);

                bs.ReadInt32();
                car.ObtainDate = new PDIDATETIME32();
                car.ObtainDate.SetRawData(bs.ReadUInt32());
                car.WinCount = bs.ReadInt16();

                car.unk1 = bs.ReadInt16();
                car.unk2 = bs.ReadInt16();
                car.unk3 = bs.ReadInt16();
                car._empty_ = bs.ReadUInt32();
                car._empty2_ = bs.ReadUInt16();
                car.unk4 = bs.ReadInt16();
                car.DealerColor = bs.ReadUInt16();

                car.Settings.ParseSettings(bs);
            }

            return car;
        }

        public static MCarParameter ImportFromBlob(byte[] blob)
        {
            var car = new MCarParameter();
            using (var fs = new MemoryStream(blob))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                int version = bs.ReadInt32();
                if (version != Version)
                    throw new InvalidDataException("Data is not an expected MCarParameter blob.");
                car.unk = bs.Read1Byte();
                bs.Position += 3;
                car.Condition.ParseCondition(bs);

                bs.ReadInt32();
                car.ObtainDate = new PDIDATETIME32();
                car.ObtainDate.SetRawData(bs.ReadUInt32());
                car.WinCount = bs.ReadInt16();

                car.unk1 = bs.ReadInt16();
                car.unk2 = bs.ReadInt16();
                car.unk3 = bs.ReadInt16();
                car._empty_ = bs.ReadUInt32();
                car._empty2_ = bs.ReadUInt16();
                car.unk4 = bs.ReadInt16();
                car.DealerColor = bs.ReadUInt16();

                car.Settings.ParseSettings(bs);
            }

            return car;
        }

        public byte[] ExportToBlob()
        {
            using (var ms = new MemoryStream(480))
            using (var bs = new BinaryStream(ms, ByteConverter.Big))
            {
                bs.WriteInt32(Version);
                bs.WriteByte(unk);
                bs.Position += 3;
                Condition.WriteCondition(bs);

                bs.WriteInt32(0);
                bs.WriteUInt32(ObtainDate.GetRawData());
                bs.WriteInt16(WinCount);

                bs.WriteInt16(unk1);
                bs.WriteInt16(unk2);
                bs.WriteInt16(unk3);
                bs.WriteUInt32(_empty_);
                bs.WriteUInt16(_empty2_);
                bs.WriteInt16(unk4);
                bs.WriteUInt16(DealerColor);

                Settings.WriteSettings(bs);

                return ms.ToArray();
            }
        }

        public void ExportToBlob(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                bs.WriteInt32(Version);
                bs.WriteByte(unk);
                bs.Position += 3;
                Condition.WriteCondition(bs);

                bs.WriteInt32(0);
                bs.WriteUInt32(ObtainDate.GetRawData());
                bs.WriteInt16(WinCount);

                bs.WriteInt16(unk1);
                bs.WriteInt16(unk2);
                bs.WriteInt16(unk3);
                bs.WriteUInt32(_empty_);
                bs.WriteUInt16(_empty2_);
                bs.WriteInt16(unk4);
                bs.WriteUInt16(DealerColor);

                Settings.WriteSettings(bs);
            }
        }
    }

    public class MCarCondition
    {
        public uint Odometer { get; set; }
        public int EngineLife { get; set; }
        public float OilLife { get; set; }
        public uint BodyLife { get; set; }
        public byte Dirtiness100 { get; set; }
        public byte RainX { get; set; }
        public byte BodyCoating { get; set; }
        private byte unk2;
        public int Everlasting { get; set; }
        private short unk3;
        private int _empty_;
        private short unk4;
        private short unk5;
        private float unk6;
        private short unk7;
        private byte[] _empty2_ = new byte[12];

        public void ParseCondition(BinaryStream bs)
        {
            Odometer = bs.ReadUInt32();
            EngineLife = bs.ReadInt32();
            OilLife = bs.ReadSingle();
            BodyLife = bs.ReadUInt32();
            Dirtiness100 = bs.Read1Byte();
            RainX = bs.Read1Byte();
            BodyCoating = bs.Read1Byte();
            unk2 = bs.Read1Byte();
            Everlasting = bs.ReadInt32();
            unk3 = bs.ReadInt16();
            _empty_ = bs.ReadInt32();
            unk4 = bs.ReadInt16();
            unk5 = bs.ReadInt16();
            unk6 = bs.ReadSingle();
            unk7 = bs.ReadInt16();
            bs.Read(_empty2_, 0, _empty2_.Length);
        }

        public void WriteCondition(BinaryStream bs)
        {
            bs.WriteUInt32(Odometer);
            bs.WriteInt32(EngineLife);
            bs.WriteSingle(OilLife);
            bs.WriteUInt32(BodyLife);
            bs.WriteByte(Dirtiness100);
            bs.WriteByte(RainX);
            bs.WriteByte(BodyCoating);
            bs.WriteByte(unk2);
            bs.WriteInt32(Everlasting);
            bs.WriteInt16(unk3);
            bs.WriteInt32(_empty_);
            bs.WriteInt16(unk4);
            bs.WriteInt16(unk5);
            bs.WriteSingle(unk6);
            bs.WriteInt16(unk7);
            bs.Write(_empty2_, 0, _empty2_.Length);
        }
    }

    public class MCarSettings
    {
        public short FrontWheelEx { get; set; }
        public short RearWheelEx { get; set; }
        private short WheelInchupRelated { get; set; }
        public int WheelSP { get; set; } = -1;
        public int CarCode { get; set; }
        public int GarageID { get; set; }
        public int FrontWheelCompound { get; set; } = -1;
        public int FrontTire { get; set; } = -1;
        public int RearTireCompound { get; set; } = -1;
        public int RearTire { get; set; } = -1;
        public int Brake { get; set; } = -1;
        public int Brakecontroller { get; set; } = -1;
        public int Chassis { get; set; } = -1;
        public int Engine { get; set; } = -1;
        public int DriveTrain { get; set; } = -1;
        public int Gear { get; set; } = -1;
        public int Suspension { get; set; } = -1;
        public int LSD { get; set; } = -1;
        private int Steer = -1;
        public int Lightweight { get; set; } = -1;
        public int Racingmodify { get; set; } = -1;
        public int Displacement { get; set; } = -1;
        public int Computer { get; set; } = -1;
        public int Natune { get; set; } = -1;
        public int TurbineKit { get; set; } = -1;
        public int Flywheel { get; set; } = -1;
        public int Clutch { get; set; } = -1;
        public int PropellerShaft { get; set; } = -1;
        public int Muffler { get; set; } = -1;
        public int Intercooler { get; set; } = -1;
        public int ASCC { get; set; } = -1;
        public int TCSC { get; set; } = -1;
        public int Supercharger { get; set; } = -1;
        public int IntakeManifold { get; set; } = -1;
        public int ExhaustManifold { get; set; } = -1;
        public int Catalyst { get; set; } = -1;
        public int AirCleaner { get; set; } = -1;
        public int NOS { get; set; } = -1;
        public int WindowReduction { get; set; } = -1;
        public int CarbonBonnet { get; set; } = -1;
        public int BodyKit { get; set; } = -1;
        private int FlatFloors = -1;
        public int Aero { get; set; } = -1;
        public int Wing { get; set; } = -1;
        public int[] UnkTables = new int[3];

        public short GearReverse { get; set; }
        public short Gear1st { get; set; }
        public short Gear2nd { get; set; }
        public short Gear3rd { get; set; }
        public short Gear4th { get; set; }
        public short Gear5th { get; set; }
        public short Gear6th { get; set; }
        public short Gear7th { get; set; }
        public short Gear8th { get; set; }
        public short Gear9th { get; set; }
        public short Gear10th { get; set; }
        public short Gear11st { get; set; }
        public short FinalGearRatio { get; set; }
        public byte MaxSpeed_10 { get; set; }
        public short GearLastFinal { get; set; }
        public byte Param4WD { get; set; }
        public byte FrontABS { get; set; }
        public byte RearABS { get; set; }
        public short DownforceFront { get; set; }
        public short DownforceRear { get; set; }
        public byte turbo_Boost1 { get; set; }
        public byte turbo_peakRpm1 { get; set; }
        public byte turbo_response1 { get; set; }
        public byte turbo_Boost2 { get; set; }
        public byte turbo_peakRpm2 { get; set; }
        public byte turbo_response2 { get; set; }

        public byte FrontCamber { get; set; }
        public byte RearCamber { get; set; }
        public short FrontRideHeight { get; set; }
        public short RearRideHeight { get; set; }
        public sbyte FrontToe { get; set; }
        public sbyte RearToe { get; set; }
        public short FrontSpringRate { get; set; }
        public short RearSpringRate { get; set; }
        public short LeverRatioF { get; set; }
        public short LevelRarioR { get; set; }
        public byte FrontDamperF1B { get; set; }
        public byte FrontDamperF2B { get; set; }
        public byte FrontDamperF1R { get; set; }
        public byte FrontDamperF2R { get; set; }
        public byte RearDamperF1B { get; set; }
        public byte RearDamperF2B { get; set; }
        public byte RearDamperF1R { get; set; }
        public byte RearDamperF2R { get; set; }
        public byte FrontStabilizer { get; set; }
        public byte RearStabilizer { get; set; }
        public byte FrontLSDParam { get; set; }
        public byte RearLSDParam { get; set; }
        public byte FrontLSDParam2 { get; set; }
        public byte RearLSDParam2 { get; set; }
        public byte FrontLSDParam3 { get; set; }
        public byte RearLSDParam3 { get; set; }
        public byte TCSC_UserValueDF { get; set; }
        public byte ASCC_VSCParamLevel { get; set; }
        public byte ASCC_VSCParam1DF { get; set; }
        public byte ASCC_VUCParamLevel { get; set; }
        public byte ASCC_VUCParam11DF { get; set; }
        public byte BallastWeight { get; set; }
        public sbyte BallastPosition { get; set; }
        public byte SteerLimit { get; set; }
        private short unk3;
        public short WeightModifyRatio { get; set; } = 100;
        public short PowerModifyRatio { get; set; }
        public byte NOSTorqueVolume { get; set; }
        public byte GripMultiplier { get; set; }
        public byte FrontBrakeBalanceLevel { get; set; }
        public byte RearBrakeBalanceLevel { get; set; } = 5;
        public byte ABSCorneringControlLevel { get; set; } = 1;
        private byte unk5;
        private short unk6;
        private short gasCapacity;
        public short PowerLimiter { get; set; } = 1000;

        public int HornSoundID { get; set; }
        private byte wheel_color;
        public short BodyPaintID { get; set; } = -1;
        public short WheelPaintID { get; set; } = -1;
        public short BrakePaintID { get; set; } = -1;
        public short CustomRearWingPaintID { get; set; } = -1;
        public short FrontWheelWidth { get; set; }
        public short FrontWheelDiameter { get; set; }
        public short RearWheelWidth { get; set; }
        public short RearWheelDiameter { get; set; }
        public byte WheelInchup { get; set; }
        public byte DeckenPreface { get; set; }
        public byte DeckenNumber { get; set; }
        public byte DeckenType { get; set; }
        private byte[] unk7 = new byte[21];

        private byte unkCustomWing1;
        private byte unkCustomWing2;
        private byte unkCustomWing3;
        private byte customWingsStays;
        private byte unkCustomWing4;
        public short WingWidthOffset { get; set; }
        public short WingHeightOffset { get; set; }
        public short WingAngleOffset { get; set; }
        private int unk8;

        private byte unk9;
        private byte unk10;
        private byte unk11;

        public short CustomMeterData { get; set; }
        private short CustomMeterUnk { get; set; }
        public uint CustomMeterColor { get; set; }

        private short unkToeAngle1;
        private short unkToeAngle2;

        public byte[] PurchaseBits { get; set; } = new byte[0x20];

        public bool GetPurchasedPartFromBitIndex(int bitIndex)
        {
            byte byteLocation = PurchaseBits[bitIndex / 8];
            return ((byteLocation >> (bitIndex % 8)) & 1) == 1;
        }

        public void SetPurchasedPartFromBitIndex(int purchaseBitIndex)
        {
            ref byte byteLocation = ref PurchaseBits[purchaseBitIndex / 8];
            int bitIndex = purchaseBitIndex % 8;
            byteLocation = (byte)((1 << bitIndex) | byteLocation);
        }

        public void RemovePurchasedPartFromBitIndex(int purchaseBitIndex)
        {
            ref byte byteLocation = ref PurchaseBits[purchaseBitIndex / 8];
            int bitIndex = purchaseBitIndex % 8;
            byteLocation &= (byte)~(1 << bitIndex);
        }

        public void ParseSettings(BinaryStream bs)
        {
            FrontWheelEx = bs.ReadInt16();
            RearWheelEx = bs.ReadInt16();
            WheelInchupRelated = bs.ReadInt16();
            WheelSP = bs.ReadInt32();
            CarCode = bs.ReadInt32();
            GarageID = bs.ReadInt32();
            FrontWheelCompound = bs.ReadInt32();
            FrontTire = bs.ReadInt32();
            RearTireCompound = bs.ReadInt32();
            RearTire = bs.ReadInt32();
            Brake = bs.ReadInt32();
            Brakecontroller = bs.ReadInt32();
            Chassis = bs.ReadInt32();
            Engine = bs.ReadInt32();
            DriveTrain = bs.ReadInt32();
            Gear = bs.ReadInt32();
            Suspension = bs.ReadInt32();
            LSD = bs.ReadInt32();
            Steer = bs.ReadInt32();
            Lightweight = bs.ReadInt32();
            Racingmodify = bs.ReadInt32();
            Displacement = bs.ReadInt32();
            Computer = bs.ReadInt32();
            Natune = bs.ReadInt32();
            TurbineKit = bs.ReadInt32();
            Flywheel = bs.ReadInt32();
            Clutch = bs.ReadInt32();
            PropellerShaft = bs.ReadInt32();
            Muffler = bs.ReadInt32();
            Intercooler = bs.ReadInt32();
            ASCC = bs.ReadInt32();
            TCSC = bs.ReadInt32();
            Supercharger = bs.ReadInt32();
            IntakeManifold = bs.ReadInt32();
            ExhaustManifold = bs.ReadInt32();
            Catalyst = bs.ReadInt32();
            AirCleaner = bs.ReadInt32();
            NOS = bs.ReadInt32();
            WindowReduction = bs.ReadInt32();
            CarbonBonnet = bs.ReadInt32();
            BodyKit = bs.ReadInt32();
            FlatFloors = bs.ReadInt32();
            Aero = bs.ReadInt32();
            Wing = bs.ReadInt32();
            UnkTables = bs.ReadInt32s(3);

            GearReverse = bs.ReadInt16();
            Gear1st = bs.ReadInt16();
            Gear2nd = bs.ReadInt16();
            Gear3rd = bs.ReadInt16();
            Gear4th = bs.ReadInt16();
            Gear5th = bs.ReadInt16();
            Gear6th = bs.ReadInt16();
            Gear7th = bs.ReadInt16();
            Gear8th = bs.ReadInt16();
            Gear9th = bs.ReadInt16();
            Gear10th = bs.ReadInt16();
            Gear11st = bs.ReadInt16();

            FinalGearRatio = bs.ReadInt16();
            MaxSpeed_10 = bs.Read1Byte();
            GearLastFinal = bs.ReadInt16();

            Param4WD = bs.Read1Byte();
            FrontABS = bs.Read1Byte();
            RearABS = bs.Read1Byte();
            DownforceFront = bs.ReadInt16();
            DownforceRear = bs.ReadInt16();

            turbo_Boost1 = bs.Read1Byte();
            turbo_peakRpm1 = bs.Read1Byte();
            turbo_response1 = bs.Read1Byte();
            turbo_Boost2 = bs.Read1Byte();
            turbo_peakRpm2 = bs.Read1Byte();
            turbo_response2 = bs.Read1Byte();

            FrontCamber = bs.Read1Byte();
            RearCamber = bs.Read1Byte();
            FrontRideHeight = bs.ReadInt16();
            RearRideHeight = bs.ReadInt16();
            FrontToe = bs.ReadSByte();
            RearToe = bs.ReadSByte();
            FrontSpringRate = bs.ReadInt16();
            RearSpringRate = bs.ReadInt16();
            LeverRatioF = bs.ReadInt16();
            LevelRarioR = bs.ReadInt16();

            FrontDamperF1B = bs.Read1Byte();
            FrontDamperF2B = bs.Read1Byte();
            FrontDamperF1R = bs.Read1Byte();
            FrontDamperF2R = bs.Read1Byte();
            RearDamperF1B = bs.Read1Byte();
            RearDamperF2B = bs.Read1Byte();
            RearDamperF1R = bs.Read1Byte();
            RearDamperF2R = bs.Read1Byte();

            FrontStabilizer = bs.Read1Byte();
            RearStabilizer = bs.Read1Byte();
            FrontLSDParam = bs.Read1Byte();
            RearLSDParam = bs.Read1Byte();
            FrontLSDParam2 = bs.Read1Byte();
            RearLSDParam2 = bs.Read1Byte();
            FrontLSDParam3 = bs.Read1Byte();
            RearLSDParam3 = bs.Read1Byte();
            TCSC_UserValueDF = bs.Read1Byte();
            ASCC_VSCParamLevel = bs.Read1Byte();
            ASCC_VSCParam1DF = bs.Read1Byte();
            ASCC_VUCParamLevel = bs.Read1Byte();
            ASCC_VUCParam11DF = bs.Read1Byte();
            BallastWeight = bs.Read1Byte();
            BallastPosition = bs.ReadSByte();
            SteerLimit = bs.Read1Byte();
            unk3 = bs.ReadInt16();
            WeightModifyRatio = bs.ReadInt16();
            PowerModifyRatio = bs.ReadInt16();
            NOSTorqueVolume = bs.Read1Byte();
            GripMultiplier = bs.Read1Byte();
            FrontBrakeBalanceLevel = bs.Read1Byte();
            RearBrakeBalanceLevel = bs.Read1Byte();
            ABSCorneringControlLevel = bs.Read1Byte();
            unk5 = bs.Read1Byte();
            unk6 = bs.ReadInt16();
            gasCapacity = bs.ReadInt16();
            PowerLimiter = bs.ReadInt16();
            HornSoundID = bs.ReadInt32();
            wheel_color = bs.Read1Byte();
            BodyPaintID = bs.ReadInt16();
            WheelPaintID = bs.ReadInt16();
            BrakePaintID = bs.ReadInt16();
            CustomRearWingPaintID = bs.ReadInt16();
            FrontWheelWidth = bs.ReadInt16();
            FrontWheelDiameter = bs.ReadInt16();
            RearWheelWidth = bs.ReadInt16();
            RearWheelDiameter = bs.ReadInt16();
            bs.ReadInt16();
            WheelInchup = bs.Read1Byte();
            DeckenPreface = bs.Read1Byte();
            DeckenNumber = bs.Read1Byte();
            DeckenType = bs.Read1Byte();
            bs.Read(unk7, 0, unk7.Length);
            unkCustomWing1 = bs.Read1Byte();
            unkCustomWing2 = bs.Read1Byte();
            unkCustomWing3 = bs.Read1Byte();
            customWingsStays = bs.Read1Byte();
            unkCustomWing4 = bs.Read1Byte();
            WingWidthOffset = bs.ReadInt16();
            WingHeightOffset = bs.ReadInt16();
            WingAngleOffset = bs.ReadInt16();
            unk8 = bs.ReadInt32();

            unk9 = bs.Read1Byte();
            unk10 = bs.Read1Byte();
            unk11 = bs.Read1Byte();

            CustomMeterData = bs.ReadInt16();
            CustomMeterUnk = bs.ReadInt16();
            CustomMeterColor = bs.ReadUInt32();
            unkToeAngle1 = bs.ReadInt16();
            unkToeAngle2 = bs.ReadInt16();
            bs.ReadInt16();
            bs.Read(PurchaseBits, 0, PurchaseBits.Length);
        }

        public void WriteSettings(BinaryStream bs)
        {
            bs.WriteInt16(FrontWheelEx);
            bs.WriteInt16(RearWheelEx);
            bs.WriteInt16(WheelInchupRelated);
            bs.WriteInt32(WheelSP);
            bs.WriteInt32(CarCode);
            bs.WriteInt32(GarageID);
            bs.WriteInt32(FrontWheelCompound);
            bs.WriteInt32(FrontTire);
            bs.WriteInt32(RearTireCompound);
            bs.WriteInt32(RearTire);
            bs.WriteInt32(Brake);
            bs.WriteInt32(Brakecontroller);
            bs.WriteInt32(Chassis);
            bs.WriteInt32(Engine);
            bs.WriteInt32(DriveTrain);
            bs.WriteInt32(Gear);
            bs.WriteInt32(Suspension);
            bs.WriteInt32(LSD);
            bs.WriteInt32(Steer);
            bs.WriteInt32(Lightweight);
            bs.WriteInt32(Racingmodify);
            bs.WriteInt32(Displacement);
            bs.WriteInt32(Computer);
            bs.WriteInt32(Natune);
            bs.WriteInt32(TurbineKit);
            bs.WriteInt32(Flywheel);
            bs.WriteInt32(Clutch);
            bs.WriteInt32(PropellerShaft);
            bs.WriteInt32(Muffler);
            bs.WriteInt32(Intercooler);
            bs.WriteInt32(ASCC);
            bs.WriteInt32(TCSC);
            bs.WriteInt32(Supercharger);
            bs.WriteInt32(IntakeManifold);
            bs.WriteInt32(ExhaustManifold);
            bs.WriteInt32(Catalyst);
            bs.WriteInt32(AirCleaner);
            bs.WriteInt32(NOS);
            bs.WriteInt32(WindowReduction);
            bs.WriteInt32(CarbonBonnet);
            bs.WriteInt32(BodyKit);
            bs.WriteInt32(FlatFloors);
            bs.WriteInt32(Aero);
            bs.WriteInt32(Wing);
            bs.WriteInt32s(UnkTables);

            bs.WriteInt16(GearReverse);
            bs.WriteInt16(Gear1st);
            bs.WriteInt16(Gear2nd);
            bs.WriteInt16(Gear3rd);
            bs.WriteInt16(Gear4th);
            bs.WriteInt16(Gear5th);
            bs.WriteInt16(Gear6th);
            bs.WriteInt16(Gear7th);
            bs.WriteInt16(Gear8th);
            bs.WriteInt16(Gear9th);
            bs.WriteInt16(Gear10th);
            bs.WriteInt16(Gear11st);

            bs.WriteInt16(FinalGearRatio);
            bs.WriteByte(MaxSpeed_10);
            bs.WriteInt16(GearLastFinal);
            bs.WriteByte(Param4WD);
            bs.WriteByte(FrontABS);
            bs.WriteByte(RearABS);

            bs.WriteInt16(DownforceFront);
            bs.WriteInt16(DownforceRear);

            bs.WriteByte(turbo_Boost1);
            bs.WriteByte(turbo_peakRpm1);
            bs.WriteByte(turbo_response1);
            bs.WriteByte(turbo_Boost2);
            bs.WriteByte(turbo_peakRpm2);
            bs.WriteByte(turbo_response2);

            bs.WriteByte(FrontCamber);
            bs.WriteByte(RearCamber);

            bs.WriteInt16(FrontRideHeight);
            bs.WriteInt16(RearRideHeight);
            bs.WriteSByte(FrontToe);
            bs.WriteSByte(RearToe);
            bs.WriteInt16(FrontSpringRate);
            bs.WriteInt16(RearSpringRate);
            bs.WriteInt16(LeverRatioF);
            bs.WriteInt16(LevelRarioR);

            bs.WriteByte(FrontDamperF1B);
            bs.WriteByte(FrontDamperF2B);
            bs.WriteByte(FrontDamperF1R);
            bs.WriteByte(FrontDamperF2R);
            bs.WriteByte(RearDamperF1B);
            bs.WriteByte(RearDamperF2B);
            bs.WriteByte(RearDamperF1R);
            bs.WriteByte(RearDamperF2R);

            bs.WriteByte(FrontStabilizer);
            bs.WriteByte(RearStabilizer);
            bs.WriteByte(FrontLSDParam);
            bs.WriteByte(RearLSDParam);
            bs.WriteByte(FrontLSDParam2);
            bs.WriteByte(RearLSDParam2);
            bs.WriteByte(FrontLSDParam3);
            bs.WriteByte(RearLSDParam3);
            bs.WriteByte(TCSC_UserValueDF);
            bs.WriteByte(ASCC_VSCParamLevel);
            bs.WriteByte(ASCC_VSCParam1DF);
            bs.WriteByte(ASCC_VUCParamLevel);
            bs.WriteByte(ASCC_VUCParam11DF);
            bs.WriteByte(BallastWeight);
            bs.WriteSByte(BallastPosition);
            bs.WriteByte(SteerLimit);
            bs.WriteInt16(unk3);
            bs.WriteInt16(WeightModifyRatio);
            bs.WriteInt16(PowerModifyRatio);
            bs.WriteByte(NOSTorqueVolume);
            bs.WriteByte(GripMultiplier);
            bs.WriteByte(FrontBrakeBalanceLevel);
            bs.WriteByte(RearBrakeBalanceLevel);
            bs.WriteByte(ABSCorneringControlLevel);
            bs.WriteByte(unk5);
            bs.WriteInt16(unk6);
            bs.WriteInt16(gasCapacity);
            bs.WriteInt16(PowerLimiter);
            bs.WriteInt32(HornSoundID);
            bs.WriteByte(wheel_color);
            bs.WriteInt16(BodyPaintID);
            bs.WriteInt16(WheelPaintID);
            bs.WriteInt16(BrakePaintID);
            bs.WriteInt16(CustomRearWingPaintID);
            bs.WriteInt16(FrontWheelWidth);
            bs.WriteInt16(FrontWheelDiameter);
            bs.WriteInt16(RearWheelWidth);
            bs.WriteInt16(RearWheelDiameter);
            bs.WriteInt16(0);
            bs.WriteByte(WheelInchup);
            bs.WriteByte(DeckenPreface);
            bs.WriteByte(DeckenNumber);
            bs.WriteByte(DeckenType);
            bs.Write(unk7, 0, unk7.Length);
            bs.WriteByte(unkCustomWing1);
            bs.WriteByte(unkCustomWing2);
            bs.WriteByte(unkCustomWing3);
            bs.WriteByte(customWingsStays);
            bs.WriteByte(unkCustomWing4);
            bs.WriteInt16(WingWidthOffset);
            bs.WriteInt16(WingHeightOffset);
            bs.WriteInt16(WingAngleOffset);
            bs.WriteInt32(unk8);

            bs.WriteByte(unk9);
            bs.WriteByte(unk10);
            bs.WriteByte(unk11);

            bs.WriteInt16(CustomMeterData);
            bs.WriteInt16(CustomMeterUnk);
            bs.WriteUInt32(CustomMeterColor);
            bs.WriteInt16(unkToeAngle1);
            bs.WriteInt16(unkToeAngle2);
            bs.WriteInt16(0);
            bs.Write(PurchaseBits, 0, PurchaseBits.Length);
        }
    }

    public enum PurchaseFlagsA : ulong
    {
        Brake_Stock = 0x02,
        Racing_Brake_Calipers = 0x04,

        Suspension_Stock = 0x01_00,
        Suspension_RacingSoft = 0x02_00,
        Suspension_RacingHard = 0x04_00,
        Suspension_Rally = 0x08_00,
        Suspension_Custom = 0x10_00,

        Weight_Stage1 = 0x08_00_00,
        Weight_Stage2 = 0x10_00_00,
        Weight_Stage3 = 0x20_00_00,
        Weight_Stage4 = 0x40_00_00,
        Weight_Stage5 = 0x80_00_00,

        Transmission_Stock = 0x40_00_00_00,
        Transmission_FiveSpeed = 0x80_00_00_00,
        Transmission_SixSpeed = 0x01_00_00_00_00,
        Transmission_Custom = 0x02_00_00_00_00,

        Engine_Stock = 0x04_00_00_00_00,
        Engine_Stage1 = 0x08_00_00_00_00,
        Engine_Stage2 = 0x10_00_00_00_00,
        Engine_Stage3 = 0x20_00_00_00_00,
        Engine_Stage4 = 0x40_00_00_00_00,
        Engine_Stage5 = 0x80_00_00_00_00,

        Turbo_Stock = 0x01_00_00_00_00_00,
        Turbo_Low = 0x02_00_00_00_00_00,
        Turbo_Mid = 0x04_00_00_00_00_00,
        Turbo_High = 0x08_00_00_00_00_00,
        Turbo_Super = 0x10_00_00_00_00_00,
        Turbo_Ultra = 0x20_00_00_00_00_00,

        Computer_Sports = 0x08_00_00_00_00_00_00,

        Exhaust_Sports = 0x08_00_00_00_00_00_00_00,
        Exhaust_SemiRacing = 0x10_00_00_00_00_00_00_00,
        Exhaust_Racing = 0x20_00_00_00_00_00_00_00,

        Clutch_SinglePlate = 0x40_00_00_00_00_00_00_00,
        Clutch_TwinPlate = 0x80_00_00_00_00_00_00_00,
    }

    public enum PurchaseFlagsB : ulong
    {
        Clutch_TriplePlate = 0x01,
        Clutch_Unk = 0x02,

        PropellerShaft_Carbon = 0x80,

        LSD_Stock = 0x01_00,
        LSD_Custom = 0x02_00,
        LSD_Custom2 = 0x04_00,
        FrontTire_ComfortHard = 0x80_00,
        FrontTire_ComfortMedium = 0x01_00_00,
        FrontTire_ComfortSoft = 0x02_00_00,
        FrontTire_SportsHard = 0x04_00_00,
        FrontTire_SportsMedium = 0x08_00_00,
        FrontTire_SportsSoft = 0x10_00_00,
        FrontTire_SportsSuperSoft = 0x20_00_00,
        FrontTire_RacingHard = 0x40_00_00,
        FrontTire_RacingMedium = 0x80_00_00,
        FrontTire_RacingSoft = 0x01_00_00_00,
        FrontTire_RacingSuperSoft = 0x02_00_00_00,
        FrontTire_Intermediate = 0x04_00_00_00,
        FrontTire_HeavyWet = 0x08_00_00_00,
        FrontTire_Dirt = 0x10_00_00_00,
        FrontTire_Snow = 0x20_00_00_00,
        RearTire_ComfortHard = 0x40_00_00_00,
        RearTire_ComfortMedium = 0x80_00_00_00,
        RearTire_ComfortSoft = 0x01_00_00_00_00,
        RearTire_SportsHard = 0x02_00_00_00_00,
        RearTire_SportsMedium = 0x04_00_00_00_00,
        RearTire_SportsSoft = 0x08_00_00_00_00,
        RearTire_SportsSuperSoft = 0x10_00_00_00_00,
        RearTire_RacingHard = 0x20_00_00_00_00,
        RearTire_RacingMedium = 0x40_00_00_00_00,
        RearTire_RacingSoft = 0x80_00_00_00_00,
        RearTire_RacingSuperSoft = 0x01_00_00_00_00_00,
        RearTire_Intermediate = 0x02_00_00_00_00_00,
        RearTire_HeavyWet = 0x04_00_00_00_00_00,
        RearTire_Dirt = 0x08_00_00_00_00_00,
        RearTire_Snow = 0x10_00_00_00_00_00,

        Supercharger = 0x40_00_00_00_00_00,

        IntakeManifold_Tuning = 0x01_00_00_00_00_00_00,
        ExhaustManifold_Isometric = 0x02_00_00_00_00_00_00,
    }
}
