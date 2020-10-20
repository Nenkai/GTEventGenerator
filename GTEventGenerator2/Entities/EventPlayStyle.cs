using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using GTEventGenerator.Utils;

namespace GTEventGenerator.Entities
{
    public class EventPlayStyle
    {
        public SpecType SpecType { get; set; }
        public PlayType PlayType { get; set; }
        public bool NoQuickMenu { get; set; }
        public bool ReplayRecordEnable { get; set; } = true;
        //rentcar_setting_enable
        //window_num (Used only in runviewer.adc..?)
        //time_limit - demo seconds
        //leave_limit
        //no_instant_replay

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("play_style");
            {
                xml.WriteElementValue("bspec_type", SpecType.ToString());

                if (PlayType != PlayType.GAMBLE)
                    xml.WriteElementValue("play_type", PlayType.ToString());

                xml.WriteElementValue("bspec_type", SpecType.ToString());
                xml.WriteElementBoolIfTrue("no_quickmenu", NoQuickMenu);
                xml.WriteElementBool("replay_record_enable", ReplayRecordEnable);
            }
            xml.WriteEndElement();
        }

        public void ParsePlayStyle(XmlNode node)
        {
            foreach (XmlNode pNode in node.ChildNodes)
            {
                switch (pNode.Name)
                {
                    case "bspec_type":
                        SpecType = pNode.ReadValueEnum<SpecType>();
                        break;
                    case "play_type":
                        PlayType = pNode.ReadValueEnum<PlayType>();
                        break;

                    case "no_quickmenu":
                        NoQuickMenu = pNode.ReadValueBool();
                        break;
                    case "replay_record_enable":
                        ReplayRecordEnable = pNode.ReadValueBool();
                        break;
                }
            }
        }
    }

    public enum SpecType
    {
        [Description("A-Spec Only")]
        ONLY_A,

        [Description("B-Spec Only")]
        ONLY_B,

        [Description("Both A And B")]
        BOTH_A_AND_B,
    }

    public enum PlayType
    {
        [Description("Race")]
        RACE,

        [Description("Demo Mode")]
        DEMO,

        [Description("Gamble (unused/implemented?)")]
        GAMBLE,
    }
}
