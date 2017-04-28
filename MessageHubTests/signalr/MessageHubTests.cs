using Microsoft.VisualStudio.TestTools.UnitTesting;
using signalr.MessageHub;
using System;
using System.Collections.Generic;
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
            Assert.IsTrue(MessageHub.QueueList.Count==0);
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
    }
}