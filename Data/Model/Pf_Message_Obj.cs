using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    /// <summary>
    /// JSON总体消息对象
    /// </summary>
    /// 
    public class Pf_Message_Obj<T> where T :new()
    {
      
        /// <summary>
        /// 消息类型 指令或状态或作业
        /// </summary>
        public string message_type { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public object message_content { get; set; }

        public Pf_Message_Obj(string type, T content)
        {
            message_type = type;
            message_content = content;

        }

    }

    public class Pf_Message_lane_Object
    {
        public string lane_code { get; set; }
        public string send_time { get; set; }
        public object lane { get; set; }
    }

    public class Pf_Messge_Queue_Object
    {
        public string lane_code { get; set; }
        public string queue_code { get; set; }

        public string action { get; set; }
        public string create_time { get; set; }
        public string send_time { get; set; }
        public object queue { get; set; }

    }

    public class Pf_Message_Directive_Object
    {
        public string recipient { get; set; }
        public string recipient_code { get; set; }
        public string send_time { get; set; }
        public object directive { get; set; }
    }

    //public class pf_Message_Directive
    //{
    //    public string directive_id { get; set; }
    //    public string lane_code { get; set; }
    //    public string lane_name { get; set; }
    //    public string directive_code { get; set; }
    //    public string[] parameters { get; set; }
    //    public string send_time { get; set; }
    //}

    public class PF_Message_Server
    {
        public string server_code { get; set; }
        public string send_time { get; set; }
        public object server { get; set; }
    }

    /// <summary>
    /// WatchDog状态对象
    /// </summary>
    public class PF_Message_Server_Object
    {
        public string server_code { get; set; }
        public string server_name { get; set; }
        public string ip_address { get; set; }
        public string subnet_mask { get; set; }
        public string watchdog_name { get; set; }
        public string watchdog_status { get; set; }
        public object[] lane_apps { get; set; }
    }




}
