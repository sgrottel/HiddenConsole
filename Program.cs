using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    static class Program {
        private static ContextMenuStrip menu;
        private static SG.Utilities.Forms.TrayIcon icon;
        private static ToolStripSeparator afterProcList;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(afterProcList = new ToolStripSeparator());
            menu.Items.Add("Exit").Click += Exit_Click;

            icon = new SG.Utilities.Forms.TrayIcon();
            icon.ShowContextMenuOnClick = true;
            icon.Menu = menu;
            icon.Visible = true;
            icon.Icon = new System.Drawing.Icon(@"D:\dev\MegaMol\[misc]\supplement.graphics\icons\MegaMol_Fallback.ico");

            StartInfo i = new StartInfo();
            i.Application = "ipconfig";
            i.Arguments = "/all";
            i.WorkingDirectory = @"D:\tmp";

            SpawnedProcess sp = new SpawnedProcess();
            sp.Run(i);
            ToolStripMenuItem pmi = new ToolStripMenuItem();
            menu.Items.Insert(menu.Items.IndexOf(afterProcList), pmi);
            pmi.Text = sp.Name;
            pmi.Tag = sp;

            Application.Run();

            sp.Wait();

            icon.Visible = false;
        }

        private static void Exit_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
