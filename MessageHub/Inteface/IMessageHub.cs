using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Inteface
{
    /// <summary>
    /// 接口只定义了作为一个消息服务要实现的几个基本方法
    /// </summary>
    public interface IMessageHub
    {
        /// <summary>
        /// 刷新会话集合
        /// </summary>
        void F5();
        /// <summary>
        /// 刷新车道集合
        /// </summary>
        void refreshLaneList();
        /// <summary>
        /// 刷新作业集合
        /// </summary>
        void refreshQueueList();
        /// <summary>
        /// 刷新watchDog集合
        /// </summary>
        void refreshServerList();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="reciveCode">接收者</param>
        /// <param name="JsonMessage">JSON格式的消息</param>
        void SendMessage(string reciveCode, string JsonMessage);
        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="Value">日志的值</param>
        void InsertLog(string Value);
        /// <summary>
        /// 自我状态改变
        /// </summary>
        /// <param name="code">"我"的code</param>
        /// <param name="jsonMessage">"我"要改变的内容</param>
        void Change(string code, string jsonMessage);
        /// <summary>
        /// 添加到会话缓存列表
        /// </summary>
        void AddToSession();
        /// <summary>
        /// 重启消息服务
        /// </summary>
        void HubRestart();
    }
}
