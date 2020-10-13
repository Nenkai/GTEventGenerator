using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ComponentModel;

namespace GTEventGenerator.Entities
{
    public class EventEvalConditions : INotifyPropertyChanged
    {
        public bool NeedsPopulating { get; set; } = true;

        public EvalConditionType ConditionType { get; set; }
        public string GhostDataPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _gold;
        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;

                if (value > _silver) // New Gold is higher than silver? set silver to it
                    Silver = _gold;

                if (value > _bronze) // New Gold is higher than bronze? set bronze to it
                    Bronze = _gold;

            }
        }

        private int _silver;
        public int Silver
        {
            get => _silver;
            set
            {
                if (value < _gold) // Silver is lower than gold? set silver to gold
                    value = _gold;

                if (value > _bronze) // Silver is higher than bronze? set bronze to silver
                    Bronze = value;

                _silver = value;
                OnPropertyChanged("Silver");
            }
        }

        private int _bronze;
        public int Bronze
        {
            get => _bronze;
            set
            {
                if (value < _gold) // Bronze lower than gold? set it to the same time as gold
                    value = _gold;

                if (value < _silver) // Bronze lower than silver? set it to the same time as silver
                    value = _silver;

                _bronze = value;
                OnPropertyChanged("Bronze");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("eval_condition");
            {
                if (ConditionType != EvalConditionType.NONE)
                    xml.WriteElementValue("type", ConditionType.ToString());
                xml.WriteElementInt("gold", Gold);
                xml.WriteElementInt("bronze", Silver);
                xml.WriteElementInt("silver", Bronze);

                if (!string.IsNullOrEmpty("ghost_data_path"))
                    xml.WriteElementValue("ghost_data_path", GhostDataPath);
            }
            xml.WriteEndElement();
        }

        public void ParseEvalConditionData(XmlNode node)
        {
            foreach (XmlNode evalNode in node.ChildNodes)
            {
                switch (evalNode.Name)
                {
                    case "bronze":
                        Bronze = evalNode.ReadValueInt();
                        break;
                    case "silver":
                        Silver = evalNode.ReadValueInt();
                        break;
                    case "gold":
                        Gold = evalNode.ReadValueInt();
                        break;

                    case "type":
                        ConditionType = evalNode.ReadValueEnum<EvalConditionType>();
                        break;

                    case "ghost_data_path":
                        GhostDataPath = evalNode.ReadValueString();
                        break;
                }
            }
        }
    }

    public enum EvalConditionType
    {
        [Description("None")]
        NONE,

        [Description("By Time (in MS)")]
        TIME,

        [Description("By Drift Score")]
        DRIFT,

        [Description("By Overtake Count")]
        OVERTAKE,
    }
}
