﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using GTEventGenerator.Utils;

namespace GTEventGenerator.Entities
{
    public class EventFailureConditions
    {
        public FailCondition FailConditions { get; set; }

        public void WriteToXml(XmlWriter xml)
        {
            if (FailConditions != FailCondition.NONE)
            {
                xml.WriteStartElement("failure_condition");

                xml.WriteStartElement("type_list");
                {
                    var values = Enum.GetValues(typeof(FailCondition))
                        .Cast<int>()
                        .Where(f => f != (int)FailCondition.NONE && (f & (int)FailConditions) == f)
                        .ToList();

                    foreach (var value in values)
                        xml.WriteElementValue("type", ((FailCondition)value).ToString());
                }
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
            
        }

        public void ParseFailConditions(XmlNode node)
        {
            foreach (XmlNode pNode in node.ChildNodes)
            {
                switch (pNode.Name)
                {
                    case "type_list":
                        foreach (XmlNode type in pNode.SelectNodes("type"))
                            FailConditions |= type.ReadValueEnum<FailCondition>();
                        break;

                }
            }
        }
    }

    [Flags]
    public enum FailCondition
    {
        NONE,
        WRONGWAY = 0x01,
        COURSE_OUT = 0x02,
        HIT_CAR = 0x04,
        HIT_CAR_HARD = 0x08,
        PYLON = 0x10,
        HIT_WALL = 0x20,
        HIT_WALL_HARD = 0x40,
        WRONGWAY_LOOSE = 0x80,
    }
}

