using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging
{
    /// <summary>
    /// Core details relevant for each log entry.
    /// </summary>
    public sealed class RxLogEntryMeta
    {
        /// <summary>
        /// Get the type of class calling the logger
        /// </summary>
        public Type CallingClass { get; private set; }

        /// <summary>
        /// Get the name of the member in the calling class calling the logger
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Get the line number where the call to the logger occured
        /// </summary>
        public int LineNo { get; private set; }

        /// <summary>
        /// Get the UTC time when the log entry was created
        /// </summary>
        public DateTime TimeStampUtc { get; private set; }


        private Dictionary<string, string> _custom;

        /// <summary>
        /// Custom dictionary of optional logged meta data
        /// </summary>
        public Dictionary<string, string> Custom => _custom ?? (_custom = new Dictionary<string, string>());

        public RxLogEntryMeta(Type callingClass, string memberName, int lineNo)
        {
            CallingClass = callingClass;
            MemberName = memberName;
            LineNo = lineNo;
            TimeStampUtc = DateTime.UtcNow;
        }
    }
}
