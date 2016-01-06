using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    static class Program {
        private static MainMenu menu;
        private static SG.Utilities.Forms.TrayIcon icon;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            menu = new MainMenu();

            icon = new SG.Utilities.Forms.TrayIcon();
            icon.ShowContextMenuOnClick = true;
            icon.Menu = menu.Menu;
            icon.Visible = true;
            icon.Icon = Properties.Resources.cmd;

            StartInfo i = new StartInfo();
            i.Application = "ipconfig";
            i.Arguments = "/all";
            i.WorkingDirectory = @"D:\tmp";

            SpawnedProcess sp = new SpawnedProcess();
            sp.Run(i);
            menu.AddProcess(sp);

            Application.Run();

            menu.WaitForAllProcesses();
            icon.Visible = false;
        }
    }
}
