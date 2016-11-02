using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Metrics;
using Xunit;

namespace Vistian.Reactive.UnitTests.Metrics
{
    public class TimingContextTests
    {
        private TimerContext _timerContext;
        private TimingBlock _timingBlock;

        private const string MarkLabel = "Mark";

        public TimingContextTests()
        {
            _timingBlock = new TimingBlock("name");

            _timerContext = new TimerContext(_timingBlock);
        }

        [Fact]
        public void CreatedSuccessfullyTest()
        {
            Assert.Same(_timingBlock,_timerContext.TimingBlock);
        }

        [Fact]
        public void DisposeStopsTimingTest()
        {
            Assert.Equal(0, _timingBlock.Count);

            Task.Delay(1);

            _timerContext.Dispose();

            Assert.Equal(1,_timingBlock.Count);
        }

        [Fact]
        public void MarkPassedToBlockTest()
        {
            _timerContext.Mark(MarkLabel);

            _timerContext.Dispose();

            Assert.Equal(1,_timingBlock.Entries.Count);

            var item = _timingBlock.Entries[0];
            Assert.Equal(MarkLabel,item.Label);
        }


    }
}
