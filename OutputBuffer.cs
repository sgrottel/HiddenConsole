using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenConsole {
    public class OutputBuffer {
        public event Action<string> WriteLine;
        private List<String> content = new List<string>();
        public void Add(OutputLineType t, string l) {
            content.Add(l);
            Action<string> e = WriteLine;
            if (e != null) {
                e(l);
            }
        }
        public IEnumerable<string> Content { get { return content; } }
    }
}
