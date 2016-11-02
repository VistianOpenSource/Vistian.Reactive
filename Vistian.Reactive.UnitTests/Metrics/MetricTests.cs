using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Metrics;
using Xunit;

namespace Vistian.Reactive.UnitTests.Metrics
{
    public class MetricTests
    {
        private Counter _counterMetric;
        private Timer _timerMetric;
        private Discrete<int> _discreteMetric;
        // verify returns each type
        // verify when already returned the existing one returned.

        private const string MetricCounterName = "testCounter";
        private const string MetricTimerName = "testTimer";
        private const string MetricDiscreteName = "testDiscrete";

        public MetricTests()
        {
            _counterMetric = Metric.Counter(MetricCounterName, Unit.None);
            _timerMetric = Metric.Timer(MetricTimerName);
            _discreteMetric = Metric.Discrete<int>(MetricDiscreteName,Unit.None);
        }

        [Fact]
        public void CounterCreatedTest()
        {
            Assert.NotNull(_counterMetric);
        }


        [Fact]
        public void TimerCreatedTest()
        {
            Assert.NotNull(_timerMetric);    
        }

        [Fact]
        public void DiscreteCreatedTest()
        {
            Assert.NotNull(_discreteMetric);
        }

        [Fact]
        public void PriorCounterReturnedTest()
        {
            var metric2 = Metric.Counter(MetricCounterName, Unit.None);

            Assert.Same(_counterMetric,metric2);
        }

        [Fact]
        public void PriorDiscreteReturnedTest()
        {
            var metric2 = Metric.Discrete<int>(MetricDiscreteName,Unit.None);

            Assert.Same(_discreteMetric, metric2);
        }

        [Fact]
        public void PriorTimerReturnedTest()
        {
            var metric2 = Metric.Timer(MetricTimerName);

            Assert.Same(_timerMetric, metric2);
        }


    }
}
