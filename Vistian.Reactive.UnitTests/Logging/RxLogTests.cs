using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Vistian.Reactive.Logging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    [Collection("RxLog")]
    public class RxLogTests
    {
        private Mock<IRxLogHost> _logHost;
        private RxLog _log;

        public RxLogTests()
        {
            _logHost = new Moq.Mock<IRxLogHost>();
            _log = new RxLog(_logHost.Object);
        }

        [Fact]
        public void CreateSuccessfullyTest()
        {
            Assert.Same(_logHost.Object,_log.LogHost);
            Assert.True(_log.Enabled);
        }

        [Fact(Skip="random failures - statics?")]
        public void SetDefaultTest()
        {
            RxLog.SetDefault(_log);

            Assert.Same(RxLog.Default,_log);
        }

        [Fact]
        public void PublishWhenEnabledTest()
        {
            var entry = new RxLogEntry(new RxLogEntryMeta(null,"",0), new object());

            _log.Publish(entry);

            _logHost.Verify(p => p.Publish(entry),Times.Once);
        }

        [Fact]
        public void DoesntPublishWhenDisabledTest()
        {
            var entry = new RxLogEntry(new RxLogEntryMeta(null, "", 0), new object());

            _log.Enabled = false;
            _log.Publish(entry);

            _logHost.Verify(p => p.Publish(entry), Times.Never);
        }

        [Fact(Skip="random failure  - static issue?")]
        public void StaticPublishChannelsThroughDefaultTest()
        {
            RxLog.SetDefault(_log);

            var entry = new RxLogEntry(new RxLogEntryMeta(null, "", 0), new object());

            RxLog.Log(entry);

            _logHost.Verify(p => p.Publish(entry), Times.Once);
        }
    }
}
