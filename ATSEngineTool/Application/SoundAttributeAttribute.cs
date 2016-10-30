using System;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public class SoundAttributeAttribute : Attribute
    {
        public SoundAttribute Attribute { get; set; }

        public SoundAttributeAttribute(SoundAttribute attribute)
        {
            this.Attribute = attribute;
        }
    }
}
