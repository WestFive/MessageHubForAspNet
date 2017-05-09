using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MessageHub.Inteface;
using System.Threading.Tasks;

namespace Example.SignalR
{
    public class MessageHub : Hub, IMessageHub
    {
        private static List<string> onlinelist = new List<string>();

        /// <summary>
        /// 添加到会话缓存
        /// </summary>
        public void AddToSession()
        {
            onlinelist.Add(Context.ConnectionId);
        }

        public override Task OnConnected()
        {
            AddToSession();
            Clients.All.ReciveStatus(Context.ConnectionId + "上线了");//所有人接收状态
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.ReciveStatus(Context.ConnectionId + "离线了");
            if (onlinelist.Count(x => x == Context.ConnectionId) > 0)
            {
                onlinelist.Remove(onlinelist[onlinelist.FindIndex(x => x == Context.ConnectionId)]);
            }
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// 自我改变的动作
        /// </summary>
        /// <param name="code"></param>
        /// <param name="jsonMessage"></param>
        public void Change(string code, string jsonMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 刷新的动作
        /// </summary>
        public void F5()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 服务重启
        /// </summary>
        public void HubRestart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 插入到日志
        /// </summary>
        /// <param name="Value"></param>
        public void InsertLog(string Value)
        {
            throw new NotImplementedException();
        }

        //刷新LaneLIST
        public void refreshLaneList()
        {
            //获取在线列表
            Clients.All.reciveOnline(onlinelist);
        }

        /// <summary>
        /// 刷新queuelist
        /// </summary>
        public void refreshQueueList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 刷新serverlist
        /// </summary>
        public void refreshServerList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="reciveCode">接收方</param>
        /// <param name="JsonMessage">消息内容</param>
        public void SendMessage(string reciveCode, string JsonMessage)
        {
            //发送给所有人
            Clients.All.ReciveMessage(reciveCode + JsonMessage);
            //or点对点 reciveCode为connectionID或者能找到CONNECTIONID的条件。
            Clients.Client(reciveCode).reciveP2p(JsonMessage);
        }
    }
}