using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Metrics;
using Xunit;

namespace Vistian.Reactive.UnitTests.Metrics
{
    public class CounterTests
    {
        private const string RootName = "Root";
        private const string NamedCounter = "named";

        private static readonly Unit RootUnit = Unit.Bytes;
        private readonly Counter _counter;

        public CounterTests()
        {
            _counter = new Counter(RootName, RootUnit);
        }

        [Fact]
        public void CreateSuccessfullyTest()
        {
            Assert.Equal(RootUnit,_counter.Unit);
            Assert.Equal(RootName,_counter.Name);
            Assert.Equal(0,_counter.Instances[string.Empty]);
            Assert.Equal(1,_counter.Instances.Count);
            Assert.Equal(MetricType.Counter,_counter.MetricType);
        }

        [Fact]
        public void IncrementDefaultCounterTest()
        {
            _counter.Increment();            

            Assert.Equal(1,_counter.Instances[string.Empty]);
        }

        [Fact]
        public void DecrementDefaultCounterTest()
        {
            _counter.Decrement();

            Assert.Equal(-1, _counter.Instances[string.Empty]);
        }


        [Fact]
        public void IncrementNamedCounterTest()
        {
            _counter.Increment(NamedCounter);

            Assert.Equal(1, _counter.Instances[NamedCounter]);
        }

        [Fact]
        public void DecrementNamedCounterTest()
        {
            _counter.Decrement(NamedCounter);

            Assert.Equal(-1, _counter.Instances[NamedCounter]);
        }

        [Fact]
        public void CreateDefaultTest()
        {
            var c = Counter.Create();

            Assert.Equal(string.Empty,c.Name);
            Assert.Equal(Unit.None,c.Unit);
        }
    }
}
