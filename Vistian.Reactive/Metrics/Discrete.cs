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
    /// Represents a discrete, instance value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Discrete<T> : Discrete
    {
        public static Discrete<T> Create()
        {
            return new Discrete<T>(string.Empty, Unit.None);
        }

        public Discrete(string name, Unit unit) : base(name, unit)
        {
        }

        public T Value
        {
            get
            {
                return (T)this.LastValue;
            }
            set
            {
                SetValue(value);
            }
        }
    }

    /// <summary>
    /// Create a discrete metric.
    /// </summary>
    [Formatter(typeof(DiscreteFormatter))]
    public abstract class Discrete : IMetric
    {

        public Unit Unit { get; private set; }

        /// <summary>
        /// Get the last recorded value
        /// </summary>
        public object LastValue { get; private set; }

        /// <summary>
        /// Get the number of times the value has been assigned.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Get the first time the value was assigned.
        /// </summary>
        public DateTimeOffset FirstDateTimeOffset { get; private set; }

        /// <summary>
        /// Get the last time the value was assigned.
        /// </summary>
        public DateTimeOffset LastDateTimeOffset { get; private set; }

        /// <summary>
        /// Used to control unified access to the timings & values.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Get the name of the discrete
        /// </summary>
        public string Name { get; }

        protected Discrete(string name, Unit unit)
        {
            Unit = unit;
            Name = name;
        }

        /// <summary>
        /// Set a new value
        /// </summary>
        /// <param name="newValue"></param>
        protected void SetValue(object newValue)
        {
            lock (_lock)
            {

                LastValue = newValue;
                if (Count == 0)
                {
                    FirstDateTimeOffset = DateTimeOffset.UtcNow;
                }

                LastDateTimeOffset = DateTimeOffset.UtcNow;

                // perhaps we need to increment a value ?
                ++Count;
            }
        }

        public MetricType MetricType => MetricType.Discrete;
    }
}
