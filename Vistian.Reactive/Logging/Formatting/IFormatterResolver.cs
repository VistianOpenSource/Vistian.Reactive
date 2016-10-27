using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Formatting resolver specification.
    /// </summary>
    public interface IFormatterResolver
    {
        IRxLogFormatter GetFor(RxLogEntry entry);

        /// <summary>
        /// Determine if there is a formatter for
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        bool HasFormatter(RxLogEntry logEntry);
    }
}
