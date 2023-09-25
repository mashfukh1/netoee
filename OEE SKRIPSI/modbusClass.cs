using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using EasyModbus;
using static System.Net.Mime.MediaTypeNames;

namespace OEE_SKRIPSI 
{
    public class modbusClass
    {
        public string inIPfill, fillConnection, fillVal, fillState, fillDefect;
        public string inIPmix, mixConnection, mixVal, mixState, mixDefect;

        List<ModbusClient> modbusClients = new List<ModbusClient>();

        static String[] ip = { Properties.Settings.Default.ipMixing, Properties.Settings.Default.ipFilling};

        public void addClient()
        {
            for (int i = 0; i < ip.Count(); i++)
            {
                ModbusClient mc = new ModbusClient();
                modbusClients.Add(mc);
            }
        }

        public void modbusInit()
        {

            for (int i = 0; i < ip.Count(); i++)
            {
                if (modbusClients[i].Connected == false)
                {
                    try
                    {
                        modbusClients[i].IPAddress = ip[i];
                        modbusClients[i].Port = 502;
                    }
                    catch
                    {

                    }
                }
            }

        }
        public void readData()
        {
           for (int i = 0; i < ip.Count(); i++)
                {
                    try
                    {
                        modbusClients[i].Connect();
                        int[] datas = modbusClients[i].ReadHoldingRegisters(0, 3);
                        string s = string.Join(",", datas);
                        setmessage("1" + ":" + ip[i] + ":" + s);
                        modbusClients[i].Disconnect();

                    }
                    catch (Exception e)
                    {
                        setmessage("0" + ":" + ip[i] + ":" + "0,0,0");
                    }

            }           
        }
        void setmessage(string message)
        {
            char[] separatorA = { ':' };
            char[] separatorB = { ',' };


            string[] modbus = message.Split(separatorA);
            string[] data = modbus[2].Split(separatorB);

            string stateData = modbus[0];
            string ipData = modbus[1];

            if (ipData == ip[0])
            {
                inIPmix       = modbus[1]; 
                mixConnection = modbus[0];

                mixState       = data[0];
                mixVal         = data[1];
                mixDefect      = data[2];

            }
            if (ipData == ip[1])
            {

                inIPfill = modbus[1];
                fillConnection = modbus[0];

                fillState = data[0];
                fillVal = data[1];
                fillDefect = data[2];
            }

        }

    }
}
