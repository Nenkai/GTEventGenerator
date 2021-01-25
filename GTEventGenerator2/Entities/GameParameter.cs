using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;

using System.IO;
using PDTools.Utils;
using GTEventGenerator.Database;

namespace GTEventGenerator.Entities
{
    public class GameParameter
    {
        public const ulong BaseFolderID = 1000;
        public const ulong BaseEventID = 100_000;

        public ulong FolderId { get; set; } = BaseFolderID;
        public ulong FirstEventID { get; set; } = BaseEventID;
        public string FolderFileName { get; set; }

        public List<Event> Events { get; set; }

        public GameParameterEventList EventList { get; set; }

        public OnlineRoomParameter OnlineRoom { get; set; }
        public EventRewards SeriesRewards { get; set; }
        public EditorInfo EditorInfo { get; set; }

        public GameParameter()
        {
            SeriesRewards = new EventRewards();
            SeriesRewards.MoneyPrizes[0] = 25_000;
            SeriesRewards.MoneyPrizes[1] = 12_750;
            SeriesRewards.MoneyPrizes[2] = 7_500;
            SeriesRewards.MoneyPrizes[3] = 5_000;
            SeriesRewards.MoneyPrizes[4] = 2_500;
            SeriesRewards.MoneyPrizes[5] = 1_000;
            for (int i = 6; i < 16; i++)
                SeriesRewards.MoneyPrizes[i] = -1;

            Events = new List<Event>();
            EventList = new GameParameterEventList();
            OnlineRoom = new OnlineRoomParameter();
            EditorInfo = new EditorInfo();
            FolderFileName = Regex.Replace(EventList.Title.Replace(" ", "").Replace(".", ""), "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToLower();
        }

        public void OrderEventIDs()
        {
            for (int i = 0; i < Events.Count; i++)
                Events[i].EventID = FirstEventID + (ulong)i;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("GameParameter"); xml.WriteAttributeString("version", "106");
            xml.WriteElementBool("championship", EventList.IsChampionship);
            xml.WriteElementULong("folder_id", FolderId);

            xml.WriteStartElement("events");
            foreach (var evnt in Events)
                evnt.WriteToXml(xml);
            xml.WriteEndElement();

            xml.WriteStartElement("series_reward");
            {
                xml.WriteStartElement("prize_table");
                {
                    if (EventList.IsChampionship)
                    {
                        foreach (var cr in SeriesRewards.MoneyPrizes)
                        {
                            if (cr != -1)
                                xml.WriteElementInt("prize", cr);
                            else
                                xml.WriteElementInt("prize", 0);
                        }
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
                        newEvent.FixInvalidNodesIfNeeded();
                        Events.Add(newEvent);
                    }

                    FirstEventID = Events.FirstOrDefault()?.EventID ?? BaseEventID;
                }
                else if (parentNode.Name == "championship")
                    EventList.IsChampionship = parentNode.ReadValueBool();
                else if (parentNode.Name == "series_reward")
                {
                    foreach (XmlNode rewardNode in parentNode.ChildNodes)
                    {
                        if (rewardNode.Name == "prize_table")
                        {
                            int i = 0;
                            foreach (XmlNode prizeNode in parentNode.SelectNodes("prize"))
                            {
                                if (i > 16)
                                    break;
                                SeriesRewards.MoneyPrizes[i++] = prizeNode.ReadValueInt();
                            }
                        }
                    }
                    
                }
            }
        }

        public void ReadFromCache(string path)
        {
            BitStream reader = new BitStream(File.ReadAllBytes(path));
            reader.SeekToByte(0x12);

            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E54F17 && magic != 0xE6E64F17)
                ;

            int version = reader.ReadInt32();
            FolderId = reader.ReadUInt64();
            int event_index = reader.ReadInt32();
            int event_count = reader.ReadInt32();

            Events = new List<Event>(event_count);
            for (int i = 0; i < event_count; i++)
            {
                Event evnt = new Event();
                evnt.ReadFromCache(ref reader);
            }
        }

        public byte[] WriteToCache(GameDB db)
        {
            BitStream bs = new BitStream(1024);
            bs.WriteUInt32(0xE6_E6_4F_17);
            bs.WriteInt32(1_01); // Version
            bs.WriteUInt64(FolderId);
            bs.WriteInt32(0); // Event Index;

            bs.WriteInt32(Events.Count);
            foreach (var evnt in Events)
                evnt.WriteToCache(ref bs, db);

            OnlineRoom.WriteToCache(ref bs);
            SeriesRewards.WriteToCache(ref bs, db);
            new EventInformation().WriteToCache(ref bs);
            EditorInfo.WriteToCache(ref bs);
            bs.WriteBool(EventList.IsChampionship);
            bs.WriteBool(false); // arcade
            bs.WriteBool(false); // keep_sequence
            bs.WriteBool(false); // launch_context

            bs.WriteUInt32(0xE6_E6_4F_18); // End Magic Terminator

            return bs.GetBuffer().ToArray();
        }
    }
}
