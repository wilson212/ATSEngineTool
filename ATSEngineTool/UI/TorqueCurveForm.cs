using System;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class TorqueCurveForm : Form
    {
        protected int PeakTorque;

        public bool UsePercentage
        {
            get { return radioButton1.Checked; }
        }

        public TorqueCurveForm(int maxTorque, TorqueRatio ratio = null)
        {
            InitializeComponent();
            this.DialogResult = DialogResult.No;
            PeakTorque = maxTorque;

            // If this is an existing ratio, set form values
            if (ratio != null)
            {
                rpmLevelBox.Value = ratio.RpmLevel;
                torqueLevelBox.Value = (ratio.Ratio * 100);
            }

            // Setup label
            if (Program.Config.UnitSystem == UnitSystem.Metric)
            {
                labelTorque.Text = "N·m Percentage:";
            }

            radioButton1.Checked = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
        }

        public TorqueRatio GetRatio()
        {
            var val = (!radioButton1.Checked)
                ? Math.Round(torqueLevelBox.Value / PeakTorque, 2)
                : Math.Round(torqueLevelBox.Value / 100, 2);

            return new TorqueRatio()
            {
                Ratio = val,
                RpmLevel = (int)rpmLevelBox.Value
            };
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var current = torqueLevelBox.Value;
            string prefix = (Program.Config.UnitSystem == UnitSystem.Imperial)
                ? "Torque "
                : "N·m ";

            if (radioButton1.Checked)
            {
                torqueLevelBox.Maximum = 100;
                var newValue = Math.Round((current / PeakTorque) * 100, 0);
                torqueLevelBox.Value = newValue;
                labelTorque.Text = prefix + "Percent:";
            }
            else
            {
                torqueLevelBox.Maximum = PeakTorque;
                var newValue = Math.Round((current / 100) * PeakTorque, 0);
                torqueLevelBox.Value = newValue;
                labelTorque.Text = prefix + "Value:";
            }
        }
    }
}
