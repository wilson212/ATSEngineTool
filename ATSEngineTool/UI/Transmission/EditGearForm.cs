using System;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class EditGearForm : Form
    {
        public EditGearForm(TransmissionGear gear = null)
        {
            InitializeComponent();
            if (gear != null)
            {
                gearName.Text = gear.Name;
                ratioBox.SetValueInRange(gear.Ratio);
            }
        }

        public TransmissionGear GetGear()
        {
            var gear = new TransmissionGear();
            gear.Name = gearName.Text;
            gear.Ratio = ratioBox.Value;
            return gear;
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
    }
}
