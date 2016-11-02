using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    public class RxLogEntryMetaTests
    {
        private int _lineNo;
        private string _memberName;
        private RxLogEntryMeta _meta;

        public RxLogEntryMetaTests()
        {
            _lineNo = 121;
            _memberName = "member";

            _meta = new RxLogEntryMeta(this.GetType(), _memberName, _lineNo);
        }
        [Fact]
        public void CreateSuccessfullyTest()
        {
            var diff = DateTimeOffset.UtcNow - _meta.TimeStampUtc;
            Assert.True(diff.TotalSeconds < 1);
            Assert.Same(this.GetType(),_meta.CallingClass);
            Assert.Equal(_memberName,_meta.MemberName);
            Assert.Equal(_lineNo,_meta.LineNo);
        }

        [Fact]
        public void AddSpecificsToMetaTest()
        {
            _meta.Custom["test"] = "23";

            Assert.Equal("23",_meta.Custom["test"]);
        }
    }
}
