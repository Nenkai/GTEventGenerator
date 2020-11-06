using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

namespace GTEventGenerator.Entities
{
    public class EventRaceParameters
    {
        public bool NeedsPopulating { get; set; } = true;

        public bool AcademyEvent { get; set; }
        public bool Accumulation { get; set; }
        public bool AllowCoDriver { get; set; }
        public bool AutostartPitout { get; set; }
        public BehaviorDamageType BehaviorDamage { get; set; } = BehaviorDamageType.WEAK;
        public bool BoostFlag { get; set; }
        public CompleteType CompleteType { get; set; } = CompleteType.BYLAPS;
        public DateTime? Date { get; set; } = new DateTime(1970, 6, 1, 12, 00, 00);
        public DecisiveWeatherType DecisiveWeather { get; set; } = DecisiveWeatherType.SUNNY;
        public bool DisableRecordingReplay { get; set; }
        public bool DisableCollision { get; set; }
        public bool EnableDamage { get; set; }
        public bool EnablePit { get; set; }
        public bool Endless { get; set; }
        public float? EventGoalV { get; set; }
        public float? EventGoalWidth { get; set; }
        public FinishType FinishType { get; set; } = FinishType.TARGET;
        public Flagset Flagset { get; set; } = Flagset.FLAGSET_NORMAL;
        public int FuelUseMultiplier { get; set; }
        public bool FixedRetention { get; set; }
        public GhostType GhostType { get; set; } = GhostType.NONE;
        public GhostPresenceType GhostPresenceType { get; set; } = GhostPresenceType.NORMAL;
        public GridSortType GridSortType { get; set; } = GridSortType.NONE;
        public int TireUseMultiplier { get; set; }
        public bool ImmediateFinish { get; set; }
        public int LapCount { get; set; } = 1;
        public LightingMode LightingMode { get; set; } = LightingMode.AUTO;
        public LineGhostRecordType LineGhostRecordType { get; set; }
        public int? LineGhostPlayMax { get; set; }
        public int MinutesCount { get; set; }
        public bool OnlineOn { get; set; }
        public bool PaceNote { get; set; }
        public PenaltyLevel PenaltyLevel { get; set; } = PenaltyLevel.DEFAULT;
        public bool PenaltyNoLevel { get; set; }
        public RaceType RaceType { get; set; } = RaceType.COMPETITION;
        public int RacersMax { get; set; } = 8;

        private int _trackWetness;
        public int TrackWetness
        {
            get => _trackWetness;
            set => _trackWetness = value > 10 ? 10 : value;
        }

        public bool KeepLoadGhost { get; set; }

        private float _timeProgressSpeed;
        public float TimeProgressSpeed
        {
            get => _timeProgressSpeed;
            set => _timeProgressSpeed = value > 3f ? 3f : value;
        }

        public TimeSpan TimeToStart { get; set; } = TimeSpan.FromSeconds(6);
        public TimeSpan TimeToFinish { get; set; } 
        public StartType StartType { get; set; } = StartType.GRID;
        public SlipstreamBehavior SlipstreamBehavior { get; set; } = SlipstreamBehavior.GAME;
        public bool WithGhost { get; set; }
        public bool ReplaceAtCourseOut { get; set; }
        public int WeatherAccel { get; set; } 
        public int WeatherBaseCelsius { get; set; } = 24;
        public int WeatherMaxCelsius { get; set; } = 3;
        public int WeatherMinCelsius { get; set; } = 3;
        public bool WeatherNoPrecipitation { get; set; }
        public bool WeatherNoSchedule { get; set; }
        public bool WeatherNoWind { get; set; }
        public int WeatherPointNum { get; set; }
        public bool WeatherPrecRainOnly { get; set; }
        public bool WeatherPrecSnowOnly { get; set; }
        public float WeatherTotalSec { get; set; }
        public bool WeatherRandom { get; set; }
        public int WeatherRandomSeed { get; set; }

        public void WriteToXml(Event parent, XmlWriter xml)
        {
            xml.WriteStartElement("race");
            xml.WriteElementBool("academy_event", AcademyEvent);
            xml.WriteElementBool("accumulation", Accumulation);
            xml.WriteElementBool("allow_codriver", AllowCoDriver);
            xml.WriteElementInt("auto_standing_delay", 0);
            xml.WriteElementBool("autostart_pitout", AutostartPitout);
            xml.WriteElementValue("behavior_damage_type", BehaviorDamage.ToString());
            xml.WriteElementValue("behavior_slip_stream_type", SlipstreamBehavior.ToString());
            xml.WriteElementInt("boost_type", 0);
            xml.WriteElementInt("bspec_vitality10", 10);
            xml.WriteElementValue("complete_type", CompleteType.ToString());
            xml.WriteElementInt("consume_fuel", FuelUseMultiplier);
            xml.WriteElementInt("consume_tire", TireUseMultiplier);

            xml.WriteStartElement("datetime");
            if (Date is null)
                xml.WriteAttributeString("datetime", "1970/00/00 00:00:00");
            else
                xml.WriteAttributeString("datetime", Date.Value.ToString("yyyy/MM/dd HH:mm:ss"));
            xml.WriteEndElement();

            xml.WriteElementValue("decisive_weather", DecisiveWeather.ToString());
            xml.WriteElementBool("disable_collision", DisableCollision);
            xml.WriteElementBool("disable_recording_replay", DisableRecordingReplay);
            xml.WriteElementBool("enable_damage", EnableDamage);
            xml.WriteElementBool("enable_pit", EnablePit);
            xml.WriteElementBool("endless", Endless);
            xml.WriteElementFloatOrNull("event_goal_v", EventGoalV);
            xml.WriteElementFloatOrNull("event_goal_width", EventGoalWidth);
            xml.WriteElementValue("finish_type", FinishType.ToString());
            xml.WriteElementBool("fixed_retention", FixedRetention);
            xml.WriteElementValue("flagset", Flagset.ToString());
            xml.WriteElementValue("ghost_presence_type", GhostPresenceType.ToString());
            xml.WriteElementValue("ghost_type", GhostType.ToString());
            xml.WriteElementValue("grid_sort_type", GridSortType.ToString());
            xml.WriteElementBool("immediate_finish", ImmediateFinish);
            xml.WriteElementInt("initial_retention10", TrackWetness);
            xml.WriteElementBool("keep_load_ghost", KeepLoadGhost);
            xml.WriteElementValue("lighting_mode", LightingMode.ToString());
            xml.WriteElementValue("line_ghost_record_type", LineGhostRecordType.ToString());
            xml.WriteElementIntIfSet("line_ghost_play_max", LineGhostPlayMax);
            xml.WriteElementValue("low_mu_type", "MODERATE");
            xml.WriteElementInt("mu_ratio100", 100);
            xml.WriteElementBool("online_on", OnlineOn);
            xml.WriteElementBool("pace_note", PaceNote);
            xml.WriteElementInt("penalty_level", (int)PenaltyLevel);
            xml.WriteElementBool("penalty_no_level", PenaltyNoLevel);
            xml.WriteElementInt("race_limit_laps", LapCount);
            xml.WriteElementInt("race_limit_minutes", MinutesCount);
            xml.WriteElementValue("race_type", RaceType.ToString());
            xml.WriteElementValue("start_type", StartType.ToString());
            xml.WriteElementFloat("time_progress_speed", TimeProgressSpeed);
            xml.WriteElementInt("time_to_finish", (int)TimeToFinish.TotalMilliseconds);
            xml.WriteElementInt("time_to_start", (int)TimeToStart.TotalMilliseconds);
            xml.WriteElementInt("weather_base_celsius", WeatherBaseCelsius);
            xml.WriteElementInt("weather_max_celsius", WeatherMaxCelsius);
            xml.WriteElementInt("weather_min_celsius", WeatherMinCelsius);
            xml.WriteElementBool("weather_no_precipitation", WeatherNoPrecipitation);
            xml.WriteElementBool("weather_no_schedule", WeatherNoSchedule);
            xml.WriteElementBool("weather_no_wind", WeatherNoWind);
            xml.WriteElementInt("weather_point_num", WeatherPointNum);
            xml.WriteElementBool("weather_prec_rain_only", WeatherPrecRainOnly);
            xml.WriteElementBool("weather_prec_snow_only", WeatherPrecSnowOnly);
            xml.WriteElementBool("weather_random", WeatherRandom);
            xml.WriteElementInt("weather_random_seed", WeatherRandomSeed);
            xml.WriteElementFloat("weather_total_sec", WeatherTotalSec);
            xml.WriteElementInt("over_entry_max", 0);
            xml.WriteElementBool("with_ghost", WithGhost);
            xml.WriteElementBool("replace_at_courseout", ReplaceAtCourseOut);
            xml.WriteElementInt("weather_accel10", WeatherAccel);
            xml.WriteElementInt("weather_accel_water_retention10", 0);
            xml.WriteElementBool("boost_flag", BoostFlag);

            /*
            sw.WriteLine(string.Format("                    <new_weather_data>"));
            sw.WriteLine(string.Format("                        <point>"));
            sw.WriteLine(string.Format("                            <time_rate value=\"0\" />"));
            sw.WriteLine(string.Format("                            <low value=\"1\" />"));
            sw.WriteLine(string.Format("                            <high value=\"1\" />"));
            sw.WriteLine(string.Format("                        </point>"));
            sw.WriteLine(string.Format("                    </new_weather_data>"));
            */
            xml.WriteElementInt("entry_max", RacersMax);
            xml.WriteElementInt("racers_max", RacersMax);
            xml.WriteEmptyElement("boost_table_array");
            xml.WriteEndElement();
        }

        public void ParseRaceData(XmlNode node)
        {
            foreach (XmlNode raceNode in node.ChildNodes)
            {
                switch (raceNode.Name)
                {
                    case "academy_event": AcademyEvent = raceNode.ReadValueBool();
                        break;
                    case "accumulation":
                        Accumulation = raceNode.ReadValueBool(); break;
                    case "autostart_pitout":
                        AutostartPitout = raceNode.ReadValueBool(); break;
                    case "allow_codriver":
                        AllowCoDriver = raceNode.ReadValueBool(); break;
                    //case "auto_standing_delay":
                    //   Stand = raceNode.ReadValueBool();
                    // break;
                    case "behavior_damage_type":
                        BehaviorDamage = raceNode.ReadValueEnum<BehaviorDamageType>(); break;
                    case "behavior_slip_stream_type":
                        SlipstreamBehavior = raceNode.ReadValueEnum<SlipstreamBehavior>(); break;
                    case "complete_type": 
                        CompleteType = raceNode.ReadValueEnum<CompleteType>(); break;
                    case "consume_fuel": 
                        FuelUseMultiplier = raceNode.ReadValueInt(); break;
                    case "consume_tire": 
                        TireUseMultiplier = raceNode.ReadValueInt(); break;
                    case "datetime":
                        var dateStr = raceNode.Attributes["datetime"].Value;
                        if (dateStr.Equals("1970/00/00 00:00:00"))
                            Date = null;
                        else
                        {
                            string date = dateStr.Replace("/00", "/01");
                            if (DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
                                Date = time;
                            else
                                Date = null;
                        }
                        break;
                    case "decisive_weather":
                        DecisiveWeather = raceNode.ReadValueEnum<DecisiveWeatherType>();
                       break;
                    case "disable_collision":
                        DisableCollision = raceNode.ReadValueBool(); break;
                    case "disable_recording_replay":
                        DisableRecordingReplay = raceNode.ReadValueBool(); break;
                    case "enable_damage":
                        EnableDamage = raceNode.ReadValueBool(); break;
                    case "enable_pit":
                        EnablePit = raceNode.ReadValueBool(); break;
                    case "endless":
                        Endless = raceNode.ReadValueBool(); break;
                    //case "entry_max":
                    //    EntriesCount = raceNode.ReadValueInt(); break;
                    case "event_goal_v":
                        EventGoalV = raceNode.ReadValueInt(); break;
                    case "event_goal_width":
                        EventGoalWidth = raceNode.ReadValueInt(); break;
                    case "finish_type":
                        FinishType = raceNode.ReadValueEnum<FinishType>(); break;
                    case "fixed_retention":
                        FixedRetention = raceNode.ReadValueBool(); break;
                    case "flagset":
                        Flagset = raceNode.ReadValueEnum<Flagset>(); break;
                    case "ghost_presence_type":
                        GhostPresenceType = raceNode.ReadValueEnum<GhostPresenceType>(); break;
                    case "ghost_type":
                        GhostType = raceNode.ReadValueEnum<GhostType>(); break;
                    case "grid_sort_type":
                        GridSortType = raceNode.ReadValueEnum<GridSortType>(); break;
                    case "immediate_finish":
                        ImmediateFinish = raceNode.ReadValueBool(); break;
                    case "initial_retention10":
                        TrackWetness = raceNode.ReadValueInt(); break;
                    case "keep_load_ghost":
                        KeepLoadGhost = raceNode.ReadValueBool(); break;
                    case "lighting_mode":
                        LightingMode = raceNode.ReadValueEnum<LightingMode>(); break;
                    case "line_ghost_record_type":
                        LineGhostRecordType = raceNode.ReadValueEnum<LineGhostRecordType>(); break;
                    case "line_ghost_play_max":
                        LineGhostPlayMax = raceNode.ReadValueInt(); break;
                    case "online_on":
                        OnlineOn = raceNode.ReadValueBool(); break;
                    case "pace_note":
                        PaceNote = raceNode.ReadValueBool(); break;
                    case "penalty_level":
                        PenaltyLevel = raceNode.ReadValueEnum<PenaltyLevel>(); break;
                    case "penalty_no_level":
                        PenaltyNoLevel = raceNode.ReadValueBool(); break;
                    case "race_limit_laps": 
                        LapCount = raceNode.ReadValueInt(); break;
                    case "race_limit_minutes": 
                        MinutesCount = raceNode.ReadValueInt(); break;
                    case "race_type":
                        RaceType = raceNode.ReadValueEnum<RaceType>(); break;
                    case "racers_max":
                        RacersMax = raceNode.ReadValueInt(); break;

                    case "replace_at_courseout":
                        ReplaceAtCourseOut = raceNode.ReadValueBool(); break;
                    case "start_type":
                        StartType = raceNode.ReadValueEnum<StartType>(); break;
                    case "time_progress_speed":
                        TimeProgressSpeed = float.Parse(raceNode.ReadValueString()); break;
                    case "time_to_finish":
                        TimeToFinish = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                    case "time_to_start":
                        TimeToStart = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                    case "weather_base_celsius":
                        WeatherBaseCelsius = raceNode.ReadValueInt(); break;
                    case "weather_max_celsius":
                        WeatherMaxCelsius = raceNode.ReadValueInt(); break;
                    case "weather_min_celsius":
                        WeatherMinCelsius = raceNode.ReadValueInt(); break;
                    case "weather_no_precipitation":
                        WeatherNoPrecipitation = raceNode.ReadValueBool(); break;
                    case "weather_no_schedule":
                        WeatherNoSchedule = raceNode.ReadValueBool(); break;
                    case "weather_no_wind":
                        WeatherNoSchedule = raceNode.ReadValueBool(); break;
                    case "weather_point_num":
                        WeatherPointNum = raceNode.ReadValueInt(); break;
                    case "weather_prec_rain_only":
                        WeatherPrecRainOnly = raceNode.ReadValueBool(); break;
                    case "weather_prec_snow_only":
                        WeatherPrecSnowOnly = raceNode.ReadValueBool(); break;
                    case "weather_random":
                        WeatherRandom = raceNode.ReadValueBool(); break;
                    case "weather_random_seed":
                        WeatherRandomSeed = raceNode.ReadValueInt(); break;
                    case "weather_total_sec":
                        WeatherTotalSec = float.Parse(raceNode.ReadValueString()); break;
                    case "with_ghost":
                        WithGhost = raceNode.ReadValueBool(); break;
                    case "weather_accel10":
                        WeatherAccel = raceNode.ReadValueInt(); break;
                    case "boost_flag":
                        BoostFlag = raceNode.ReadValueBool(); break;
                }
            }
        }
    }

    public enum BehaviorDamageType
    {
        [Description("Visual only")]
        WEAK,

        [Description("Repairable Mech Damage")]
        MIDDLE,

        [Description("Permanent Mech Damage")]
        STRONG
    }

    public enum PenaltyLevel
    {
        [Description("Default (Game Mode dependant)")]
        DEFAULT = -1,

        [Description("No Penalties")]
        NONE,

        [Description("Weak")]
        WEAK,

        [Description("Strong")]
        STRONG,
    }

    public enum StartType
    {
        NONE,

        [Description("Grid")]
        GRID,

        [Description("For Time Trial")]
        ATTACK, 

        [Description("Rolling Start (Start Line)")]
        ROLLING,

        [Description("Rolling (Same Accel. as Own Car)")]
        ROLLING2,

        [Description("Rolling (Define Start Time)")]
        ROLLING3,

        [Description("Rolling (Define Start & Accel)")]
        ROLLING_NOLIMIT,

        [Description("Standing (Set Coordinates)")]
        STANDING,

        [Description("Double-File Rolling (Left)")]
        ROLLING_L,

        [Description("Double-File Rolling (Right)")]
        ROLLING_R,
        
        [Description("Pit Start")]
        PIT,

        PITWORK,

        [Description("Same Grid (collisions OFF)")]
        SAME_GRID,

        [Description("Dispersed")]
        DISPERSED,

        [Description("Drift Position (Standing)")]
        COURSEINFO,

        [Description("Drift Position (Rolling)")]
        COURSEINFO_ROLLING,

        [Description("Rolling Start - Dbl. File, Left Ahead")]
        ROLLING_DL,

        [Description("Rolling Start - Dbl. File, Right Ahead")]
        ROLLING_DR,

        FREE,
    }
    public enum GridSortType
    {
        [Description("None")]
        NONE,

        [Description("Random")]
        RANDOM,

        [Description("By Points")]
        POINT_UP,

        [Description("Reverse Points Grid")]
        POINT_DOWN,

        [Description("Fastest First")]
        FASTEST_UP,

        [Description("Fastest Last")]
        FASTEST_DOWN,

        [Description("Based on ranks")]
        PREV_RANK,

        [Description("Reverse Ranks")]
        PREV_RANK_REVESE,
    }

    public enum GhostType
    {
        [Description("No Ghost")]
        NONE,

        [Description("One Lap")]
        ONELAP,

        RECORD,
        SECTOR_ATTACK,
        NORMAL,
        TRGRANK_ALL,

        [Description("Full (GT5?)")]
        FULL, 
    }

    public enum LineGhostRecordType
    {
        OFF,
        ONE,
        TRACKDAY
    }

    public enum GhostPresenceType
    {
        [Description("Normal - Transparent?")]
        NORMAL,

        [Description("None")]
        NONE,

        [Description("Real - Shows an actual car?")]
        REAL,
    }


    public enum SlipstreamBehavior
    {
        GAME,
    }

    public enum RaceType
    {
        COMPETITION,
        DEMO,
        TIMEATTACK,
        DRIFTATTACK
    }

    public enum LightingMode
    {
        AUTO,
        OFF,
        POSITION,
        LOW_BEAM,
        HIGH_BEAM
    }

    public enum Flagset
    {
        FLAGSET_NORMAL
    }

    public enum DecisiveWeatherType
    {
        [Description("None")]
        NONE,

        [Description("Sunny")]
        SUNNY,

        [Description("Rainy")]
        RAINY,

        [Description("Snowy")]
        SNOWY,
    }

    public enum FinishType
    {
        [Description("None")]
        NONE,

        [Description("Target")]
        TARGET,

        [Description("Fastest Car")]
        FASTEST,
    }

    public enum CompleteType
    {
        [Description("None")]
        NONE,

        [Description("Finish After a Number of Laps")]
        BYLAPS,

        [Description("By Section")]
        BYSECTION,

        [Description("Finish After Time (Endurance)")]
        BYTIME,

        [Description("By Stop (licenses)")]
        BYSTOP,
    }
}
