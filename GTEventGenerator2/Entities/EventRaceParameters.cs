using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

using PDTools.Utils;

namespace GTEventGenerator.Entities
{
    public class EventRaceParameters
    {
        public bool NeedsPopulating { get; set; } = true;

        public bool AcademyEvent { get; set; }
        public bool Accumulation { get; set; }
        public bool AllowCoDriver { get; set; }
        public bool AutostartPitout { get; set; }
        public byte AutoStandingDelay { get; set; }
        public BehaviorDamageType BehaviorDamage { get; set; } = BehaviorDamageType.WEAK;
        public bool BoostFlag { get; set; }
        public byte BoostType { get; set; }
        public byte BoostLevel { get; set; }
        public CompleteType CompleteType { get; set; } = CompleteType.BYLAPS;
        public DateTime? Date { get; set; } = new DateTime(1970, 6, 1, 12, 00, 00);
        public DecisiveWeatherType DecisiveWeather { get; set; } = DecisiveWeatherType.SUNNY;
        public bool DisableRecordingReplay { get; set; }
        public bool DisableCollision { get; set; }
        public bool EnableDamage { get; set; }
        public bool EnablePit { get; set; }
        public bool Endless { get; set; }
        public int? EventStartV { get; set; }
        public int? EventGoalV { get; set; }
        public sbyte? EventGoalWidth { get; set; }
        public FinishType FinishType { get; set; } = FinishType.TARGET;
        public Flagset Flagset { get; set; } = Flagset.FLAGSET_NORMAL;
        public byte FuelUseMultiplier { get; set; }
        public bool FixedRetention { get; set; }
        public GhostType GhostType { get; set; } = GhostType.NONE;
        public bool GoalTimeUseLapTotal { get; set; }
        public GhostPresenceType GhostPresenceType { get; set; } = GhostPresenceType.NORMAL;
        public GridSortType GridSortType { get; set; } = GridSortType.NONE;
        public bool RollingPlayerGrid { get; set; }
        public byte TireUseMultiplier { get; set; }
        public bool ImmediateFinish { get; set; }
        public short LapCount { get; set; } = 1;
        public LightingMode LightingMode { get; set; } = LightingMode.AUTO;
        public LineGhostRecordType LineGhostRecordType { get; set; }
        public int? LineGhostPlayMax { get; set; }
        public short MinutesCount { get; set; }
        public bool OnlineOn { get; set; }
        public bool PaceNote { get; set; }
        public PenaltyLevel PenaltyLevel { get; set; } = PenaltyLevel.DEFAULT;
        public bool PenaltyNoReset { get; set; }
        public RaceType RaceType { get; set; } = RaceType.COMPETITION;
        public short RacersMax { get; set; } = 8;

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
            set => _timeProgressSpeed = value > 300f ? 300f : value;
        }

        public TimeSpan TimeToStart { get; set; } = TimeSpan.FromSeconds(6);
        public TimeSpan TimeToFinish { get; set; }
        public StartType StartType { get; set; } = StartType.GRID;
        public SessionType SessionType { get; set; }
        public SlipstreamBehavior SlipstreamBehavior { get; set; } = SlipstreamBehavior.GAME;
        public byte StartTimeOffset { get; set; }
        public bool WithGhost { get; set; }
        public bool ReplaceAtCourseOut { get; set; }
        public short WeatherAccel { get; set; }
        public short WeatherAccelWaterRetention { get; set; }
        public sbyte WeatherBaseCelsius { get; set; } = 24;
        public sbyte WeatherMaxCelsius { get; set; } = 3;
        public sbyte WeatherMinCelsius { get; set; } = 3;
        public bool WeatherNoPrecipitation { get; set; }
        public bool WeatherNoSchedule { get; set; }
        public bool WeatherNoWind { get; set; }
        public byte WeatherPointNum { get; set; }
        public bool WeatherPrecRainOnly { get; set; }
        public bool WeatherPrecSnowOnly { get; set; }
        public short WeatherTotalSec { get; set; }
        public bool WeatherRandom { get; set; }
        public int WeatherRandomSeed { get; set; }
        public List<WeatherData> NewWeatherData { get; set; } = new List<WeatherData>();

        public byte[] GridList { get; set; } = new byte[32];
        public byte[] DelayStartList { get; set; } = new byte[32];

        public BoostTable[] BoostTables { get; set; } = new BoostTable[2] { new BoostTable(), new BoostTable() };

        private byte[] LaunchSpeedList { get; set; } = new byte[32];
        private short[] LaunchPositionList { get; set; } = new short[32];
        private short[] StartTypeSlotList { get; set; } = new short[32];

        public int[] EventVList { get; set; } = new int[30];
        public PenaltyParameter PenaltyParameter { get; set; } = new PenaltyParameter();

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
            xml.WriteElementInt("boost_type", BoostType);
            if (BoostLevel != 0)
                xml.WriteElementInt("boost_level", BoostLevel);

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
            xml.WriteElementIntIfSet("event_start_v", EventStartV);
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
            xml.WriteElementBool("penalty_no_level", PenaltyNoReset);
            xml.WriteElementInt("race_limit_laps", LapCount);
            xml.WriteElementInt("race_limit_minute", MinutesCount);
            xml.WriteElementBoolIfTrue("rolling_player_grid", RollingPlayerGrid);
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
            xml.WriteElementInt("weather_accel_water_retention10", WeatherAccelWaterRetention);
            xml.WriteElementBool("boost_flag", BoostFlag);

            if (NewWeatherData.Count != 0)
            {
                xml.WriteStartElement("new_weather_data");
                foreach (var data in NewWeatherData)
                {
                    xml.WriteStartElement("point");
                    xml.WriteElementInt("time_rate", data.TimeRate);
                    xml.WriteElementFloat("low", data.Low);
                    xml.WriteElementFloat("high", data.High);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }

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
                    case "boost_type":
                        BoostType = raceNode.ReadValueByte(); break;
                    case "boost_level":
                        BoostLevel = raceNode.ReadValueByte(); break;
                    case "complete_type": 
                        CompleteType = raceNode.ReadValueEnum<CompleteType>(); break;
                    case "consume_fuel": 
                        FuelUseMultiplier = raceNode.ReadValueByte(); break;
                    case "consume_tire": 
                        TireUseMultiplier = raceNode.ReadValueByte(); break;
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
                        EventGoalWidth = (sbyte)raceNode.ReadValueInt(); break;
                    case "finish_type":
                        FinishType = raceNode.ReadValueEnum<FinishType>(); break;
                    case "fixed_retention":
                        FixedRetention = raceNode.ReadValueBool(); break;
                    case "flagset":
                        Flagset = raceNode.ReadValueEnum<Flagset>(); break;
                    case "goal_time_use_lap_total":
                        GoalTimeUseLapTotal = raceNode.ReadValueBool(); break;

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
                        PenaltyNoReset = raceNode.ReadValueBool(); break;
                    case "race_limit_laps": 
                        LapCount = raceNode.ReadValueShort(); break;
                    case "race_limit_minute": 
                        MinutesCount = raceNode.ReadValueShort(); break;
                    case "race_type":
                        RaceType = raceNode.ReadValueEnum<RaceType>(); break;
                    case "racers_max":
                        RacersMax = (short)raceNode.ReadValueShort(); break;

                    case "replace_at_courseout":
                        ReplaceAtCourseOut = raceNode.ReadValueBool(); break;
                    case "rolling_player_grid":
                        RollingPlayerGrid = raceNode.ReadValueBool(); break;
                    case "start_type":
                        StartType = raceNode.ReadValueEnum<StartType>(); break;
                    case "time_progress_speed":
                        TimeProgressSpeed = float.Parse(raceNode.ReadValueString()); break;
                    case "time_to_finish":
                        TimeToFinish = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                    case "time_to_start":
                        TimeToStart = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                    case "weather_base_celsius":
                        WeatherBaseCelsius = raceNode.ReadValueSByte(); break;
                    case "weather_max_celsius":
                        WeatherMaxCelsius = raceNode.ReadValueSByte(); break;
                    case "weather_min_celsius":
                        WeatherMinCelsius = raceNode.ReadValueSByte(); break;
                    case "weather_no_precipitation":
                        WeatherNoPrecipitation = raceNode.ReadValueBool(); break;
                    case "weather_no_schedule":
                        WeatherNoSchedule = raceNode.ReadValueBool(); break;
                    case "weather_no_wind":
                        WeatherNoSchedule = raceNode.ReadValueBool(); break;
                    case "weather_point_num":
                        WeatherPointNum = raceNode.ReadValueByte(); break;
                    case "weather_prec_rain_only":
                        WeatherPrecRainOnly = raceNode.ReadValueBool(); break;
                    case "weather_prec_snow_only":
                        WeatherPrecSnowOnly = raceNode.ReadValueBool(); break;
                    case "weather_random":
                        WeatherRandom = raceNode.ReadValueBool(); break;
                    case "weather_random_seed":
                        WeatherRandomSeed = raceNode.ReadValueInt(); break;
                    case "weather_accel_water_retention10":
                        WeatherAccelWaterRetention = raceNode.ReadValueShort(); break;
                    case "weather_total_sec":
                        WeatherTotalSec = raceNode.ReadValueShort(); break;
                    case "with_ghost":
                        WithGhost = raceNode.ReadValueBool(); break;
                    case "weather_accel10":
                        WeatherAccel = raceNode.ReadValueShort(); break;
                    case "boost_flag":
                        BoostFlag = raceNode.ReadValueBool(); break;
                    case "new_weather_data":
                        {
                            foreach (XmlNode point in raceNode.SelectNodes("point"))
                            {
                                var data = new WeatherData();
                                foreach (XmlNode pointNode in point.ChildNodes)
                                {
                                    switch (pointNode.Name)
                                    {
                                        case "time_rate":
                                            data.TimeRate = pointNode.ReadValueInt(); break;
                                        case "low":
                                            data.Low = float.Parse(pointNode.ReadValueString(), CultureInfo.InvariantCulture.NumberFormat); break;
                                        case "high":
                                            data.High = float.Parse(pointNode.ReadValueString(), CultureInfo.InvariantCulture.NumberFormat); break;
                                    }
                                }
                                NewWeatherData.Add(data);
                            }
                        }
                        break;
                }
            }
        }

        public void ReadFromCache(ref BitStream reader)
        {
            int currentPos = reader.BytePosition;
            short bufferSize = reader.ReadInt16();
            short rpVersion = reader.ReadInt16();

            reader.BufferByteSize = bufferSize;
            if (rpVersion < 122)
                ReadFromCacheOld(ref reader, rpVersion); // Read Old
            else
                ;

            ReadBoostPenaltyParametersAndNewer(ref reader, rpVersion);

            reader.SeekToByte(currentPos + bufferSize); // pRVar2->BufferSize + param_2;

        }

        private void ReadFromCacheOld(ref BitStream reader, int rpVersion)
        {
            reader.ReadBits(3); // session_type
            RaceType = (RaceType)reader.ReadBits(13); // race_type
            StartType = (StartType)reader.ReadInt16();
            CompleteType = (CompleteType)reader.ReadInt16();
            FinishType = (FinishType)reader.ReadInt16();
            LapCount = reader.ReadInt16();
            MinutesCount = reader.ReadInt16();
            TimeToStart = TimeSpan.FromMilliseconds(reader.ReadInt32());
            TimeToFinish = TimeSpan.FromMilliseconds(reader.ReadInt32());
            reader.ReadInt16(); // entry_max
            reader.ReadInt16(); // unk_entry
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadByte(); // course_layout_no
            reader.ReadByte();
            reader.ReadByte(); // race_initial_laps
            reader.ReadByte(); // keep_load_ghost
            reader.ReadInt32(); // course_code
            reader.ReadByte(); // race_class_id
            reader.ReadBits(1); // goal_time_use_lap_total

            ulong unk;
            if (rpVersion < 117)
            {
                unk = reader.ReadBits(23); // 17 bits
            }
            else
            {
                reader.ReadBits(2); // 2bit skip
                if (rpVersion < 119)
                {
                    unk = reader.ReadBits(21); // 15 bits
                }
                else
                {
                    reader.ReadBits(1); // force_pitcrew_off
                    unk = reader.ReadBits(20);
                }
            } 

            // do something with unk

            if (rpVersion >= 121)
            {
                reader.ReadInt32(); // scenery_code
                reader.ReadInt32(); // unk
                reader.ReadInt64(); // unk
            }

            reader.ReadInt16(); // packet_timeout_interval
            reader.ReadInt16(); // packet_timeout_latency
            reader.ReadInt16(); // packet_timeout_lag
            RacersMax = reader.ReadInt16();
            reader.ReadInt32(); // unk

            reader.ReadBits(7); // skipped
            AutostartPitout = reader.ReadBoolBit(); // autostart_pitout
            reader.ReadBits(5); // unk ((uVar2 & 0x1f) << 0x2f | param_1->BoolBits)
            reader.ReadBits(3); // unk (((uVar2 & 0x7) << 0x3c | param_1->BoolBits)
            AutoStandingDelay = reader.ReadByte(); // auto_standing_delay
            reader.ReadBits(3); // unk (uVar2 & 0x7) << 0x39
            reader.ReadBits(2); // start_signal_type
            reader.ReadBits(2); // skipped
            reader.ReadBoolBit(); // unk (Bits & 0x01 << 0x38)
            reader.ReadBoolBit(); // bench_test
            reader.ReadByte(); // mu_ratio_100
            reader.ReadBoolBit(); // unk (Bits & 0x1 << 0x17)
            EnableDamage = reader.ReadBoolBit(); // enable_damage
            reader.ReadBits(2); // low_mu_type?  (Bits & 0x3 << 0x14)
            reader.ReadBits(2); // unk (Bits & 0x3 << 0x12)
            reader.ReadBoolBit(); // gps
            PenaltyNoReset = reader.ReadBoolBit(); // penalty_no_reset
            reader.ReadBits(2); // behavior_slipstream_type
            reader.ReadBits(4); // unk (Bits & 0xf << 0xa)
            reader.ReadBoolBit(); // need_tire_change
            reader.ReadBits(4); // after_race_penalty_sec5? (Bits & 0xf << 5)
            reader.ReadBoolBit(); // is_speedtest_milemode
            LineGhostRecordType = (LineGhostRecordType)reader.ReadBits(2); // line_ghost_record_type
            reader.ReadBits(2); // attack_seperate_type? (Bits & 0x03)
            reader.ReadByte(); // penalty_level
            reader.ReadByte(); // auto_start_with_session
            reader.ReadByte(); // auto_end_session_with_finish
            ImmediateFinish = reader.ReadBool(); // immediate_finish
            OnlineOn = reader.ReadBool(); // online_on
            Endless = reader.ReadBool(); // endless
            reader.ReadByte(); // use grid list
            GhostType = (GhostType)reader.ReadByte(); // ghost_type
            GridSortType = (GridSortType)reader.ReadByte(); // grid_sort_type
            Accumulation = reader.ReadBool(); // accumulation
            EnablePit = reader.ReadBool(); // enable_pit
            Flagset = (Flagset)reader.ReadByte(); // flagset
            reader.ReadByte(); // unk (Bits & 0x1 << 0x26)
            DisableCollision = reader.ReadBool(); // disable_collision
            reader.ReadByte(); // penalty_condition
            reader.ReadByte(); // academy_event
            reader.ReadByte(); // consume_fuel
            reader.ReadByte(); // bspec_vitality_10
            reader.ReadByte(); // consideration_type
            TireUseMultiplier = reader.ReadByte(); // consume_tire
            reader.ReadByte(); // temperature_tire
            reader.ReadByte(); // temperature_engine
            reader.ReadByte(); // unk field_5a
            LightingMode = (LightingMode)reader.ReadByte(); // lighting_mode
            reader.ReadBits(6); //   *(ulonglong *)&param_1->temperature_tire = (uVar3 & 0x3f) << 0x1a | *(ulonglong*)&param_1->temperature_tire & 0xffffffff03ffffff;
            reader.ReadBits(4); //   *(ulonglong *)&param_1->temperature_tire = (uVar3 & 0xf) << 0x16 | *(ulonglong*)&param_1->temperature_tire & 0xfffffffffc3fffff;
            reader.ReadBits(5); //   *(ulonglong *)&param_1->temperature_tire = (uVar3 & 0x1f) << 0x11 | *(ulonglong*)&param_1->temperature_tire & 0xffffffffffc1ffff;
            reader.ReadBits(5); //   *(ulonglong *)&param_1->temperature_tire = (uVar3 & 0x1f) << 0xc | *(ulonglong*)&param_1->temperature_tire & 0xfffffffffffe0fff;
            reader.ReadBits(6); //   *(ulonglong *)&param_1->temperature_tire = (uVar3 & 0x3f) << 0x6 | *(ulonglong*)&param_1->temperature_tire & 0xfffffffffffff03f;
            reader.ReadBits(6); //   *(ulonglong *)&param_1->temperature_tire = uVar3 & 0x3f | *(ulonglong*)&param_1->temperature_tire & 0xffffffffffffffc0;
            reader.ReadByte(); // time_progress_speed
            AllowCoDriver = reader.ReadBool(); // allow_codriver
            reader.ReadByte(); // pace_note
            reader.ReadByte(); // team_count
            reader.ReadIntoByteArray(32, new byte[32], BitStream.Byte_Bits); // grid_list
            reader.ReadIntoByteArray(32, new byte[32], BitStream.Byte_Bits); // delay_start_sec_list
            if (rpVersion >= 113)
                EventStartV = reader.ReadInt32(); // event_start_v
            EventGoalV = reader.ReadInt32(); // event_goal_v
            EventGoalWidth = reader.ReadSByte(); // event_goal_width
            reader.ReadBoolBit(); // fixed_retention
            reader.ReadBits(4); // initial_retention10 (?)

            // entering weather zone
            reader.ReadBits(3); // decisive_weather
            WeatherTotalSec = reader.ReadInt16(); // weather points? confirm this
            reader.ReadBits(4); // unk (field_0xae & 0x3ff)
            reader.ReadBits(4); // unk (field_0xae & 0xfc00)

            for (int i = 1; i < 15; i++)
                reader.ReadBits(0x0c); // Do thing weather related with it

            for (int i = 0; i < 16; i++)
                reader.ReadBits(6); // Do thing weather related with it
            for (int i = 0; i < 16; i++)
                reader.ReadBits(6); // Again, and same function

            WeatherRandomSeed = reader.ReadInt32();
            WeatherNoPrecipitation = reader.ReadBoolBit(); //   weather_no_precipitation
            WeatherNoWind = reader.ReadBoolBit(); //   weather_no_wind
            WeatherPrecRainOnly = reader.ReadBoolBit(); //   weather_prec_rain_only
            WeatherPrecSnowOnly = reader.ReadBoolBit(); //   weather_prec_snow_only
            WeatherNoSchedule = reader.ReadBoolBit(); //   weather_no_schedule
            WeatherRandom = reader.ReadBoolBit(); //   weather_random
            reader.ReadBoolBit(); //   param_1->field_0xe0 = (uVar3 & 0x1) << 0x39 | param_1->field_0xe0 & 0xfdffffffffffffff;
            WeatherBaseCelsius = (sbyte)reader.ReadBits(7);
            WeatherMinCelsius = (sbyte)reader.ReadBits(4);
            WeatherMaxCelsius = (sbyte)reader.ReadBits(4);
            WeatherAccel = (short)reader.ReadBits(10);
            WeatherAccelWaterRetention = (short)reader.ReadBits(10);
            reader.ReadBits(6); //   param_1->field_0xe0 = (uVar3 & 0x3f) << 0x10 | param_1->field_0xe0 & 0xffffffffffc0ffff;

            if (rpVersion >= 123)
                reader.ReadBoolBit(); // unk

        }

        private void ReadBoostPenaltyParametersAndNewer(ref BitStream reader, int rpVersion)
        {
            reader.ReadByte(); // useLaunchData
            reader.ReadIntoByteArray(32, new byte[32], BitStream.Byte_Bits); // launch speed list
            reader.ReadIntoShortArray(32, new short[64], BitStream.Short_Bits); // launch_position_list
            reader.ReadIntoShortArray(32, new short[64], BitStream.Short_Bits); // start_type_slot_list

            // boost_table
            for (int i = 0; i < 2; i++)
            {
                var arr = new byte[4];
                reader.ReadIntoByteArray(4, arr, BitStream.Byte_Bits); // Front
                reader.ReadIntoByteArray(4, arr, BitStream.Byte_Bits); // Rear
                reader.ReadByte(); // boost_reference_rank
                reader.ReadByte(); // unk boost
            }

            // boost_params? Maybe?
            for (int i = 0; i < 32; i++)
            {
                reader.ReadIntoByteArray(6, new byte[6], BitStream.Byte_Bits); // Front
                reader.ReadIntoByteArray(6, new byte[6], BitStream.Byte_Bits); // Rear
            }

            reader.ReadByte(); // boost_level
            reader.ReadByte(); // rolling_player_grid
            reader.ReadByte(); // field_0x323
            reader.ReadByte(); // boost_flag
            reader.ReadByte(); // boost_type
            reader.ReadByte(); // disable_recording_replay
            reader.ReadByte(); // ghost_presence_type
            reader.ReadIntoByteArray(30, new byte[60], BitStream.Short_Bits); // event_v_list

            ReadPenaltyParameter(ref reader);

            var arr2 = new short[1];
            // Unk
            reader.ReadIntoShortArray(1, arr2, BitStream.Short_Bits); // field_0x3f0

            var arr3 = new byte[4];
            reader.ReadIntoByteArray(4, arr3, BitStream.Byte_Bits); // field_0x3f2
            reader.ReadByte(); // field_0x3f6
            reader.ReadByte(); // large_entry_max

            if (rpVersion >= 114)
                reader.ReadByte(); // pitstage_revision

            if (rpVersion >= 115)
                reader.ReadByte(); // vehicle_freeze_mode

            if (rpVersion >= 116)
                reader.ReadByte(); // course_out_penalty_margine

            if (rpVersion >= 118)
                reader.ReadInt32(); // behavior_fallback

            if (rpVersion >= 120)
            {
                // pilot stuff
                reader.ReadBits(1); //  param_1->pilot_commands = uVar2 << 0x3f | param_1->pilot_commands & 0x7fffffffffffffff;
                reader.ReadBits(7); //  param_1->pilot_commands = (uVar2 & 0x7f) << 0x38 | param_1->pilot_commands & 0x80ffffffffffffff;
            }
        }

        private void ReadPenaltyParameter(ref BitStream reader)
        {
            PenaltyParameter.Enable = reader.ReadBool(); // enable
            PenaltyParameter.CarCrashInterval = reader.ReadByte(); // car_crash_interval
            PenaltyParameter.ScoreSumThreshold = reader.ReadInt16(); // score_sum_threshold
            PenaltyParameter.CondType = reader.ReadByte(); // cond_type
            PenaltyParameter.RatioDiffMin = reader.ReadByte(); // ratio_diff_min
            PenaltyParameter.ScoreDiffMin = reader.ReadInt16(); // score_diff_min
            PenaltyParameter.CarImpactThreshold = reader.ReadByte(); // car_impact_threshold
            PenaltyParameter.CarImpactThreshold2 = reader.ReadByte(); // car_impact_threshold2
            PenaltyParameter.VelocityDirAngle0 = reader.ReadByte(); // velocity_dir_angle0
            PenaltyParameter.VelocityDirAngle1 = reader.ReadByte(); // velocity_dir_angle1
            PenaltyParameter.VelocityDirScore0 = reader.ReadInt16(); // velocity_dir_score0
            PenaltyParameter.VelocityDirScore1 = reader.ReadInt16(); // velocity_dir_score1
            PenaltyParameter.SteeringAngle0 = reader.ReadByte(); // steering_angle0
            PenaltyParameter.SteeringAngle1 = reader.ReadByte(); // steering_angle1
            PenaltyParameter.SpeedScore0 = reader.ReadInt16(); // speed_score0
            PenaltyParameter.SpeedScore1 = reader.ReadInt16(); // speed_score1
            PenaltyParameter.Speed0 = reader.ReadByte(); // speed0
            PenaltyParameter.Speed1 = reader.ReadByte(); // speed1
            reader.ReadInt16(); // unk field_0x18
            reader.ReadInt16(); // unk field_0x1a
            PenaltyParameter.BackwardAngle = reader.ReadByte(); // backward_angle
            PenaltyParameter.BackwardMoveRatio = reader.ReadByte(); // backward_move_ratio
            PenaltyParameter.WallImpactThreshold = reader.ReadByte(); // wall_impact_threshold
            PenaltyParameter.WallAlongTimer = reader.ReadByte(); // wall_along_timer
            PenaltyParameter.WallAlongCounter = reader.ReadByte(); // wall_along_counter
            PenaltyParameter.PunishSpeedLimit = reader.ReadByte(); // punish_speed_limit
            PenaltyParameter.PunishImpactThreshold0 = reader.ReadByte(); // punish_impact_threshold0
            PenaltyParameter.PunishImpactThreshold1 = reader.ReadByte(); // punish_impact_threshold1

            // Needs research
            for (int i = 0; i < 8; i++)
            {
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }

            PenaltyParameter.PunishCollision = reader.ReadBool(); // punish_collision
            PenaltyParameter.CollisionRecoverDelay = reader.ReadByte(); // collision_recover_delay
            PenaltyParameter.ShortcutRadius = reader.ReadInt16(); // shortcut_radius
            PenaltyParameter.ShortcutMinSpeed = reader.ReadByte(); // shortcut_min_speed
            PenaltyParameter.FreeCrashedbyAutodrive = reader.ReadBool(); // free_crashed_by_autodrive
            PenaltyParameter.FreeRatioByAutodrive = reader.ReadByte(); // free_ratio_by_autodrive
            PenaltyParameter.PitPenalty = reader.ReadBool(); // pit_penalty
            PenaltyParameter.SideSpeed0 = reader.ReadByte(); // side_speed0
            PenaltyParameter.SideSpeed1 = reader.ReadByte(); // side_speed1
            PenaltyParameter.SideSpeedScore0 = reader.ReadInt16(); // side_speed_score0
            PenaltyParameter.SideSpeedScore1 = reader.ReadInt16(); // side_speed_score1
            PenaltyParameter.ShortcutCancelTime1 = reader.ReadByte(); // shortcut_cancel_time1
            PenaltyParameter.ShortcutCancelTime0 = reader.ReadByte(); // shortcut_cancel_time0
            PenaltyParameter.CollisionOffScore0 = reader.ReadInt16(); // collision_off_score0
            PenaltyParameter.CollisionOffScore1 = reader.ReadInt16(); // collision_off_score1
            PenaltyParameter.CollisionOffScoreType = reader.ReadByte(); // collision_off_score_type
            reader.ReadByte(); // unk field_0x79
            PenaltyParameter.FreeLessRatio = reader.ReadByte(); // free_less_ratio
            PenaltyParameter.CancelSteeringAngleDiff = reader.ReadByte(); // cancel_steering_angle_diff
            PenaltyParameter.CollisionOffDispScore0 = reader.ReadInt16(); // collision_off_disp_score0
            PenaltyParameter.ShortcutCancelInJamSpeed = reader.ReadByte(); // shortcut_cancel_in_jam_speed_ratio
            reader.ReadByte(); // unk field_0x7f
            PenaltyParameter.SteeringScoreRatioMin = reader.ReadByte(); // steering_score_ratio_min
            PenaltyParameter.SteeringScoreRatioMax = reader.ReadByte(); // steering_score_ratio_max
            PenaltyParameter.WallImpactThreshold = reader.ReadByte(); // wall_impact_threshold0
            PenaltyParameter.ShortcutRatio = reader.ReadByte(); // shortcut_ratio
            PenaltyParameter.SideSpeed0Steering = reader.ReadByte(); // side_speed0_steering
            PenaltyParameter.SideSpeed1Steering = reader.ReadByte(); // side_speed1_steering
            PenaltyParameter.SideSpeedSteeringScore0 = reader.ReadByte(); // side_speed_steering_score0
            PenaltyParameter.SideSpeedSteeringScore1 = reader.ReadByte(); // side_speed_steering_score1
            PenaltyParameter.ShortcutType = reader.ReadByte(); // shortcut_type
            PenaltyParameter.PenaSpeedRatio1 = reader.ReadByte(); // pena_speed_ratio1
            PenaltyParameter.PenaSpeedRatio2 = reader.ReadByte(); // pena_speed_ratio2
            PenaltyParameter.PenaSpeedRatio3 = reader.ReadByte(); // pena_speed_ratio3
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteInt16(0x2BB); // Buffer Size.
            bs.WriteInt16(1_23); // Version

            WriteGeneralSettings(ref bs);
            WriteOtherSettings(ref bs);
        }

        private void WriteGeneralSettings(ref BitStream bs)
        {
            bs.WriteBits((ulong)SessionType, 3);
            bs.WriteByte((byte)RaceType);
            bs.WriteByte((byte)StartType);
            bs.WriteByte((byte)CompleteType);
            bs.WriteByte((byte)FinishType);
            bs.WriteInt16(LapCount);
            bs.WriteInt16(MinutesCount);
            bs.WriteInt32((int)TimeToStart.TotalMilliseconds);
            bs.WriteInt32((int)TimeToFinish.TotalMilliseconds);
            bs.WriteInt16(RacersMax); // EntryMax
            bs.WriteInt16(16); // ? field_0x16 - Related to racers max
            bs.WriteBits(0, 7); // Unk
            bs.WriteInt32(0); // Unk
            bs.WriteByte(0); // CourseLayoutNo
            bs.WriteByte(0); // Race Initial Laps
            bs.WriteBoolBit(KeepLoadGhost);
            bs.WriteInt32(0); // course_code
            bs.WriteByte((byte)(LineGhostPlayMax ?? -1)); // Line Ghost Play Max
            bs.WriteBoolBit(GoalTimeUseLapTotal);
            bs.WriteBits(0, 2); // Intentional
            bs.WriteBoolBit(false); // force_pitcrew_off
            bs.WriteInt32(0); // scenery_code
            bs.WriteInt32(0); // Unk field_0x3c
            bs.WriteInt64(0); // unk field_0x40
            bs.WriteInt16(4000); // packet_timeout_interval
            bs.WriteInt16(4000); // packet_timeout_latency
            bs.WriteInt64(4000); // packet_timeout_lag
            bs.WriteInt16(RacersMax);
            bs.WriteInt32(0); // unk field 0x2c
            bs.WriteBoolBit(AutostartPitout);
            bs.WriteBits(StartTimeOffset, 5);
            bs.WriteBits(0, 3); // unk
            bs.WriteByte(AutoStandingDelay);
            bs.WriteBits(0, 3); // Unk
            bs.WriteBits(0/*StartSignalType*/, 2);
            bs.WriteBoolBit(false); // Unk
            bs.WriteBoolBit(false); // bench_test
            bs.WriteByte(0); // mu_ratio100
            bs.WriteBoolBit(false); // unk
            bs.WriteBoolBit(EnableDamage);
            bs.WriteBits(0, 2); // low_mu_type
            bs.WriteBits((ulong)BehaviorDamage, 2);
            bs.WriteBoolBit(false); // gps
            bs.WriteBoolBit(PenaltyNoReset);
            bs.WriteBits((ulong)SlipstreamBehavior, 2);
            bs.WriteBits(0, 4); // pit_constraint
            bs.WriteBoolBit(false); // need_tire_change
            bs.WriteBits(0, 4); // after_race_penalty_sec5
            bs.WriteBoolBit(false); // is_speedtest_milemode
            bs.WriteBits((ulong)LineGhostRecordType, 2);
            bs.WriteBits(0, 2); // attack_seperate_type
            bs.WriteByte((byte)PenaltyLevel);
            bs.WriteBoolBit(false); // auto_start_with_session
            bs.WriteBoolBit(false); // auto_end_with_finish
            bs.WriteBoolBit(ImmediateFinish);
            bs.WriteBoolBit(OnlineOn);
            bs.WriteBoolBit(Endless);
            bs.WriteBits(0, 2); // use grid list
            bs.WriteByte((byte)GhostType);
            bs.WriteByte((byte)GridSortType);
            bs.WriteBool(Accumulation);
            bs.WriteBoolBit(EnablePit);
            bs.WriteByte((byte)Flagset);
            bs.WriteBoolBit(false); // Unk
            bs.WriteBoolBit(DisableCollision);
            bs.WriteByte(0); // penalty_condition
            bs.WriteBool(AcademyEvent);
            bs.WriteByte(FuelUseMultiplier);
            bs.WriteByte(0); // bspec_vitality_10
            bs.WriteByte(0); // consideration_type
            bs.WriteByte(TireUseMultiplier);
            bs.WriteByte(0); // temperature_tire
            bs.WriteByte(0); // temperature_engine
            bs.WriteByte(0); // unk field_0x5a
            bs.WriteByte((byte)LightingMode);

            // datetime is written seperately normally, we do it in one go of 1 uint
            bs.WriteUInt32(0);
            bs.WriteByte((byte)TimeProgressSpeed);
            bs.WriteBool(AllowCoDriver);
            bs.WriteBool(PaceNote);
            bs.WriteByte(0); // team_count, not in xmls

            for (int i = 0; i < 32; i++)
                bs.WriteByte(GridList[i]);
            for (int i = 0; i < 32; i++)
                bs.WriteByte(DelayStartList[i]);

            bs.WriteInt32(EventStartV ?? -1);
            bs.WriteInt32(EventGoalV ?? -1);
            bs.WriteSByte(EventGoalWidth ?? -1);

            bs.WriteBoolBit(FixedRetention);
            bs.WriteBits((ulong)TrackWetness, 4); // initial_retention10
            bs.WriteBits((ulong)DecisiveWeather, 3);

            bs.WriteInt16(WeatherTotalSec);
            bs.WriteBits(WeatherPointNum, 4);
            bs.WriteBits(WeatherPointNum, 4); // Again Maybe?

            // Stub while figured
            for (int i = 1; i < 15; i++)
                bs.WriteBits(0, 0x0c); // Do thing weather related with it

            for (int i = 0; i < 16; i++)
                bs.WriteBits(0, 6); // Do thing weather related with it
            for (int i = 0; i < 16; i++)
                bs.WriteBits(0, 6); // Again, and same function

            bs.WriteInt32(WeatherRandomSeed);
            bs.WriteBoolBit(WeatherNoPrecipitation);
            bs.WriteBoolBit(WeatherNoWind);
            bs.WriteBoolBit(WeatherPrecRainOnly);
            bs.WriteBoolBit(WeatherPrecSnowOnly);
            bs.WriteBoolBit(WeatherNoSchedule);
            bs.WriteBoolBit(WeatherRandom);
            bs.WriteBoolBit(false); // Unk
            bs.WriteBits((ulong)WeatherBaseCelsius, 7); // weather_base_celsius
            bs.WriteBits((ulong)WeatherMinCelsius, 4);
            bs.WriteBits((ulong)WeatherMaxCelsius, 4);
            bs.WriteBits((ulong)WeatherAccel, 10); //   weather_accel10
            bs.WriteBits((ulong)WeatherAccelWaterRetention, 10); // weather_accel_water_retention10
            bs.WriteBits(42, 6); //   param_1->field_0xe0 = (uVar3 & 0x3f) << 0x10 | param_1->field_0xe0 & 0xffffffffffc0ffff; - Unk
            bs.WriteBoolBit(false); // Unk
        }

        private void WriteOtherSettings(ref BitStream bs)
        {
            bs.WriteByte(0); // Use launch data
            for (int i = 0; i < 32; i++)
                bs.WriteByte(LaunchSpeedList[i]);
            for (int i = 0; i < 32; i++)
                bs.WriteInt16(LaunchPositionList[i]);
            for (int i = 0; i < 32; i++)
                bs.WriteInt16(StartTypeSlotList[i]);

            for (int i = 0; i < 2; i++)
            {
                bs.WriteByte(BoostTables[i].FrontLimit);
                bs.WriteSByte(BoostTables[i].FrontMaximumRate);
                bs.WriteByte(BoostTables[i].FrontStart);
                bs.WriteSByte(BoostTables[i].FrontInitialRate);

                bs.WriteByte(BoostTables[i].RearLimit);
                bs.WriteSByte(BoostTables[i].RearMaximumRate);
                bs.WriteByte(BoostTables[i].RearStart);
                bs.WriteSByte(BoostTables[i].RearInitialRate);

                bs.WriteByte(BoostTables[i].ReferenceRank);
                bs.WriteByte(0);
            }

            bs.WriteByte(BoostLevel);
            bs.WriteBool(RollingPlayerGrid);
            bs.WriteBool(false); // Unk field_0x323
            bs.WriteBool(BoostFlag);
            bs.WriteByte(BoostType);
            bs.WriteBool(DisableRecordingReplay);
            bs.WriteByte((byte)GhostPresenceType);

            for (int i = 0; i < 30; i++)
                bs.WriteInt32(EventVList[i]);

            // Write Penalty Parameter
            bs.WriteBool(PenaltyParameter.Enable);
            bs.WriteByte(PenaltyParameter.CarCrashInterval);
            bs.WriteInt16(PenaltyParameter.ScoreSumThreshold);
            bs.WriteByte(PenaltyParameter.CondType);
            bs.WriteByte(PenaltyParameter.RatioDiffMin);
            bs.WriteInt16(PenaltyParameter.ScoreDiffMin);
            bs.WriteByte(PenaltyParameter.CarImpactThreshold);
            bs.WriteByte(PenaltyParameter.CarImpactThreshold2);
            bs.WriteByte(PenaltyParameter.VelocityDirAngle0);
            bs.WriteByte(PenaltyParameter.VelocityDirAngle1);
            bs.WriteInt16(PenaltyParameter.VelocityDirScore0);
            bs.WriteInt16(PenaltyParameter.VelocityDirScore1);
            bs.WriteByte(PenaltyParameter.SteeringAngle0);
            bs.WriteByte(PenaltyParameter.SteeringAngle1);
            bs.WriteInt16(PenaltyParameter.SpeedScore0);
            bs.WriteInt16(PenaltyParameter.SpeedScore1);
            bs.WriteByte(PenaltyParameter.Speed0);
            bs.WriteByte(PenaltyParameter.Speed1);
            bs.WriteInt16(PenaltyParameter.unk0);
            bs.WriteInt16(PenaltyParameter.unk1);
            bs.WriteByte(PenaltyParameter.BackwardAngle);
            bs.WriteByte(PenaltyParameter.BackwardMoveRatio);
            bs.WriteByte(PenaltyParameter.WallImpactThreshold);
            bs.WriteByte(PenaltyParameter.WallAlongTimer);
            bs.WriteByte(PenaltyParameter.WallAlongCounter);
            bs.WriteByte(PenaltyParameter.PunishSpeedLimit);
            bs.WriteByte(PenaltyParameter.PunishImpactThreshold0);
            bs.WriteByte(PenaltyParameter.PunishImpactThreshold1);

            for (int i = 0; i < 8; i++)
            {
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk1);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk2);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk3);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk4);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk5);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk6);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk7);
                bs.WriteByte(PenaltyParameter.UnkPenaltyDatas[i].unk8);
            }

            bs.WriteBool(PenaltyParameter.PunishCollision);
            bs.WriteByte(PenaltyParameter.CollisionRecoverDelay);
            bs.WriteInt16(PenaltyParameter.ShortcutRadius);
            bs.WriteByte(PenaltyParameter.ShortcutMinSpeed);
            bs.WriteBool(PenaltyParameter.FreeCrashedbyAutodrive);
            bs.WriteByte(PenaltyParameter.FreeRatioByAutodrive);
            bs.WriteBool(PenaltyParameter.PitPenalty);
            bs.WriteByte(PenaltyParameter.SideSpeed0);
            bs.WriteByte(PenaltyParameter.SideSpeed1);
            bs.WriteInt16(PenaltyParameter.SideSpeedScore0);
            bs.WriteInt16(PenaltyParameter.SideSpeedScore1);
            bs.WriteByte(PenaltyParameter.ShortcutCancelTime0);
            bs.WriteByte(PenaltyParameter.ShortcutCancelTime1);
            bs.WriteInt16(PenaltyParameter.CollisionOffScore0);
            bs.WriteInt16(PenaltyParameter.CollisionOffScore1);
            bs.WriteByte(PenaltyParameter.CollisionOffScoreType);
            bs.WriteByte(0);
            bs.WriteByte(PenaltyParameter.FreeLessRatio);
            bs.WriteByte(PenaltyParameter.CancelSteeringAngleDiff);
            bs.WriteInt16(PenaltyParameter.CollisionOffDispScore0);
            bs.WriteByte(PenaltyParameter.ShortcutCancelInJamSpeed);
            bs.WriteByte(0);
            bs.WriteByte(PenaltyParameter.SteeringScoreRatioMin);
            bs.WriteByte(PenaltyParameter.SteeringScoreRatioMax);
            bs.WriteByte(PenaltyParameter.WallImpactThreshold0);
            bs.WriteByte(PenaltyParameter.ShortcutRatio);
            bs.WriteByte(PenaltyParameter.SideSpeed0Steering);
            bs.WriteByte(PenaltyParameter.SideSpeed1Steering);
            bs.WriteByte(PenaltyParameter.SideSpeedSteeringScore0);
            bs.WriteByte(PenaltyParameter.SideSpeedSteeringScore1);
            bs.WriteByte(PenaltyParameter.ShortcutType);
            bs.WriteByte(PenaltyParameter.PenaSpeedRatio1);
            bs.WriteByte(PenaltyParameter.PenaSpeedRatio2);
            bs.WriteByte(PenaltyParameter.PenaSpeedRatio3);

            bs.WriteInt16(0); // field_0x3f0 - Array of 1
            for (int i = 0; i < 4; i++)
                bs.WriteByte(0); // field_0x3f2 - Array of 4

            bs.WriteByte(0); // field_0x3f6
            bs.WriteByte(0); // large_entry_max
            bs.WriteByte(0); // Pitstage_revision
            bs.WriteByte(0); // vehicle_freeze_mode
            bs.WriteByte(0); // course_out_penalty_margine
            bs.WriteInt32(0); // behavior_fallback

            bs.WriteBits(0, 1);
            bs.WriteBits(0, 7);

        }
    }

    public enum BehaviorDamageType
    {
        [Description("None")]
        WEAK,

        [Description("Light")]
        MIDDLE,

        [Description("Heavy")]
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

    public enum SessionType
    {
        FINAL,
        QUALIFY,
        PRACTICE,
    }
}
