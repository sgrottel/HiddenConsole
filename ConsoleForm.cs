using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    public partial class ConsoleForm : Form {
        private SpawnedProcess proc;
        private const int maxLines = 10000;
        internal SpawnedProcess SpawnedProcess {
            get { return proc; }
            set {
                if (proc != value) {
                    if (proc != null) {
                        proc.Output.WriteLine -= WriteLine;
                        proc.RunningChanged -= Proc_RunningChanged;
                    }
                    proc = value;
                    if (proc != null) {
                        Text = proc.Name;
                        proc.RunningChanged += Proc_RunningChanged;
                        Proc_RunningChanged(null, null);
                        foreach (string l in proc.Output.Content) WriteLine(l);
                        proc.Output.WriteLine += WriteLine;
                    } else {
                        Text = "HiddenConsole";
                        commandTextBox.Enabled = false;
                        commandEnterButton.Enabled = false;
                    }
                }
            }
        }
        private void Proc_RunningChanged(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(Proc_RunningChanged), new object[] { sender, e });
                return;
            }
            commandPanel.Visible
                = commandTextBox.Enabled
                = commandEnterButton.Enabled
                = proc.Running;
        }
        public ConsoleForm() {
            InitializeComponent();
            Icon = Properties.Resources.cmd;
        }
        private void WriteLine(string s) {
            if (InvokeRequired) {
                Invoke(new Action<string>(WriteLine), new object[] { s });
                return;
            }
            int selStart = this.conOutRichTextBox.SelectionStart;
            int selLen = this.conOutRichTextBox.SelectionLength;
            bool moveCaret
                = (selLen == 0)
                && (selStart == this.conOutRichTextBox.TextLength);
            if (!s.EndsWith("\n")) s += "\n";
            this.conOutRichTextBox.AppendText(s);
            if (this.conOutRichTextBox.Lines.Count() > maxLines) {
                this.conOutRichTextBox.Lines =
                    this.conOutRichTextBox.Lines.Skip(this.conOutRichTextBox.Lines.Count() - maxLines).ToArray();
            }
            if (moveCaret) {
                this.conOutRichTextBox.Select(this.conOutRichTextBox.TextLength, 0);
                this.conOutRichTextBox.ScrollToCaret();
            } else {
                this.conOutRichTextBox.Select(selStart, selLen);
            }
        }
        private void commandEnterButton_Click(object sender, EventArgs e) {
            if (!commandEnterButton.Enabled) return;

            string cmd = this.commandTextBox.Text.Trim();
            this.commandTextBox.Clear();
            if (String.IsNullOrWhiteSpace(cmd)) return;
            this.commandTextBox.AutoCompleteCustomSource.Add(cmd);

            proc.EnterLine(cmd);
        }
        private void commandTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (!commandEnterButton.Enabled) return;

            if (e.KeyCode == Keys.Enter) {
                this.commandEnterButton.PerformClick();
                e.Handled = true;
            }
        }
        private void commandTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (!commandEnterButton.Enabled) return;

            if (e.KeyChar == 13) { e.Handled = true; }
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            conOutRichTextBox.Copy();
        }
    }
}
