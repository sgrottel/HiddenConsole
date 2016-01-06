using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    class MainMenu {
        public ContextMenuStrip Menu { get; private set; }
        private static ToolStripSeparator beforeProcList;
        private static ToolStripSeparator afterProcList;
        public MainMenu() {
            Menu = new ContextMenuStrip();
            Menu.Items.Add("Load and Run Start Info ..."); // TODO
            Menu.Items.Add("Edit Start Info ..."); // TODO
            Menu.Items.Add(new ToolStripSeparator());
            Menu.Items.Add("Options ..."); // TODO
            Menu.Items.Add(beforeProcList = new ToolStripSeparator());
            Menu.Items.Add("No Processes").Enabled = false;
            Menu.Items.Add(afterProcList = new ToolStripSeparator());
            Menu.Items.Add("Exit").Click += Exit_Click;
        }
        public void AddProcess(SpawnedProcess proc) {
            int i1 = Menu.Items.IndexOf(beforeProcList);
            int i2 = Menu.Items.IndexOf(afterProcList);
            int i = Math.Min(i1, i2);
            int ic = Math.Max(i1, i2);

            // remove non-process items from the menu
            for (++i; i < ic; ++i) {
                ToolStripItem tsi = Menu.Items[i];
                if ((tsi.Tag as SpawnedProcess) == null) {
                    Menu.Items.RemoveAt(i);
                    i--;
                    ic--;
                }
            }

            ToolStripMenuItem pmi = new ToolStripMenuItem();
            Menu.Items.Insert(Menu.Items.IndexOf(afterProcList), pmi);
            pmi.Text = proc.Name;
            pmi.Tag = proc;
        }
        public SpawnedProcess[] Processes { get
            {
                List<SpawnedProcess> p = new List<SpawnedProcess>();
                foreach (ToolStripItem i in Menu.Items) {
                    SpawnedProcess s = i.Tag as SpawnedProcess;
                    if (s != null) p.Add(s);
                }
                return p.ToArray();
            } }
        public void WaitForAllProcesses() {
            foreach (SpawnedProcess s in Processes) {
                if (s.Running) s.Wait();
            }
        }
        private void Exit_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
