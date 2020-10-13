using System.Collections.Generic;
using System.Drawing;
using System.Xml;

using GTEventGenerator.Entities;

namespace GTEventGenerator
{
    public class GameParameter
    {
        public int FolderId { get; set; }
        public List<Event> Events { get; set; }

        public GameParameterEventList EventList { get; set; } = new GameParameterEventList();
        public int[] SeriesRewardCredits { get; set; } = new int[16];

        public GameParameter()
        {
            SeriesRewardCredits[0] = 25_000;
            SeriesRewardCredits[1] = 12_750;
            SeriesRewardCredits[2] = 7_500;
            SeriesRewardCredits[3] = 5_000;
            SeriesRewardCredits[4] = 2_500;
            SeriesRewardCredits[5] = 1_000;
            for (int i = 6; i < 16; i++)
                SeriesRewardCredits[i] = -1;

            Events = new List<Event>();
            FolderId = -1;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("GameParameter"); xml.WriteAttributeString("version", "106");
            xml.WriteElementBool("championship", EventList.IsChampionship);
            xml.WriteElementInt("folder_id", FolderId);

            xml.WriteStartElement("events");
            foreach (var evnt in Events)
                evnt.WriteToXml(xml);
            xml.WriteEndElement();

            xml.WriteStartElement("series_reward");
            {
                xml.WriteStartElement("prize_table");
                {
                    foreach (var cr in SeriesRewardCredits)
                    {
                        if (cr != -1)
                            xml.WriteElementInt("prize", cr);
                        else
                            xml.WriteElementInt("prize", 0);
                    }
                }
                xml.WriteEndElement();

                xml.WriteEmptyElement("point_table");
            }
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void ParseEventsFromFile(XmlDocument doc)
        {
            var nodes = doc["xml"]["GameParameter"];
            foreach (XmlNode parentNode in nodes)
            {
                if (parentNode.Name == "events")
                {
                    foreach (XmlNode eventNode in parentNode.ChildNodes)
                    {
                        var newEvent = new Event();

                        foreach (XmlNode node in eventNode.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "constraint":
                                    newEvent.Constraints.ParseRaceConstraints(node);
                                    break;

                                case "entry_set":
                                    newEvent.Entries.ParseRaceEntrySet(node);
                                    foreach (XmlNode entryNode in node.ChildNodes[0]) // entry_generate
                                    {
                                        if (entryNode.Name == "player_pos")
                                        {
                                            newEvent.PlayerPos = entryNode.ReadValueInt() + 1;
                                            break;
                                        }
                                    }
                                    break;

                                case "event_id":
                                    newEvent.Id = node.ReadValueInt();
                                    break;

                                case "information":
                                    newEvent.Information.ParseRaceInformation(newEvent, node);
                                    break;

                                case "race":
                                    newEvent.RaceParameters.ParseRaceData(node);
                                    break;

                                case "reward":
                                    newEvent.Rewards.ParseRaceRewards(newEvent, node);
                                    break;
                                case "eval_condition":
                                    newEvent.EvalConditions.ParseEvalConditionData(node);
                                    break;

                                case "track":
                                    newEvent.Course.ReadEventCourse(node);
                                    break;
                                case "regulation":
                                    newEvent.Regulations.ParseRegulations(node);
                                    break;
                            }
                        }

                        Events.Add(newEvent);
                    }
                }
            }
        }
    }
}
