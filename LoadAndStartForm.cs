using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace HiddenConsole {
    public partial class LoadAndStartForm : Form {
        public LoadAndStartForm() {
            InitializeComponent();
            Icon = Properties.Resources.cmd;
        }
        private void LoadAndStartForm_Shown(object sender, EventArgs e) {
            //try {
            //    openFileDialog2.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog2.FileName);
            //} catch { }
            if (openFileDialog2.ShowDialog() == DialogResult.OK) {
                StartInfo si = null;
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                    TextReader reader = new StreamReader(openFileDialog2.FileName);
                    si = (StartInfo)ser.Deserialize(reader);
                    reader.Close();
                    try {
                        SpawnedProcess sp = new SpawnedProcess();
                        sp.Run(si);
                        Program.Menu.AddProcess(sp);
                    } catch (Exception ex) {
                        MessageBox.Show("Failed to start process: " + ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show("Failed to load file: " + ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.Close();
        }
    }
}
