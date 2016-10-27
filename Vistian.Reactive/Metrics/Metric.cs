using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Static wrapper class to easily create global metrics.
    /// </summary>
    public static class Metric
    {
        /// <summary>
        /// The collection of Metrics we have created so far.
        /// </summary>
        private static readonly Lazy<Metrics> Metrics = new Lazy<Metrics>(() => new Metrics());

        /// <summary>
        /// Get a <see cref="Counter"/> as specified by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Counter Counter(string name, Unit unit)
        {
            return Metrics.Value.GetCounter(name, unit);
        }

        /// <summary>
        /// Get a <see cref="Discrete{T}"/> instance by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Discrete<T> Discrete<T>(string name, Unit unit)
        {
            return Metrics.Value.GetDiscrete<T>(name, unit);
        }

        /// <summary>
        /// Get a <see cref="Timer"/> instance by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Timer Timer(string name)
        {
            return Metrics.Value.GetTimer(name);
        }
    }
}
