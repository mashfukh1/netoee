using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OEE_SKRIPSI
{
    public class machineState
    {
        private string state;
        public string Name
        {
            get { return state; }
        }

        public machineState(string initialName)
        {
            state = initialName;
        }
    }
}
