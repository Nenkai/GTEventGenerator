using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

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
        public DateTime Date { get; set; } = new DateTime(1970, 1, 1, 12, 00, 59);
        public DecisiveWeatherType DecisiveWeather { get; set; } = DecisiveWeatherType.NONE;
        public bool DisableRecordingReplay { get; set; }
        public bool DisableCollision { get; set; }
        public bool EnableDamage { get; set; }
        public bool EnablePit { get; set; }
        public bool Endless { get; set; }
        public int EntriesCount { get; set; }
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
        public LineGhostRecordType? LineGhostRecordType { get; set; }
        public int? LineGhostPlayMax { get; set; }
        public int MinutesCount { get; set; }
        public bool OnlineOn { get; set; }
        public bool PaceNote { get; set; }
        public PenaltyLevel PenaltyLevel { get; set; } = PenaltyLevel.NONE;
        public bool PenaltyNoReset { get; set; }
        public RaceType RaceType { get; set; } = RaceType.COMPETITION;

        private int _trackWetness;
        public int TrackWetness
        {
            get => _trackWetness;
            set => _trackWetness = value > 10 ? value : 10;
        }

        public bool KeepLoadGhost { get; set; }

        private float _timeProgressSpeed;
        public float TimeProgressSpeed
        {
            get => _timeProgressSpeed;
            set => _timeProgressSpeed = value > 3f ? value : 3f;
        }

        public TimeSpan TimeToStart { get; set; } = TimeSpan.FromSeconds(3);
        public TimeSpan TimeToFinish { get; set; }
        public StartType StartType { get; set; } = StartType.GRID;
        public SlipstreamBehavior SlipstreamBehavior { get; set; } = SlipstreamBehavior.GAME;
        public bool WithGhost { get; set; }
        public bool ReplaceAtCourseOut { get; set; }
        public int WeatherAccel { get; set; } = 10;
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
        public bool? WeatherRandom { get; set; }
        public int WeatherRandomSeed { get; set; }

        public void WriteToXml(XmlWriter xml)
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
            xml.WriteAttributeString("datetime", Date.ToString("yyyy/dd/MM HH:mm:ss"));
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
            xml.WriteElementEnumIntIfSet("line_ghost_record_type", LineGhostRecordType);
            xml.WriteElementIntIfSet("line_ghost_play_max", LineGhostPlayMax);
            xml.WriteElementValue("low_mu_type", "MODERATE");
            xml.WriteElementInt("mu_ratio100", 100);
            xml.WriteElementBool("online_on", OnlineOn);
            xml.WriteElementBool("pace_note", PaceNote);
            xml.WriteElementInt("penalty_level", (int)PenaltyLevel);
            xml.WriteElementBool("penalty_no_level", PenaltyNoReset);
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
            xml.WriteElementBoolOrNull("weather_random", WeatherRandom);
            xml.WriteElementInt("weather_random_seed", WeatherRandomSeed);
            xml.WriteElementFloat("weather_total_sec", WeatherTotalSec);
            xml.WriteElementInt("over_entry_max", 0);
            xml.WriteElementBool("with_ghost", WithGhost);
            xml.WriteElementBool("replace_at_courseout", ReplaceAtCourseOut);
            xml.WriteElementInt("weather_accel10", WeatherAccel);
            xml.WriteElementInt("weather_accel_water_retention10", 10);
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
            xml.WriteElementInt("entry_max", EntriesCount);
            xml.WriteElementInt("racers_max", EntriesCount);
            xml.WriteEmptyElement("boost_table_array");
            xml.WriteEndElement();
        }

        public void ParseRaceData(XmlNode node)
        {
            foreach (XmlNode raceNode in node.ChildNodes)
            {
                switch (raceNode.Name)
                {
                    case "complete_type":
                        CompleteType = raceNode.ReadValueEnum<CompleteType>();
                        break;

                    case "consume_fuel":
                        FuelUseMultiplier = raceNode.ReadValueInt();
                        break;

                    case "consume_tire":
                        TireUseMultiplier = raceNode.ReadValueInt();
                        break;

                    case "race_limit_laps":
                        LapCount = raceNode.ReadValueInt();
                        break;

                    case "race_limit_minutes":
                        MinutesCount = raceNode.ReadValueInt();
                        break;

                    case "entry_max":
                        EntriesCount = raceNode.ReadValueInt();
                        break;
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

        [Description("Rolling (Define Start Time")]
        ROLLING3,

        [Description("Rolling (Define Start & Accel)")]
        ROLLING_NOLIMIT,

        [Description("Standing (Set Coordinates)")]
        STANDING,

        ROLLING_L,
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
    }

    public enum LineGhostRecordType
    {
        OFF,
        ON,
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
        NONE,
        SUNNY,
        RAINY
    }

    public enum FinishType
    {
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
