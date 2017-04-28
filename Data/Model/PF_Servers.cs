using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
    /// <summary>
    /// 服务托管车道模型
    /// </summary>
    public class PF_Servers
    {
        public string server_code { get; set; }
        public string server_name { get; set; }
        public string ip_address { get; set; }
        public string subnet_mask { get; set; }
        public string watchdog_name { get; set; }
        public string watchdot_status { get; set; }
        public object[] lane_apps { get; set; }

    }


}
