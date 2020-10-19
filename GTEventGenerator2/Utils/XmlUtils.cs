using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace GTEventGenerator
{
    public static class XmlUtils
    {
        public static void WriteElementValue(this XmlWriter xml, string localName, string value)
        {
            xml.WriteStartElement(localName);
            xml.WriteAttributeString("value", value);
            xml.WriteEndElement();
        }

        public static void WriteEmptyElement(this XmlWriter xml, string localName)
        {
            xml.WriteStartElement(localName);
            xml.WriteEndElement();
        }

        public static void WriteElementBool(this XmlWriter xml, string localName, bool value)
            => WriteElementValue(xml, localName, value ? "1" : "0");

        public static void WriteElementBoolOrNull(this XmlWriter xml, string localName, bool? value)
        {
            if (value is null)
                WriteElementValue(xml, localName, "-1");
            else
                WriteElementBool(xml, localName, value.Value);
        }

        public static void WriteElementBoolIfSet(this XmlWriter xml, string localName, bool? value)
        {
            if (value != null)
                WriteElementValue(xml, localName, value.Value ? "1" : "0");
        }

        public static void WriteElementBoolIfTrue(this XmlWriter xml, string localName, bool value)
        {
            if (value)
                WriteElementValue(xml, localName, "1");
        }

        public static void WriteElementInt(this XmlWriter xml, string localName, int value)
            => WriteElementValue(xml, localName, value.ToString());

        public static void WriteElementIntIfSet(this XmlWriter xml, string localName, int? value)
        {
            if (value != 0)
                WriteElementValue(xml, localName, value.ToString());
        }

        public static void WriteElementFloat(this XmlWriter xml, string localName, float value)
            => WriteElementValue(xml, localName, value.ToString());

        public static void WriteElementFloatIfSet(this XmlWriter xml, string localName, float value)
        {
            if (value != 0)
                WriteElementValue(xml, localName, value.ToString());
        }

        public static void WriteElementFloatOrNull(this XmlWriter xml, string localName, float? value)
        {
            if (value is null)
                WriteElementValue(xml, localName, "-1");
            else
                WriteElementFloat(xml, localName, value.Value);
        }

        public static void WriteElementEnumInt<T>(this XmlWriter xml, string localName, T value) where T : Enum
            => WriteElementValue(xml, localName, ((int)(object)value).ToString());

        public static void WriteElementEnumIntIfSet<T>(this XmlWriter xml, string localName, Nullable<T> value) where T : struct
        { 
            if (value != null)
                WriteElementEnumInt(xml, localName, (Enum)(object)value.Value);
        }

        public static T ReadValueEnum<T>(this XmlNode node) where T : Enum
            => (T)Enum.Parse(typeof(T), node.Attributes["value"].Value);

        public static string ReadValueString(this XmlNode node)
        {
            if (node.Attributes.Count > 0)
            {
                var attr = node.Attributes["value"];
                if (attr != null)
                   return attr.Value;
            }

            return null;
        }

        public static int ReadValueInt(this XmlNode node)
        {
            if (node.Attributes.Count > 0)
            {
                var attr = node.Attributes["value"];
                if (attr != null)
                {
                    if (int.TryParse(attr.Value, out int value))
                        return value;

                }
            }

            return 0;
        }

        public static bool ReadValueBool(this XmlNode node)
           => ReadValueInt(node) == 1;

        public static bool? ReadValueBoolNull(this XmlNode node)
        {
            var val = ReadValueInt(node);
            if (val == -1)
                return null;
            else
                return val == 1;
        }
    }
}
