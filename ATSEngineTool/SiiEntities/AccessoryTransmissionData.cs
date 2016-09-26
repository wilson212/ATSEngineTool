using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sii;

namespace ATSEngineTool.SiiEntities
{
    [SiiUnit("accessory_transmission_data")]
    class AccessoryTransmissionData
    {
        [SiiAttribute("name")]
        public string Name { get; private set; }

        [SiiAttribute("price")]
        public ushort Price { get; private set; }

        [SiiAttribute("unlock")]
        public byte UnlockLevel { get; private set; }

        [SiiAttribute("icon")]
        public string Icon { get; private set; }

        [SiiAttribute("differential_ratio")]
        public decimal DifferentialRatio { get; private set; }

        [SiiAttribute("transmission_names")]
        public TransmissionNames GearNames { get; private set; }

        [SiiAttribute("ratios_forward")]
        public decimal[] ForwardRatios { get; private set; }

        [SiiAttribute("ratios_reverse")]
        public decimal[] ReverseRatios { get; private set; }
    }
}
