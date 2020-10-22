using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace GTEventGenerator.Entities
{
    public class Event
    {
        public EventConstraints Constraints { get; set; }
        public EventInformation Information { get; set; }
        public EventRewards Rewards { get; set; }
        public EventRaceParameters RaceParameters { get; set; }
        public EventRegulations Regulations { get; set; }
        public EventEntries Entries { get; set; }
        public EventCourse Course { get; set; }
        public EventEvalConditions EvalConditions { get; set; }
        public EventPlayStyle PlayStyle { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RankingDisplayType RankingDisplayType { get; set; } = RankingDisplayType.NONE;
        public DateTime RankingStartDate { get; set; }
        public DateTime RankingEndDate { get; set; }

        public int EventID { get; set; } = GameParameter.BaseEventID;
        public int Index { get; set; }

        private string _name { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                Information.SetTitle(value); // In the maintime
                _name = value;
            }
        }
        public GameMode GameMode { get; set; } = GameMode.EVENT_RACE;

        public int PlayerPos { get; set; }

        public int[] MoneyPrizes { get; set; } = new int[16];

        public List<KeyValuePair<int, string>> otherPrizes { get; set; }

        public bool IsSeasonalEvent { get; set; }
        public string PenaltyScriptName { get; set; }
        public string AIScriptName { get; set; }

        public Event()
        {
            Constraints = new EventConstraints();
            Information = new EventInformation();
            Rewards = new EventRewards();
            RaceParameters = new EventRaceParameters();
            Regulations = new EventRegulations();
            Entries = new EventEntries();
            Course = new EventCourse();
            EvalConditions = new EventEvalConditions();
            PlayStyle = new EventPlayStyle();

            MoneyPrizes[0] = 25_000;
            MoneyPrizes[1] = 12_750;
            MoneyPrizes[2] = 7_500;
            MoneyPrizes[3] = 5_000;
            MoneyPrizes[4] = 2_500;
            MoneyPrizes[5] = 1_000;
            for (int i = 6; i < 16; i++)
                MoneyPrizes[i] = -1;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteComment($"{EventID} - {Name}");
            xml.WriteStartElement("event");
            {
                Constraints.WriteToXml(xml);
                Entries.WriteToXml(xml);
                //sw.WriteLine(string.Format("                <failure_condition>"));
                //sw.WriteLine(string.Format("                    <type_list />"));
                //sw.WriteLine(string.Format("                    <no_failure_at_result value=\"1\" />"));
                //sw.WriteLine(string.Format("                </failure_condition>"));
                xml.WriteElementInt("event_id", EventID);
                xml.WriteElementValue("event_type", "RACE");
                xml.WriteElementValue("game_mode", GameMode.ToString());
                Information.WriteToXml(xml);
                xml.WriteElementBool("inheritance", false);
                RaceParameters.WriteToXml(this, xml);
                Rewards.WriteToXml(xml);
                Course.WriteToXml(xml);
                PlayStyle.WriteToXml(xml);
                Regulations.WriteToXml(xml);
                /*
                sw.WriteLine(string.Format("                <play_style>"));
                sw.WriteLine(string.Format("                    <bspec_type value=\"BOTH_A_AND_B\" />"));
                sw.WriteLine(string.Format("                </play_style>"));
                */
                xml.WriteElementValue("penalty_script", PenaltyScriptName);
                xml.WriteElementValue("ai_script", AIScriptName);
                EvalConditions.WriteToXml(xml);
                xml.WriteElementBool("is_seasonal_event", IsSeasonalEvent);
                xml.WriteStartElement("begin_date"); xml.WriteString(StartDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
                xml.WriteStartElement("end_date"); xml.WriteString(EndDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();

                if (IsSeasonalEvent && RankingDisplayType != RankingDisplayType.NONE)
                {
                    xml.WriteStartElement("ranking");
                    xml.WriteStartElement("begin_date"); xml.WriteString(RankingStartDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
                    xml.WriteStartElement("end_date"); xml.WriteString(RankingEndDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
                    xml.WriteElementValue("type", RankingDisplayType.ToString());
                    xml.WriteElementInt("board_id", EventID);
                    xml.WriteElementInt("display_rank_limit", -1);
                    xml.WriteElementBool("is_local", false);
                    xml.WriteElementInt("replay_rank_limit", 0);
                    xml.WriteElementInt("registration", 0);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        public void ParseFromXml(XmlNode eventNode)
        {
            foreach (XmlNode node in eventNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "constraint":
                        Constraints.ParseRaceConstraints(node);
                        break;

                    case "entry_set":
                        Entries.ParseRaceEntrySet(node);
                        foreach (XmlNode entryNode in node.ChildNodes[0]) // entry_generate
                        {
                            if (entryNode.Name == "player_pos")
                            {
                                PlayerPos = entryNode.ReadValueInt() + 1;
                                break;
                            }
                        }
                        break;

                    case "game_mode":
                        GameMode = node.ReadValueEnum<GameMode>(); break;

                    case "event_id":
                        EventID = node.ReadValueInt();
                        break;

                    case "information":
                        Information.ParseRaceInformation(this, node);
                        break;

                    case "race":
                        RaceParameters.ParseRaceData(node);
                        break;

                    case "reward":
                        Rewards.ParseRaceRewards(this, node);
                        break;
                    case "eval_condition":
                        EvalConditions.ParseEvalConditionData(node);
                        break;

                    case "track":
                        Course.ReadEventCourse(node);
                        break;

                    case "regulation":
                        Regulations.ParseRegulations(node);
                        break;
                    case "play_style":
                        PlayStyle.ParsePlayStyle(node);
                        break;

                    case "is_seasonal_event":
                        IsSeasonalEvent = node.ReadValueBool();
                        break;

                    case "begin_date":
                        string date = node.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
                        StartDate = time;
                        break;
                    case "end_date":
                        string eDate = node.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(eDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eTime);
                        EndDate = eTime;
                        break;

                    case "ranking":
                        ParseRankingData(node);
                        break;
                }
            }
        }

        public void ParseRankingData(XmlNode node)
        {
            foreach (XmlNode rNode in node.ChildNodes)
            {
                switch (rNode.Name)
                {
                    case "begin_date":
                        string date = rNode.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
                        RankingStartDate = time;
                        break;
                    case "end_date":
                        string eDate = rNode.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(eDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eTime);
                        RankingEndDate = eTime;
                        break;
                    case "type":
                        RankingDisplayType = rNode.ReadValueEnum<RankingDisplayType>();
                        break;
                }
            }
        }


        public void MarkUnpopulated()
        {
            Constraints.NeedsPopulating = true;
            Rewards.NeedsPopulating = true;
            RaceParameters.NeedsPopulating = true;
            Regulations.NeedsPopulating = true;
            Entries.NeedsPopulating = true;
            Course.NeedsPopulating = true;
            EvalConditions.NeedsPopulating = true;
            Information.NeedsPopulating = true;
        }
    }

    public enum GameMode
    {
        [Description("GT Mode Race")]
        EVENT_RACE,

        [Description("License")]
        LICENSE,

        [Description("Mission")]
        MISSION,

        [Description("Coffee Break (GT6)")]
        COFFEE_BREAK,

        [Description("Special Event (GT5 School)")]
        SCHOOL,

        [Description("Rally Event (GT5)")]
        EVENT_RALLY,

        [Description("Arcade Race")]
        SINGLE_RACE,

        [Description("Seasonal Race")]
        ONLINE_SINGLE_RACE,

        [Description("Seasonal Time Trial")]
        ONLINE_TIME_ATTACK,

        [Description("Seasonal Drift Attack")]
        ONLINE_DRIFT_ATTACK,

        [Description("Arcade Time Trial")]
        TIME_ATTACK,

        [Description("Arcade Drift Attack")]
        DRIFT_ATTACK,

        [Description("Overtake Mission (GT6)")]
        ARCADE_STYLE_RACE,
    }

    public enum RankingDisplayType
    {
        [Description("No Rankings")]
        NONE,

        [Description("By Best Time")]
        TIME,

        [Description("By Drift Score")]
        DRIFT,
    }
}
