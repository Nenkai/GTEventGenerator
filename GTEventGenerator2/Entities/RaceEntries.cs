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

        public int EntryCount { get; set; } = 12;
        public int PlayerPos { get; set; } = 1;
        public int RollingStartV { get; set; }
        public int GapForRollingDistance { get; set; }
        public int AIRoughness { get; set; }
        public EntryGenerateType AIEntryGenerateType { get; set; } = EntryGenerateType.ENTRY_BASE_SHUFFLE;
        public EnemySortType AISortType { get; set; } = EnemySortType.NONE;

        public bool NeedsPopulating { get; set; } = true;

        public void WriteToXml(XmlWriter xml)
        {
            if (AI.Count == 0 && AIBases.Count == 0 && Player is null)
                return;

            xml.WriteStartElement("entry_set");
            if (Player != null)
                Player.WriteToXml(xml);

            if (AIBases.Count != 0)
            {
                xml.WriteStartElement("entry_generate");
                {
                    xml.WriteStartElement("delays"); xml.WriteEndElement();
                    xml.WriteElementInt("entry_num", EntryCount);
                    xml.WriteElementInt("player_pos", PlayerPos - 1);
                    xml.WriteElementValue("generate_type", AIEntryGenerateType.ToString());
                    xml.WriteElementValue("enemy_sort_type", AISortType.ToString());
                    xml.WriteElementInt("use_rolling_start_value", 0);
                    xml.WriteElementInt("rolling_start_v", RollingStartV);
                    xml.WriteElementInt("gap_for_rolling_start_distance", GapForRollingDistance);
                    xml.WriteElementInt("ai_skill_starting", AIBases.Max(x => x.BaseSkill));
                    xml.WriteElementInt("ai_roughness", 0);

                    xml.WriteStartElement("entry_base_array");
                    {
                        foreach (var ai in AIBases)
                            ai.WriteToXml(xml);
                    }
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
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
                            case "entry_base_array":
                                foreach (XmlNode entryBaseNode in entryGenerateNode.SelectNodes("entry_base"))
                                {
                                    var newEntry = ParseEntry(entryBaseNode);
                                    AIBases.Add(newEntry);
                                }
                                break;

                            case "entry_num":
                                EntryCount = entryGenerateNode.ReadValueInt();
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
                            case "gap_for_rolling_start_distance":
                                GapForRollingDistance = entryGenerateNode.ReadValueInt();
                                break;

                        }
                    }
                }
                else if (entrySetNode.Name == "entry")
                {
                    var newEntry = ParseEntry(entrySetNode);
                    if (!newEntry.IsAI && !string.IsNullOrEmpty(newEntry.CarLabel))
                        Player = newEntry;
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
                        newEntry.InitialVelocity = entryDetailNode.ReadValueInt();
                        break;
                    case "initial_position":
                        newEntry.InitialVCoord = entryDetailNode.ReadValueInt();
                        break;
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
        [Description("None")]
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
