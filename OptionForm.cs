using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    public partial class OptionForm : Form {
        public int KeepConsoles { get; set; }
        public OptionForm() {
            InitializeComponent();
            Icon = Properties.Resources.cmd;
            this.textBox1.Text = (this.KeepConsoles = Properties.Settings.Default.KeepConsoles).ToString();
            if (SG.Utilities.Forms.Elevation.IsElevationRequired) {
                SG.Utilities.Forms.Elevation.ShowButtonShield(button3, true);
                SG.Utilities.Forms.Elevation.ShowButtonShield(button4, true);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e) {
            int nv;
            if (int.TryParse(this.textBox1.Text, out nv)) {
                if (nv > 0) KeepConsoles = nv;
            }
        }
        private void textBox1_Leave(object sender, EventArgs e) {
            this.textBox1.Text = KeepConsoles.ToString();
        }
        private void button3_Click(object sender, EventArgs e) {
            if (SG.Utilities.Forms.Elevation.IsElevationRequired) {
                if (SG.Utilities.Forms.Elevation.RestartElevated("?#REG " + Application.ExecutablePath + " " + this.Handle.ToString()) == int.MinValue) {
                    MessageBox.Show("Failed to start elevated Process. Most likely a security conflict.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                FileTypeRegistration.Register(this, Application.ExecutablePath);
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            if (SG.Utilities.Forms.Elevation.IsElevationRequired) {
                if (SG.Utilities.Forms.Elevation.RestartElevated("?#UNREG " + Application.ExecutablePath + " " + this.Handle.ToString()) == int.MinValue) {
                    MessageBox.Show("Failed to start elevated Process. Most likely a security conflict.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                FileTypeRegistration.Unregister(this, Application.ExecutablePath);
            }
        }
    }
}
