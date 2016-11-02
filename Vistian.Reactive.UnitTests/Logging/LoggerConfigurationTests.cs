using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    public class LoggerConfigurationTests
    {
        private string _testMemberName;
        private int _testLineNo;

        public LoggerConfigurationTests()
        {
            _testMemberName = "member";
            _testLineNo = 23;
        }

        [Fact]
        public void CreateValidConfigurationTest()
        {
            var config = RxLoggerConfiguration.Create();

            var meta = config.CreateMeta(this.GetType(), _testMemberName, _testLineNo);

            Assert.NotNull(config.Host);
            Assert.NotNull(config.Formatting);

        }

        [Fact]
        public void MetaCreationValidTest()
        {
            var config = RxLoggerConfiguration.Create();

            var meta = config.CreateMeta(this.GetType(), _testMemberName, _testLineNo);

            Assert.Same(meta.CallingClass, this.GetType());
            Assert.Equal(_testMemberName, meta.MemberName);
            Assert.Equal(_testLineNo, meta.LineNo);
        }

        [Fact]
        public void CustomCreateWIthNoSubscriberTest()
        {
            var hc = HostConfiguration.Default;
            var lc = LogFormattingConfiguration.Default;

            var  subs = new List<Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>>();
            var config = new RxLoggerConfiguration(hc,lc,subs);

            Assert.Same(config.Host,hc);
            Assert.Same(config.Formatting,lc);
            Assert.Equal(0,config.Subscribers.Count);
        }

        [Fact]
        public void CustomCreateWithSubscriberTest()
        {
            var hc = HostConfiguration.Default;
            var lc = LogFormattingConfiguration.Default;

            Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>> sub = (r, o) => Observable.Return(Unit.Default);

            var subs = new List<Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>> {sub};

            var config = new RxLoggerConfiguration(hc, lc, subs);

            Assert.Equal(1, config.Subscribers.Count);
            Assert.Same(sub,config.Subscribers[0]);
        }

    }
}
