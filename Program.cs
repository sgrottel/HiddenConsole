using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace HiddenConsole {
    static class Program {
        public static MainMenu Menu { get; private set; }
        private static SG.Utilities.Forms.TrayIcon icon;
        private static IPCNamedPipe appServerPipe;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (CheckAdminCmdLineArgs(args)) return;

            appServerPipe = new IPCNamedPipe("HiddenConsole");
            if (!appServerPipe.ServerPipeOpen) {
                // I am a secondary instance
                if (args.Length > 0) {
                    try {
                        appServerPipe.SendStrings(args);
                    } catch (Exception ex) {
                        MessageBox.Show("Failed to communicate parameters: " + ex.ToString(),
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            } else {
                appServerPipe.StringReceived += AppServerPipe_StringReceived;
            }

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

            if (appServerPipe != null) appServerPipe.Close();
            Menu.WaitForAllProcesses();
            icon.Visible = false;
        }
        private static void AppServerPipe_StringReceived(object sender, string e) {
            string error = TryLoadRunFile(e);
            if (error != null) {
                MessageBox.Show(error, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static bool CheckAdminCmdLineArgs(string[] args) {
            if ((args.Length == 3) && (args[0].StartsWith("?#"))) {
                NativeWindow nw = new NativeWindow();
                long hv;
                if (long.TryParse(args[2], out hv)) {
                    nw.AssignHandle((IntPtr)hv);
                }

                if (args[0] == "?#REG") {
                    FileTypeRegistration.Register(nw, args[1]);
                } else if (args[0] == "?#UNREG") {
                    FileTypeRegistration.Unregister(nw, args[1]);
                } else {
                    MessageBox.Show("Unrecognized admin command: " + args[0], Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return true;
            }
            return false;
        }
        private static void ShowEdit(string filename, StartInfo si) {
            if (Menu.Menu.InvokeRequired) {
                Menu.Menu.Invoke(new Action<string, StartInfo>(ShowEdit), new object[] { filename, si });
                return;
            }
            StartInfoEditForm form = new StartInfoEditForm();
            form.FileName = filename;
            form.StartInfo = si;
            form.Show();
        }
        private static string TryLoadRunFile(string path) {
            if (path.StartsWith("-Edit:")) {
                path = path.Substring(6);
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                    TextReader reader = new StreamReader(path);
                    StartInfo si = (StartInfo)ser.Deserialize(reader);
                    reader.Close();
                    ShowEdit(path, si);
                } catch (Exception ex) {
                    return "Failed to load file " + path + ": " + ex.ToString() + "\n\n";
                }
                return null;
            }

            try {
                XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                TextReader reader = new StreamReader(path);
                StartInfo si = (StartInfo)ser.Deserialize(reader);
                reader.Close();
                if (si == null) throw new Exception("Generic loading error");
                try {
                    SpawnedProcess sp = new SpawnedProcess();
                    sp.Run(si);
                    Program.Menu.AddProcess(sp);
                } catch (Exception ex) {
                    return "Failed to start process " + si.Name + ": " + ex.ToString() + "\n\n";
                }
            } catch (Exception ex) {
                return "Failed to load file " + path + ": " + ex.ToString() + "\n\n";
            }
            return null;
        }
        private static void ParsingCommandLineArguments(string[] args) {
            string error = string.Empty;
            foreach (string arg in args) {
                string e = TryLoadRunFile(arg);
                if (e != null) error += e;
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
        internal static void Options_Click(object sender, EventArgs e) {
            OptionForm op = new OptionForm();
            icon.ModalDialog = op;
            icon.Menu = null;
            if (op.ShowDialog() == DialogResult.OK) {
                Properties.Settings.Default.KeepConsoles = op.KeepConsoles;
                Properties.Settings.Default.Save();
            }
            icon.ModalDialog = null;
            icon.Menu = Menu.Menu;
        }
    }
}
