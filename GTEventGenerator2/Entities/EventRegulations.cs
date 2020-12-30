using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

namespace GTEventGenerator.Entities
{
    public class EventRegulations
    {
        public static Dictionary<string, string> CountryDefinitions = new Dictionary<string, string>()
        {
            {"DE", "Germany"},
            {"FR", "France"},
            {"GB", "United Kingdom"},
            {"IT", "Italy"},
            {"JP", "Japan"},
            {"SE", "Sweden" },
            {"US", "USA" },
            {"AU", "Australia" },
            {"BE", "Belgium" },
            {"ES", "Spain" },
            {"KR", "South Korea" },
            {"NL", "Netherlands" },
            {"CA", "Canada" },
            {"AE", "UAE" },
            {"AR", "Argentina" },
            {"AT", "Austria" },
            {"CH", "Switzerland" },
            {"PT", "Portugal" },
            {"NZ", "New Zealand" },
        };

        public static Dictionary<CarCategoryRestriction, string> CategoryDefinitions = new Dictionary<CarCategoryRestriction, string>()
        {
            {CarCategoryRestriction.NORMAL, "Normal Cars" },
            {CarCategoryRestriction.RACING, "Racing Cars"},
            {CarCategoryRestriction.TUNING, "Tuned Cars"},
            {CarCategoryRestriction.CONCEPT, "Concept Cars"},
        };

        public bool NeedsPopulating { get; set; } = true;

        public List<string> AllowedManufacturers { get; set; }
        public List<string> RestrictedVehicles { get; set; }
        public List<string> AllowedVehicles { get; set; }
        public List<string> AllowedCountries { get; set; }
        public List<CarCategoryRestriction> AllowedCategories { get; set; }

        private int _ppMax = -1;
        public int PPMax { get => _ppMax; set => _ppMax = value > -1 ? value : -1; }
        private int _ppMin = -1;
        public int PPMin { get => _ppMin; set => _ppMin = value > -1 ? value : -1; }

        private int _yearMax = -1;
        public int YearMax { get => _yearMax; set => _yearMax = value > -1 ? value : -1; }
        private int _yearMin = -1;
        public int YearMin { get => _yearMin; set => _yearMin = value > -1 ? value : -1; }

        private int _torqueMin = -1;
        public int TorqueMin { get => _torqueMin; set => _torqueMin = value > -1 ? value : -1; }
        private int _torqueMax = -1;
        public int TorqueMax { get => _torqueMax; set => _torqueMax = value > -1 ? value : -1; }

        private int _powerMax = -1;
        public int PowerMax { get => _powerMax; set => _powerMax = value > -1 ? value : -1; }

        private int _powerMin = -1;
        public int PowerMin { get => _powerMin; set => _powerMin = value > -1 ? value : -1; }

        private int _weightMax = -1;
        public int WeightMax { get => _weightMax; set => _weightMax = value > -1 ? value : -1; }

        private int _weightMin = -1;
        public int WeightMin { get => _weightMin; set => _weightMin = value > -1 ? value : -1; }

        private int _carLengthMax = -1;
        public int CarLengthMax { get => _carLengthMax; set => _carLengthMax = value > -1 ? value : -1; }

        public bool? KartPermitted { get; set; }

        public bool NOSRegulated { get; set; }
        private bool? _nosNeeded;
        public bool? NOSNeeded
        {
            get => NOSRegulated ? _nosNeeded : null;
            set
            {
                _nosNeeded = value;
                NOSRegulated = _nosNeeded.HasValue;
            }
        }

        public TireType TireCompoundMinFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireCompoundMinRear { get; set; } = TireType.NONE_SPECIFIED;

        public TireType TireCompoundMaxFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireCompoundMaxRear { get; set; } = TireType.NONE_SPECIFIED;

        public AspirationBits AspirationNeeded { get; set; } = AspirationBits.None;
        public DrivetrainBits DrivetrainNeeded { get; set; } = DrivetrainBits.None;

        public EventRegulations()
        {
            AllowedManufacturers = new List<string>();
            RestrictedVehicles = new List<string>();
            AllowedVehicles = new List<string>();
            AllowedCountries = new List<string>();
            AllowedCategories = new List<CarCategoryRestriction>();
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("regulation");

            xml.WriteStartElement("countries");
            foreach (string country in AllowedCountries)
                xml.WriteElementValue("country", country);
            xml.WriteEndElement();

            xml.WriteStartElement("car_categories");
            foreach (CarCategoryRestriction category in AllowedCategories)
                xml.WriteElementValue("category", category.ToString());
            xml.WriteEndElement();

            xml.WriteElementInt("limit_aspec_level", -1);
            xml.WriteElementInt("limit_bspec_level", -1);
            xml.WriteElementInt("limit_power", PowerMax);
            xml.WriteElementInt("limit_pp", PPMax);
            xml.WriteElementInt("limit_torque", TorqueMax);
            xml.WriteElementInt("limit_weight", WeightMax);
            xml.WriteElementEnumInt("limit_tire_f", TireCompoundMaxFront);
            xml.WriteElementEnumInt("limit_tire_r", TireCompoundMaxRear);
            xml.WriteElementInt("limit_year", YearMax);
            xml.WriteElementInt("need_aspec_level", -1);
            xml.WriteElementInt("need_bspec_level", -1);
            xml.WriteElementInt("need_power", PowerMin);
            xml.WriteElementInt("need_pp", PPMin);
            xml.WriteElementInt("need_torque", TorqueMin);
            xml.WriteElementInt("need_weight", WeightMin);
            xml.WriteElementEnumInt("need_tire_f", TireCompoundMinFront);
            xml.WriteElementEnumInt("need_tire_r", TireCompoundMinRear);
            xml.WriteElementInt("need_year", YearMin);
            xml.WriteElementInt("need_aspiration", AspirationNeeded > 0 ? (int)AspirationNeeded : -1 );
            xml.WriteElementInt("need_drivetrain", DrivetrainNeeded > 0 ? (int)DrivetrainNeeded : -1 );
            xml.WriteElementInt("need_license", -1);
            xml.WriteElementInt("limit_length", CarLengthMax);
            xml.WriteElementBoolOrNull("kart_permitted", KartPermitted);
            xml.WriteElementInt("restrictor_limit", -1);
            xml.WriteElementBoolIfSet("NOS", NOSNeeded);

            xml.WriteStartElement("cars");
            foreach (string vehicle in AllowedVehicles)
            {
                xml.WriteStartElement("car");
                xml.WriteAttributeString("label", vehicle);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.WriteStartElement("ban_cars");
            foreach (string vehicle in RestrictedVehicles)
            {
                xml.WriteStartElement("car");
                xml.WriteAttributeString("label", vehicle);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.WriteStartElement("tuners");
            foreach (string manufacturer in AllowedManufacturers)
                xml.WriteElementValue("tuner", manufacturer);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void ParseRegulations(XmlNode node)
        {
            foreach (XmlNode regulationNode in node.ChildNodes)
            {
                switch (regulationNode.Name)
                {
                    case "ban_cars":
                        ParseRaceDisallowedVehicles(regulationNode);
                        break;

                    case "cars":
                        ParseRaceAllowedVehicles(regulationNode);
                        break;

                    case "countries":
                        ParseAllowedCountries(regulationNode);
                        break;
                    case "car_categories":
                        ParseAllowedCategories(regulationNode);
                        break;

                    case "limit_length":
                        CarLengthMax = regulationNode.ReadValueInt();
                        break;

                    case "need_year":
                        YearMin = regulationNode.ReadValueInt(); break;
                    case "limit_year":
                        YearMax = regulationNode.ReadValueInt(); break;

                    case "need_torque":
                        TorqueMin = regulationNode.ReadValueInt(); break;
                    case "limit_torque":
                        TorqueMax = regulationNode.ReadValueInt(); break;

                    case "need_power":
                        PowerMin = regulationNode.ReadValueInt(); break;
                    case "limit_power":
                        PowerMax = regulationNode.ReadValueInt(); break;

                    case "need_weight":
                        WeightMin = regulationNode.ReadValueInt(); break;
                    case "limit_weight":
                        WeightMax = regulationNode.ReadValueInt(); break;

                    case "need_pp":
                        PPMin = regulationNode.ReadValueInt(); break;
                    case "limit_pp":
                        PPMax = regulationNode.ReadValueInt(); break;

                    case "limit_tire_f":
                        TireCompoundMaxFront = regulationNode.ReadValueEnum<TireType>(); break;

                    case "limit_tire_r":
                        TireCompoundMaxRear = regulationNode.ReadValueEnum<TireType>(); break;

                    case "need_tire_f":
                        TireCompoundMinFront = regulationNode.ReadValueEnum<TireType>();
                        break;

                    case "need_tire_r":
                        TireCompoundMinRear = regulationNode.ReadValueEnum<TireType>(); break;

                    case "need_aspiration":
                        int val = regulationNode.ReadValueInt();
                        AspirationNeeded = (AspirationBits)(val == -1 ? 0 : val);
                        break;

                    case "need_drivetrain":
                        int val2 = regulationNode.ReadValueInt();
                        DrivetrainNeeded = (DrivetrainBits)(val2 == -1 ? 0 : val2);
                        break;

                    case "NOS":
                        NOSNeeded = regulationNode.ReadValueBoolNull();
                        break;

                    case "tuners":
                        ParseRaceAllowedManufacturers(regulationNode);
                        break;
                }
            }
        }

        private void ParseRaceAllowedManufacturers(XmlNode node)
        {
            AllowedManufacturers = new List<string>();
            foreach (XmlNode manufacturerNode in node.SelectNodes("tuner"))
                AllowedManufacturers.Add(manufacturerNode.ReadValueString());
        }

        private void ParseRaceAllowedVehicles(XmlNode node)
        {
            AllowedVehicles = new List<string>();
            foreach (XmlNode vehicleNode in node.SelectNodes("car"))
            {
                string vehicle = vehicleNode.Attributes["label"].Value;
                AllowedVehicles.Add(vehicle);
            }
        }

        private void ParseRaceDisallowedVehicles(XmlNode node)
        {
            RestrictedVehicles = new List<string>();
            foreach (XmlNode vehicleNode in node.SelectNodes("car"))
            {
                string vehicle = vehicleNode.Attributes["label"].Value;
                RestrictedVehicles.Add(vehicle);
            }
        }

        private void ParseAllowedCountries(XmlNode node)
        {
            AllowedCountries = new List<string>();
            foreach (XmlNode countryNode in node.SelectNodes("country"))
                AllowedCountries.Add(countryNode.ReadValueString());
        }

        private void ParseAllowedCategories(XmlNode node)
        {
            AllowedCategories = new List<CarCategoryRestriction>();
            foreach (XmlNode countryNode in node.SelectNodes("category"))
            {
                if (Enum.TryParse(countryNode.ReadValueString(), out CarCategoryRestriction category))
                    AllowedCategories.Add(category);
            }
        }
    }

    public enum CarCategoryRestriction
    {
        [Description("Normal Cars")]
        NORMAL,

        [Description("Racing Cars")]
        RACING,

        [Description("Tuned Cars")]
        TUNING,

        [Description("Concept Cars")]
        CONCEPT
    }

    [Flags]
    public enum AspirationBits
    {
        None = 0,
        NA = 0x02,
        Turbo = 0x04,
        Supercharger = 0x08,
    }


    [Flags]
    public enum DrivetrainBits
    {
        None = 0,

        FR = 0x02,
        FF = 0x04,
        AWD = 0x08,
        MR = 0x10,
        RR = 0x20,

        All = FR | FF | AWD | MR | RR,
    }
}
