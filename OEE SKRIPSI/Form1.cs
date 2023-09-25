using LiveCharts.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using LiveCharts.WinForms;
using System.Timers;
using Org.BouncyCastle.Asn1.X509.SigI;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Interop;
using static System.Net.WebRequestMethods;
using Org.BouncyCastle.Crypto.IO;

namespace OEE_SKRIPSI
{
    public partial class Form1 : Form
    {
        private static Thread updateDataThread;
        private static Thread updateDataChart;
        private static Thread updateDataModbus;
        private static Thread updateTimeMixDB;
        private static Thread updateTimeFillDB;

        private SeriesCollection seriesCollection;

        private List<DataPoint> data;

        public int mixAvaibilityData = 1;
        public int mixPerformanceData = 2;
        public int mixQualityData = 3;
        public int mixOeeData = 4;

        public int fillAvaibilityData = 5;
        public int fillPerformanceData = 6;
        public int fillQualityData = 7;
        public int fillOeeData = 8;



        public int On_M_State = 0;
        public int Off_M_State = 0;

        public int On_F_State = 0;
        public int Off_F_State = 0;

        modbusClass modbus = new modbusClass();

        Database DB = new Database();
        Database insertDB = new Database();

        private static volatile bool stopThread = false;
        
        setValMixing set = new setValMixing();
        Querys querys= new Querys();



        public Form1()
        {
            InitializeComponent();
            
            modbus.addClient();

            updateDataThread = new Thread(updateAllData);  
            updateDataChart = new Thread(updateChart);
            updateDataModbus = new Thread(updateModbus);
            updateTimeMixDB = new Thread(updateTimeMix);
            updateTimeFillDB = new Thread(updateTimeFill);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            modbus.modbusInit();

            updateDataThread.Start();
            updateDataChart.Start();
            updateDataModbus.Start();
            updateTimeMixDB.Start();
            updateTimeFillDB.Start();

        }
        public void updateModbus()
        {
            while (!stopThread)
             {
                
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
               
                string mC   = convertToText(modbus.mixConnection);
                string mIP  = modbus.inIPmix;
                string mSt  = modbus.mixState;
                string mVal = modbus.mixVal;
                string mDf  = modbus.mixDefect;

                string fC   = convertToText(modbus.fillConnection);
                string fIP  = modbus.inIPfill;
                string fSt  = modbus.fillState;
                string fVal = modbus.fillVal;
                string fDf  = modbus.fillDefect;

                if (mSt == "0")
                {
                    mSt = "4";
                }
                if (fSt == "0")
                {
                    fSt= "4";
                }

                modbus.readData();
                
                tbLog.Invoke((MethodInvoker)delegate
                {
                    if (tbLog.TextLength > 0)
                    {
                        tbLog.AppendText(Environment.NewLine);
                    }

                    tbLog.AppendText($"[{timestamp}] : Mixing Machine => Connection  : {mC}, IP : {mIP}, Mix state : {mSt}, Mix val : {mVal}, Mix Defect : {mDf}" + Environment.NewLine);
                    tbLog.AppendText($"[{timestamp}] : Filling Machine => Connection : {fC}, IP : {fIP}, Mix state : {fSt}, Mix val : {fVal}, Mix Defect : {fDf}" + Environment.NewLine);

                    DB.insertToDB("mixing", mSt, mVal, mDf);
                    DB.insertToDB("filling", fSt, fVal, fDf);

                });

                Thread.Sleep(10000);
            }
        }

        public void updateTimeMix()

        {
           while (!stopThread)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string mSt = modbus.mixState;

                tbLog.Invoke((MethodInvoker)delegate
                {
                   
                if (mSt != DB.readMachineStatus(1))
                {
                    while (Off_M_State < 1)
                    {
                        Off_M_State += 1;
                        DB.insertTimeToDB("mixing", mSt,convertStateToText(mSt), timestamp, timestamp, "0");
                        tbLog.AppendText($"[{timestamp}] : Mixing Machine => change state detected to {mSt} on mixing machine, result : {DB.resultSend}" + Environment.NewLine);
                    }                   
                }

                else
                {

                        while (On_M_State < 1)
                        {
                            On_M_State += 1;
                            DB.insertTimeToDB("mixing", DB.readMachineStatus(1),convertStateToText(DB.readMachineStatus(1)), timestamp, timestamp, "0");
                            tbLog.AppendText($"[{timestamp}] : Mixing Machine => insert first state 0 on mixing machine, result : {DB.resultSend}" + Environment.NewLine);
                        }

                        Off_M_State = 0;
                        DB.updateTimeToDB("mixing");
                        tbLog.AppendText($"[{timestamp}] : Mixing Machine => update time in state {mSt} mixing machine, result : {DB.resultSend}" + Environment.NewLine);
                }

                });
                Thread.Sleep(3000);
            }
            }

        public void updateTimeFill()

        {
            while (!stopThread)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                string fSt = modbus.fillState;
               
                if (fSt == null || fSt == "0")
                {
                    fSt = "4";
                }


                tbLog.Invoke((MethodInvoker)delegate
                {

                    if (fSt != DB.readMachineStatus(2))
                    {
                        while (Off_F_State < 1)
                        {
                            Off_F_State += 1;
                            DB.insertTimeToDB("filling",fSt,convertStateToText(fSt), timestamp, timestamp, "0");
                            tbLog.AppendText($"[{timestamp}] : Filling Machine => change state detected to {fSt} on filling machine, result : {DB.resultSend}" + Environment.NewLine);
                        }
                    }

                    else
                    {
                        while (On_F_State < 1)
                        {
                            On_F_State += 1;
                            DB.insertTimeToDB("filling", DB.readMachineStatus(2), convertStateToText(DB.readMachineStatus(2)), timestamp, timestamp,"0");
                            tbLog.AppendText($"[{timestamp}] : Filling Machine => insert first state 0 on filling machine, result : {DB.resultSend}" + Environment.NewLine);
                        }

                        Off_F_State = 0;
                        DB.updateTimeToDB("filling");
                        tbLog.AppendText($"[{timestamp}] : Filling Machine => update time in state {fSt} filling machine, result : {DB.resultSend}" + Environment.NewLine);
                    }

                });
                Thread.Sleep(3000);
            }
        }
        public void indvMachineState(string Machine, GroupBox box, Label lbl)           
            {

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (Machine == "1")
                    {
                        machineState state = new machineState("Production");
                        box.BackColor = Color.LimeGreen;
                        lbl.Text = state.Name;
                    }
                    else if (Machine == "2")
                    {
                        machineState state = new machineState("Idle");
                        box.BackColor = Color.Orange;
                        lbl.Text = state.Name;
                    }
                    else if (Machine == "3")
                    {
                        machineState state = new machineState("Stop");
                        box.BackColor = Color.Red;
                        lbl.Text = state.Name;
                    }
                    else if (Machine == "4")
                    {
                        machineState state = new machineState("Error");
                        box.BackColor = Color.Gray;
                        lbl.Text = state.Name;
                    }
                    else if (Machine == "5")
                    {
                        machineState state = new machineState("Planned");
                        box.BackColor = Color.Yellow;
                        lbl.Text = state.Name;
                    }
                    else
                    {
                        machineState state = new machineState("Lost");
                        box.BackColor = Color.Brown;
                        lbl.Text = state.Name;
                    }
                });
            }
            catch
            {

            }
        }

        public int countCaracters(string s, char c)
        {
            int count = 0;
            foreach (var ch in s)
            {
                if (ch == c)
                    count++;
            }
            return count;
        }

        void updateAllData()
        {
            while (!stopThread)
            {
                updateStateLable();
                updateGauge();
                updateTable();
                updateAllRecordTime();
                Thread.Sleep(1000);
            }

        }

       void updateAllRecordTime()
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    lblErrTimeSubMix.Text  = countTime(Convert.ToInt32(DB.totalTime("mixing", "4")));
                    lblProdTimeSubMix.Text = countTime(Convert.ToInt32(DB.totalTime("mixing", "1")));
                    lblIdleTimeSubMix.Text = countTime(Convert.ToInt32(DB.totalTime("mixing", "2")));
                    lblStopTimeSubMix.Text = countTime(Convert.ToInt32(DB.totalTime("mixing", "3")));
                    lblPlanTimeSubMix.Text = countTime(Convert.ToInt32(DB.totalTime("mixing", "5")));

                    lblErrTimeSubFill.Text  = countTime(Convert.ToInt32(DB.totalTime("filling", "4")));
                    lblProdTimeSubFill.Text = countTime(Convert.ToInt32(DB.totalTime("filling", "1")));
                    lblIdleTimeSubFill.Text = countTime(Convert.ToInt32(DB.totalTime("filling", "2")));
                    lblStopTimeSubFill.Text = countTime(Convert.ToInt32(DB.totalTime("filling", "3")));
                    lblPlanTimeSubFill.Text = countTime(Convert.ToInt32(DB.totalTime("filling", "5")));
                }
                catch { 
                
                }
            });
        }
        void filterStateLable(char state, Label lbl, string data)
        {
            lbl.Invoke((MethodInvoker)delegate {
                lbl.Text = countCaracters(data, state).ToString();
            });
        }

        void updateGauge()
        {
            setGauge(AvSubMixGauge, DB.readMachine(mixAvaibilityData));
            setGauge(PerfSubMixGauge, DB.readMachine(mixPerformanceData));
            setGauge(QualSubMixGauge, DB.readMachine(mixQualityData));
            setGauge(OeeMixingGauge, DB.readMachine(mixOeeData));

            setGauge(AvSubFilGauge, DB.readMachine(fillAvaibilityData));
            setGauge(PerfSubFilGauge, DB.readMachine(fillPerformanceData));
            setGauge(QualSubFilGauge, DB.readMachine(fillQualityData));
            setGauge(OeeFillingGauge, DB.readMachine(fillOeeData));
        }
        void updateTable()
        {
            DB.showTableFromDB(dgvFill, querys.GetTableProductionHistory[0]);
            DB.showTableFromDB(dgvMix, querys.GetTableProductionHistory[1]);
            DB.showTableFromDB(dgvFillEvent, querys.GetTableEventState[0]);
            DB.showTableFromDB(dgvMixEvent, querys.GetTableEventState[1]);
        }

        void updateStateLable()
        {
            indvMachineState(DB.readMachineStatus(1), gbMcnStateMix, lblMcnStateMix);
            indvMachineState(DB.readMachineStatus(2), gbMcnStateFil, lblMcnStateFil);

            string stateCount = DB.readMachineStatus(1) + DB.readMachineStatus(2);

            filterStateLable('1', lblTotOperatMcn, stateCount);
            filterStateLable('2', lblTotIdleMcn, stateCount);
            filterStateLable('3', lblTotStopMcn, stateCount);
            filterStateLable('4', lblTotErrMcn, stateCount);
            filterStateLable('5', lblTotPlanMcn, stateCount);
        }

        private void cbRealtimeAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRealtimeAll.Checked)
            {
                stopThread = false;
                updateDataThread = new Thread(updateAllData);
                updateDataThread.Start();

                updateDataChart = new Thread(updateChart);
                updateDataChart.Start();

                updateDataModbus = new Thread(updateModbus);
                updateDataModbus.Start();

                updateTimeMixDB = new Thread(updateTimeMix);
                updateTimeMixDB.Start();

                updateTimeFillDB = new Thread(updateTimeFill);
                updateTimeFillDB.Start();
            }
            else
            {
                if (updateDataThread != null && updateDataThread.IsAlive && updateDataChart != null && updateDataChart.IsAlive && updateDataModbus != null && 
                    updateDataModbus.IsAlive && updateTimeMixDB != null && updateTimeMixDB.IsAlive && updateTimeFillDB != null && updateTimeFillDB.IsAlive)
                {
                    stopThread = true;

                    updateDataThread.Join();
                    updateDataChart.Join();
                    updateDataModbus.Join();
                    updateTimeMixDB.Join();
                    updateTimeFillDB.Join();
                }
            }
        }

        public class DataPoint
        {
            public long Timestamp { get; set; }
            public double Value { get; set; }

            public DataPoint(long timestamp, double value)
            {
                Timestamp = timestamp;
                Value = value;
            }
        }

        private void lblTotOperatMcn_Click(object sender, EventArgs e)
        {

        }

        private void lblTotStopMcn_Click(object sender, EventArgs e)
        {

        }

        private void performanceMixChart_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void cartesianChart2_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        public void updateChart()
        {
            while (!stopThread)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    DB.showChart(performanceFilChart,"filling");
                    DB.showChart(performanceMixChart,"mixing");

                    performanceFilChart.DisableAnimations = true;
                    performanceMixChart.DisableAnimations = true;
                });

                Thread.Sleep(5000);
            }
        }

        public void setGauge(SolidGauge gauge, int value)
        {

           this.Invoke((MethodInvoker)delegate {

                gauge.Uses360Mode = false;
                gauge.From = 0;
                gauge.To = 100;
                gauge.Value = value;    
              
            });
        }

        public string countTime(int second)
        {
            int totalSeconds = second; 
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
           
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            
            return formattedTime;
        }

        public string resultText;
        string convertToText(string input)
        {
            if (input == null) {
                resultText = "Failed";
            }
            if (input == "1")
            {
                resultText = "True";
            }
            else
            {
                resultText = "False";
            }
            return resultText;
        }

        public string stateString;

        string convertStateToText(string Machine)
        {
            if (Machine == "1")
            {
                stateString = "Production";
            }
            else if (Machine == "2")
            {
                stateString = "Idle";
            }
            else if (Machine == "3")
            {
                stateString = "Stop";
            }
            else if (Machine == "4")
            {
                stateString = "Error";
            }
            else if (Machine == "5")
            {
                stateString = "Planned";
            }
            else
            {
                stateString = "Unknow";
            }
            return stateString;
        }
        private void tm_idvMachine_Tick(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void lblQualQuality_Click(object sender, EventArgs e)
        {

        }
    }

  }
