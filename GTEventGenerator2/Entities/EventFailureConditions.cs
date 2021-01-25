﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using GTEventGenerator.Utils;
using PDTools.Utils;

namespace GTEventGenerator.Entities
{
    public class EventFailureConditions
    {
        public List<FailCondition> FailConditions { get; set; } = new List<FailCondition>();
        public List<int> DataList { get; set; } = new List<int>();
        public bool NoFailureAtResult { get; set; }

        public void WriteToXml(XmlWriter xml)
        {
            if (FailConditions.Count != 0)
            {
                xml.WriteStartElement("failure_condition");

                xml.WriteStartElement("type_list");
                {
                    foreach (var value in FailConditions)
                        xml.WriteElementValue("type", value.ToString());
                }
                xml.WriteEndElement();

                if (DataList.Count != 0)
                {
                    xml.WriteStartElement("data_list");
                    foreach (var data in DataList)
                        xml.WriteElementInt("data", data);
                    xml.WriteEndElement();
                }

                xml.WriteElementBool("no_failure_at_result", NoFailureAtResult);

                xml.WriteEndElement();
            }
            
        }

        public void ReadFromCache(ref BitStream reader)
        {
            if (reader.ReadUInt32() != 0xE6_E6_DC_CE)
                throw new Exception("Failure condition magic did not match expected (E6 E6 DC CE)");

            uint version = reader.ReadUInt32();
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_DC_CE);
            bs.WriteUInt32(1_00);

            bs.WriteInt32(FailConditions.Count);
            foreach (var value in FailConditions)
                bs.WriteInt32((int)value);

            bs.WriteInt32(DataList.Count);
            foreach (var value in DataList)
                bs.WriteInt32(value);
            bs.WriteBool(NoFailureAtResult);
        }

        public void ParseFailConditions(XmlNode node)
        {
            foreach (XmlNode pNode in node.ChildNodes)
            {
                switch (pNode.Name)
                {
                    case "type_list":
                        foreach (XmlNode type in pNode.SelectNodes("type"))
                        {
                            FailCondition cond = type.ReadValueEnum<FailCondition>();
                            if (cond == FailCondition.NONE || FailConditions.Contains(cond))
                                continue;
                            FailConditions.Add(cond);
                        }
                        break;

                    case "data_list":
                        foreach (XmlNode data in pNode.SelectNodes("data"))
                            DataList.Add(data.ReadValueInt());
                        break;

                    case "no_failure_at_result":
                        NoFailureAtResult = pNode.ReadValueBool();
                        break;

                }
            }
        }
    }

    [Flags]
    public enum FailCondition
    {
        NONE,
        COURSE_OUT,
        HIT_WALL_HARD,
        HIT_CAR_HARD,
        HIT_CAR,
        PYLON,
        HIT_WALL,
        SPIN_FULL,
        SPIN_HALF,
        WHEEL_SPIN,
        LOCK_BRAKE,
        SLIP_ANGLE,
        LESS_SPEED,
        MORE_SPEED,
        MORE_GFORCE,
        PENALTY_ROAD,
        LOW_MU_ROAD,
        SLALOM,
        WRONGWAY,
        WRONGWAY_LOOSE,
        MAX,
    }
}

