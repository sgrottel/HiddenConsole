using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    static internal class FileTypeRegistration {
        static public void Register(string exePath) {
            MessageBox.Show("Registering *.HCSI Files not implemented yet", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        static public void Unregister(string exePath) {
            MessageBox.Show("Unregistering *.HCSI Files not implemented yet", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
