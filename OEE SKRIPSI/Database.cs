using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Org.BouncyCastle.Asn1.Crmf;
using System.Data;
using System.Collections;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Definitions.Charts;
using Google.Protobuf.WellKnownTypes;
using Mysqlx.Crud;
using Org.BouncyCastle.Utilities.Collections;
using System.Windows.Media.Effects;
using Org.BouncyCastle.Crypto.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Data.SqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Windows.Controls.Primitives;
using static Mysqlx.Datatypes.Scalar.Types;

namespace OEE_SKRIPSI
{
    internal class Database
    {

        public string mixingStateResult;
        public string fillingStateResult;

        public string mixingTimeResult;
        public string fillingTimeResult;


        public string mixingAvaibilityResult;
        public string mixingPerformanceResult;
        public string mixingQualityResult;
        public string mixingOeeResult;


        public string fillingAvaibilityResult;
        public string fillingPerformanceResult;
        public string fillingQualityResult;
        public string fillingOeeResult;
        public string nQuery;


        public static string dbConnect = "Server=" + Properties.Settings.Default.dbServer +
                                         ";Port=" + Properties.Settings.Default.dbPort +
                                         ";Database=" + Properties.Settings.Default.dbName +
                                         ";Uid=" + Properties.Settings.Default.dbUsername +
                                         ";Pwd=;";

        
        public MySqlConnection mySqlConnection = new MySqlConnection(dbConnect);
        private System.Windows.Forms.Timer refreshTimer;


        public string resultSend;
        public int timeResult;
        public string valResult;

        private DataGridView dgv;
        private DataGridView dataGridView1;
        

        public string readMachineStatus(int machine)
        {
            Querys querys= new Querys();

            using (MySqlConnection connection = new MySqlConnection(dbConnect))
            {
                using (MySqlCommand command = new MySqlCommand(

                    querys.GetMixState +
                    querys.GetFillState

                    , connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mixingStateResult = reader["state"].ToString();
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    fillingStateResult = reader["state"].ToString();
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        
                    }
                        if (machine == 1)
                           {
                             return mixingStateResult;
                           }
                        else
                           {
                             return fillingStateResult;
                           }
                }    
            }
        }

        public int readMachine(int data)
        {
            Querys querys = new Querys(); 
            
            using (MySqlConnection connection = new MySqlConnection(dbConnect))
            {
                using (MySqlCommand command = new MySqlCommand(

                  querys.mixAvaibilityVal +
                  querys.mixPerformanceVal +
                  querys.mixQualityVal +
                  querys.mixOeeVal+
                  querys.filAvaibilityVal +
                  querys.filPerformanceVal +
                  querys.filQualityVal +
                  querys.filOeeVal

                    , connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mixingAvaibilityResult = reader.GetString(0);
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    mixingPerformanceResult = reader.GetString(0);
                                }
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    mixingQualityResult = reader.GetString(0);
                                }
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    mixingOeeResult = reader.GetString(0);
                                }
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    fillingAvaibilityResult = reader.GetString(0);
                                }
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                     fillingPerformanceResult = reader.GetString(0);
                                }
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    fillingQualityResult = reader.GetString(0);
                                }
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    fillingOeeResult = reader.GetString(0);
                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {

                    }

                    if (data == 1)
                    {
                        return Convert.ToInt32(mixingAvaibilityResult);
                    }
                    else if (data == 2)
                    {
                        return Convert.ToInt32(mixingPerformanceResult);
                    }
                    else if (data == 3)
                    {
                        return Convert.ToInt32(mixingQualityResult);
                    }
                    else if  (data == 4) 
                    {
                        return Convert.ToInt32(mixingOeeResult);
                    }
                    else if (data == 5)
                    {
                        return Convert.ToInt32(fillingAvaibilityResult);
                    }
                    else if (data == 6)
                    {
                        return Convert.ToInt32(fillingPerformanceResult);
                    }
                    else if (data == 7)
                    {
                        return Convert.ToInt32(fillingQualityResult);
                    }
                    else 
                    {
                        return Convert.ToInt32(fillingOeeResult);
                    }
                }
            }
        }

        public void showTableFromDB(DataGridView dgv, string querys)
        {
            Querys query = new Querys();

            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(dbConnect))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(querys,connection))
                {
                    using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                        
                        dgv.Invoke((MethodInvoker)delegate {
                            dgv.DataSource = dataTable;
                        });
                    }
                }
            }
        }

        public DataTable FillChart()
        {
            using (MySqlConnection connection = new MySqlConnection(dbConnect))
            {
              
              DataTable dt = new DataTable();
              connection.Open();
              MySqlDataAdapter da = new MySqlDataAdapter("SELECT datetime, performance FROM value_oee_filling ;", connection);
              da.Fill(dt);
              return dt;
              connection.Close();
             }
        }

        public void showChart(LiveCharts.WinForms.CartesianChart chart, string querys)
        {

            Querys query = new Querys();
        
            string connectionString = dbConnect;
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                if (querys == "mixing")
                {
                    nQuery = query.mixPerformanceValChart;
                }

                if (querys == "filling")
                {
                    nQuery = query.filPerformanceValChart;
                }

                MySqlCommand cmd = new MySqlCommand(nQuery, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                ChartValues<double> values = new ChartValues<double>();
                ChartValues<string> labels = new ChartValues<string>();

                while (reader.Read())
                {
                    labels.Add(reader.GetDateTime("datetime").ToString("yyyy-MM-dd"));
                    values.Add(reader.GetDouble("performance"));
                }

                reader.Close();
                connection.Close();

                PopulateChart(values, labels, chart);
            }
            catch
            {

            }

        }

        public void insertToDB(string machine, string state, string val, string defect)
        {
            Querys querys = new Querys();
            
            string dateOnly = DateTime.Now.ToString("yyyy-MM-dd");
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string connectionString = dbConnect;
            try
            {
                if (machine != null && state != null && val != null && defect != null) {

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        if (machine == "mixing")
                        {
                            string insertQuery = "INSERT INTO realtime_mixing_machine_state (id,datetime,date,state,value,defect) " +
                                                 "VALUES " +
                                                 "(@id, @datetime,@date,@state,@value,@defect)";

                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                            {
                                // Replace @value1 and @value2 with the actual values you want to insert
                                cmd.Parameters.AddWithValue("@id", null);
                                cmd.Parameters.AddWithValue("@datetime", datetime);
                                cmd.Parameters.AddWithValue("@date", dateOnly);
                                cmd.Parameters.AddWithValue("@state", state);
                                cmd.Parameters.AddWithValue("@value", val);
                                cmd.Parameters.AddWithValue("@defect", defect);

                                cmd.ExecuteNonQuery();
                                
                            }
                        }

                        else 
                        {
                            string insertQuery = "INSERT INTO realtime_filling_machine_state (id,datetime,date,state,value,defect) " +
                                                 "VALUES " +
                                                 "(@id, @datetime,@date,@state,@value,@defect)";

                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                            {
                                // Replace @value1 and @value2 with the actual values you want to insert
                                cmd.Parameters.AddWithValue("@id", null);
                                cmd.Parameters.AddWithValue("@datetime", datetime);
                                cmd.Parameters.AddWithValue("@date", dateOnly);
                                cmd.Parameters.AddWithValue("@state", state);
                                cmd.Parameters.AddWithValue("@value", val);
                                cmd.Parameters.AddWithValue("@defect", defect);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        connection.Close();
                    }
                }
            }
            catch
            {

            }

        }

        public void updateTimeToDB(string machine)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Querys querys = new Querys();
            string connectionString = dbConnect;
            try
            {
                if (machine != null )
                {

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        if (machine == "mixing")
                        {
                            string updateQuery = "UPDATE result_time_mixing SET stop_datetime = @newDateTime, time_result = @timeNow ORDER BY id DESC LIMIT 1";

                            using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                            {
                                string timeNow = selectStartTime("mixing");
                               
                                DateTime startTime = DateTime.Parse(timeNow);
                                DateTime stopTime = DateTime.Parse(timestamp);

                                TimeSpan timeDifference = stopTime - startTime;
                                double secondsDifference = timeDifference.TotalSeconds;

                                cmd.Parameters.AddWithValue("@newDateTime", timestamp);
                                cmd.Parameters.AddWithValue("@timeNow", secondsDifference);
                                
                                cmd.ExecuteNonQuery();

                                resultSend = "success update time in : " + machine;
                            }
                        }
                        else
                        {
                            string updateQuery = "UPDATE result_time_filling SET stop_datetime = @newDateTime, time_result = @timeNow ORDER BY id DESC LIMIT 1";

                            using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                            {
                                string timeNow = selectStartTime("filling");

                                DateTime startTime = DateTime.Parse(timeNow);
                                DateTime stopTime = DateTime.Parse(timestamp);

                                TimeSpan timeDifference = stopTime - startTime;
                                double secondsDifference = timeDifference.TotalSeconds;

                                cmd.Parameters.AddWithValue("@newDateTime", timestamp);
                                cmd.Parameters.AddWithValue("@timeNow", secondsDifference.ToString());

                                cmd.ExecuteNonQuery();

                                resultSend = "success update time in : " + machine;
                            }
                        }

                        connection.Close();
                    }
                }
            }
            catch
            {
                string timeNow = selectStartTime("mixing");
                resultSend = "failed update time in : " + timeNow;
            }

        }

        public string selectStartTime(string machine)
        {
            Querys querys = new Querys();

            using (MySqlConnection connection = new MySqlConnection(dbConnect))
            {
                using (MySqlCommand command = new MySqlCommand(

                    querys.selectTimeUpdateMixing +
                    querys.selectTimeUpdateFilling

                    , connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mixingTimeResult = reader["start_datetime"].ToString();
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    fillingTimeResult = reader["start_datetime"].ToString();
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    if (machine == "mixing")
                    {
                        return mixingTimeResult;
                    }
                    else
                    {
                        return fillingTimeResult;
                    }
                }
            }
        }
        public void insertTimeToDB(string machine, string state, string description, string start, string stop, string result)
        {
            Querys querys = new Querys();

            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string connectionString = dbConnect;
            try
            {
                if (machine != null && state != null && start != null && stop != null && result != null && description != null)
                {

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        if (machine == "mixing")
                        {
                            string insertQuery = "INSERT INTO result_time_mixing (id,state,Description,start_datetime,stop_datetime,time_result) VALUES (@id,@state,@Description,@Startdatetime,@Stopdatetime,@Timeresult)";

                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                            {
                               
                                cmd.Parameters.AddWithValue("@id", null);
                                cmd.Parameters.AddWithValue("@state", state);
                                cmd.Parameters.AddWithValue("@Description", description);
                                cmd.Parameters.AddWithValue("@Startdatetime",start);
                                cmd.Parameters.AddWithValue("@Stopdatetime", stop);
                                cmd.Parameters.AddWithValue("@Timeresult", result);

                                cmd.ExecuteNonQuery();

                                resultSend = "success insert data to mixing";

                            }
                        }

                        else if(machine == "filling")
                        {
                            string insertQuery = "INSERT INTO result_time_filling (id,state,Description,start_datetime,stop_datetime,time_result) VALUES (@id,@state,@Description,@Startdatetime,@Stopdatetime,@Timeresult)";

                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                            {
                                // Replace @value1 and @value2 with the actual values you want to insert
                                cmd.Parameters.AddWithValue("@id", null);
                                cmd.Parameters.AddWithValue("@state", state);
                                cmd.Parameters.AddWithValue("@Description", description);
                                cmd.Parameters.AddWithValue("@Startdatetime", start);
                                cmd.Parameters.AddWithValue("@Stopdatetime", stop);
                                cmd.Parameters.AddWithValue("@Timeresult", result);

                                cmd.ExecuteNonQuery();

                                resultSend = "success insert data to filling";

                            }
                        }
                        else
                        {
                            resultSend = "machine not available";
                        }

                        connection.Close();
                    }
                }
            }
            catch
            {
                resultSend = "failed insert data to DB";
            }

        }

        public string totalTime(string machine, string state)
        {
            string connectionString = dbConnect;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (machine == "mixing")
                {
                    // Specify the criteria
                    string query = "SELECT SUM(time_result) FROM result_time_mixing WHERE state = @criteria";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@criteria", state);

                    object sumResult = command.ExecuteScalar();

                    // Check if the result is not null
                    if (sumResult != null)
                    {
                        valResult = Convert.ToString(sumResult);
                        if (!string.IsNullOrEmpty(valResult))
                        {
                            Console.WriteLine(valResult);
                        }
                    }
                    else
                    {

                    }
                }
                else
                { 
                    string query = "SELECT SUM(time_result) FROM result_time_filling WHERE state = @criteria";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@criteria", state);

                    object sumResult = command.ExecuteScalar();

                    // Check if the result is not null
                    if (sumResult != null)
                    {
                        valResult = Convert.ToString(sumResult);
                        if (!string.IsNullOrEmpty(valResult))
                        {
                            Console.WriteLine(valResult);
                        }
                    }
                    else
                    {

                    }
                }

            }
            return valResult;
        }
        private void PopulateChart(ChartValues<double> values, ChartValues<string> labels, LiveCharts.WinForms.CartesianChart chart)
        {

            chart.Invoke((MethodInvoker)delegate
            {
                chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    
                    Title = "Performance",
                    Values = values,
                }

               };

            });
        }
    }

 }

