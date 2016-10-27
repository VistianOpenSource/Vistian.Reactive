using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Specification of a metric.
    /// </summary>
    public interface IMetric
    {
        /// <summary>
        /// Get the optional name for this metric.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the type of this metric.
        /// </summary>
        MetricType MetricType { get; }
    }
}
