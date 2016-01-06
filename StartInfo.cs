﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenConsole {
    public class StartInfo {
        public String Application { get; set; }
        public String WorkingDirectory { get; set; }
        public String Arguments { get; set; }
        public String Name {
            get {
                if (String.IsNullOrWhiteSpace(Application)) return "";
                string n = Application;
                try {
                    n = System.IO.Path.GetFileNameWithoutExtension(n);
                } catch { }
                return n;
            }
        }
    }
}
