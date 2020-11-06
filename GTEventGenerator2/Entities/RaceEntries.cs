using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

namespace GTEventGenerator.Entities
{
    public class EventEntries
    {
        public List<RaceEntry> AI { get; set; } = new List<RaceEntry>();
        public List<RaceEntry> AIBases { get; set; } = new List<RaceEntry>();
        public RaceEntry Player { get; set; }

        private int aisToPickFromPool;
        public int AIsToPickFromPool
        {
            get => aisToPickFromPool;
            set
            {
                if (value > 16)
                    value = 16;

                aisToPickFromPool = value;
            } 
        }

        public int PlayerPos { get; set; } = 1;
        public int RollingStartV { get; set; }
        public int GapForRollingDistance { get; set; }

        public int AIRoughness { get; set; } = -1;
        public int AIBaseSkillStarting { get; set; } = 80;
        public int AICornerSkillStarting { get; set; } = 80;
        public int AIBrakingSkillStarting { get; set; } = 80;
        public int AIAccelSkillStarting { get; set; } = 80;
        public int AIStartSkillStarting { get; set; } = 80;

        public EntryGenerateType AIEntryGenerateType { get; set; } = EntryGenerateType.ENTRY_BASE_SHUFFLE;
        public EnemySortType AISortType { get; set; } = EnemySortType.NONE;

        public bool NeedsPopulating { get; set; } = true;

        public void WriteToXml(XmlWriter xml)
        {
            if (AI.Count == 0 && AIBases.Count == 0 && Player is null)
                return;

            xml.WriteStartElement("entry_set");
            bool hasGeneratedAI = AIBases.Count != 0 && AIsToPickFromPool != 0;
            if (hasGeneratedAI || AI.Count != 0) // entry_generate is also present if using <entry>
            {
                xml.WriteStartElement("entry_generate");
                {
                    xml.WriteStartElement("delays"); xml.WriteEndElement();

                    if (AIBases.Count < AIsToPickFromPool)
                        AIsToPickFromPool = AIBases.Count;

                    xml.WriteElementInt("entry_num", AI.Count + AIsToPickFromPool + 1); // + 1 as it includes player
                    xml.WriteElementInt("player_pos", PlayerPos - 1);

                    xml.WriteElementValue("generate_type", AIEntryGenerateType.ToString());
                    xml.WriteElementValue("enemy_sort_type", AISortType.ToString());
                    

                    if (GapForRollingDistance != 0 || RollingStartV != 0)
                    {
                        xml.WriteElementBool("use_rolling_start_param", GapForRollingDistance != 0 || RollingStartV != 0);
                        xml.WriteElementInt("rolling_start_v", RollingStartV);
                        xml.WriteElementInt("gap_for_start_rolling_distance", GapForRollingDistance);
                    }

                    if (hasGeneratedAI)
                    {
                        xml.WriteElementInt("ai_roughness", AIRoughness);
                        xml.WriteElementInt("ai_skill_starting", AIBases.Max(x => x.BaseSkill));
                        xml.WriteElementInt("ai_skill", AIBaseSkillStarting);
                        xml.WriteElementInt("ai_skill_breaking", AIBrakingSkillStarting);
                        xml.WriteElementInt("ai_skill_cornering", AICornerSkillStarting);
                        xml.WriteElementInt("ai_skill_accelerating", AIAccelSkillStarting);
                        xml.WriteElementInt("ai_skill_starting", AIStartSkillStarting);
                    }

                    if (AIBases.Count > 0)
                    {
                        xml.WriteStartElement("entry_base_array");
                        {
                            foreach (var ai in AIBases)
                                ai.WriteToXml(xml, false);
                        }
                        xml.WriteEndElement();
                    }
                }
                xml.WriteEndElement();

                foreach (var fixedAI in AI)
                    fixedAI.WriteToXml(xml, true);

                if (Player != null)
                    Player.WriteToXml(xml, true);
            }

            xml.WriteEndElement();
        }

        public void ParseRaceEntrySet(XmlNode node)
        {
            AIBases = new List<RaceEntry>();
            foreach (XmlNode entrySetNode in node.ChildNodes)
            {
                if (entrySetNode.Name == "entry_generate")
                {
                    foreach (XmlNode entryGenerateNode in entrySetNode)
                    {
                        switch (entryGenerateNode.Name)
                        {
                            case "ai_skill":
                                AIBaseSkillStarting = entryGenerateNode.ReadValueInt();
                                break;
                            case "ai_skill_breaking":
                                AIBrakingSkillStarting = entryGenerateNode.ReadValueInt();
                                break;
                            case "ai_skill_cornering":
                                AICornerSkillStarting = entryGenerateNode.ReadValueInt();
                                break;
                            case "ai_skill_accelerating":
                                AIAccelSkillStarting = entryGenerateNode.ReadValueInt();
                                break;
                            case "ai_skill_starting":
                                AIStartSkillStarting = entryGenerateNode.ReadValueInt();
                                break;
                            case "rolling_start_v":
                                RollingStartV = entryGenerateNode.ReadValueInt();
                                break;

                            case "entry_base_array":
                                foreach (XmlNode entryBaseNode in entryGenerateNode.SelectNodes("entry_base"))
                                {
                                    var newEntry = ParseEntry(entryBaseNode);
                                    AIBases.Add(newEntry);
                                }
                                break;

                            case "entry_num":
                                AIsToPickFromPool = entryGenerateNode.ReadValueInt();
                                break;
                            case "player_pos":
                                PlayerPos = entryGenerateNode.ReadValueInt() + 1;
                                break;
                            case "enemy_sort_type":
                                AISortType = entryGenerateNode.ReadValueEnum<EnemySortType>();
                                break;
                            case "generate_type":
                                AIEntryGenerateType = entryGenerateNode.ReadValueEnum<EntryGenerateType>();
                                break;
                            case "gap_for_start_rolling_distance":
                                GapForRollingDistance = entryGenerateNode.ReadValueInt();
                                break;

                        }
                    }
                }
                else if (entrySetNode.Name == "entry")
                {
                    var newEntry = ParseEntry(entrySetNode);
                    if (!newEntry.IsAI && !string.IsNullOrEmpty(newEntry.CarLabel))
                    {
                        if (!string.IsNullOrEmpty(newEntry.CarLabel))
                            Player = newEntry;
                    }
                    else
                        AI.Add(newEntry);
                }
            }
        }

        public RaceEntry ParseEntry(XmlNode entryNode)
        {
            var newEntry = new RaceEntry();
            foreach (XmlNode entryDetailNode in entryNode)
            {
                switch (entryDetailNode.Name)
                {
                    case "driver_name":
                        newEntry.DriverName = entryDetailNode.ReadValueString();
                        break;

                    case "driver_region":
                        newEntry.DriverRegion = entryDetailNode.ReadValueString();
                        break;

                    case "car":
                        newEntry.ColorIndex = int.Parse(entryDetailNode.Attributes["color"].Value);
                        newEntry.CarLabel = entryDetailNode.Attributes["label"].Value;
                        break;

                    case "race_class_id":
                        break;

                    case "delay":
                        newEntry.Delay = entryDetailNode.ReadValueInt();
                        break;

                    case "ai_skill":
                        newEntry.BaseSkill = entryDetailNode.ReadValueInt();
                        break;

                    case "ai_skill_breaking":
                        newEntry.BrakingSkill = entryDetailNode.ReadValueInt();
                        break;

                    case "ai_skill_cornering":
                        newEntry.CorneringSkill = entryDetailNode.ReadValueInt();
                        break;

                    case "ai_skill_accelerating":
                        newEntry.AccelSkill = entryDetailNode.ReadValueInt();
                        break;

                    case "initial_velocity":
                        newEntry.InitialVelocity = entryDetailNode.ReadValueInt(); break;
                    case "initial_position":
                        newEntry.InitialVCoord = entryDetailNode.ReadValueInt(); break;

                    case "engine_na_tune_stage":
                        newEntry.EngineStage = entryDetailNode.ReadValueEnum<EngineNATuneState>(); break;
                    case "engine_turbo_kit":
                        newEntry.TurboKit = entryDetailNode.ReadValueEnum<EngineTurboKit>(); break;
                    case "engine_computer":
                        newEntry.Computer = entryDetailNode.ReadValueEnum<EngineComputer>(); break;
                    case "muffler":
                        newEntry.Exhaust = entryDetailNode.ReadValueEnum<Muffler>(); break;
                    case "suspension":
                        newEntry.Suspension = entryDetailNode.ReadValueEnum<Suspension>(); break;
                    case "transmission":
                        newEntry.Transmission = entryDetailNode.ReadValueEnum<Transmission>(); break;

                    case "power_limiter":
                        newEntry.PowerLimiter = float.Parse(entryDetailNode.ReadValueString()); break;
                    case "wheel":
                        newEntry.WheelID = entryDetailNode.ReadValueInt(); break;
                    case "wheel_color":
                        newEntry.WheelPaintID = entryDetailNode.ReadValueInt(); break;
                    case "wheel_inch_up":
                        newEntry.WheelInchUp = entryDetailNode.ReadValueInt(); break;
                    case "ballast_weight":
                        newEntry.BallastWeight = entryDetailNode.ReadValueInt(); break;
                    case "ballast_position":
                        newEntry.BallastPosition = entryDetailNode.ReadValueInt(); break;
                    case "downforce_f":
                        newEntry.DownforceFront = entryDetailNode.ReadValueInt(); break;
                    case "downforce_r":
                        newEntry.DownforceRear = entryDetailNode.ReadValueInt(); break;
                    case "paint_id":
                        newEntry.DownforceRear = entryDetailNode.ReadValueInt(); break;
                    case "aero_1":
                        newEntry.AeroKit = entryDetailNode.ReadValueInt(); break;
                    case "aero_2":
                        newEntry.FlatFloor = entryDetailNode.ReadValueInt(); break;
                    case "aero_3":
                        newEntry.AeroOther = entryDetailNode.ReadValueInt(); break;
                    case "gear_max_speed":
                        newEntry.MaxGearSpeed = entryDetailNode.ReadValueInt(); break;

                    case "tire_f":
                        newEntry.TireFront = (TireType)Enum.Parse(typeof(TireType), entryDetailNode.ReadValueString());
                        break;

                    case "tire_r":
                        newEntry.TireRear = (TireType)Enum.Parse(typeof(TireType), entryDetailNode.ReadValueString());
                        break;

                    case "player_no":
                        newEntry.IsAI = entryDetailNode.ReadValueInt() == -1;
                        break;
                }
            }
            return newEntry;
        }

    }


    public enum EntryGenerateType
    {
        [Description("None (Pool Ignored)")]
        NONE,

        [Description("Shuffle and Randomly Pick")]
        ENTRY_BASE_SHUFFLE,

        [Description("Pick entries by Order")]
        ENTRY_BASE_ORDER,
    }

    public enum EnemySortType
    {
        [Description("No sorting")]
        NONE,

        [Description("Sort selected race entries by Ascending PP")]
        PP_ASCEND,

        [Description("Sort selected race entries by Descending PP")]
        PP_DESCEND,
    }
}
