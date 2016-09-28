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
    public class Transmission
    {
        /// <summary>
        /// The Unique Transmission ID
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.TransmissionSeries"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int SeriesId { get; set; }

        /// <summary>
        /// Gets or Sets the unique unit name for this transmission
        /// </summary>
        [Column, Required]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this transmission
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the buying price for this transmission
        /// </summary>
        [Column, Required]
        public int Price { get; set; }

        /// <summary>
        /// Gets or Sets the Players level at which this transmission will
        /// be available for purchase
        /// </summary>
        [Column, Default(0)]
        public int Unlock { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the Transmissions Diff. Ratio
        /// </summary>
        [Column, Required]
        public decimal DifferentialRatio { get; set; }

        /// <summary>
        /// Gets or Sets the Stall torque Ratio
        /// </summary>
        [Column, Required, Default(0.0)]
        public decimal StallTorqueRatio { get; set; } = 0.0m;

        /// <summary>
        /// Gets or Sets the number of retarder settings (0 = no retarder)
        /// </summary>
        [Column, Required, Default(0)]
        public int Retarder { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        [Column("Defaults"), Default("")]
        protected string _defaults { get; set; }

        [Column("Comment"), Default("")]
        protected string _comment { get; set; }

        [Column("Conflicts"), Default("")]
        protected string _conflicts { get; set; }

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

        #region Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("SeriesId", 
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<TransmissionSeries> FK_Series { get; set; }

        #endregion

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.EngineList"/> that 
        /// this truck will use in game.
        /// </summary>
        public TransmissionSeries Series
        {
            get
            {
                return FK_Series?.Fetch();
            }
            set
            {
                SeriesId = value.Id;
                FK_Series?.Refresh();
            }
        }

        /// <summary>
        /// Gets a list of <see cref="TransmissionGear"/> entities that reference this 
        /// <see cref="Transmission"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Transmissions
        /// that are bound by the foreign key and this TransmissionSeries.Id.
        /// </remarks>
        public virtual IEnumerable<TransmissionGear> Gears { get; set; }

        /// <summary>
        /// English culture for numbers
        /// </summary>
        protected static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-US");

        public override string ToString() => Name;

        /// <summary>
        /// Serializes this transmission into SII format, and returns the result
        /// </summary>
        /// <returns></returns>
        public string ToSiiFormat()
        {
            // Create local variables
            var series = this.Series;
            StringBuilder builder = new StringBuilder();
            string name = this.UnitName + ".{{{NAME}}}.transmission";
            string decvalue;

            bool hasNames = Gears.Any(x => !String.IsNullOrEmpty(x.Name));

            // Write file intro
            builder.AppendLine("SiiNunit");
            builder.AppendLine("{");

            // Make sure we have a file comment
            if (Comment == null || Comment.Length == 0)
                Comment = new string[] { "Generated with the ATS Engine Generator Tool by Wilson212" };

            // Write file comment
            builder.AppendLine("\t/**");
            foreach (string line in Comment)
                builder.AppendLine($"\t * {line}");
            builder.AppendLine("\t */");

            // Begin the engine accessory
            builder.AppendLine($"\taccessory_transmission_data : {name}");
            builder.AppendLine("\t{");

            // Generic Info
            builder.AppendLine($"\t\tname: \"{this.Name}\"");
            builder.AppendLine($"\t\tprice: {this.Price}\t# Engine price");
            builder.AppendLine($"\t\tunlock: {this.Unlock}\t\t# Unlocks @ Level");
            builder.AppendLine($"\t\ticon: \"{series.Icon}\"");
            builder.AppendLine();

            // Add names if we have them
            if (hasNames)
            {
                builder.AppendLine("\t\t# Transmission gear names");
                builder.AppendLine($"\t\ttransmission_names: .names");
                builder.AppendLine();
            }

            // Diff Ratio
            decvalue = this.DifferentialRatio.ToString(Culture);
            builder.AppendLine("\t\t# Differential Ratio: 2.64, 2.85, 2.93, 3.08, 3.25, 3.36, 3.40?, 3.42, 3.55, 3.58(single) 3.70, 3.73?, 3.78, 3.91, 4.10");
            builder.AppendLine($"\t\tdifferential_ratio: {decvalue}");
            builder.AppendLine();

            // Create gear lists
            var reverseGears = new List<TransmissionGear>(this.Gears.Where(x => x.IsReverse));
            var forwardGears = new List<TransmissionGear>(this.Gears.Where(x => !x.IsReverse));

            // Reverse Gears
            int i = 0;
            builder.AppendLine($"\t\t# reverse gears");
            foreach (var gear in reverseGears)
            {
                decvalue = gear.Ratio.ToString(Culture);
                builder.AppendLine($"\t\tratios_reverse[{i++}]: {decvalue}");
            }
            builder.AppendLine();

            // Forward Gears
            i = 0;
            builder.AppendLine($"\t\t# forward gears");
            foreach (var gear in forwardGears)
            {
                decvalue = gear.Ratio.ToString(Culture);
                builder.AppendLine($"\t\tratios_forward[{i++}]: {decvalue}");
            }

            // Write the default[]...
            if (Defaults != null && Defaults.Length > 0)
            {
                builder.AppendLine();
                builder.AppendLine("\t\t# Attachments");
                foreach (string line in Defaults)
                    builder.AppendLine($"\t\tdefaults[]: \"{line}\"");
            }

            // Write the conflict_with[]...
            if (Conflicts != null && Conflicts.Length > 0)
            {
                builder.AppendLine();
                builder.AppendLine("\t\t# Conflicts");
                foreach (string line in Conflicts)
                    builder.AppendLine($"\t\tconflict_with[]: \"{line}\"");
            }

            // Close brackets
            builder.AppendLine("\t}");

            // Do we have gear names?
            if (hasNames)
            {
                builder.AppendLine();
                builder.AppendLine("transmission_names : .names");
                builder.AppendLine("\t{");

                // Neutral always first
                builder.AppendLine("\t\tneutral:\t\t\"N\"");
                if (forwardGears.Any(x => !String.IsNullOrEmpty(x.Name)))
                {
                    i = 0;
                    builder.AppendLine();
                    builder.AppendLine("\t\t# Forward Gear Names");
                    foreach (var gear in forwardGears)
                    {
                        name = GetGearNameAtIndex(i, gear, forwardGears);
                        builder.AppendLine($"\t\tforward[{i++}]: \"{name}\"");
                    }
                }

                // Reverse Gears
                if (reverseGears.Any(x => !String.IsNullOrEmpty(x.Name)))
                {
                    i = 0;
                    builder.AppendLine();
                    builder.AppendLine("\t\t# Reverse Gear Names");
                    foreach (var gear in reverseGears)
                    {
                        name = GetGearNameAtIndex(i, gear, reverseGears);
                        builder.AppendLine($"\t\treverse[{i++}]: \"{name}\"");
                    }
                }

                builder.AppendLine("\t}");
            }

            // End brace
            builder.AppendLine("}");

            // Define file paths
            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Gets the gear name, or if the gear does not have a name, generates a name based off 
        /// of the gear index.
        /// </summary>
        /// <param name="index">The index of the gear in the list (sorted by ratio desc)</param>
        /// <param name="gear">The gear we are fetching the name for</param>
        /// <returns></returns>
        private string GetGearNameAtIndex(int index, TransmissionGear gear, List<TransmissionGear> gears)
        {
            if (String.IsNullOrWhiteSpace(gear.Name))
            {
                int i = (index / 2);
                var mod = ((index + 2) % 2 == 1) ? "H" : "L";

                if (gear.IsReverse)
                {
                    switch (gears.Count)
                    {
                        case 1: return "R";
                        case 2: return (index == 0) ? "R1" : "R2";
                        case 3:
                        case 4:
                            switch (index)
                            {
                                case 0:
                                case 1: return "R1" + mod;
                                case 2:
                                case 3: return "R2" + mod;
                            }
                            break;
                    }
                }
                else
                {
                    if (gears.Count < 7)
                        return (index + 1).ToString();
                    else
                        return (i == 0) ? $"L{mod}" : i + mod;
                }
            }

            return gear.Name;
        }
    }
}
