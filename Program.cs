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
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Menu = new MainMenu();

            icon = new SG.Utilities.Forms.TrayIcon();
            icon.ShowContextMenuOnClick = true;
            icon.Menu = Menu.Menu;
            icon.Visible = true;
            icon.Icon = new System.Drawing.Icon(Properties.Resources.cmd, new System.Drawing.Size(16, 16));

            ParsingCommandLineArguments(args);

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
        private static void ParsingCommandLineArguments(string[] args) {
            int argParseState = 0;
            string error = string.Empty;
            foreach (string arg in args) {
                switch (argParseState) {
                    case 1:
                        argParseState = 0;
                        break;
                    case 2:
                        argParseState = 0;
                        break;
                    case 0:
                        if (arg.ToLower() == "?#REG") {
                            argParseState = 1;
                        } else if (arg.ToLower() == "?#UNREG") {
                            argParseState = 2;
                        } else
                            try {
                                XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                                TextReader reader = new StreamReader(arg);
                                StartInfo si = (StartInfo)ser.Deserialize(reader);
                                reader.Close();
                                if (si == null) throw new Exception("Generic loading error");
                                try {
                                    SpawnedProcess sp = new SpawnedProcess();
                                    sp.Run(si);
                                    Program.Menu.AddProcess(sp);
                                } catch (Exception ex) {
                                    error += "Failed to start process " + si.Name + ": " + ex.ToString() + "\n\n";
                                }
                            } catch (Exception ex) {
                                error += "Failed to load file " + arg + ": " + ex.ToString() + "\n\n";
                            }
                        break;
                }
            }
            if (!String.IsNullOrWhiteSpace(error)) {
                MessageBox.Show(error, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static void SpawnProcess(StartInfo startInfo) {
            SpawnedProcess sp = new SpawnedProcess();
            sp.Run(startInfo);
            Menu.AddProcess(sp);
        }
        static internal void LoadStartInfo_Click(object sender, EventArgs e) {
            LoadAndStartForm lasf = new LoadAndStartForm();
            icon.ModalDialog = lasf;
            icon.Menu = null;
            lasf.ShowDialog();
            icon.ModalDialog = null;
            icon.Menu = Menu.Menu;
        }
    }
}
