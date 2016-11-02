using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vistian.Reactive.Metrics;
using Xunit;

namespace Vistian.Reactive.UnitTests.Metrics
{
    public class TimingBlockTests
    {
        private TimingBlock _block;
        private const string Name = "timingblock";
        private const string Mark1 = "mark1";
        private const string Mark2 = "mark2";

        public TimingBlockTests()
        {
            _block = new TimingBlock(Name);
        }

        [Fact]
        public void CreateSuccessfullyTest()
        {
            Assert.Equal(0,_block.Count);
            Assert.Equal(0,_block.LastMilliseconds);
            Assert.Equal(0,_block.Max);
            Assert.Equal(0,_block.Mean);
            Assert.Equal(0,_block.Min);

            Assert.Equal(Name,_block.Name);
            Assert.Equal(0,_block.Entries.Count);
        }

        [Fact]
        public void EntryCreatedSuccessFullyTest()
        {
            _block.Begin();
            _block.End();

            Assert.Equal(1,_block.Count);
        }

        [Fact]
        public void CreateValidEntryTest()
        {
            _block.Begin();

            Thread.Sleep(10);

            _block.End();

            Assert.True(_block.LastMilliseconds >= 10);
            Assert.True(_block.Min >= 10);
            Assert.True(_block.Max >= 10);
            Assert.True(_block.Mean >= 10);
        }

        [Fact]
        public void MarkEntriesTest()
        {
            _block.Begin();

            Thread.Sleep(1);
            _block.Mark(Mark1);
            Thread.Sleep(1);
            _block.Mark(Mark2);

            _block.End();

            Assert.True(_block.LastMilliseconds >= 2);
            Assert.Equal(2,_block.Entries.Count);

            var item1 = _block.Entries[0];

            Assert.Equal(Mark1,item1.Label);
            Assert.True(item1.Milliseconds >= 1);

            var item2 = _block.Entries[1];

            Assert.Equal(Mark2, item2.Label);
            Assert.True(item2.Milliseconds >= 2);

        }
    }
}
