using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEE_SKRIPSI
{
    public class setValFilling
    {
        private int valReadFilState;
        private int valReadFilVal;
        private int valReadFilDefect;
        private string valRecvData;

        public setValFilling(int valReadFilState, int valReadFilVal, int valReadFilDefect, string valRecvData)
        {
            this.valReadFilState = valReadFilState;
            this.valReadFilVal = valReadFilVal;
            this.valReadFilDefect = valReadFilDefect;
            this.valRecvData = valRecvData;
        }
      public int readFilState
        {
            get
            {
                return valReadFilState;
            }
        }

        public int readFilVal

        {
            get
            {
                return valReadFilVal;
            }
        }

        public int readFilDefect

        {
            get
            {
                return valReadFilDefect;
            }
        }

        public string RecvData
        {
            get
            {
                return valRecvData;
            }
        }
    }
}
