using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenConsole {
    class SpawnedProcess {
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
            p = Process.Start(psi);
        }
        public void Wait() {
            if (p != null) p.WaitForExit();
        }
        private Process p = null;
    }
}
