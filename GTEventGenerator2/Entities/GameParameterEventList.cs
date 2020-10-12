using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using GTEventGenerator;

namespace GTEventGenerator.Entities
{
    public class GameParameterEventList
    {
        public string Title { get; set; } = "Folder Title";
        public string Description { get; set; } = "Folder Description";
        public string FileName { get; set; } = string.Empty;
        public string FileNameID { get; set; } = string.Empty;
        public int Stars { get; set; }
        public int StarsNeeded { get; set; }
        public bool IsChampionship { get; set; }

        public List<string> eventIds = new List<string>();
        public EventCategory Category { get; set; }

        // TODO: Move these globals somewhere else
        public static List<string> LocaliseLanguages { get; set; } = new List<string>();
        public static List<EventCategory> EventCategories = new List<EventCategory>();

        public GameParameterEventList()
        {
            Category = new EventCategory("", 1000);
        }

        public void WriteToXML(GameParameter parent, string dir, ref int lastEventRaceId, ref int eventRaceId, string eventType)
        {
            using (var xml = XmlWriter.Create(Path.Combine(dir, FileName+".xml"), new XmlWriterSettings() { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("event_list");
                xml.WriteStartElement("event");

                xml.WriteStartElement("title");
                foreach (string lang in LocaliseLanguages)
                {
                    xml.WriteStartElement(lang);
                    xml.WriteString(Title);
                    xml.WriteEndElement();
                }
                    
                xml.WriteEndElement();

                xml.WriteStartElement("description");
                foreach (string lang in LocaliseLanguages)
                {
                    xml.WriteStartElement(lang);
                    xml.WriteString(Description);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                xml.WriteStartElement("copy");
                foreach (string lang in LocaliseLanguages)
                    xml.WriteEmptyElement(lang);

                xml.WriteEndElement();

                xml.WriteEmptyElement("ranking_list");
                xml.WriteStartElement("id"); xml.WriteString(FileNameID.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("voucher");
                xml.WriteStartElement("registration"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("bg_image");
                xml.WriteEmptyElement("icon_image");
                xml.WriteEmptyElement("folder_image");
                xml.WriteStartElement("event_type"); xml.WriteString( EventCategories.Find(x => x.name == eventType ).typeID.ToString() ); xml.WriteEndElement();
                xml.WriteStartElement("gameitem_type"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("gameitem_category"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("gameitem_id"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("gameitem_value");
                xml.WriteStartElement("dlc_flag"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("star"); xml.WriteString(Stars.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("need_star"); xml.WriteString(StarsNeeded.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("championship_value"); xml.WriteString(IsChampionship ? "1" : "0"); xml.WriteEndElement();
                xml.WriteEmptyElement("need_folder_id");

                xml.WriteStartElement("event_id_list");
                foreach (Event evnt in parent.Events)
                {
                    evnt.eventRaceId = lastEventRaceId = eventRaceId;

                    if (evnt == parent.Events.Last())
                    {
                        xml.WriteString(eventRaceId.ToString());
                        eventRaceId++;
                    }
                    else
                    {
                        xml.WriteString($"{eventRaceId.ToString()},");
                        eventRaceId++;
                    }
                }
                xml.WriteEndElement();
                xml.WriteStartElement("argument1"); xml.WriteString((-1).ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("argument2");
                xml.WriteStartElement("argument3"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("argument4");

                xml.WriteEndElement();
                xml.WriteEndElement();
            }
        }

        public void ParseEventList(XmlDocument doc)
        {
            ParseEventText(doc);
            ParseEventData(doc);
        }

        private void ParseEventText(XmlDocument doc)
        {
            foreach (XmlNode node in doc.DocumentElement.ChildNodes[0])
            {
                switch (node.Name)
                {
                    // Use GB only without localisation for the time being
                    case "title":
                        foreach (XmlNode titleNode in node.ChildNodes)
                        {
                            if (titleNode.Name == "GB")
                                Title = titleNode.InnerText;
                        }
                        break;

                    case "description":
                        foreach (XmlNode descNode in node.ChildNodes)
                        {
                            if (descNode.Name == "GB")
                                Description = descNode.InnerText;
                        }
                        break;
                }
            }
        }

        private void ParseEventData(XmlDocument doc)
        {
            foreach (XmlNode node in doc.DocumentElement.ChildNodes[0])
            {
                switch (node.Name)
                {
                    case "id":
                        FileNameID = node.InnerText;
                        break;

                    case "event_type":
                        Category = new EventCategory("", int.Parse(node.InnerText));
                        break;

                    case "star":
                        Stars = int.Parse(node.InnerText);
                        break;

                    case "need_star":
                        StarsNeeded = int.Parse(node.InnerText);
                        break;

                    case "championship_value":
                        IsChampionship = node.InnerText == "1";
                        break;

                    case "event_id_list":
                        foreach (string id in node.InnerText.Split(','))
                            eventIds.Add(id);
                        break;
                }
            }

        }
    }
}
