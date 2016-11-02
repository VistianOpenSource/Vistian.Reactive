using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Metrics;
using Xunit;

namespace Vistian.Reactive.UnitTests.Metrics
{
    public class TimerTests
    {
        private readonly Timer _timer;
        public const string Name = "timer";
        public const string ContextName = "context";

        public TimerTests()
        {
            _timer = new Timer(Name);
        }

        [Fact]
        public void CreateSuccessfullyTest()
        {
            Assert.Equal(Name,_timer.Name);
            Assert.Equal(MetricType.Timer,_timer.MetricType);
            Assert.Equal(0,_timer.Entries.Count);
        }

        [Fact]
        public void CreateNewContextTest()
        {
            using (var newContext = _timer.NewContext(ContextName))
            {                    
            }

            Assert.Equal(1,_timer.Entries.Count);

            var instance = _timer.Entries[ContextName];
            Assert.NotNull(instance);
        }

        [Fact]
        public void ReuseContextTest()
        {
            using (var newContext = _timer.NewContext(ContextName))
            {
            }

            using (var newContext = _timer.NewContext(ContextName))
            {
            }

            Assert.Equal(1, _timer.Entries.Count);
        }
    }
}
