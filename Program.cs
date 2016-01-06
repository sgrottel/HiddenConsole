using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            icon.Icon = Properties.Resources.cmd;

            StartInfo i = new StartInfo();
            i.Application = "ipconfig";
            i.Arguments = "/all";
            i.WorkingDirectory = @"D:\tmp";

            SpawnedProcess sp = new SpawnedProcess();
            sp.Run(i);
            Menu.AddProcess(sp);

            Application.Run();

            Menu.WaitForAllProcesses();
            icon.Visible = false;
        }
    }
}
