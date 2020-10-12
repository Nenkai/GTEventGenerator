using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GTEventGenerator.Entities
{
    public class EventCourse
    {
        public string CourseLabel { get; set; } = "mini";
        public int CourseLayoutNumber { get; set; }

        public bool NeedsPopulating { get; set; } = true;

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("track");

            xml.WriteStartElement("course_code");
            xml.WriteAttributeString("label", CourseLabel);
            xml.WriteEndElement();

            xml.WriteElementInt("course_layout_no", CourseLayoutNumber);
            xml.WriteElementBool("is_omedeto_difficulty", false);
            xml.WriteElementInt("map_offset_world_x", 0);
            xml.WriteElementInt("map_offset_world_y", 0);
            xml.WriteElementInt("map_scale", 0);
            xml.WriteElementBool("use_generator", false);

            xml.WriteStartElement("course_generator_param");
            {
                xml.WriteElementBool("use_random_seed", false);
                xml.WriteElementInt("seed", 0);
                xml.WriteElementValue("course_generator_kind", "GENERATOR_CIRCUIT");
                xml.WriteElementValue("course_generator_length_type", "LENGTH");
                xml.WriteElementInt("lengthy", 0);
                xml.WriteElementValue("course_name", "");
            }
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void ReadEventCourse(XmlNode node)
        {
            foreach (XmlNode trackNode in node.ChildNodes)
            {
                if (trackNode.Name == "course_code")
                {
                    CourseLabel = trackNode.ReadValueString();
                    break;
                }
            }
        }
    }
}
