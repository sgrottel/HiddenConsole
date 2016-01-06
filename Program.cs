using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace HiddenConsole {
    static class Program {
        public static MainMenu Menu { get; private set; }
        private static SG.Utilities.Forms.TrayIcon icon;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Menu = new MainMenu();

            icon = new SG.Utilities.Forms.TrayIcon();
            icon.ShowContextMenuOnClick = true;
            icon.Menu = Menu.Menu;
            icon.Visible = true;
            icon.Icon = new System.Drawing.Icon(Properties.Resources.cmd, new System.Drawing.Size(16, 16));

            #region for Debug only
            //StartInfo i = new StartInfo();
            //i.Application = "ipconfig";
            //i.Arguments = "/all";
            //i.WorkingDirectory = @"D:\tmp";
            //SpawnProcess(i);
            #endregion

            Application.Run();

            Menu.WaitForAllProcesses();
            icon.Visible = false;
        }
        private static void SpawnProcess(StartInfo startInfo) {
            SpawnedProcess sp = new SpawnedProcess();
            sp.Run(startInfo);
            Menu.AddProcess(sp);
        }
        static internal void LoadStartInfo_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "StartInfo|*.hcsi|All Files|*.*";
            openFileDialog2.Title = "Load Start Info ...";
            // HAZARD : Dialog windows is not blocking and does not show in Task Bar!
           // icon.ModalDialog = openFileDialog2;
            if (openFileDialog2.ShowDialog() == DialogResult.OK) {
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                    TextReader reader = new StreamReader(openFileDialog2.FileName);
                    StartInfo si = (StartInfo)ser.Deserialize(reader);
                    reader.Close();
                    SpawnProcess(si);
                } catch (Exception ex) {
                    MessageBox.Show("Failed to load file: " + ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
