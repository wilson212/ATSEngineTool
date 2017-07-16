using System.Windows.Forms;

namespace System
{
    public static class NumericUpDownExtensions
    {
        /// <summary>
        /// Sets the value of the <see cref="NumericUpDown"/>, respecting the <see cref="NumericUpDown.Minimum"/>
        /// and <see cref="NumericUpDown.Maximum"/> boundaries. If the value is out of range, the value will
        /// be adjusted to fit inside the boundaries.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="value"></param>
        public static void SetValueInRange(this NumericUpDown container, decimal value)
        {
            var range = new Range<decimal>(container.Minimum, container.Maximum);
            switch (range.CompareTo(value))
            {
                // Value is greater than the maximum
                case -1:
                    container.Value = container.Maximum;
                    break;
                // Value is less than the Minimum
                case 1:
                    container.Value = container.Minimum;
                    break;
                // Value is in range
                default:
                    container.Value = value;
                    break;
            }
        }
    }
}
