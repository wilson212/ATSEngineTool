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
    public class Transmission
    {
        /// <summary>
        /// The Unique Transmission ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="TransmissionSeries"/> object
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

        #region Child Database Sets

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
        /// Gets a list of <see cref="AccessoryConflict"/> entities that reference this 
        /// <see cref="Transmission"/>
        /// </summary>
        public virtual IEnumerable<AccessoryConflict> EngineConflicts { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SuitableAccessory"/> entities that reference this 
        /// <see cref="Transmission"/>
        /// </summary>
        public virtual IEnumerable<SuitableAccessory> SuitableEngines { get; set; }

        /// <summary>
        /// Gets a list of <see cref="TruckTransmission"/> entities that reference this 
        /// <see cref="Transmission"/>
        /// </summary>
        public virtual IEnumerable<TruckTransmission> ItemOf { get; set; }

        #endregion

        public override string ToString() => Name;

        /// <summary>
        /// Serializes this transmission into SII format, and returns the result
        /// </summary>
        /// <returns></returns>
        public string ToSiiFormat(string truckName)
        {
            // Create local variables
            var series = this.Series;
            var builder = new SiiFileBuilder();
            var name = $"{this.UnitName}.{truckName}.transmission";
            var hasNames = Gears.Any(x => !String.IsNullOrWhiteSpace(x.Name));

            // Make sure we have a file comment
            if (Comment == null || Comment.Length == 0)
                Comment = new string[] { "Generated with the ATS Engine Generator Tool by Wilson212" };

            // Write file intro
            builder.WriteStartDocument();

            // Write file comment
            builder.WriteCommentBlock(Comment);

            // Begin the engine accessory
            builder.WriteStructStart("accessory_transmission_data", name);

            // Generic Info
            builder.WriteAttribute("name", this.Name);
            builder.WriteAttribute("price", this.Price, "Transmission price", 1);
            builder.WriteAttribute("unlock", this.Unlock, "Unlocks @ Level", 2);
            builder.WriteAttribute("icon", series.Icon);
            builder.WriteLine();

            // Add names if we have them
            if (hasNames)
            {
                builder.WriteLine("# Transmission gear names");
                builder.WriteAttribute("transmission_names", ".names", false);
                builder.WriteLine();
            }

            // Diff Ratio
            builder.WriteLine("# Differential Ratio: 2.64, 2.85, 2.93, 3.08, 3.25, 3.36, 3.40?, 3.42, 3.55, 3.58(single) 3.70, 3.73?, 3.78, 3.91, 4.10");
            builder.WriteAttribute("differential_ratio", this.DifferentialRatio);
            builder.WriteLine();

            // Add Retarder
            if (Retarder > 0)
            {
                builder.WriteLine("# Retarder");
                builder.WriteAttribute("retarder", Retarder);
                builder.WriteLine();
            }

            if (StallTorqueRatio > 0.0m)
            {
                builder.WriteLine("# Torque Converter: 2.42, 2.34, 1.9, 1.79, 1.58");
                builder.WriteAttribute("stall_torque_ratio", StallTorqueRatio);
                builder.WriteLine();
            }

            // Create gear lists
            var reverseGears = new List<TransmissionGear>(this.Gears.Where(x => x.IsReverse));
            var forwardGears = new List<TransmissionGear>(this.Gears.Where(x => !x.IsReverse));

            // Reverse Gears
            int i = 0;
            builder.WriteLine($"# reverse gears");
            foreach (var gear in reverseGears)
            {
                builder.WriteAttribute($"ratios_reverse[{i++}]", gear.Ratio);
            }
            builder.WriteLine();

            // Forward Gears
            i = 0;
            builder.WriteLine($"# forward gears");
            foreach (var gear in forwardGears)
            {
                builder.WriteAttribute($"ratios_forward[{i++}]", gear.Ratio);
            }

            // Write the default[]...
            if (Defaults != null && Defaults.Length > 0)
            {
                builder.WriteLine();
                builder.WriteLine("# Attachments");
                foreach (string line in Defaults)
                    builder.WriteAttribute("defaults[]", line);
            }

            // Define is we output suitible_for and conflict_with for engines
            bool writeSuitables = Program.Config.CompileOption == CompileOption.TransmissionOnly
                               || Program.Config.CompileOption == CompileOption.Both;
            var conflicts = this.EngineConflicts.ToList();
            var suitables = this.SuitableEngines.ToList();

            // Write the conflict_with[]...
            if (conflicts.Count > 0 || (Conflicts != null && Conflicts.Length > 0))
            {
                builder.WriteLine();
                builder.WriteLine("# Conflicts");

                // Engines
                foreach (string eng in conflicts.Select(x => x.Engine.UnitName))
                    builder.WriteAttribute("conflict_with[]", $"{eng}.{truckName}.engine");

                // Other Conflicts
                if (Conflicts != null)
                    foreach (string line in Conflicts)
                        builder.WriteAttribute("conflict_with[]", line);
            }

            // Write the conflict_with[]...
            if ((writeSuitables && suitables.Count > 0) || (SuitableFor != null && SuitableFor.Length > 0))
            {
                builder.WriteLine();
                builder.WriteLine("# Suitables");

                // Engines?
                if (writeSuitables)
                    foreach (string eng in suitables.Select(x => x.Engine.UnitName))
                        builder.WriteAttribute("suitable_for[]", $"{eng}.{truckName}.engine");

                // Other Suitables
                if (SuitableFor != null)
                    foreach (string line in SuitableFor)
                        builder.WriteAttribute("suitable_for[]", line);
            }

            // Close brackets
            builder.WriteStructEnd();

            // Do we have gear names?
            if (hasNames)
            {
                builder.WriteLine();
                builder.WriteStructStart("transmission_names", ".names");

                // Neutral always first
                builder.WriteAttribute("neutral", "N");

                // Forward Gears
                i = 0; // Reset
                builder.WriteLine();
                builder.WriteLine("# Forward Gear Names");
                foreach (var gear in forwardGears)
                {
                    name = GetGearNameAtIndex(i, gear, forwardGears);
                    builder.WriteAttribute($"forward[{i++}]", name);
                }

                // Reverse Gears
                i = 0; // Reset
                builder.WriteLine();
                builder.WriteLine("# Reverse Gear Names");
                foreach (var gear in reverseGears)
                {
                    name = GetGearNameAtIndex(i, gear, reverseGears);
                    builder.WriteAttribute($"reverse[{i++}]", name);
                }

                builder.WriteStructEnd();
            }

            // End brace
            builder.WriteEndDocument();

            // Define file paths
            return builder.ToString().Replace("{{{NAME}}}", truckName).TrimEnd();
        }

        /// <summary>
        /// Gets the gear name, or if the gear does not have a name, generates a name based off 
        /// of the gear index (Eaton Fuller Style).
        /// </summary>
        /// <param name="index">The index of the gear in the list (sorted by ratio desc)</param>
        /// <param name="gear">The gear we are fetching the name for</param>
        /// <returns></returns>
        public static string GetGearNameAtIndex(int index, TransmissionGear gear, List<TransmissionGear> gears)
        {
            if (String.IsNullOrWhiteSpace(gear.Name))
            {
                int i = (index / 2);
                var affix = ((index + 2) % 2 == 1) ? "H" : "L";

                if (gear.IsReverse)
                {
                    switch (gears.Count)
                    {
                        case 1: return "R";
                        case 2: return (index == 0) ? "R1" : "R2";
                        default:
                            i = (index + 2) / 2;
                            return $"R{i}{affix}";
                    }
                }
                else
                {
                    // 10 speeds are just 1- 10
                    if (gears.Count < 11)
                    {
                        return (index + 1).ToString();
                    }
                    else if (gears.Count < 14)
                    {
                        // Since 1 - 4 is numbered with no affix, add that to the gear index
                        i = ((index + 5) / 2);
                        affix = ((index + 5) % 2 == 1) ? "H" : "L";

                        if (index == 0) return "L";
                        else if (index < 5) return index.ToString();
                        else return i + affix;
                    }
                    else
                    {
                        return (i == 0) ? $"L{affix}" : i + affix;
                    }
                }
            }

            return gear.Name;
        }
    }
}
