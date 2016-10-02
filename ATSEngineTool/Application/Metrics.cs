using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEngineTool
{
    public static class Metrics
    {
        public static double MilesToKilometers(double miles)
        {
            return miles * 1.6092;
        }

        public static double KilometersToMiles(double kilometers)
        {
            return kilometers * 0.62137;
        }
    }
}
