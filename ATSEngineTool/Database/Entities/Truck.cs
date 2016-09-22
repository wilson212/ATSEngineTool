﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class Truck
    {
        /// <summary>
        /// The Truck Id
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the SII Unitname of this truck
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or Sets the Name of this truck
        /// </summary>
        [Column, Required, Unique]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets whether this is an SCS truck, or a Modded truck
        /// </summary>
        [Column, Required, Default(false)]
        public bool IsScsTruck { get; set; } = false;

        #region Foreign Keys

        /// <summary>
        /// Gets a list of <see cref="Engine"/> entities that reference this 
        /// <see cref="EngineSeries"/>
        /// </summary>
        public virtual IEnumerable<TruckEngine> TruckEngines { get; set; }

        #endregion

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is Truck)
            {
                Truck compare = (Truck)obj;
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