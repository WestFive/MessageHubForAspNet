using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Data.Common;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using Data.Model;

namespace signalr.MessageHub
{
    public class MessageHub : Hub
    {
        #region 日志及初始化
        ///日志记录
        //private readonly ILogger<MessageHub> //;

        //private bool SetValue = false;//调试赋值此项设正
        public MessageHub()
        {
            //// = logger;
            Loger.FilePath = AppDomain.CurrentDomain.BaseDirectory + "MessageLog";
            //Data.MySqlHelper.GetList();、、

            if (laneList.Count == 0)
            {

                List<LaneCache> lanes = JsonHelper.DeserializeJsonToList<LaneCache>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "conf/lanes.json"));
                foreach (var item in lanes)
                {
                    laneList.Add(item.lane_code, item.lane);

                }

            }
            if (serverList.Count == 0)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "conf/servers.json"))
                {
                    serverList = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "conf/servers.json"));

                }
                else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "conf/servers-example.json"))
                {
                    List<ServerCache> servers = JsonHelper.DeserializeJsonToList<ServerCache>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "conf/servers-example.json"));
                    foreach (var item in servers)
                    {
                        serverList.Add(item.server_code, item.server);
                    }

                }
            }


        }
        #endregion
        #region 全局变量
        ///// <summary>
        ///// 消息信息列表。
        ///// </summary>
        //public static List<Pf_Message_Obj> StatusList = new List<Pf_Message_Obj>();

        public static string ReCode;
        /// <summary>
        /// 车道信息列表。
        /// </summary>
        public static Dictionary<string, object> laneList = new Dictionary<string, object>();



        public static Dictionary<string, object> serverList = new Dictionary<string, object>();
        /// <summary>
        /// 作业信息列表
        /// </summary>
        //public static List<Pf_Messge_Queue_Object> QueueList = new List<Pf_Messge_Queue_Object>();
        public static ConcurrentDictionary<int, Pf_Messge_Queue_Object> QueueList = new ConcurrentDictionary<int, Pf_Messge_Queue_Object>();

        public static int queue_number = 1;
        /// <summary>
        /// 会话信息列表。
        /// </summary>
        public static List<SessionObj> sessionObjectList = new List<SessionObj>();

        #endregion
        #region 刷新
        /// <summary>
        /// 刷新列表并推送至所有已连接上的车道客户端。
        /// </summary>
        public void F5()
        {
            try
            {
                Clients.All.GetSessionList(JsonHelper.SerializeObject(sessionObjectList));
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("刷新会话", ex);
            }

        }
        /// <summary>
        /// 刷新车道
        /// </summary>
        public void refreshLaneList()
        {
            if (laneList.Count != 0)
            {
                laneList.OrderBy(x => x.Key[x.Key.Length - 1]);
                List<object> lanes = new List<object>();
                //foreach (var item in laneList)
                //{
                //    lanes.Add(item.Value);

                //}
                Parallel.ForEach(laneList, item =>
                {
                    lanes.Add(item.Value);
                });
                try
                {
                    Clients.All.GetLaneList(JsonHelper.SerializeObject(lanes));
                }
                catch (Exception ex)
                {
                    Loger.AddErrorText("车道刷新模块", ex);
                }
            }


        }

        /// <summary>
        /// 刷新作业
        /// </summary>
        public void refreshQueueList()
        {
            if (laneList.Count != 0)
            {
                List<object> queues = new List<object>();
                if (QueueList.Count != 0)
                {

                    //foreach (var item in QueueList)
                    //{
                    //    queues.Add(item.Value.queue);
                    //}
                    Parallel.ForEach(QueueList, item =>
                    {
                        queues.Add(item.Value.queue);
                    });

                    queues.Reverse();
                }
                try
                {


                    Clients.All.GetQueueList(JsonHelper.SerializeObject(queues));

                }
                catch (Exception ex)
                {
                    Loger.AddErrorText("作业缓存刷新模块", ex);

                }
            }

        }

        /// <summary>
        /// 刷新WatchDog
        /// </summary>
        public void refreshServerList()
        {
            if (laneList.Count != 0)
            {
                List<object> servers = new List<object>();
                if (serverList.Count != 0)
                {
                    //WatchDogCheck();//检查
                    Parallel.ForEach(serverList, item =>
                    {
                        servers.Add(item.Value);
                    });

                }
                try
                {

                    Clients.All.GetServerList(JsonHelper.SerializeObject(servers));

                }
                catch (Exception ex)
                {
                    Loger.AddErrorText("刷新模块", ex);

                }
            }

        }
        #endregion

        #region 车道监控发送消息给车道代理
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="laneID"></param>
        /// <param name="JsonMessage"></param>
        [HubMethodName("SendMessage")]
        public void SendMessage(string laneCode, string JsonMessage)
        {
            try
            {
                //.LogWarning("发送消息:" + laneCode + JsonMessage);
                Pf_Message_Obj<object> obj = JsonHelper.DeserializeJsonToObject<Pf_Message_Obj<object>>(JsonMessage);
                switch (obj.message_type)
                {
                    case "lane":

                        lock (laneList)
                        {
                            Pf_Message_lane_Object lanecontent = JsonHelper.DeserializeJsonToObject<Pf_Message_lane_Object>(JsonHelper.SerializeObject(obj.message_content));

                            if (sessionObjectList.Count(x => x.ClientName == laneCode) > 0)
                            {
                                Clients.Client(sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == laneCode)].ConnectionID).reciveMessage(JsonHelper.SerializeObject(obj));

                                InsertLog("投递信息给:" + lanecontent.lane_code + "\r" + "信息内容是" + JsonHelper.SerializeObject(obj));
                                GetMessageHubStatus("已通知车道laneID为:" + laneCode + "信息类型：lane");
                            }

                        }
                        break;
                    case "directive":
                        Pf_Message_Directive_Object directivecontent = JsonHelper.DeserializeJsonToObject<Pf_Message_Directive_Object>(JsonHelper.SerializeObject(obj.message_content));
                        if (sessionObjectList.Count(x => x.ClientName == directivecontent.recipient_code) > 0)
                        {
                            Clients.Client(sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == directivecontent.recipient_code)].ConnectionID).reciveMessage(JsonHelper.SerializeObject(obj));
                            InsertLog("投递信息给" + directivecontent.recipient_code + "\r" + "信息内容是" + JsonHelper.SerializeObject(obj));
                            GetMessageHubStatus("已通知车道laneID为:" + laneCode + "信息类型：directive");
                        }
                        break;
                    case "queue":
                        lock (QueueList)
                        {
                            Pf_Messge_Queue_Object queuecontent = JsonHelper.DeserializeJsonToObject<Pf_Messge_Queue_Object>(JsonHelper.SerializeObject(obj.message_content));

                            if (sessionObjectList.Count(x => x.ClientName == laneCode) > 0)
                            {

                                Clients.Client(sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == laneCode)].ConnectionID).reciveMessage(JsonHelper.SerializeObject(obj));

                                InsertLog("投递信息给" + queuecontent.lane_code + "\r" + "信息内容是" + JsonHelper.SerializeObject(obj));
                                GetMessageHubStatus("已通知车道laneID为:" + queuecontent.lane_code + "信息类型：queue");
                            }


                        }
                        break;
                    case "server":
                        lock (serverList)
                        {
                            PF_Servers servers = JsonHelper.DeserializeJsonToObject<PF_Servers>(JsonHelper.SerializeObject(obj.message_content));
                            if (serverList.Count(x => x.Key == laneCode) > 0)
                            {
                                Clients.Client(sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == laneCode)].ConnectionID).reciveMessage(JsonHelper.SerializeObject(obj));
                                InsertLog("投递信息给" + servers.server_code + "\r信息内容是" + JsonHelper.SerializeObject(obj));
                                GetMessageHubStatus("已通知WatchDogID为" + servers.server_code + "信息");
                            }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {

                ReCode = "状态刷新/修改失败";
                GetMessageHubStatus("状态刷新/修改失败");
                Loger.AddErrorText("更新状态失败", ex);

            }

        }

        public void WatchDogAction(string serverid, string JsonMessage)
        {
            try
            {
                Clients.Client(sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == serverid)].ConnectionID).GetServerAction(JsonMessage);
            }
            catch (Exception ex)
            {
                //.LogError(ex.ToString());
            }
        }

        private void InsertLog(string Value)
        {
            try
            {
                Loger.AddLogText("Time:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-Info:" + JsonHelper.SerializeObject(Value));
            }
            catch (Exception ex)
            {
                //.LogError(ex.ToString());
                Loger.AddErrorText("INsertlog", ex);
            }
        }
        #endregion
        #region 车道代理自我状态改变修改缓存 
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="laneID"></param>
        /// <param name="JsonMessage"></param>
        [HubMethodName("Change")]
        public void Change(string laneCode, string JsonMessage)
        {
            //.LogWarning("进入了修改");
            try
            {
                Pf_Message_Obj<object> obj = JsonHelper.DeserializeJsonToObject<Pf_Message_Obj<object>>(JsonMessage);
                //.LogWarning("第一步成功，下一步做判断。");
                //.LogWarning("数据正常吗?" + laneCode + "json" + JsonMessage);
                switch (obj.message_type)
                {
                    case "lane":
                        lock (laneList)
                        {
                            Pf_Message_lane_Object lanecontent = JsonHelper.DeserializeJsonToObject<Pf_Message_lane_Object>(JsonHelper.SerializeObject(obj.message_content));
                            if (laneList.Count(x => x.Key == lanecontent.lane_code) > 0)
                            {
                                laneList[lanecontent.lane_code] = lanecontent.lane;
                                GetMessageHubStatus(lanecontent.lane_code + "修改了自身的车道缓存");
                            }

                        }
                        refreshLaneList();//刷新车道
                        break;
                    case "queue":
                        lock (QueueList)
                        {
                            Pf_Messge_Queue_Object queuecontent = JsonHelper.DeserializeJsonToObject<Pf_Messge_Queue_Object>(JsonHelper.SerializeObject(obj.message_content));
                            switch (queuecontent.action)
                            {

                                case "create":
                                    if (QueueList.Count(x => x.Value.queue_code == queuecontent.queue_code) == 0)//没有这个元素时才能创建
                                    {
                                        if (queue_number == 65534)
                                        {
                                            queue_number = 1;
                                            //.LogWarning("服务已处理了65535票作业，计数归零");
                                        }
                                        queuecontent.create_time = DateTime.Now.ToString();
                                        QueueList.TryAdd(queue_number++, queuecontent);
                                        //.LogWarning("服务已处理了" + queue_number + "条作业");
                                        GetMessageHubStatus(queuecontent.lane_code + "创建了一票作业" + queuecontent.queue_code);
                                    }
                                    break;
                                case "update":
                                    if (QueueList.Count(x => x.Value.queue_code == queuecontent.queue_code) > 0)
                                    {
                                        QueueList[QueueList.First(x => x.Value.queue_code == queuecontent.queue_code).Key] = queuecontent;
                                        GetMessageHubStatus(queuecontent.lane_code + "更新作业" + queuecontent.queue_code + "中");
                                    }
                                    break;
                                case "delete":
                                    if (QueueList.Count(x => x.Value.queue_code == queuecontent.queue_code) > 0)
                                    {
                                        Pf_Messge_Queue_Object outobj = new Pf_Messge_Queue_Object();
                                        QueueList.TryRemove(QueueList.FirstOrDefault(x => x.Value.queue_code == queuecontent.queue_code).Key, out outobj);
                                        GetMessageHubStatus(queuecontent.lane_code + "删除作业" + queuecontent.queue_code);
                                    }

                                    break;
                            }


                        }
                        refreshQueueList();//刷新作业
                        break;
                    case "server":
                        lock (serverList)
                        {
                            PF_Message_Server servercontent = JsonHelper.DeserializeJsonToObject<PF_Message_Server>(JsonHelper.SerializeObject(obj.message_content));
                            if (serverList.Count(x => x.Key == servercontent.server_code) > 0)
                            {
                                serverList[servercontent.server_code] = servercontent.server;//更新server
                                GetMessageHubStatus("看门狗" + servercontent.server_code + "修改了自身缓存");
                            }

                        }
                        refreshServerList();//刷新WatchDog;
                        break;
                        //case "serve"
                }
            }
            catch (Exception ex)
            {
                //.LogWarning("报错了" + ex.ToString());
                ReCode = "状态刷新/修改失败";
                GetMessageHubStatus(ReCode);
                Loger.AddErrorText("更新状态失败", ex);
            }
            finally
            {
                F5();//刷新会话列表
            }

        }
        #endregion
        #region 会话列表

        /// <summary>
        /// 刷新推送获取会话列表。
        /// </summary>
        public void SessionList()
        {
            try
            {
                Clients.All.GetSessionList(JsonHelper.SerializeObject(sessionObjectList));
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("会话列表", ex);
                //.LogError("会话列表信息推送模块" + ex.ToString());

            }
        }

        #endregion
        #region WatchDog获取自身的缓存
        public void WathDogCache(string server_code)
        {
            try
            {
                if (serverList.Count(x => x.Key == server_code) > 0)
                {

                    Clients.Client(Context.ConnectionId).GetWatchDog(JsonHelper.SerializeObject(serverList[server_code]));
                }
            }
            catch (Exception ex)
            {
                //.LogError(ex.ToString());
                Loger.AddErrorText("WatchDog获取自身缓存", ex);
            }
        }

        public void WatchDogCheck()
        {
            List<object> list = new List<object>();
            foreach (var item in serverList)
            {
                list.Add(item.Value);
            }
            for (int i = 0; i < list.Count; i++)
            {
                PF_Message_Server_Object server = JsonHelper.DeserializeJsonToObject<PF_Message_Server_Object>(JsonHelper.SerializeObject(list[i]));
                if (sessionObjectList.Count(x => x.ClientName == server.server_code) == 0)
                {
                    server.watchdog_status = "离线";
                    serverList[server.server_code] = JsonHelper.DeserializeJsonToObject<object>(JsonHelper.SerializeObject(server));

                }
            }

        }


        /// <summary>
        /// 回存WatchDog缓存
        /// </summary>
        public void SaveServerCache()
        {
            try
            {
                if (serverList.Count != 0)
                {

                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "conf/servers.json", JsonHelper.SerializeObject(serverList));
                }


            }
            catch (Exception ex)
            {
                Loger.AddErrorText("回存server缓存失败", ex);
            }
        }

        #endregion
        #region  添加到会话缓存列表
        private void AddToSession()
        {
            try
            {
                sessionObjectList.Add(new SessionObj
                {
                    ConnectionID = Context.ConnectionId,
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    Port = HttpContext.Current.Request.Url.Port.ToString(),
                    ClientName = Context.QueryString["Name"],
                    ClientType = Context.QueryString["Type"],

                    ConnectionTime = DateTime.Now.ToString()
                });//添加会话对象
                //.LogWarning(DateTime.Now.ToString() + "{0}{1}连接了", Context.QueryString["Type"], Context.QueryString["Name"]);
                Loger.AddLogText(DateTime.Now.ToString() + Context.QueryString["Type"] + ":" + Context.QueryString["ID"] + "连接了");
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("添加到会话缓存列表", ex);
                //.LogError(ex.ToString());
            }
        }
        #endregion
        #region 连接事件
        public override Task OnConnected()
        {

            //连接角色判断。
            try
            {

                switch (Context.QueryString["Type"])
                {
                    case "Client":
                        //.LogWarning("连接的对象是客户端" + Context.QueryString["Name"]);
                        if (sessionObjectList.Count(x => x.ClientName == Context.QueryString["Name"]) > 0)
                        {
                            //.LogWarning("更新客户端连接ID" + Context.QueryString["Name"]);
                            GetMessageHubStatus("连接的对象是客户端" + "更新客户端连接ID" + Context.QueryString["Name"]);
                            sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == Context.QueryString["Name"])].ConnectionID = Context.ConnectionId;//替换连接ID
                        }
                        else
                        {
                            GetMessageHubStatus("连接的对象是客户端" + "更新客户端连接ID" + Context.QueryString["Name"]);
                            AddToSession();
                        }
                        break;
                    case "LaneWatch":
                        //.LogWarning("连接的对象是监控端" + Context.QueryString["Name"]);
                        GetMessageHubStatus("连接的对象是监控端" + Context.QueryString["Name"]);
                        AddToSession();//加入车道缓存。
                        break;
                    case "Broswer":
                        //.LogWarning("连接的对象是浏览器" + Context.QueryString["Name"]);
                        GetMessageHubStatus("连接的对象是浏览器" + Context.QueryString["Name"]);
                        AddToSession();//加入车道缓存。
                        break;
                    case "WatchDog":
                        //.LogWarning("连接的对象是看门狗" + Context.QueryString["Name"]);
                        GetMessageHubStatus("连接的对象是看门狗" + Context.QueryString["Name"]);
                        if (sessionObjectList.Count(x => x.ClientName == Context.QueryString["Name"]) > 0)
                        {
                            
                            sessionObjectList[sessionObjectList.FindIndex(x => x.ClientName == Context.QueryString["Name"])].ConnectionID = Context.ConnectionId;
                            WathDogCache(Context.QueryString["Name"]);
                        }
                        else//新加入的看门狗，给其自己的缓存
                        {
                            GetMessageHubStatus("连接的对象是看门狗" + Context.QueryString["Name"]);
                            AddToSession();//加入车道缓存。
                            WathDogCache(Context.QueryString["Name"]);

                        }
                        break;
                    default:
                        //.LogWarning("连接的对象是匿名者");
                        GetMessageHubStatus("连接的对象是匿名者");
                        AddToSession();//加入车道缓存。
                        break;
                }

                F5();
                refreshLaneList();//刷新车道。
                refreshQueueList();
                refreshServerList();//刷新WatchDog

                #region 测试用

                #endregion
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("连接事件", ex);

            }

            return base.OnConnected();
        }

        #endregion
        #region 断开连接事件
        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                ///判断是否已经存在该条车道
                if (sessionObjectList.Count(x => x.ConnectionID == Context.ConnectionId) > 0)
                {
                    var thesession = sessionObjectList[sessionObjectList.FindIndex(x => x.ConnectionID == Context.ConnectionId)];
                    var temp = laneList.FirstOrDefault(x => x.Key == thesession.ClientName);
                    if (temp.Value != null)
                    {
                        //temp.lane = null;//离线清空

                        InsertLog(temp.Key + "与服务器断开连接");
                    }
                }
                if (sessionObjectList.Count(x => x.ConnectionID == Context.ConnectionId) > 0)
                {
                    var thesession = sessionObjectList[sessionObjectList.FindIndex(x => x.ConnectionID == Context.ConnectionId)];
                    var temp = serverList.FirstOrDefault(x => x.Key == thesession.ClientName);
                    if (temp.Value != null)
                    {
                        //temp.lane = null;//离线清空

                        InsertLog(temp.Key + "与服务器断开连接");
                        GetMessageHubStatus(temp.Key + "与服务器断开连接");
                    }
                }
                //判断是否存在会话。
                if (sessionObjectList.Count(x => x.ConnectionID == Context.ConnectionId) > 0)
                {
                    var temp = sessionObjectList.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
                    sessionObjectList.Remove(temp);//包含则移除。
                    Loger.AddLogText(DateTime.Now.ToString() + temp.ConnectionID + "与服务断开连接");
                    GetMessageHubStatus(temp.ConnectionID + "与服务断开连接");
                }

                F5();
                refreshLaneList();//刷新车道。
                refreshServerList();//刷新WatchDog
                WatchDogCheck();

            }
            catch (Exception ex)
            {
                Loger.AddErrorText("断开连接模块", ex);
                //.LogError("断开连接模块", ex);
            }

            //addTolog("断开服务器");
            return base.OnDisconnected(stopCalled);
        }
        #endregion
        #region 重连事件
        /// <summary>
        /// 重连触发连接事件。
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            return OnConnected();
        }
        #endregion
        #region 给予前端修改的执行结果反馈
        //给予前端执行结果指令。
        public void GetMessageHubStatus(string str)
        {
            Clients.All.messageStatus(DateTime.Now.ToString() + str);
            InsertLog(str);
        }
        #endregion

        #region AdminMethod
        #region 读取还原servers为初始缓存
        public void LoadServerDefaultCache()
        {
            try
            {
                List<ServerCache> servers = JsonHelper.DeserializeJsonToList<ServerCache>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "conf/servers-example.json"));
                foreach (var item in servers)
                {
                    serverList.Add(item.server_code, item.server);
                }
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("还原出厂缓存失败", ex);
            }
        }
        #endregion
        #region 清空作业
        public void Clear()
        {
            QueueList.Clear();
            F5();
            refreshQueueList();
        }
        #endregion
        public void HubRestart()
        {
            this.Dispose();
            MessageHub hub = new MessageHub();

        }

        public void GetTodayLog(int days)
        {
            List<string> list = Loger.ReadFromLogTxt(DateTime.Now, days);
            Clients.All.ReadLogs(JsonHelper.SerializeObject(list));
        }
        #endregion 

    }
}
