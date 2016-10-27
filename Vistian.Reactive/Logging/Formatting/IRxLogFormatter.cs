using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Specification of how log entries can be formatted into a textual representation
    /// </summary>
    public interface IRxLogFormatter
    {
        /// <summary>
        /// Format the log entry using the specified configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="entry"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        string Formatted(RxLoggerConfiguration configuration, RxLogEntry entry, FormatFlags format = FormatFlags.IncludeMeta);
    }
}
