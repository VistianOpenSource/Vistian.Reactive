using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Xunit;

namespace Vistian.Reactive.UnitTests.Contract
{
    public class ContractTests
    {
        [Fact]
        public void ObjectImplementsExceptionTest()
        {
            var o = new ContractTests();

            var ex = Assert.Throws<ArgumentException>(() => Guard.Implements<IDisposable>(o, nameof(o)));

            Assert.Equal("o", ex.ParamName);
        }

    }
}
