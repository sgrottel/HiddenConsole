using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenConsole {
    class SpawnedProcess {
        public String Name { get; private set; }
        public void Run(StartInfo s) {
            if (p != null) throw new Exception("Process already started");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.Arguments = s.Arguments;
            psi.CreateNoWindow = true;
            psi.ErrorDialog = true;
            psi.FileName = s.Application;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false; // required for stream redirection
            psi.WorkingDirectory = s.WorkingDirectory;
            Name = s.Name;
            p = Process.Start(psi);
            p.EnableRaisingEvents = true;
            p.StandardOutput.ReadAsync(stdOutBuf, 0, stdOutBuf.Length).ContinueWith(stdOutRead);
            p.StandardError.ReadAsync(stdErrBuf, 0, stdErrBuf.Length).ContinueWith(stdErrRead);
            p.Exited += P_Exited;
            P_Exited(null, null); // fire running changed, just to be sure (most likely a false positive)
        }
        public void Wait() {
            if (p != null) p.WaitForExit();
        }
        public OutputBuffer Output { get { return outBuf; } }
        public bool Running { get { return (p != null) ? !p.HasExited : false; } }
        public event EventHandler RunningChanged;

        private OutputBuffer outBuf = new OutputBuffer();
        #region output collection
        private StringBuilder stdOutStrBuf = new StringBuilder();
        private StringBuilder stdErrStrBuf = new StringBuilder();
        private char[] stdOutBuf = new char[200];
        private char[] stdErrBuf = new char[200];
        private void stdOutRead(Task<int> size) {
            this.receiveBuffer(ref stdOutBuf, ref stdOutStrBuf, size.Result, p.StandardOutput, stdOutRead, StdOutLine);
        }
        private void stdErrRead(Task<int> size) {
            this.receiveBuffer(ref stdErrBuf, ref stdErrStrBuf, size.Result, p.StandardError, stdErrRead, StdErrLine);
        }
        private void receiveBuffer(ref char[] charBuf, ref StringBuilder strBuf, int size, StreamReader stream, Action<Task<int>> onRead, Action<string> outLine) {
            if (size > 0) {
                strBuf.Append(charBuf, 0, size);
                this.manageBuffer(ref strBuf, outLine);
            }

            if (size == 0) {
                if (p.HasExited) {
                    if (strBuf.Length > 0) {
                        strBuf.Append("\n");
                        this.manageBuffer(ref strBuf, outLine);
                    }
                    return;
                }
            }

            if (stream.BaseStream.CanRead) {
                stream.ReadAsync(charBuf, 0, charBuf.Length).ContinueWith(onRead);
            }
        }
        private void manageBuffer(ref StringBuilder strBuf, Action<string> wrt) {
            strBuf.Replace(Environment.NewLine, "\n");
            string s = strBuf.ToString();
            int nlp = s.IndexOf('\n');
            while (nlp >= 0) {
                string line = s.Substring(0, nlp);
                strBuf.Remove(0, nlp + 1);
                if (wrt != null) {
                    wrt(line);
                }
                s = strBuf.ToString();
                nlp = s.IndexOf('\n');
            }
        }
        private void StdOutLine(string line) {
            outBuf.Add(OutputLineType.Output, line);
        }
        private void StdErrLine(string line) {
            outBuf.Add(OutputLineType.Error, line);
        }
        #endregion
        private void P_Exited(object sender, EventArgs e) {
            EventHandler eh = RunningChanged;
            if (eh != null) {
                eh(this, null);
            }
        }
        private Process p = null;
        internal void RequestExit() {
            if (p != null) p.Kill(); // not nice, but I do not care
        }
        internal void EnterLine(string cmd) {
            try {
                p.StandardInput.WriteLine(cmd);
            } catch { }
        }
    }
}
