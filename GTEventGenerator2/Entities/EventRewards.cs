using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ComponentModel;
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
        public PresentOrderType PresentType { get; set; } = PresentOrderType.ORDER;
        public ParticipationPresentType ParticipationPresentType { get; set; }

        public int[] MoneyPrizes = new int[16];
        public int[] PointTable = new int[16];
        public int Stars { get; set; }

        public bool NeedsPopulating { get; set; } = true;

        public EventPresent[] RewardPresents = new EventPresent[3];
        public EventPresent[] ParticipatePresents = new EventPresent[3];

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

        public void SetRewardPresent(int index, EventPresent present)
            => RewardPresents[index] = present;

        public void SetParticipatePresent(int index, EventPresent present)
            => ParticipatePresents[index] = present;

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
                if (PointTable.Any(e => e != 0))
                {
                    for (int i = 0; i < PointTable.Length; i++)
                    {
                        if (MoneyPrizes[i] == -1)
                            xml.WriteElementInt("point", 0);
                        else
                            xml.WriteElementInt("point", PointTable[i]);
                    }
                }
                xml.WriteEndElement();

                xml.WriteStartElement("prize_table");
                if (MoneyPrizes.Any(e => e != 0))
                {
                    for (int i = 0; i < MoneyPrizes.Length; i++)
                    {
                        if (MoneyPrizes[i] == -1)
                            xml.WriteElementInt("prize", 0);
                        else
                            xml.WriteElementInt("prize", MoneyPrizes[i]);
                    }
                }
                xml.WriteEndElement();

                xml.WriteStartElement("star_table");
                if (Stars >= 1)
                {
                    xml.WriteElementValue("star", "RANK_1");
                    if (Stars == 3)
                    {
                        xml.WriteElementValue("star", "RANK_3");
                        xml.WriteElementValue("star", "COMPLETE");
                    }
                }
                xml.WriteEndElement();

                if (RewardPresents.Any(e => e != null))
                {
                    xml.WriteElementValue("present_type", PresentType.ToString());
                    xml.WriteStartElement("present");
                    foreach (var present in RewardPresents)
                    {
                        if (present is null)
                            xml.WriteEmptyElement("item");
                        else
                            present.WriteToXml(xml);
                    }
                    xml.WriteEndElement();
                }

                if (ParticipatePresents.Any(e => e != null))
                {
                    xml.WriteElementValue("entry_present_type", ParticipationPresentType.ToString());
                    xml.WriteStartElement("entry_present");
                    foreach (var present in ParticipatePresents)
                    {
                        if (present is null)
                            xml.WriteEmptyElement("item");
                        else
                            present.WriteToXml(xml);
                    }
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        public void ParseRaceRewards(Event evnt, XmlNode node)
        {
            int i = 0;
            Array.Clear(MoneyPrizes, 0, MoneyPrizes.Length);
            Array.Clear(PointTable, 0, PointTable.Length);

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
                        PresentType = rewardNode.ReadValueEnum<PresentOrderType>(); break;
                    case "present":
                        ParsePresentRewards(rewardNode); break;

                    case "entry_present_type":
                        ParticipationPresentType = rewardNode.ReadValueEnum<ParticipationPresentType>(); break;

                    case "entry_present":
                        ParseParticipateRewards(rewardNode); break;

                    case "prize_type":
                        GivesAllTrophyRewards = rewardNode.ReadValueBool(); break;

                    case "point_table":
                        foreach (XmlNode prizeNode in rewardNode.ChildNodes)
                            PointTable[i++] = prizeNode.ReadValueInt();
                        break;

                    case "prize_table":
                        foreach (XmlNode prizeNode in rewardNode.ChildNodes)
                            MoneyPrizes[i++] = prizeNode.ReadValueInt();
                        break;

                    case "star_table":
                        if (rewardNode.ChildNodes.Count == 3)
                            Stars = 3;
                        else if (rewardNode.ChildNodes.Count >= 1)
                            Stars = 1;
                        else
                            Stars = 0;
                        break;
                }
            }
        }

        public void ParsePresentRewards(XmlNode node)
        {
            int i = 0;
            foreach (XmlNode itemNode in node.SelectNodes("item"))
            {
                if (i >= 3)
                    return;

                if (itemNode.Attributes.Count == 0) // No present for this tier
                {
                    i++;
                    continue;
                }

                PresentType type = (PresentType)int.Parse(itemNode.Attributes["category_id"].Value);
                switch (type)
                {
                    case Entities.PresentType.CAR:
                        RewardPresents[i] = EventPresent.FromCar(itemNode.Attributes["f_name"].Value);
                        break;
                    case Entities.PresentType.PAINT:
                        RewardPresents[i] = EventPresent.FromPaint(int.Parse(itemNode.Attributes["argument1"].Value));
                        break;
                }

                i++;
            }
        }

        public void ParseParticipateRewards(XmlNode node)
        {
            int i = 0;
            foreach (XmlNode itemNode in node.SelectNodes("item"))
            {
                if (i >= 3)
                    return;

                if (itemNode.Attributes.Count == 0) // No present for this tier
                {
                    i++;
                    continue;
                }

                PresentType type = (PresentType)int.Parse(itemNode.Attributes["category_id"].Value);
                switch (type)
                {
                    case Entities.PresentType.CAR:
                        ParticipatePresents[i] = EventPresent.FromCar(itemNode.Attributes["f_name"].Value);
                        break;
                    case Entities.PresentType.PAINT:
                        ParticipatePresents[i] = EventPresent.FromPaint(int.Parse(itemNode.Attributes["argument1"].Value));
                        break;
                }

                i++;
            }
        }
    }

    public class EventPresent
    {
        public PresentType PresentType { get; set; }
        public int PaintID { get; set; }
        public string CarLabel { get; set; }
        public int SuitID { get; set; }

        public static EventPresent FromCar(string carLabel)
        {
            var present = new EventPresent();
            present.PresentType = PresentType.CAR;
            present.CarLabel = carLabel;
            return present;
        }

        public static EventPresent FromPaint(int paintID)
        {
            var present = new EventPresent();
            present.PresentType = PresentType.PAINT;
            present.PaintID = paintID;
            return present;
        }

        public static EventPresent FromSuit(int suitID)
        {
            var present = new EventPresent();
            present.PresentType = PresentType.SUIT;
            present.SuitID = suitID;
            return present;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("item");
            if (PresentType == PresentType.PAINT)
                xml.WriteAttributeString("argument1", PaintID.ToString());
            else if (PresentType == PresentType.SUIT)
                xml.WriteAttributeString("argument1", "0");
            else
                xml.WriteAttributeString("argument1", "-1");

            xml.WriteAttributeString("argument2", "");
            xml.WriteAttributeString("argument3", "0");

            if (PresentType == PresentType.SUIT)
                xml.WriteAttributeString("argument4", SuitID.ToString());
            else
                 xml.WriteAttributeString("argument4", "");

            xml.WriteAttributeString("category_id", ((int)PresentType).ToString());
            if (PresentType == PresentType.CAR)
            {
                xml.WriteAttributeString("f_name", CarLabel);
                xml.WriteAttributeString("type_id", "9");
            }
            else if (PresentType == PresentType.PAINT)
            {
                xml.WriteAttributeString("f_name", "");
                xml.WriteAttributeString("type_id", "3");
            }

            xml.WriteEndElement();

        }

    }

    public enum PresentType
    {
        NONE,
        SUIT = 302,
        PAINT = 601,
        CAR = 901,
    }

    public enum PresentOrderType
    {
        [Description("By Placement Order (1st/2nd/3rd)")]
        ORDER,

        [Description("Randomly regardless of Placement")]
        RANDOM,
    }

    public enum ParticipationPresentType
    {
        [Description("Finishing Event")]
        FINISH,

        [Description("All (?)")]
        ORDER,

        [Description("Completing One Lap")]
        RANDOM,
    }
}
