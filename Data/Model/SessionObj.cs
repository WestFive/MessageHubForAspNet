using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
    public class SessionObj
    {

        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnectionID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 会话对象类型
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 会话对象
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 首次连接时间。
        /// </summary>
        public string ConnectionTime { get; set; }
    }
}
