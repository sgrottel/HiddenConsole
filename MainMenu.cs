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
            Menu.Items.Add("Load and Run Start Info ...").Click += Program.LoadStartInfo_Click;
            Menu.Items.Add("Edit Start Info ...").Click += EditStartInfo_Click;
            Menu.Items.Add(new ToolStripSeparator());
            Menu.Items.Add("Options ...").Click += Program.Options_Click;
            Menu.Items.Add(beforeProcList = new ToolStripSeparator());
            Menu.Items.Add("No Processes").Enabled = false;
            Menu.Items.Add(afterProcList = new ToolStripSeparator());
            Menu.Items.Add("Exit").Click += Exit_Click;
        }
        private void EditStartInfo_Click(object sender, EventArgs e) {
            StartInfoEditForm form = new StartInfoEditForm();
            form.Show();
        }
        public void AddProcess(SpawnedProcess proc) {
            if (Menu.InvokeRequired) {
                Menu.Invoke(new Action<SpawnedProcess>(AddProcess), new object[] { proc });
                return;
            }

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
            proc.RunningChanged += Proc_RunningChanged;
            pmi.Image = proc.Running
                ? Properties.Resources.StatusAnnotations_Play_16xLG_color
                : Properties.Resources.StatusAnnotations_Stop_16xLG_color;
            pmi.Click += ProcessMenuItem_Click;
        }
        private void ProcessMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            SpawnedProcess p = (SpawnedProcess)tsmi.Tag;

            foreach (Form f in Application.OpenForms) {
                ConsoleForm c = f as ConsoleForm;
                if (c == null) continue;
                if (c.SpawnedProcess == p) {
                    c.BringToFront();
                    c.Focus();
                    return;
                }
            }

            ConsoleForm con = new ConsoleForm();
            con.SpawnedProcess = p;
            con.Show();
        }
        private void Proc_RunningChanged(object sender, EventArgs e) {
            if (Menu.InvokeRequired) {
                Menu.Invoke(new EventHandler(Proc_RunningChanged), new object[] { sender, e });
                return;
            }
            SpawnedProcess proc = (SpawnedProcess)sender;
            int cntExitedProcs = 0;
            foreach (ToolStripItem i in Menu.Items) {
                SpawnedProcess ip = i.Tag as SpawnedProcess;
                if (ip == null) continue;
                if (!ip.Running) cntExitedProcs++;
                if (i.Tag != proc) continue;
                i.Image = proc.Running
                    ? Properties.Resources.StatusAnnotations_Play_16xLG_color
                    : Properties.Resources.StatusAnnotations_Stop_16xLG_color;
            }
            if (cntExitedProcs > Properties.Settings.Default.KeepConsoles) {
                // close some consoles
                int closeConsoles = cntExitedProcs - Properties.Settings.Default.KeepConsoles;
                List<ToolStripItem> toClose = new List<ToolStripItem>();
                foreach (ToolStripItem i in Menu.Items) {
                    SpawnedProcess ip = i.Tag as SpawnedProcess;
                    if (ip == null) continue;
                    if (!ip.Running) {
                        if (closeConsoles > 0) {
                            toClose.Add(i);
                            closeConsoles--;
                        }
                        if (closeConsoles <= 0) {
                            break;
                        }
                    }
                }
                foreach (ToolStripItem i in toClose) {
                    Menu.Items.Remove(i);
                }
            }
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
            SpawnedProcess[] ps = Processes;
            foreach (SpawnedProcess p in ps) {
                if (p.Running) p.RequestExit();
            }
            Application.Exit();
        }
    }
}
