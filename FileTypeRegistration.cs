using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenConsole {
    static internal class FileTypeRegistration {
        static public void Register(IWin32Window owner, string exePath) {
            try {
                RegistryKey ext_hcsi = Registry.ClassesRoot.CreateSubKey(".hcsi");
                ext_hcsi.SetValue(null, "HiddenConsole.StartInfo");
                ext_hcsi.Close();
                RegistryKey desc_hcsi = Registry.ClassesRoot.CreateSubKey("HiddenConsole.StartInfo");
                desc_hcsi.SetValue(null, "HiddenConsole Start Info File");
                RegistryKey shell_desc_hcsi = desc_hcsi.CreateSubKey("shell");
                RegistryKey open_shell_desc_hcsi = shell_desc_hcsi.CreateSubKey("open");
                RegistryKey cmd_open_shell_desc_hcsi = open_shell_desc_hcsi.CreateSubKey("command");
                cmd_open_shell_desc_hcsi.SetValue(null, "\"" + exePath + "\" \"%1\"");
                cmd_open_shell_desc_hcsi.Close();
                open_shell_desc_hcsi.Close();
                shell_desc_hcsi.Close();
                desc_hcsi.Close();

                MessageBox.Show(owner, "File type *.hcsi registered.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show(owner, "Failed to register *.hcsi file type:\n" + ex.ToString(),
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static public void Unregister(IWin32Window owner, string exePath) {
            try {
                Registry.ClassesRoot.DeleteSubKeyTree(".hcsi", false);
                Registry.ClassesRoot.DeleteSubKeyTree("HiddenConsole.StartInfo", false);
                MessageBox.Show(owner, "File type *.hcsi unregistered.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show(owner, "Failed to unregister *.hcsi file type:\n" + ex.ToString(),
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
