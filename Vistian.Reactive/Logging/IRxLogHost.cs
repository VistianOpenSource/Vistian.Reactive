using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging
{
    /// <summary>
    /// Specification of what a Log host provides.
    /// </summary>
    public interface IRxLogHost
    {
        /// <summary>
        /// Publish a specified log entry
        /// </summary>
        /// <param name="entry"></param>
        void Publish(RxLogEntry entry);

        /// <summary>
        /// Get the configuration this host is running with.
        /// </summary>
        RxLoggerConfiguration Configuration { get; }
    }
}
