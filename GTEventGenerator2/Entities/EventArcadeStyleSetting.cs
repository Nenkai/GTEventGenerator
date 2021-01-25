using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;
namespace GTEventGenerator.Entities
{
    public class EventArcadeStyleSetting
    {
        public byte StartSeconds { get; set; } = 120;
        public byte DefaultExtendSeconds { get; set; } = 25;
        public byte LimitSeconds { get; set; } = 150;
        public byte LevelUpStep { get; set; } = 7;
        public byte OvertakeSeconds { get; set; } = 5;
        public ushort AppearStepV { get; set; } = 700;
        public ushort DisappearStepV { get; set; } = 250;
        public bool EnableSpeedTrap { get; set; } = true;
        public bool EnableJumpBonus { get; set; } = true;
        public ushort AffordTime { get; set; } = 4000;
        public ushort OvertakeScore { get; set; }
        public ushort SpeedTrapScore { get; set; }
        public ushort JumpBonusScore { get; set; }
        public ushort StartupStepV { get; set; } = 200;
        public ushort StartupOffsetV { get; set; } = 500;
        public ushort InitialVelocityL { get; set; } = 80;
        public ushort InitialVelocityH { get; set; } = 150;

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_F9_01);
            bs.WriteUInt32(1_01);

            bs.WriteByte(StartSeconds);
            bs.WriteByte(DefaultExtendSeconds);
            bs.WriteByte(LimitSeconds);
            bs.WriteByte(LevelUpStep);
            bs.WriteByte(OvertakeSeconds);
            bs.WriteUInt16(AppearStepV);
            bs.WriteUInt16(DisappearStepV);
            bs.WriteBool(EnableSpeedTrap);
            bs.WriteBool(EnableJumpBonus);
            bs.WriteUInt16(AffordTime);
            bs.WriteUInt16(OvertakeScore);
            bs.WriteUInt16(SpeedTrapScore);
            bs.WriteUInt16(JumpBonusScore);
            bs.WriteUInt16(StartupStepV);
            bs.WriteUInt16(StartupOffsetV);
            bs.WriteUInt16(InitialVelocityL);
            bs.WriteUInt16(InitialVelocityH);

            // TODO Arcade Style Sections Cache Write
            for (int i = 0; i < 16; i++)
            {
                bs.WriteByte(0);
                bs.WriteUInt32(0);
            }
        }
    }

    public class ArcadeStyleSettingSection
    {
        public byte SectionExtendSeconds { get; set; }
        public float CourseV { get; set; } 
    }
}
