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
    public partial class StartInfoEditForm : Form {
        public StartInfo StartInfo {
            get {
                StartInfo si = new StartInfo();
                si.Application = textBox1.Text;
                si.Arguments = textBox2.Text;
                si.WorkingDirectory = textBox3.Text;
                return si;
            }
            set {
                textBox1.Text = (value == null) ? "" : value.Application;
                textBox2.Text = (value == null) ? "" : value.Arguments;
                textBox3.Text = (value == null) ? "" : value.WorkingDirectory;
            }
        }
        public StartInfoEditForm() {
            InitializeComponent();
            Icon = Properties.Resources.cmd;
        }
        private void button1_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = textBox1.Text;
            try {
                openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
            } catch { }
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                textBox1.Text = openFileDialog1.FileName;
                if (string.IsNullOrWhiteSpace(textBox3.Text)) {
                    try {
                        textBox3.Text = System.IO.Path.GetDirectoryName(textBox1.Text);
                    } catch { }
                }
            }
        }
        private void button6_Click(object sender, EventArgs e) {
            folderBrowserDialog1.SelectedPath = textBox3.Text;
            if (string.IsNullOrWhiteSpace(textBox3.Text)) {
                try {
                    folderBrowserDialog1.SelectedPath = System.IO.Path.GetDirectoryName(textBox1.Text);
                } catch { }
            }
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            try {
                saveFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(saveFileDialog1.FileName);
            } catch { }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                    TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                    ser.Serialize(writer, StartInfo);
                    writer.Close();
                } catch(Exception ex) {
                    MessageBox.Show("Failed to save file: " + ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e) {
            openFileDialog2.FileName = saveFileDialog1.FileName;
            try {
                openFileDialog2.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog2.FileName);
            } catch { }
            if (openFileDialog2.ShowDialog() == DialogResult.OK) {
                saveFileDialog1.FileName = openFileDialog2.FileName;
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                    TextReader reader = new StreamReader(openFileDialog2.FileName);
                    StartInfo = (StartInfo)ser.Deserialize(reader);
                    reader.Close();
                } catch (Exception ex) {
                    MessageBox.Show("Failed to load file: " + ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        private void button2_Click(object sender, EventArgs e) {
            try {
                SpawnedProcess sp = new SpawnedProcess();
                sp.Run(StartInfo);
                Program.Menu.AddProcess(sp);
                this.Close();
            } catch (Exception ex) {
                MessageBox.Show("Failed to Start " + StartInfo.Name + ": " + ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e) {

        }
    }
}
