using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Formatting;
using Vistian.Reactive.Metrics.Logging;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Simplistic counter
    /// </summary>
    [Formatter(typeof(CounterFormatter))]
    public class Counter : IMetric
    {
        /// <summary>
        /// Our dictionary of instances of counter values
        /// </summary>
        public Dictionary<string, int> Instances { get; } = new Dictionary<string, int>();

        /// <summary>
        /// lock to ensure we correctly update the instances correctly.
        /// </summary>
        private readonly object _lock = new object();

        public static Counter Create()
        {
            return new Counter(string.Empty, Unit.None);
        }

        /// <summary>
        /// Create a named counter of a specified type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="unit"></param>
        public Counter(string name, Unit unit)
        {
            Unit = unit;
            Name = name;
        }

        /// <summary>
        /// Increment the specified counter.
        /// </summary>
        /// <param name="label"></param>
        public void Increment(string label = null)
        {
            lock (_lock)
            {
                label = label ?? string.Empty;

                int value;

                if (!Instances.TryGetValue(label, out value))
                {
                    value = 1;
                }
                else
                {
                    ++value;
                }

                Instances[label] = value;
            }
        }

        /// <summary>
        /// Decrement the specified counter
        /// </summary>
        /// <param name="label"></param>
        public void Decrement(string label = null)
        {
            lock (_lock)
            {
                label = label ?? string.Empty;

                int value;

                if (!Instances.TryGetValue(label, out value))
                {
                    value = -1;
                }
                else
                {
                    --value;
                }

                Instances[label] = value;
            }

        }

        /// <summary>
        /// Get the name of the counter
        /// </summary>
        public string Name { get; }
        public MetricType MetricType => MetricType.Counter;

        /// <summary>
        /// Get the 'type' of counter
        /// </summary>
        public Unit Unit { get; }
    }
}
