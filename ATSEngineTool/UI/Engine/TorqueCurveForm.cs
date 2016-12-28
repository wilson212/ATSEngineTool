using System;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class TorqueCurveForm : Form
    {
        /// <summary>
        /// The max power rating as defined by the constructor
        /// </summary>
        protected double MaxNewtonMeters { get; set; }

        /// <summary>
        /// The current power rating in NM
        /// </summary>
        protected double CurrentNewtonMeters { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="TorqueCurveForm"/>
        /// </summary>
        /// <param name="maxTorque">The maximum torque rating as defined by the engine</param>
        /// <param name="ratio">If editing a torque ratio, specify it here.</param>
        public TorqueCurveForm(double maxTorque, TorqueRatio ratio = null)
        {
            // Create form controls and set default dialog result
            InitializeComponent();
            DialogResult = DialogResult.No;

            // Set the max power in newton meters
            MaxNewtonMeters = (Program.Config.UnitSystem == UnitSystem.Imperial)
                ? Metrics.TorqueToNewtonMeters(maxTorque, 2)
                : maxTorque;

            // If this is an existing ratio, set form values
            if (ratio != null)
            {
                CurrentNewtonMeters = MaxNewtonMeters * ratio.Ratio;
                rpmLevelBox.Value = ratio.RpmLevel;
            }

            // Fire the checked event to get things rolling
            radioButton1.Checked = true;
        }

        /// <summary>
        /// Returns a new <see cref="TorqueRatio"/> entity with the values
        /// defined in this form.
        /// </summary>
        /// <returns></returns>
        public TorqueRatio GetRatio()
        {
            // Ensure we aren't over max value. This can happen
            // due to the value changed event not firing properly
            if (CurrentNewtonMeters > MaxNewtonMeters)
                CurrentNewtonMeters = MaxNewtonMeters;

            // Sometimes rounding can bite you in the ass... lets
            // prevent this, and thus causing problems
            var ratio = Math.Round(CurrentNewtonMeters / MaxNewtonMeters, 4);
            if (ratio > 1.0000)
                ratio = 1;

            // Return the ratio
            return new TorqueRatio()
            {
                Ratio = ratio,
                RpmLevel = (int)rpmLevelBox.Value
            };
        }

        /// <summary>
        /// Precentage radio checked event
        /// </summary>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Only process if we are selected
            if (!radioButton1.Checked) return;
            torqueLevelBox.ValueChanged -= torqueLevelBox_ValueChanged;

            // Ensure the value doesnt exceed the maximum
            var val = Math.Round(CurrentNewtonMeters, 2);
            if (val > MaxNewtonMeters) val = MaxNewtonMeters;

            // Set value
            torqueLevelBox.Value = 0;
            torqueLevelBox.Maximum = 100;
            torqueLevelBox.Value = (decimal)Math.Round((val / MaxNewtonMeters) * 100, 2);
            torqueLevelBox.ValueChanged += torqueLevelBox_ValueChanged;

            // Update label
            maxLabel.Text = $"/ {torqueLevelBox.Maximum}";
        }

        /// <summary>
        /// Newton Metres radio checked event
        /// </summary>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // Only process if we are selected
            if (!radioButton2.Checked) return;
            torqueLevelBox.ValueChanged -= torqueLevelBox_ValueChanged;

            // Ensure the value doesnt exceed the maximum
            var val = Math.Round(CurrentNewtonMeters, 2);
            if (val > MaxNewtonMeters) val = MaxNewtonMeters;

            // Set value
            torqueLevelBox.Value = 0;
            torqueLevelBox.Maximum = (decimal)MaxNewtonMeters;
            torqueLevelBox.Value = (decimal)val;
            torqueLevelBox.ValueChanged += torqueLevelBox_ValueChanged;

            // Update label
            maxLabel.Text = $"/ {torqueLevelBox.Maximum}";
        }

        /// <summary>
        /// Torque radio checked event
        /// </summary>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            // Only process if we are selected
            if (!radioButton3.Checked) return;
            torqueLevelBox.ValueChanged -= torqueLevelBox_ValueChanged;

            // Ensure the value doesnt exceed the maximum
            var val = Math.Round(CurrentNewtonMeters, 2);
            if (val > MaxNewtonMeters) val = MaxNewtonMeters;

            // Set value
            torqueLevelBox.Value = 0;
            torqueLevelBox.Maximum = (decimal)Metrics.NewtonMetersToTorque(MaxNewtonMeters, 2);
            torqueLevelBox.Value = (decimal)Metrics.NewtonMetersToTorque(val, 2);
            torqueLevelBox.ValueChanged += torqueLevelBox_ValueChanged;

            // Update label
            maxLabel.Text = $"/ {torqueLevelBox.Maximum}";
        }

        /// <summary>
        /// Horsepower radio checked event
        /// </summary>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            // Only process if we are selected
            if (!radioButton4.Checked) return;
            torqueLevelBox.ValueChanged -= torqueLevelBox_ValueChanged;

            // Convert newton metres to torque
            var torque = Metrics.NewtonMetersToTorque(CurrentNewtonMeters, 8);
            var maxTorque = Metrics.NewtonMetersToTorque(MaxNewtonMeters, 8);

            // Convert torque to Horsepower
            var maxHp = (decimal)Metrics.TorqueToHorsepower(maxTorque, (int)rpmLevelBox.Value, 2);
            var horse = (decimal)Metrics.TorqueToHorsepower(torque, (int)rpmLevelBox.Value, 2);
            if (horse > maxHp) horse = maxHp;

            // Set values
            torqueLevelBox.Value = 0;
            torqueLevelBox.Maximum = maxHp;
            torqueLevelBox.Value = horse;
            torqueLevelBox.ValueChanged += torqueLevelBox_ValueChanged;

            // Update label
            maxLabel.Text = $"/ {maxHp}";
        }

        /// <summary>
        /// Power value input box value changed event
        /// </summary>
        private void torqueLevelBox_ValueChanged(object sender, EventArgs e)
        {
            // Dont go over maximum
            var value = (torqueLevelBox.Value > torqueLevelBox.Maximum)
                ? (double)torqueLevelBox.Maximum
                : (double)torqueLevelBox.Value;


            if (radioButton1.Checked)
            {
                CurrentNewtonMeters = MaxNewtonMeters * (value / 100);
            }
            else if (radioButton2.Checked)
            {
                // NM
                CurrentNewtonMeters = value;
            }
            else if (radioButton3.Checked)
            {
                // Torque
                CurrentNewtonMeters = Metrics.TorqueToNewtonMeters(value, 4);
            }
            else
            {
                // Horsepower
                var torque = Metrics.HorsepowerToTorque(value, (int)rpmLevelBox.Value, 4);
                CurrentNewtonMeters = Metrics.TorqueToNewtonMeters(torque, 4);
            }
        }

        /// <summary>
        /// Engine RPM box value change event... Forces max horsepower based
        /// on the entered RPM
        /// </summary>
        private void rpmLevelBox_ValueChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                radioButton4_CheckedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Confirm button click event
        /// </summary>
        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        /// <summary>
        /// Cancel button click event
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
