using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sii;

namespace ATSEngineTool.SiiEntities
{
    [SiiUnit("transmission_names")]
    class TransmissionNames
    {
        [SiiAttribute("forward")]
        public string[] Forward { get; private set; }

        [SiiAttribute("reverse")]
        public string[] Reverse { get; private set; }

        [SiiAttribute("neutral")]
        public string Neutral { get; private set; }
    }
}
