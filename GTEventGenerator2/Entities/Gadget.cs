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
    public class Gadget
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public byte KindDBId { get; set; }
        public List<float> Postures { get; set; } = new List<float>();

        public void ReadGadgetNode(XmlNode node)
        {
            foreach (XmlNode childNode in node)
            {
                switch (childNode.Name)
                {
                    case "x":
                        X = childNode.ReadValueSingle(); break;
                    case "y":
                        Y = childNode.ReadValueSingle(); break;
                    case "z":
                        Z = childNode.ReadValueSingle(); break;

                    case "kind_db_id":
                        KindDBId = childNode.ReadValueByte(); break;

                    case "posture":
                        ReadPostureNode(childNode); break;
                }
            }
        }

        public void ReadPostureNode(XmlNode node)
        {
            foreach (XmlNode param in node.SelectNodes("param"))
                Postures.Add(param.ReadValueSingle());
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("gadget");
            {
                xml.WriteElementInt("kind_db_id", KindDBId);
                xml.WriteElementFloat("x", X);
                xml.WriteElementFloat("y", Y);
                xml.WriteElementFloat("z", Z);

                if (Postures.Count > 0)
                {
                    xml.WriteStartElement("posture");
                    foreach (var param in Postures)
                        xml.WriteElementFloat("param", param);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        public void ReadFromBuffer(ref BitStream reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            KindDBId = reader.ReadByte();
            int postureCount = reader.ReadInt32();
            for (int i = 0; i < postureCount; i++)
                Postures.Add(reader.ReadSingle());
        }

        public void WriteToBuffer(ref BitStream bs)
        {
            bs.WriteSingle(X);
            bs.WriteSingle(Y);
            bs.WriteSingle(Z);
            bs.WriteByte(KindDBId);

            bs.WriteInt32(Postures.Count);
            foreach (var param in Postures)
                bs.WriteSingle(param);
        }
    }
}
