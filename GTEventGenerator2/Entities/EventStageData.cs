using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;
namespace GTEventGenerator.Entities
{
    public class EventStageData
    {
        public StageLayoutType LayoutTypeAtQuick { get; set; }
        public List<StageResetData> AtQuick { get; set; } = new List<StageResetData>();

        public StageLayoutType LayoutTypeBeforeStart { get; set; }
        public List<StageResetData> BeforeStart { get; set; } = new List<StageResetData>();

        public StageLayoutType LayoutTypeCountdown { get; set; }
        public List<StageResetData> Countdown { get; set; } = new List<StageResetData>();

        public StageLayoutType LayoutTypeRaceEnd { get; set; }
        public List<StageResetData> RaceEnd { get; set; } = new List<StageResetData>();

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_04_DD);
            bs.WriteUInt32(1_02); // Version

            bs.WriteSByte((sbyte)LayoutTypeAtQuick);
            bs.WriteInt32(AtQuick.Count);
            foreach (var stageResetData in AtQuick)
                stageResetData.WriteToCache(ref bs);

            bs.WriteSByte((sbyte)LayoutTypeBeforeStart);
            bs.WriteInt32(BeforeStart.Count);
            foreach (var stageResetData in BeforeStart)
                stageResetData.WriteToCache(ref bs);

            bs.WriteSByte((sbyte)LayoutTypeCountdown);
            bs.WriteInt32(Countdown.Count);
            foreach (var stageResetData in Countdown)
                stageResetData.WriteToCache(ref bs);

            bs.WriteSByte((sbyte)LayoutTypeRaceEnd);
            bs.WriteInt32(RaceEnd.Count);
            foreach (var stageResetData in RaceEnd)
                stageResetData.WriteToCache(ref bs);
        }
    }

    public class StageResetData
    {
        public string Code { get; set; }
        public sbyte Coord { get; set; }
        public sbyte TargetID { get; set; }
        public sbyte ResourceID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float RotYDeg { get; set; }
        public float VCoord { get; set; }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_0D_DD);
            bs.WriteUInt32(1_00);
            bs.WriteNullStringAligned4(Code);
            bs.WriteSByte(Coord);
            bs.WriteSByte(TargetID);
            bs.WriteSByte(ResourceID);
            bs.WriteSByte(0); // Unk field_0x1f
            bs.WriteSingle(X);
            bs.WriteSingle(Y);
            bs.WriteSingle(Z);
            bs.WriteSingle(RotYDeg);
            bs.WriteSingle(VCoord);
        }
    }

    public enum StageLayoutType
    {
        DEFAULT,
        RANK,
        SLOT,
        FRONT_2GRID,
    }
}
