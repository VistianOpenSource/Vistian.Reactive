using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Logging.Formatting;
using Vistian.Reactive.Metrics.Logging;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Collection of permanent metrics
    /// </summary>
    [Formatter(typeof(MetricsFormatter))]
    public class Metrics
    {
        /// <summary>
        /// Our list of currently defined metrics.
        /// </summary>
        public ConcurrentDictionary<string, IMetric> Items { get; } = new ConcurrentDictionary<string, IMetric>();

        /// <summary>
        /// Get a named counter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Counter GetCounter(string name, Unit unit)
        {
            return GetOrCreate(name, (n) => new Counter(n, unit));
        }

        /// <summary>
        /// Get a named discrete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Discrete<T> GetDiscrete<T>(string name, Unit unit)
        {
            return GetOrCreate(name, (n) => new Discrete<T>(n, unit));
        }

        /// <summary>
        /// Get a named timer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Timer GetTimer(string name)
        {
            return GetOrCreate(name, (n) => new Timer(n));
        }

        /// <summary>
        /// Get or create a metric, using a specified creation function when the metric doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        private T GetOrCreate<T>(string name, Func<string, T> create) where T : IMetric
        {
            name = name ?? string.Empty;

            IMetric metric;

            if (Items.TryGetValue(name, out metric))
            {
                if (!(metric is T))
                {
                    throw new ArgumentException(name);
                }
            }
            else
            {
                metric = create(name);
                Items[name] = metric;
            }
            return (T)metric;
        }

        /// <summary>
        /// Remove the specified metric.
        /// </summary>
        /// <param name="metric"></param>
        public void RemoveMetric(IMetric metric)
        {
            Guard.NotNull(metric);

            IMetric removedMetric;

            Items.TryRemove(metric.Name, out removedMetric);

        }
    }
}
