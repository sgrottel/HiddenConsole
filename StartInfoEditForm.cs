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
    public partial class StartInfoEditForm : Form {
        public StartInfo StartInfo {
            get {
                StartInfo si = new StartInfo();
                si.Application = textBox1.Text;
                si.Arguments = textBox2.Text;
                si.WorkingDirectory = textBox3.Text;
                return si;
            }
            set {
                textBox1.Text = (value == null) ? "" : value.Application;
                textBox2.Text = (value == null) ? "" : value.Arguments;
                textBox3.Text = (value == null) ? "" : value.WorkingDirectory;
            }
        }
        public StartInfoEditForm() {
            InitializeComponent();
            Icon = Properties.Resources.cmd;
        }
    }
}
