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
        private static NamedPipeServerStream appServerPipe = null;
        private static byte[] appServerPipeInBuffer;
        private static int appServerPipeBytesReceived;
        private static int appServerPipeWaitForBytes;
        private static int appServerPipeReceiveMode;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (CheckAdminCmdLineArgs(args)) return;

            try {
                appServerPipe = new NamedPipeServerStream("HiddenConsole", PipeDirection.In, 1);
                appServerPipeInBuffer = new byte[1024];
                appServerPipe.WaitForConnectionAsync().ContinueWith(appServerPipeConnected);
            } catch { }

            if ((appServerPipe == null) && (args.Length > 0)) {
                // I am a secondary instance
                try {
                    using (NamedPipeClientStream npcs = new NamedPipeClientStream(".", "HiddenConsole", PipeDirection.Out)) {
                        npcs.Connect();
                        foreach (string arg in args) {
                            byte[] buf = System.Text.Encoding.UTF8.GetBytes(arg);
                            byte[] buf2 = BitConverter.GetBytes((int)buf.Length);
                            npcs.Write(buf2, 0, 4);
                            npcs.Write(buf, 0, buf.Length);
                        }
                    }
                } catch(Exception ex) {
                    MessageBox.Show("Failed to communicate parameters: " + ex.ToString(),
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
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
        private static void appServerPipeConnected(Task t) {
            if (!t.IsFaulted) {
                appServerPipeBytesReceived = 0;
                appServerPipeWaitForBytes = 4; // length of incoming string
                appServerPipeReceiveMode = 0; // string length
                try {
                    if (appServerPipeInBuffer.Length < appServerPipeWaitForBytes) appServerPipeInBuffer = new byte[appServerPipeWaitForBytes];
                    appServerPipe.ReadAsync(appServerPipeInBuffer, appServerPipeBytesReceived, appServerPipeWaitForBytes).ContinueWith(appServerPipeReceived);
                } catch { }
            }
        }
        private static void appServerPipeReceived(Task<int> dataSize) {
            if (!dataSize.IsFaulted) {
                appServerPipeBytesReceived += dataSize.Result;
                if (appServerPipeBytesReceived > appServerPipeWaitForBytes) {
                    // Does this happen? I hope not
                    Debug.WriteLine("Ack");
                } else if (appServerPipeBytesReceived == appServerPipeWaitForBytes) {
                    // handle message
                    switch (appServerPipeReceiveMode) {
                        case 0:
                            int strLen = BitConverter.ToInt32(appServerPipeInBuffer, 0);
                            if (strLen > 0) {
                                appServerPipeReceiveMode = 1; // string data
                                appServerPipeWaitForBytes = strLen;
                                appServerPipeBytesReceived = 0;
                            }
                            break;
                        case 1:
                            string s = System.Text.Encoding.UTF8.GetString(appServerPipeInBuffer, 0, appServerPipeWaitForBytes);
                            Debug.WriteLine(">>>" + s);
                            string error = TryLoadRunFile(s);
                            if (error != null) {
                                MessageBox.Show(error, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            appServerPipeBytesReceived = 0;
                            appServerPipeWaitForBytes = 4; // length of incoming string
                            appServerPipeReceiveMode = 0; // string length
                            break;
                    }
                }
            }

            if (appServerPipe.IsConnected) {
                // request more data
                try {
                    appServerPipe.ReadAsync(appServerPipeInBuffer, appServerPipeBytesReceived, appServerPipeWaitForBytes).ContinueWith(appServerPipeReceived);
                } catch { }

            } else {
                // pipe broken
                try {
                    appServerPipe.Disconnect();
                    appServerPipe.WaitForConnectionAsync().ContinueWith(appServerPipeConnected);
                } catch { }
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
        private static string TryLoadRunFile(string path) {
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
