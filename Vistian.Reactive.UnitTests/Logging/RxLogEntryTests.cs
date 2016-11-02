using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    public class RxLogEntryTests
    {
        [Fact]
        public void CreateSuccessfullyTest()
        {
            var meta = new RxLogEntryMeta(this.GetType(),"member4",1);

            var specifics = new object();

            var entry = new RxLogEntry(meta, specifics);

            Assert.Same(meta,entry.Meta);
            Assert.Same(specifics, entry.Specifics);
            Assert.True(entry.InstanceOfType<object>());
        }

        [Fact]
        public void CreateGenericSuccessfullyTest()
        {
            var meta = new RxLogEntryMeta(this.GetType(), "member6", 1);

            const string specifics = "Test";
            var entry = new RxLogEntry<string>(meta, specifics);

            Assert.Same(meta, entry.Meta);
            Assert.Same(specifics, entry.Specifics);
            Assert.True(entry.InstanceOfType<string>());
            Assert.Equal(specifics,entry.Instance);

        }
    }
}
