using Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using signalr.MessageHub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace signalr.MessageHub.Tests
{
    [TestClass()]
    public class MessageHubTests
    {
        [TestMethod()]
        public void MessageHubTest()
        {
            Assert.IsNotNull(MessageHub.laneList);
            Assert.IsNotNull(MessageHub.serverList);
            Assert.IsTrue(MessageHub.QueueList.Count == 0);
        }
        [TestMethod()]
        public void ClearTest()
        {
            Assert.IsTrue(MessageHub.QueueList.Count == 0);

        }
        [TestMethod()]
        public void F5Test()
        {
            Assert.IsNotNull(MessageHub.laneList);
            Assert.IsNotNull(MessageHub.serverList);
        }

        [TestMethod()]
        public void SaveServerCacheTest()
        {
            MessageHub hub = new MessageHub();
            hub.SaveServerCache();
            Assert.IsNotNull(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/conf/servers.json"));

        }

        [TestMethod()]
        public void LoadServerDefaultCacheTest()
        {
            MessageHub hub = new MessageHub();
            hub.LoadServerDefaultCache();
            Assert.IsTrue(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "conf/servers-example.json"));
            Assert.IsNotNull(MessageHub.serverList);
        }

        [TestMethod()]
        public void HubRestartTest()
        {
            MessageHub hub = new MessageHub();
            hub.HubRestart();
            Assert.IsNotNull(MessageHub.laneList);
            Assert.IsNotNull(MessageHub.serverList);
            Assert.IsTrue(MessageHub.QueueList.Count == 0);

        }
    }
}