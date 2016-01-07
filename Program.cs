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
            LoadAndStartForm lasf = new LoadAndStartForm();
            icon.ModalDialog = lasf;
            icon.Menu = null;
            lasf.ShowDialog();
            icon.ModalDialog = null;
            icon.Menu = Menu.Menu;
        }
    }
}
