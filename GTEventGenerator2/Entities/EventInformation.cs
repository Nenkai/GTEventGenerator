using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace GTEventGenerator.Entities
{
    public class EventInformation
    {
        public bool NeedsPopulating { get; set; } = true;

        public Dictionary<string, string> Titles { get; set; }
        public Dictionary<string, string> OneLineTitles { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }

        public static readonly Dictionary<string, string> Locales = new Dictionary<string, string>()
        {
            { "JP", "Japanese" },
            { "US", "American" },
            { "GB", "British" },
            { "FR", "French" },
            { "DE", "German" },
            { "IT", "Italian" },
            { "ES", "Spanish" },
            { "PT", "Portuguese" },
            { "NL", "Dutch" },
            { "RU", "Russian" },
            { "KR", "Korean" },
            { "TW", "Chinese (Taiwan)" },
            { "EL", "Greek" },
            { "TR", "Turkish" },
            { "PL", "Polish" },
            { "CZ", "Czech" },
            { "HU", "Magyar (Hungary)" },
            { "BP", "Portuguese (Brazillian)" },
            { "MS", "Spanish (Mexican)" },
        };

        public EventInformation()
        {
            Titles = InitializeLocaleStrings();
            OneLineTitles = InitializeLocaleStrings();
            Descriptions = InitializeLocaleStrings();
        }

        public void SetTitle(string title)
        {
            foreach (var locale in Locales)
                Titles[locale.Key] = title;
        }

        public void SetOneLineTitle(string title)
        {
            foreach (var locale in Locales)
                OneLineTitles[locale.Key] = title;
        }

        public void SetDescription(string description)
        {
            foreach (var locale in Locales)
                Descriptions[locale.Key] = description;
        }

        private Dictionary<string, string> InitializeLocaleStrings()
        {
            var localizedStrings = new Dictionary<string, string>();
            foreach (var locale in Locales)
                localizedStrings.Add(locale.Key, string.Empty);
            return localizedStrings;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("information");
            {
                xml.WriteEmptyElement("advanced_notice");
                xml.WriteElementValue("flier_other_info", "");
                xml.WriteElementValue("logo_other_info", "");
                xml.WriteElementValue("race_info_minute", "");

                xml.WriteStartElement("title");
                foreach (var title in Titles)
                    xml.WriteElementString(title.Key, title.Value);
                xml.WriteEndElement();

                xml.WriteStartElement("one_line_title");
                foreach (var title in OneLineTitles)
                    xml.WriteElementString(title.Key, title.Value);
                xml.WriteEndElement();

                xml.WriteStartElement("description");
                foreach (var desc in Descriptions)
                    xml.WriteElementString(desc.Key, desc.Value);
                xml.WriteEndElement();
            } 
            xml.WriteEndElement();
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_9F_40);
            bs.WriteUInt32(1_02); // Version

            WriteLocaleListToCache(ref bs, Titles);
            WriteLocaleListToCache(ref bs, OneLineTitles);
            WriteLocaleListToCache(ref bs, Descriptions);
            WriteLocaleListToCache(ref bs, null); // advanced_notice
            WriteLocaleListToCache(ref bs, null); // registration_notice

            bs.WriteInt16(0); // narration_id
            bs.WriteInt16(0); // race_info_minute

            bs.WriteNullStringAligned4(string.Empty); // logo_image_path
            bs.WriteByte(0); // logo_image_layout
            bs.WriteInt32(0); // logo_image_buffer size
            bs.WriteNullStringAligned4(string.Empty); // logo_other_info

            bs.WriteNullStringAligned4(string.Empty); // flier_image_path
            bs.WriteInt32(0); // flier_image_buffer
            bs.WriteNullStringAligned4(string.Empty); // flier_other_info

            bs.WriteNullStringAligned4(string.Empty); // race_label
        }

        private void WriteLocaleListToCache(ref BitStream bs, Dictionary<string, string> list)
        {
            bs.WriteUInt32(1_00);
            bs.WriteByte(24); // Count
            bs.WriteSByte(-1); // Unk

            if (list is null)
            {
                for (int i = 0; i < 24; i++)
                    bs.WriteNullStringAligned4(string.Empty);
            }
            else
            {
                for (int i = 0; i < 24; i++)
                    bs.WriteNullStringAligned4(list["GB"]);
            }
        }

        public void ParseRaceInformation(Event evnt, XmlNode node)
        {
            foreach (XmlNode informationNode in node.ChildNodes)
            {
                switch (informationNode.Name)
                {
                    // Use GB only without localisation for the time being
                    case "title":
                        foreach (XmlNode titleNode in informationNode.ChildNodes)
                        {
                            if (Titles.TryGetValue(titleNode.Name, out _))
                            {
                                if (titleNode.Name == "GB")
                                    evnt.Name = titleNode.InnerText;
                                Titles[titleNode.Name] = titleNode.InnerText;
                            } 
                        }
                        break;

                    case "description":
                        foreach (XmlNode descNode in informationNode.ChildNodes)
                        {
                            if (Descriptions.TryGetValue(descNode.Name, out _))
                            {
                                Descriptions[descNode.Name] = descNode.InnerText;
                            }
                        }
                        break;
                    case "one_line_title":
                        foreach (XmlNode descNode in informationNode.ChildNodes)
                        {
                            if (Descriptions.TryGetValue(descNode.Name, out _))
                            {
                                OneLineTitles[descNode.Name] = descNode.InnerText;
                            }
                        }
                        break;
                }
            }
        }
    }
}
