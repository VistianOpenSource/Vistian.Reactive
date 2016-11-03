using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Vistian.Reactive.Metrics;
using Xunit;

namespace Vistian.Reactive.UnitTests.Metrics
{
    public class DiscreteTests
    {
        private const string Name = "discrete";
        private static readonly Unit DefaultUnit = Unit.Bytes;
        private readonly Discrete<int> _discrete;
        private const int InitialSetValue = 121;
        private const int SubsequentSetValue = 122;

        public DiscreteTests()
        {
            _discrete = new Discrete<int>(Name,DefaultUnit);
        }

        [Fact]
        public void CreateSuccessfullyTest()
        {
            Assert.Equal(Name,_discrete.Name);    
            Assert.Equal(DefaultUnit,_discrete.Unit);
            Assert.Equal(MetricType.Discrete,_discrete.MetricType);

            DateTimeOffset dto = new DateTimeOffset();

            Assert.Equal(dto,_discrete.FirstDateTimeOffset);
            Assert.Equal(dto, _discrete.LastDateTimeOffset);
        }

        [Fact]
        public void SetValueTest()
        {
            _discrete.Value = InitialSetValue;

            Assert.Equal(InitialSetValue,_discrete.Value);
            Assert.Equal(1,_discrete.Count);

            // check that first & last time are close if not the same
            Assert.True(_discrete.FirstDateTimeOffset == _discrete.LastDateTimeOffset);
        }

        [Fact]
        public void SetValueAgainResultsInDifferingTimeTest()
        {
            _discrete.Value = InitialSetValue;

            _discrete.Value = SubsequentSetValue;

            Assert.Equal(SubsequentSetValue,_discrete.Value);
            Assert.Equal(2,_discrete.Count);

            Assert.True(_discrete.LastDateTimeOffset >= _discrete.FirstDateTimeOffset);
        }

        [Fact]
        public void CreateDefaultTest()
        {
            var d = Discrete<string>.Create();

            Assert.Equal(string.Empty,d.Name);
            Assert.Equal(Unit.None,d.Unit);
        }
    }
}
