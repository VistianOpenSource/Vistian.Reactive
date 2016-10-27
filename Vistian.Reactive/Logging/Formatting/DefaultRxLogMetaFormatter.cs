using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Default implementation of a <see cref="IRxLogMetaFormatter"/>
    /// </summary>
    public class DefaultRxLogMetaFormatter : IRxLogMetaFormatter
    {
        public string Formatted(RxLogEntryMeta entryMeta)
        {
            Guard.NotNull(entryMeta);

            return string.Format("{2:hh:mm:ss.fff} {0}.{1}:{3}", entryMeta.CallingClass != null ? entryMeta.CallingClass.Name : string.Empty, entryMeta.MemberName, entryMeta.TimeStampUtc, entryMeta.LineNo);
        }
    }
}
