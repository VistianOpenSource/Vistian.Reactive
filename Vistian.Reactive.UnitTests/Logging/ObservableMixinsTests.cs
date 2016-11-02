using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reactive.Logging.Providers;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    [Collection("RxLog")]
    public class ObservableMixinsTests
    {
        private Subject<string> _testSubject;
        private Mock<IRxLogHost> _mockLogHost;

        const string testValue = "test";

        public ObservableMixinsTests()
        {
            _testSubject = new Subject<string>();

            _mockLogHost = new Mock<IRxLogHost>();
            _mockLogHost.Setup(p => p.Configuration).Returns(RxLoggerConfiguration.Create());
            RxLog.SetDefault(new RxLog(_mockLogHost.Object));
        }

        [Fact]
        public void ValueCorrectlyPublishedTest()
        {
            var xx = RxLog.Default.LogHost;

            var lastVal = "";

            _testSubject.Log(this,(s) => s).Subscribe(x => {
                lastVal = x;
            });



            _testSubject.OnNext(testValue);
            
            var y = RxLog.Default.LogHost;

            Assert.Same(xx,y);
            Assert.Equal(testValue,lastVal);

            _mockLogHost.Verify(p => p.Publish(It.IsAny<RxLogEntry<string>>()), Times.Once);
            _mockLogHost.Verify(p => p.Publish(It.Is((RxLogEntry<string> q)=> q.Instance == testValue)),Times.Once);
        }


        [Fact]
        public void TraceCorrectlyPublishesTest()
        {
            var traceName = "traceTest";

            var publishedItems = new List<RxLogEntry>();

            _mockLogHost.Setup(p => p.Publish(It.IsAny<RxLogEntry>())).Callback((RxLogEntry e) =>
            {
                publishedItems.Add(e);
            });

            var sub = _testSubject.Trace(traceName).Subscribe();

            _testSubject.OnNext(testValue);
            _testSubject.OnCompleted();

            sub.Dispose();

            Assert.Equal(4, publishedItems.Count);

            foreach (var t in publishedItems)
            {
                Assert.True(t.InstanceOfType<Classified>());
            }
            // should check the inidividual messages, but the id used could be interesting...

        }
    }
}
