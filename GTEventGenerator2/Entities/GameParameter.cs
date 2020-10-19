using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Linq;

namespace GTEventGenerator.Entities
{
    public class GameParameter
    {
        public const int BaseFolderID = 1000;
        public const int BaseEventID = 100_000;

        public int FolderId { get; set; } = BaseFolderID;
        public int FirstEventID { get; set; } = BaseEventID;

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

        public void OrderEventIDs()
        {
            for (int i = 0; i < Events.Count; i++)
                Events[i].EventID = FirstEventID + i;
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
                    foreach (XmlNode eventNode in parentNode.SelectNodes("event"))
                    {
                        var newEvent = new Event();
                        newEvent.ParseFromXml(eventNode);
                        Events.Add(newEvent);
                    }

                    FirstEventID = Events.FirstOrDefault()?.EventID ?? BaseEventID;
                }
            }
        }
    }
}
