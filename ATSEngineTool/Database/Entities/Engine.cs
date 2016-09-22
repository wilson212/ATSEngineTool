using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    [CompositeUnique("SeriesId", "UnitName")]
    public class Engine
    {
        #region Columns

        /// <summary>
        /// The Unique Engine ID
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.EngineSeries"/> object
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
        [Column, Required, Default(1.0)]
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
        /// 
        /// </summary>
        [Column("Defaults"), Default("")]
        protected string _defaults { get; set; }

        [Column("Comment"), Default("")]
        protected string _comment { get; set; }

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
                if (!Path.HasExtension(value))
                {
                    fileName = String.Concat(value, ".sii");
                }
                else
                {
                    fileName = value;
                }
            }
        }

        #endregion Columns

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.EngineSeries"/> entity that this engine references.
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("SeriesId", OnDelete = ReferentialIntegrity.Cascade)]
        public virtual EngineSeries Series { get; protected set; }

        #endregion
  

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="TorqueRatio"/> entities that reference this 
        /// <see cref="Engine"/>
        /// </summary>
        public virtual IEnumerable<TorqueRatio> TorqueRatios { get; set; }

        /// <summary>
        /// Gets a list of <see cref="TruckEngine"/> entities that reference this 
        /// <see cref="Engine"/>
        /// </summary>
        public virtual IEnumerable<TruckEngine> ItemOf { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// English culture for numbers
        /// </summary>
        protected CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Gets or Sets the NewtonMetre value for this engine. Changing this value
        /// will also change the <see cref="Torque"/> setting
        /// </summary>
        public int NewtonMetres
        {
            get
            {
                decimal div = 1.35582m;
                return (int)Math.Round(Torque * div, 0);
            }
            set
            {
                decimal div = 0.7376m;
                Torque = (int)Math.Round(value * div, 0);
            }
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

        #endregion Properties

        public override string ToString() => Name;

        /// <summary>
        /// Serializes this engine into SII format, and returns the result
        /// </summary>
        /// <returns></returns>
        public string ToSiiFormat()
        {
            // === Load engine.sii template
            string file = Path.Combine(Program.RootPath, "engine.sii");
            string contents = String.Empty;
            if (File.Exists(file))
            {
                contents = File.ReadAllText(file);
            }
            else
            {
                contents = Program.GetResourceAsString("ATSEngineTool.engine.sii");
                using (FileStream stream = File.Create(file))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(contents);
                }
            }

            // Create local variables
            EngineSeries series = this.Series;
            StringBuilder builder = new StringBuilder();

            // Make sure we have a file comment
            if (Comment == null || Comment.Length == 0)
            {
                Comment = new string[]
                {
                    "Generated with the ATS Engine Generator Tool by Wilson212"
                };
            }

            // Write file comment
            foreach (string line in Comment)
            {
                if (builder.Length != 0)
                    builder.Append("\t * ");
                builder.AppendLine(line);
            }
            contents = contents.Replace("{{{COMMENT}}}", builder.ToString().TrimEnd());

            // Generic Info
            contents = contents.Replace("{{{ENG_NAME}}}", this.UnitName);
            contents = contents.Replace("{{{ENG_TEXT}}}", this.Name);
            contents = contents.Replace("{{{PRICE}}}", this.Price.ToString());
            contents = contents.Replace("{{{UNLOCK}}}", this.Unlock.ToString());

            // Horsepower line
            contents = contents.Replace("{{{HP_INFO}}}",
                $"{Digitize(this.Horsepower)} @@hp@@ ({Digitize(HorsepowerToKilowatts(this.Horsepower))}@@kw@@)");

            // Torque line
            contents = contents.Replace("{{{TRQ_INFO}}}",
                $"{Digitize(this.Torque)} @@lb_ft@@ ({Digitize(TorqueToNm(this.Torque))} @@nm@@)");

            // Rpm line
            contents = contents.Replace("{{{RPM_INFO}}}", $"{Digitize(this.PeakRpm)} @@rpm@@");

            // Icon
            contents = contents.Replace("{{{ICON}}}", series.EngineIcon);

            // Performance
            contents = contents.Replace("{{{TRQ}}}", TorqueToNm(this.Torque).ToString());
            contents = contents.Replace("{{{RPM}}}", this.RpmLimit.ToString());
            contents = contents.Replace("{{{RPM_IDLE}}}", this.IdleRpm.ToString());
            contents = contents.Replace("{{{RPM_NEUTRAL}}}", this.RpmLimitNeutral.ToString());
            contents = contents.Replace("{{{VOLUME}}}", series.Displacement.ToString());

            // RPM Ranges
            contents = contents.Replace("{{{RPM_RANGE_LOW_LOW}}}", this.MinRpmRange_LowGear.ToString());
            contents = contents.Replace("{{{RPM_RANGE_HIGH_LOW}}}", this.MinRpmRange_HighGear.ToString());
            contents = contents.Replace("{{{RPM_RANGE_LOW_HIGH}}}", this.MaxRpmRange_LowGear.ToString());
            contents = contents.Replace("{{{RPM_RANGE_HIGH_HIGH}}}", this.MaxRpmRange_HighGear.ToString());

            contents = contents.Replace("{{{RPM_RANGE_POWER_LOW}}}", this.LowRpmRange_PowerBoost.ToString());
            contents = contents.Replace("{{{RPM_RANGE_POWER_HIGH}}}", this.HighRpmRange_PowerBoost.ToString());

            // Ebrake
            contents = contents.Replace("{{{EBRK_STR}}}", this.BrakeStrength.ToString());
            contents = contents.Replace("{{{EBRK_POS}}}", this.BrakePositions.ToString());
            contents = contents.Replace("{{{EBRK_DWN}}}", this.BrakeDownshift ? "1" : "0");

            // Torque Curves
            builder.Clear();
            builder.AppendLine("# Torque Curves");
            foreach (TorqueRatio ratio in TorqueRatios.OrderBy(x => x.RpmLevel))
            {
                builder.AppendLine($"\t\ttorque_curve[]: ({ratio.RpmLevel}, {ratio.Ratio})");
            }
            contents = contents.Replace("{{{TORQUE_CURVES}}}", builder.ToString().TrimEnd());

            // Sound data
            builder.Clear();

            SoundPackage sound = series.SoundPackage;
            string intpath = "/def/vehicle/truck/{{{NAME}}}/sound/" + sound.InteriorFileName;
            string extpath = "/def/vehicle/truck/{{{NAME}}}/sound/" + sound.ExteriorFileName;
            builder.AppendLine("# Sound Data");
            builder.AppendLine($"\t\tdefaults[]: \"{intpath}\"");
            builder.AppendLine($"\t\tdefaults[]: \"{extpath}\"");

            // Write the default[]...
            if (Defaults != null && Defaults.Length > 0)
            {
                builder.AppendLine();
                builder.AppendLine("# Attachments");
                foreach (string line in Defaults)
                    builder.AppendLine($"\t\tdefaults[]: \"{line}\"");
            }

            // Final Replacement
            contents = contents.Replace("{{{DEFAULTS}}}", builder.ToString().TrimEnd());

            // Define file paths
            return contents;
        }

        /// <summary>
        /// Converts a decimal value into a number with a "@@dg@@"
        /// to seperate the thousands places.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Digitize(decimal value)
        {
            int val = (int)value;
            return val.ToString("N0", Culture).Replace(",", "@@dg@@");
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

        /// <summary>
        /// Converts a hosepower value into Kilowatts
        /// </summary>
        /// <param name="horsepower"></param>
        /// <returns></returns>
        public static int HorsepowerToKilowatts(decimal horsepower)
        {
            decimal div = 0.7457m;
            return (int)Math.Round(horsepower * div, 0);
        }

        /// <summary>
        /// Converts a torque value into Newton-Metres
        /// </summary>
        /// <param name="Torque"></param>
        /// <returns></returns>
        public static int TorqueToNm(decimal torque)
        {
            decimal div = 1.35582m;
            return (int)Math.Round(torque * div, 0);
        }

        /// <summary>
        /// Converts a Newton-Metres value into Torque
        /// </summary>
        /// <param name="Nm"></param>
        /// <returns></returns>
        public static int NmToTorque(decimal Nm)
        {
            decimal div = 0.7376m;
            return (int)Math.Round(Nm * div, 0);
        }
    }
}
