using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Individual timer entry.
    /// </summary>
    public struct TimerEntry
    {
        /// <summary>
        /// Get the elapsed milliseconds for this.
        /// </summary>
        public long Milliseconds { get; set; }

        /// <summary>
        /// Get the optional label
        /// </summary>
        public string Label { get; set; }
    }
}
