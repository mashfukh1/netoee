using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEE_SKRIPSI
{
    internal class Querys
    {
        public string GetMixState  = "SELECT state FROM  realtime_mixing_machine_state;";
        public string GetFillState = "SELECT state FROM  realtime_filling_machine_state;";


        public string[] machines = { "MIXING", "FILLING" };

        public string[] GetTableProductionHistory = { "SELECT * FROM  production_history_filling ORDER BY ID DESC LIMIT 24",
                                     "SELECT * FROM  production_history_mixing ORDER BY ID DESC LIMIT 24"
                                   };

        public string[] GetTableEventState = { "SELECT Description,start_datetime,stop_datetime,time_result FROM  result_time_filling ORDER BY ID DESC LIMIT 100",
                                               "SELECT Description,start_datetime,stop_datetime,time_result FROM  result_time_mixing ORDER BY ID DESC LIMIT 100"
                                             };

        public string mixValueTime = "SELECT datetime FROM value_oee_mixing";
        public string filValueTime = "SELECT datetime FROM value_oee_filling";

        public string selectTimeUpdateMixing = "SELECT start_datetime FROM  result_time_mixing ORDER BY ID DESC LIMIT 1;";
        public string selectTimeUpdateFilling = "SELECT start_datetime FROM  result_time_filling ORDER BY ID DESC LIMIT 1;";

        public string mixAvaibilityVal  = "SELECT avaibility FROM value_oee_mixing;";
        public string mixPerformanceVal = "SELECT performance FROM value_oee_mixing;";
        public string mixQualityVal     = "SELECT quality FROM value_oee_mixing;";
        public string mixOeeVal         = "SELECT oee FROM value_oee_mixing;";

        public string filAvaibilityVal  = "SELECT avaibility FROM value_oee_filling;";
        public string filPerformanceVal = "SELECT performance FROM value_oee_filling;";
        public string filQualityVal     = "SELECT quality FROM value_oee_filling;";
        public string filOeeVal         = "SELECT oee FROM value_oee_filling;";

        public string filPerformanceValChart = "SELECT datetime, performance FROM (SELECT * FROM value_oee_filling ORDER BY id DESC LIMIT 8 )AS A ORDER BY id";
        public string mixPerformanceValChart = "SELECT datetime, performance FROM (SELECT * FROM value_oee_mixing ORDER BY id DESC LIMIT 8 )AS A ORDER BY id";

        public string countMixVal = "SELECT SUM(time_result) FROM result_time_mixing WHERE state = '0';";

    }
}
