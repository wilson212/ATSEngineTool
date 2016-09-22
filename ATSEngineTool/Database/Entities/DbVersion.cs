using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class DbVersion
    {
        /// <summary>
        /// Gets the Unique update ID in the table
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int UpdateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("Version"), Required, Unique]
        protected string VersionString { get; set; }

        /// <summary>
        /// Gets or Sets the Version value for this update entry
        /// </summary>
        public Version Version
        {
            get
            {
                return Version.Parse(VersionString);
            }
            set
            {
                VersionString = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Column("AppliedOn"), Required]
        [Default("(strftime('%s', 'now'))", Quote = false)]
        protected int AppliedOnEpoch { get; set; }

        public DateTime AppliedOn
        {
            get
            {
                return Epoch.FromUnix(AppliedOnEpoch);
            }
            set
            {
                AppliedOnEpoch = Epoch.ToUnix(value);
            }
        }
    }
}
