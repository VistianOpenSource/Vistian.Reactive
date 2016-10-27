using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Formatting;
using Vistian.Reactive.Metrics.Logging;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Represents a timer metric.
    /// </summary>
    [Formatter(typeof(TimerFormatter))]
    public class Timer : IMetric
    {
        /// <summary>
        /// Get the number of discrete <see cref="TimingBlock"/> instances recorded.
        /// </summary>
        public Dictionary<string, TimingBlock> Entries { get; } = new Dictionary<string, TimingBlock>();

        private readonly object _lock = new object();

        /// <summary>
        /// Create a timer of a specified name.
        /// </summary>
        /// <param name="name"></param>
        public Timer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Create a <see cref="TimerContext"/> which is used for creating <see cref="TimingBlock"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TimerContext NewContext(string context)
        {
            context = context ?? string.Empty;

            lock (_lock)
            {
                TimingBlock timingBlock;

                if (!Entries.TryGetValue(context, out timingBlock))
                {
                    timingBlock = new TimingBlock(context);
                    Entries[context] = timingBlock;
                }

                return new TimerContext(timingBlock);
            }
        }

        /// <summary>
        /// Set the <see cref="TimingBlock"/> for a specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="block"></param>
        public void Set(string name, TimingBlock block)
        {
            Entries[name] = block;
        }

        /// <summary>
        /// Get the name
        /// </summary>
        public string Name { get; }

        public MetricType MetricType => MetricType.Timer;
    }

}
