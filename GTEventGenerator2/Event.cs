using System.Collections.Generic;
using GTEventGenerator.Entities;

using System.ComponentModel;
using System.Xml;
namespace GTEventGenerator
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

        public int eventRaceId { get; set; }
        public int Id { get; set; }

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
            xml.WriteStartElement("event");
            {
                Constraints.WriteToXml(xml);
                Entries.WriteToXml(xml);
                //sw.WriteLine(string.Format("                <failure_condition>"));
                //sw.WriteLine(string.Format("                    <type_list />"));
                //sw.WriteLine(string.Format("                    <no_failure_at_result value=\"1\" />"));
                //sw.WriteLine(string.Format("                </failure_condition>"));
                xml.WriteElementInt("event_id", eventRaceId);
                xml.WriteElementValue("event_type", "RACE");
                xml.WriteElementValue("game_mode", GameMode.ToString());
                Information.WriteToXml(xml);
                xml.WriteElementBool("inheritance", false);
                RaceParameters.WriteToXml(xml);
                Rewards.WriteToXml(xml);
                Course.WriteToXml(xml);
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
            }
            xml.WriteEndElement();
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
}
