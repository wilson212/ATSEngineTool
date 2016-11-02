using System;

namespace ATSEngineTool
{
    /// <summary>
    /// Provides methods to convert values between Imperial and Metric measurements
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        /// Converts distance in miles to kilometers
        /// </summary>
        /// <param name="miles"></param>
        /// <returns></returns>
        public static double MilesToKilometers(double miles)
        {
            return miles * 1.6092;
        }

        /// <summary>
        /// Converts distance in kilometers to miles
        /// </summary>
        /// <param name="kilometers"></param>
        /// <returns></returns>
        public static double KilometersToMiles(double kilometers)
        {
            return kilometers * 0.62137;
        }

        /// <summary>
        /// Converts a torque value into Newton-Metres
        /// </summary>
        /// <param name="Torque"></param>
        /// <param name="decimals">The number of decimal places to include in the return value</param>
        /// <returns></returns>
        public static decimal TorqueToNewtonMetres(decimal torque, int decimals = 0)
        {
            return Math.Round(torque * 1.35582m, decimals);
        }

        /// <summary>
        /// Converts a torque value into Newton-Metres
        /// </summary>
        /// <param name="Torque"></param>
        /// <param name="decimals">The number of decimal places to include in the return value</param>
        /// <returns></returns>
        public static double TorqueToNewtonMetres(double torque, int decimals = 0)
        {
            return Math.Round(torque * 1.35582, decimals);
        }

        /// <summary>
        /// Converts a torque value into Newton-Metres
        /// </summary>
        /// <param name="Torque"></param>
        /// <returns></returns>
        public static int TorqueToNewtonMetres(int torque)
        {
            return (int)Math.Round(torque * 1.35582, 0);
        }

        /// <summary>
        /// Converts a Newton-Metres value into Torque
        /// </summary>
        /// <param name="Nm"></param>
        /// <param name="decimals">The number of decimal places to include in the return value</param>
        /// <returns></returns>
        public static decimal NewtonMetresToTorque(decimal Nm, int decimals = 0)
        {
            return Math.Round(Nm * 0.7376m, decimals);
        }

        /// <summary>
        /// Converts a Newton-Metres value into Torque
        /// </summary>
        /// <param name="Nm"></param>
        /// <param name="decimals">The number of decimal places to include in the return value</param>
        /// <returns></returns>
        public static double NewtonMetresToTorque(double Nm, int decimals = 0)
        {
            return Math.Round(Nm * 0.7376, decimals);
        }

        /// <summary>
        /// Converts a Newton-Metres value into Torque
        /// </summary>
        /// <param name="Nm"></param>
        /// <returns></returns>
        public static int NewtonMetresToTorque(int Nm)
        {
            return (int)Math.Round(Nm * 0.7376, 0);
        }

        /// <summary>
        /// Converts a mechanical hosepower value into Kilowatts
        /// </summary>
        /// <param name="horsepower"></param>
        /// <param name="decimals">The number of decimal places to include in the return value</param>
        /// <returns></returns>
        public static decimal HorsepowerToKilowatts(decimal horsepower, int decimals = 0)
        {
            return Math.Round(horsepower * 0.7457m, decimals);
        }

        /// <summary>
        /// Converts a mechanical hosepower value into Kilowatts
        /// </summary>
        /// <param name="horsepower"></param>
        /// <returns></returns>
        public static int HorsepowerToKilowatts(int horsepower)
        {
            return (int)Math.Round(horsepower * 0.7457m, 0);
        }

        /// <summary>
        /// Converts a kilowatts value into mechanical hosepower
        /// </summary>
        /// <param name="kilowatts"></param>
        /// <param name="decimals">The number of decimal places to include in the return value</param>
        /// <returns></returns>
        public static decimal KilowattsToHorsepower(decimal kilowatts, int decimals = 0)
        {
            return Math.Round(kilowatts * 1.34102m, decimals);
        }

        /// <summary>
        /// Converts a kilowatts value into mechanical hosepower
        /// </summary>
        /// <param name="kilowatts"></param>
        /// <returns></returns>
        public static int KilowattsToHorsepower(int kilowatts)
        {
            return (int)Math.Round(kilowatts * 1.34102m, 0);
        }
    }
}
