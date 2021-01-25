using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTEventGenerator.Entities
{
    public class PenaltyParameter
    {
        public bool Enable { get; set; } = true;
        public byte CarCrashInterval { get; set; } = 100;
        public short ScoreSumThreshold { get; set; } = 500;
        public byte CondType { get; set; }
        public byte RatioDiffMin { get; set; }
        public short ScoreDiffMin { get; set; }
        public byte CarImpactThreshold { get; set; }
        public byte CarImpactThreshold2 { get; set; } = 30;
        public byte VelocityDirAngle0 { get; set; } = 20;
        public byte VelocityDirAngle1 { get; set; } = 60;
        public short VelocityDirScore0 { get; set; }
        public short VelocityDirScore1 { get; set; } = 100;
        public byte SteeringAngle0 { get; set; } = 5;
        public byte SteeringAngle1 { get; set; } = 20;
        public short SpeedScore0 { get; set; }
        public short SpeedScore1 { get; set; }
        public byte Speed0 { get; set; } = 15;
        public byte Speed1 { get; set; } = 120;
        public short unk0;
        public short unk1 = 100;
        public byte BackwardAngle { get; set; }
        public byte BackwardMoveRatio { get; set; } = 10;
        public byte WallImpactThreshold { get; set; } = 30;
        public byte WallAlongTimer { get; set; } = 20;
        public byte WallAlongCounter { get; set; } = 5;
        public byte PunishSpeedLimit { get; set; } = 50;
        public byte PunishImpactThreshold0 { get; set; } = 120;
        public byte PunishImpactThreshold1 { get; set; } = 50;

        public UnkPenaltyData[] UnkPenaltyDatas = new UnkPenaltyData[]
        {
            new UnkPenaltyData(10,5,0,0,0,0,0,0),
            new UnkPenaltyData(10,10,5,0,5,5,5,0),
            new UnkPenaltyData(10,10,8,5,5,5,5,0),
            new UnkPenaltyData(10,10,8,5,5,10,10,0),
            new UnkPenaltyData(0,0,0,0,0,0,5,0),
            new UnkPenaltyData(0,0,0,0,0,5,0,0),
            new UnkPenaltyData(0,5,0,0,0,5,5,0),
            new UnkPenaltyData(0,10,5,0,0,5,10,5),
        };

        public bool PunishCollision { get; set; }
        public byte CollisionRecoverDelay { get; set; } = 50;
        public short ShortcutRadius { get; set; } = 250;
        public byte ShortcutMinSpeed { get; set; }
        public bool FreeCrashedbyAutodrive { get; set; } = true;
        public byte FreeRatioByAutodrive { get; set; } = 30;
        public bool PitPenalty { get; set; }
        public byte SideSpeed0 { get; set; }
        public byte SideSpeed1 { get; set; }
        public short SideSpeedScore0 { get; set; }
        public short SideSpeedScore1 { get; set; }
        public byte ShortcutCancelTime1 { get; set; }
        public byte ShortcutCancelTime0 { get; set; }
        public short CollisionOffScore0 { get; set; }
        public short CollisionOffScore1 { get; set; }
        public byte CollisionOffScoreType { get; set; }
        public byte field_0x79;
        public byte FreeLessRatio { get; set; }
        public byte CancelSteeringAngleDiff { get; set; }
        public short CollisionOffDispScore0 { get; set; }
        public byte ShortcutCancelInJamSpeed { get; set; }
        public byte field_0x7f;
        public byte SteeringScoreRatioMin { get; set; }
        public byte SteeringScoreRatioMax { get; set; }
        public byte WallImpactThreshold0 { get; set; } = 3;
        public byte ShortcutRatio { get; set; }
        public byte SideSpeed0Steering { get; set; }
        public byte SideSpeed1Steering { get; set; }
        public byte SideSpeedSteeringScore0 { get; set; }
        public byte SideSpeedSteeringScore1 { get; set; }
        public byte ShortcutType { get; set; } = 1;
        public byte PenaSpeedRatio1 { get; set; } = 80;
        public byte PenaSpeedRatio2 { get; set; } = 120;
        public byte PenaSpeedRatio3 { get; set; } = 150;

        public class UnkPenaltyData
        {
            public byte unk1;
            public byte unk2;
            public byte unk3;
            public byte unk4;
            public byte unk5;
            public byte unk6;
            public byte unk7;
            public byte unk8;

            public UnkPenaltyData(byte unk1, byte unk2, byte unk3, byte unk4, 
                byte unk5, byte unk6, byte unk7, byte unk8)
            {
                this.unk1 = unk1;
                this.unk2 = unk2;
                this.unk3 = unk3;
                this.unk4 = unk4;
                this.unk5 = unk5;
                this.unk6 = unk6;
                this.unk7 = unk7;
                this.unk8 = unk8;
            }
        }
    }
}
