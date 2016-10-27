using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Logging;

namespace Vistian.Reactive.Metrics.Logging
{
    /// <summary>
    /// Replacement <see cref="ITimerContext"/> allowing for logging of results upon disposal
    /// </summary>
    public class LoggingTimerContext : ITimerContext
    {
        /// <summary>
        /// The associated context
        /// </summary>
        private readonly ITimerContext _context;

        /// <summary>
        /// Any associated meta data
        /// </summary>
        private readonly RxLogEntryMeta _meta;

        public LoggingTimerContext(ITimerContext context)
        {
            Guard.NotNull(context);

            _context = context;
        }

        public LoggingTimerContext(ITimerContext context, RxLogEntryMeta meta)
        {
            Guard.NotNull(context);

            _context = context;
            _meta = meta;
        }

        public void Dispose()
        {
            _context.End();

            // and log the timing block 
            RxLog.Log(_meta, _context.TimingBlock);

            _context.Dispose();
        }

        /// <summary>
        /// Get the timing block
        /// </summary>
        public TimingBlock TimingBlock => _context.TimingBlock;

        /// <summary>
        /// Mark the entry, just pass it along.
        /// </summary>
        /// <param name="label"></param>
        public void Mark(string label = null)
        {
            _context.Mark(label);
        }

        /// <summary>
        /// Stop the timing block logging.
        /// </summary>
        public void End()
        {
            _context.End();
        }
    }
}
