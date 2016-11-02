using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    [CompositeUnique(nameof(SeriesId), nameof(UnitName))]
    public class Engine
    {
        #region Columns

        /// <summary>
        /// The Unique Engine ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="EngineSeries"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int SeriesId { get; set; }

        /// <summary>
        /// Gets or Sets the unique unit name for this engine
        /// </summary>
        [Column, Required]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this engine
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the buying price for this engine
        /// </summary>
        [Column, Required]
        public int Price { get; set; }

        /// <summary>
        /// Gets or Sets the Players level at which this engine will
        /// be available for purchase
        /// </summary>
        [Column, Default(0)]
        public int Unlock { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the Horsepower limit for this engine
        /// </summary>
        [Column, Required]
        public int Horsepower { get; set; }

        /// <summary>
        /// Gets or Sets the Torque rating for this engine
        /// </summary>
        [Column, Required]
        public int Torque { get; set; }

        /// <summary>
        /// Gets or Sets the Idle RPM for this engine
        /// </summary>
        [Column, Required, Default(550)]
        public int IdleRpm { get; set; } = 550;

        /// <summary>
        /// Gets or Sets the Peak RPM for torque level
        /// </summary>
        [Column, Required, Default(1200)]
        public int PeakRpm { get; set; } = 1200;

        /// <summary>
        /// Gets or Sets the Governed RPM limit for this engine
        /// </summary>
        [Column, Required, Default(2200)]
        public int RpmLimit { get; set; } = 2200;

        /// <summary>
        /// Gets or Sets the Governed RPM limit for this engine in Neutral gear
        /// </summary>
        [Column, Required, Default(2200)]
        public int RpmLimitNeutral { get; set; } = 2200;

        /// <summary>
        /// Gets or Sets the engine brake strength
        /// </summary>
        [Column, Required, Default("1.0", Quote = false)]
        public decimal BrakeStrength { get; set; } = 1.0m;

        /// <summary>
        /// Gets or Sets whether automatic transmissions will downshift when
        /// the engine brake is applied
        /// </summary>
        [Column, Required, Default(false)]
        public bool BrakeDownshift { get; set; } = false;

        /// <summary>
        /// Gets or Sets the number of Engine brake positions available.
        /// </summary>
        [Column, Required, Default(3)]
        public int BrakePositions { get; set; } = 3;

        /// <summary>
        /// Defines the minimum RPM range that is used by automatic transmissions in low gear
        /// </summary>
        [Column, Required, Default(800)]
        public int MinRpmRange_LowGear { get; set; } = 800;

        /// <summary>
        /// Defines the miaximum RPM range that is used by automatic transmissions in low gear
        /// </summary>
        [Column, Required, Default(1600)]
        public int MaxRpmRange_LowGear { get; set; } = 1600;

        /// <summary>
        /// Defines the minimum RPM range that is used by automatic transmissions in high gear
        /// </summary>
        [Column, Required, Default(1100)]
        public int MinRpmRange_HighGear { get; set; } = 1100;

        /// <summary>
        /// Defines the miaximum RPM range that is used by automatic transmissions in high gear
        /// </summary>
        [Column, Required, Default(1600)]
        public int MaxRpmRange_HighGear { get; set; } = 1600;

        /// <summary>
        /// Gets or Sets a value that adjusts the RPM shifting range based on throttle
        /// </summary>
        /// <remarks>
        /// Transmission-behavior option used for increasing shifting intervals at throttle, 
        /// with enabled "adaptive automatic transmission" option
        /// </remarks>
        /// <seealso cref="http://forum.scssoft.com/viewtopic.php?f=34&t=186532&start=250&hilit=GearboxMOD+2.0#p573401"/>
        [Column, Required, Default(450)]
        public int LowRpmRange_PowerBoost { get; set; } = 450;

        /// <summary>
        /// Gets or Sets a value that adjusts the RPM shifting range based on throttle
        /// </summary>
        /// <remarks>
        /// Transmission-behavior option used for increasing shifting intervals at throttle, 
        /// with enabled "adaptive automatic transmission" option
        /// </remarks>
        /// <seealso cref="http://forum.scssoft.com/viewtopic.php?f=34&t=186532&start=250&hilit=GearboxMOD+2.0#p573401"/>
        [Column, Required, Default(350)]
        public int HighRpmRange_PowerBoost { get; set; } = 350;

        /// <summary>
        /// Gets or Sets a value that does ??
        /// </summary>
        [Column, Required, Default(0)]
        public int LowRpmRange_EngineBrake { get; set; } = 0;

        /// <summary>
        /// Gets or Sets a value that does ??
        /// </summary>
        [Column, Required, Default(0)]
        public int HighRpmRange_EngineBrake { get; set; } = 0;

        [Column, Required, Default(0)]
        public decimal AdblueConsumption { get; set; } = 0.00m;

        [Column, Required, Default(0)]
        public decimal NoAdbluePowerLimit { get; set; } = 0.00m;

        /// <summary>
        /// 
        /// </summary>
        [Column("Defaults"), Default("")]
        protected string _defaults { get; set; }

        [Column("Comment"), Default("")]
        protected string _comment { get; set; }

        [Column("Conflicts"), Default("")]
        protected string _conflicts { get; set; }

        [Column("SuitableFor"), Default("")]
        protected string _suitables { get; set; }

        /// <summary>
        /// The name of the SII file, without extension.
        /// </summary>
        protected string fileName = null;

        /// <summary>
        /// Gets or Sets the output file name for the engine's SII file.
        /// </summary>
        [Column, Required]
        public string FileName
        {
            get
            {
                return fileName ?? String.Concat(UnitName, ".sii");
            }
            set
            {
                // Ensure we have a file extension
                fileName = (!Path.HasExtension(value)) ? String.Concat(value, ".sii") : value;
            }
        }

        #endregion Columns

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.EngineSeries"/> entity that this engine references.
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("SeriesId", 
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        public virtual EngineSeries Series { get; private set; }

        #endregion


        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="TorqueRatio"/> entities that reference this 
        /// <see cref="Engine"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<TorqueRatio> TorqueRatios { get; set; }

        /// <summary>
        /// Gets a list of <see cref="TruckEngine"/> entities that reference this 
        /// <see cref="Engine"/>
        /// </summary>
        public virtual IEnumerable<TruckEngine> ItemOf { get; set; }

        /// <summary>
        /// Gets a list of <see cref="AccessoryConflict"/> entities that reference this 
        /// <see cref="Engine"/>
        /// </summary>
        public virtual IEnumerable<AccessoryConflict> TransmissionConflicts { get; set; }

        /// <summary>
        /// Gets a list of <see cref="AccessoryConflict"/> entities that reference this 
        /// <see cref="Engine"/>
        /// </summary>
        public virtual IEnumerable<SuitableAccessory> SuitableTransmissions { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the Newton Metre rating for this engine. Changing this value
        /// will also change the <see cref="Engine.Torque"/> value
        /// </summary>
        public int NewtonMetres
        {
            get { return Metrics.TorqueToNewtonMetres(Torque); }
            set { Torque = Metrics.NewtonMetresToTorque(value); }
        }

        /// <summary>
        /// Gets or sets the Kilowatt rating for this engine's horsepower rating.
        /// Changing this value will also change the <see cref="Engine.Horsepower"/> 
        /// value
        /// </summary>
        public int Kilowatts
        {
            get { return Metrics.HorsepowerToKilowatts(Horsepower); }
            set { Horsepower = Metrics.KilowattsToHorsepower(value); }
        }

        /// <summary>
        /// Contains an array of Defaults for the truck to load
        /// </summary>
        public string[] Defaults
        {
            get
            {
                if (String.IsNullOrEmpty(_defaults))
                    return null;

                return _defaults.Split('|');
            }
            set
            {
                if (value == null || value.Length == 0)
                    _defaults = "";
                else
                    _defaults = String.Join("|", value);
            }
        }

        /// <summary>
        /// Gets or Sets the Engine objects comment in the SII file.
        /// </summary>
        public string[] Comment
        {
            get
            {
                if (String.IsNullOrEmpty(_comment))
                    return null;

                return _comment.Split('|');
            }
            set
            {
                if (value == null || value.Length == 0)
                    _comment = "";
                else
                    _comment = String.Join("|", value);
            }
        }

        /// <summary>
        /// Gets or Sets the Engine objects comment in the SII file.
        /// </summary>
        public string[] Conflicts
        {
            get
            {
                if (String.IsNullOrEmpty(_conflicts))
                    return null;

                return _conflicts.Split('|');
            }
            set
            {
                if (value == null || value.Length == 0)
                    _conflicts = "";
                else
                    _conflicts = String.Join("|", value);
            }
        }

        /// <summary>
        /// Gets or Sets the Engine objects comment in the SII file.
        /// </summary>
        public string[] SuitableFor
        {
            get
            {
                if (String.IsNullOrEmpty(_suitables))
                    return null;

                return _suitables.Split('|');
            }
            set
            {
                if (value == null || value.Length == 0)
                    _suitables = "";
                else
                    _suitables = String.Join("|", value);
            }
        }

        #endregion Properties

        public override string ToString() => Name;

        /// <summary>
        /// Serializes this engine into SII format, and returns the result
        /// </summary>
        /// <returns></returns>
        public string ToSiiFormat(string truckName)
        {
            // Create local variables
            EngineSeries series = this.Series;
            SiiFileBuilder builder = new SiiFileBuilder();
            string name = $"{this.UnitName}.{truckName}.engine";
            string decvalue;

            // Make sure we have a file comment
            if (Comment == null || Comment.Length == 0)
                Comment = new string[] { "Generated with the ATS Engine Generator Tool by Wilson212" };

            // Write file intro
            builder.WriteStartDocument();

            // Write file comment
            builder.WriteCommentBlock(Comment);

            // Begin the engine accessory
            builder.WriteStructStart("accessory_engine_data", name);

            // Generic Info
            builder.WriteAttribute("name", this.Name);
            builder.WriteAttribute("price", this.Price, "Engine price", 1);
            builder.WriteAttribute("unlock", this.Unlock, "Unlocks @ Level", 2);
            builder.WriteLine();

            // Horsepower line
            builder.WriteLine("# Engine display info");
            builder.WriteAttribute("info[]", $"{Digitize(this.Horsepower)} @@hp@@ ({Digitize(this.Kilowatts)}@@kw@@)");

            // Torque line
            builder.Write("info[]: \"");
            if (Program.Config.TorqueOutputUnitSystem == UnitSystem.Imperial)
                builder.WriteLine($"{Digitize(this.Torque)} @@lb_ft@@ ({Digitize(this.NewtonMetres)} @@nm@@)\"");
            else
                builder.WriteLine($"{Digitize(this.NewtonMetres)} @@nm@@ ({Digitize(this.Torque)} @@lb_ft@@)\"");

            // Rpm line
            builder.WriteAttribute($"info[]", $"{Digitize(this.PeakRpm)} @@rpm@@");

            // Icon
            builder.WriteAttribute($"icon", series.EngineIcon);
            builder.WriteLine();

            // Performance
            builder.WriteLine("# Engine Specs");
            builder.WriteAttribute("torque", this.NewtonMetres, "Engine power in Newton-metres");
            builder.WriteAttribute("volume", series.Displacement, "Engine size in liters. Used for Realistic Fuel Consumption settings");
            builder.WriteLine();

            // Torque Curves
            builder.WriteLine("# Torque Curves");
            foreach (TorqueRatio ratio in TorqueRatios.OrderBy(x => x.RpmLevel))
            {
                decvalue = ratio.Ratio.ToString(Program.NumberFormat);
                builder.WriteAttribute($"torque_curve[]", $"({ratio.RpmLevel}, {decvalue})", false);
            }
            builder.WriteLine();

            // RPM datas
            builder.WriteLine("# RPM Data");
            builder.WriteAttribute("rpm_idle", this.IdleRpm, "RPM at idle", 3);
            builder.WriteAttribute("rpm_limit", this.RpmLimit, "Governed RPM limit", 3);
            builder.WriteAttribute("rpm_limit_neutral", this.RpmLimitNeutral, "RPM limit in neutral gear");
            builder.WriteAttribute("rpm_range_low_gear", $"({this.MinRpmRange_LowGear}, {this.MaxRpmRange_LowGear})", false);
            builder.WriteAttribute("rpm_range_high_gear", $"({this.MinRpmRange_HighGear}, {this.MaxRpmRange_HighGear})", false);
            builder.WriteAttribute("rpm_range_power_boost", $"({this.LowRpmRange_PowerBoost}, {this.HighRpmRange_PowerBoost})", false);

            if (HighRpmRange_EngineBrake > 0)
            {
                builder.WriteAttribute("rpm_range_engine_brake", $"({this.LowRpmRange_EngineBrake}, {this.HighRpmRange_EngineBrake})", false);
            }

            // Engine Brake
            string val = this.BrakeDownshift ? "1" : "0";
            decvalue = this.BrakeStrength.ToString("0.0", Program.NumberFormat);
            builder.WriteLine();
            builder.WriteLine("# Engine Brake data");
            builder.WriteAttribute("engine_brake", decvalue, false, "Engine Brake Strength", 3);
            builder.WriteAttribute("engine_brake_downshift", val, false, "Enable automatic downshift for Engine Brake");
            builder.WriteAttribute("engine_brake_positions", this.BrakePositions, "The number of engine brake intensities");
            builder.WriteLine();

            // AdBlue
            if (this.AdblueConsumption > 0.00m || this.NoAdbluePowerLimit > 0.00m)
            {
                builder.WriteLine("\t\t# Adblue Settings");
                if (this.AdblueConsumption > 0.00m)
                    builder.WriteAttribute("adblue_consumption", this.AdblueConsumption);

                if (this.NoAdbluePowerLimit > 0.00m)
                    builder.WriteAttribute("no_adblue_power_limit", this.NoAdbluePowerLimit);

                builder.WriteLine();
            }

            // Sound Data
            SoundPackage sound = series.SoundPackage;
            string intpath = "/def/vehicle/truck/{{{NAME}}}/sound/" + sound.InteriorFileName;
            string extpath = "/def/vehicle/truck/{{{NAME}}}/sound/" + sound.ExteriorFileName;

            builder.WriteLine("# Sound Data");
            builder.WriteAttribute("defaults[]", intpath);
            builder.WriteAttribute("defaults[]", extpath);

            // Write the default[]...
            if (Defaults != null && Defaults.Length > 0)
            {
                builder.WriteLine();
                builder.WriteLine("# Attachments");
                foreach (string line in Defaults)
                    builder.WriteAttribute($"defaults[]", line);
            }

            // Define is we output suitible_for and conflict_with for transmissions
            bool writeSuitables = Program.Config.CompileOption == CompileOption.EngineOnly
                               || Program.Config.CompileOption == CompileOption.Both;
            var conflicts = this.TransmissionConflicts.ToList();
            var suitables = this.SuitableTransmissions.ToList();

            // Write the conflict_with[]...
            if (conflicts.Count > 0 || (Conflicts != null && Conflicts.Length > 0))
            {
                builder.WriteLine();
                builder.WriteLine("# Conflicts");

                // Transmissions
                foreach (string line in conflicts.Select(x => x.Transmission.UnitName))
                    builder.WriteAttribute("conflict_with[]", $"{line}.{truckName}.transmission");

                // Other Conflicts
                if (Conflicts != null)
                    foreach (string line in Conflicts)
                        builder.WriteAttribute("conflict_with[]", line);
            }

            // Write the suitable_for[]...
            if ((writeSuitables && suitables.Count > 0) || (SuitableFor != null && SuitableFor.Length > 0))
            {
                builder.WriteLine();
                builder.WriteLine("# Suitables");

                // Transmissions?
                if (writeSuitables)
                    foreach (string trans in suitables.Select(x => x.Transmission.UnitName))
                        builder.WriteAttribute("suitable_for[]", $"{trans}.{truckName}.transmission");

                // Other Suitables
                if (SuitableFor != null)
                    foreach (string line in SuitableFor)
                        builder.WriteAttribute("suitable_for[]", line);
            }

            // Close brackets
            builder.WriteEndDocument();

            // Define file paths
            return builder.ToString().Replace("{{{NAME}}}", truckName).TrimEnd();
        }

        /// <summary>
        /// Converts a decimal value into a number with a "@@dg@@"
        /// to seperate the thousands places.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Digitize(decimal value)
        {
            return ((int)value).ToString("N0", Program.NumberFormat).Replace(",", "@@dg@@");
        }

        public override bool Equals(object obj)
        {
            if (obj is Engine)
            {
                Engine compare = (Engine)obj;
                return compare.Id == this.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
