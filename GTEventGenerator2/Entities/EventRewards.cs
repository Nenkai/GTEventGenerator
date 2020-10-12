using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GTEventGenerator.Entities
{
    public class EventRewards
    {
        /// <summary>
        /// Rewards can only be obtained once.
        /// </summary>
        public bool IsOnce { get; set; }
        public int PercentAtPP100 { get; set; }
        public int PPBase { get; set; }
        public bool GivesAllTrophyRewards { get; set; }
        public PresentType PresentType { get; set; } = PresentType.ORDER;
        public int[] MoneyPrizes = new int[16];
        public int[] PointTable = new int[16];
        public int Stars { get; set; }

        public bool NeedsPopulating { get; set; } = true;

        public EventRewards()
        {
            MoneyPrizes[0] = 25_000;
            MoneyPrizes[1] = 12_750;
            MoneyPrizes[2] = 7_500;
            MoneyPrizes[3] = 5_000;
            MoneyPrizes[4] = 2_500;
            MoneyPrizes[5] = 1_000;
            for (int i = 6; i < 16; i++)
                MoneyPrizes[i] = -1;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("reward");
            {
                xml.WriteElementBool("is_once", IsOnce);

                xml.WriteElementInt("percent_at_pp100", PercentAtPP100);
                xml.WriteElementInt("pp_base", PPBase);
                xml.WriteElementValue("present_type", PresentType.ToString());
                xml.WriteElementBool("prize_type", GivesAllTrophyRewards);
                xml.WriteElementInt("special_reward_code", 0);

                xml.WriteStartElement("point_table");
                for (int i = 0; i < PointTable.Length; i++)
                {
                    if (MoneyPrizes[i] == -1)
                        xml.WriteEmptyElement("point");
                    else
                        xml.WriteElementInt("point", PointTable[i]);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("prize_table");
                for (int i = 0; i < MoneyPrizes.Length; i++)
                {
                    if (MoneyPrizes[i] == -1)
                        xml.WriteEmptyElement("prize");
                    else
                        xml.WriteElementInt("prize", MoneyPrizes[i]);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("star_table");
                xml.WriteElementValue("star", "RANK_1");
                if (Stars == 3)
                {
                    xml.WriteElementValue("star", "RANK_3");
                    xml.WriteElementValue("star", "COMPLETE");
                }
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        public void ParseRaceRewards(Event evnt, XmlNode node)
        {
            int i = 0;
            Array.Clear(MoneyPrizes, 0, MoneyPrizes.Length);

            foreach (XmlNode rewardNode in node.ChildNodes)
            {
                switch (rewardNode.Name)
                {
                    case "is_once":
                        IsOnce = rewardNode.ReadValueBool(); break;
                    case "percent_at_pp100":
                        PercentAtPP100 = rewardNode.ReadValueInt(); break;
                    case "pp_base":
                        PPBase = rewardNode.ReadValueInt(); break;

                    case "present_type":
                        PresentType = rewardNode.ReadValueEnum<PresentType>(); break;

                    case "prize_type":
                        GivesAllTrophyRewards = rewardNode.ReadValueBool(); break;

                    case "prize_table":
                        foreach (XmlNode prizeNode in rewardNode.ChildNodes)
                            MoneyPrizes[i++] = prizeNode.ReadValueInt();
                        break;

                    case "star_table":
                        if (rewardNode.ChildNodes.Count == 3)
                            Stars = 3;
                        else
                            Stars = 1;
                        break;
                }
            }
        }
    }

    public enum PresentType
    {
        NONE,
        ORDER,
        RANDOM,
    }
}
