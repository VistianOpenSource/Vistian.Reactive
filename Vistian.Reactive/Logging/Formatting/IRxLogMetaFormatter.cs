using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Specification fo the formatting of core log information.
    /// </summary>
    public interface IRxLogMetaFormatter
    {
        string Formatted(RxLogEntryMeta entryMeta);
    }
}
