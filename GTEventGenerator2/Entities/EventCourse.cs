﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using GTEventGenerator.Utils;
using GTEventGenerator.Database;
using PDTools.Utils;

namespace GTEventGenerator.Entities
{
    public class EventCourse
    {
        public string CourseLabel { get; set; } = "mini";
        public int CourseLayoutNumber { get; set; }
        public short MapOffsetWorldX { get; set; }
        public short MapOffsetWorldY { get; set; }
        public short MapScale { get; set; }
        public bool IsOmodetoDifficulty { get; set; }

        public bool NeedsPopulating { get; set; } = true;

        public CustomCourse CustomCourse { get; set; }

        public List<Gadget> Gadgets { get; set; } = new List<Gadget>();

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("track");

            xml.WriteStartElement("course_code");
            xml.WriteAttributeString("label", CourseLabel);
            xml.WriteEndElement();

            if (CourseLabel.Equals("coursemaker") && CustomCourse != null)
            {
                xml.WriteElementInt("generated_course_id", 0);
                var ted = Convert.ToBase64String(MiscUtils.ZlibCompress(CustomCourse.Data));
                xml.WriteElementValue("edit_data", ted);
            }

            xml.WriteElementInt("course_layout_no", CourseLayoutNumber);
            xml.WriteElementBool("is_omedeto_difficulty", IsOmodetoDifficulty);
            xml.WriteElementInt("map_offset_world_x", MapOffsetWorldX);
            xml.WriteElementInt("map_offset_world_y", MapOffsetWorldY);
            xml.WriteElementInt("map_scale", MapScale);
            xml.WriteElementBool("use_generator", false);

            // GT5 Only
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

            if (Gadgets.Count > 0)
            {
                foreach (var gadget in Gadgets)
                    gadget.WriteToXml(xml);
            }

            xml.WriteEndElement();
        }

        public void ReadFromBuffer(ref BitStream reader)
        {

        }

        public void WriteToCache(ref BitStream bs, GameDB db)
        {
            bs.WriteUInt32(0xE6_E6_C3_44);
            bs.WriteInt32(1_03); // Version
            bs.WriteInt32(1_01); // Gadget Version
            bs.WriteUInt64((ulong)db.GetCourseCodeByLabel(CourseLabel));

            if (CourseLabel.Equals("coursemaker") && CustomCourse != null)
            {
                // Figure out how data is written
            }
            else
                bs.WriteInt32(0);

            bs.WriteInt32(CourseLayoutNumber);

            bs.WriteInt32(Gadgets.Count);
            foreach (var gadget in Gadgets)
                gadget.WriteToBuffer(ref bs);

            bs.WriteInt16(MapOffsetWorldX);
            bs.WriteInt16(MapOffsetWorldY);
            bs.WriteInt16(MapScale);
            bs.WriteBool(IsOmodetoDifficulty);
            bs.WriteByte(0); // field_0x3b
            bs.WriteUInt64(0); // Generated Course ID
        }

        public void ReadEventCourse(XmlNode node)
        {
            foreach (XmlNode trackNode in node.ChildNodes)
            {
                switch (trackNode.Name)
                {
                    case "course_code":
                        CourseLabel = trackNode.Attributes["label"].Value; break;

                    case "edit_data":
                        CustomCourse = CustomCourse.Read(Convert.FromBase64String(trackNode.ReadValueString())); break;

                    case "course_layout_no":
                        CourseLayoutNumber = trackNode.ReadValueInt(); break;

                    case "map_offset_world_x":
                        MapOffsetWorldX = trackNode.ReadValueShort(); break;

                    case "map_offset_world_y":
                        MapOffsetWorldY = trackNode.ReadValueShort(); break;

                    case "map_scale":
                        MapScale = trackNode.ReadValueShort(); break;

                    case "gadget":
                        var gadget = new Gadget();
                        gadget.ReadGadgetNode(trackNode);
                        Gadgets.Add(gadget); break;
                }
            }
        }
    }
}
