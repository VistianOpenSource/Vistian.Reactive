using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reactive.Logging.Formatting;

namespace Vistian.Reactive.Metrics.Logging
{
    /// <summary>
    /// Formatter for a <see cref="Metrics"/>
    /// </summary>
    public class MetricsFormatter : RxLogFormatter<Metrics>
    {
        public override string Format(RxLoggerConfiguration configuration, RxLogEntryMeta meta, Metrics instance, FormatFlags format)
        {
            var sb = new StringBuilder();

            // now then, we now need to do a default format of the meta

            var formattedMeta = configuration.Formatting.FormatMeta(meta);

            sb.AppendLine(formattedMeta);

            // need to find the formatter for the metric type...
            var keys = instance.Items.Keys.ToList();

            foreach (var key in keys)
            {
                // now need to dump each of the metrics.
                IMetric metric;

                if (instance.Items.TryGetValue(key, out metric))
                {
                    // now need to try and resolve a formatter for each type

                    var formatter = configuration.Formatting.Resolver.GetFor(RxLogEntry.WithNoMeta(metric));

                    if (formatter != null)
                    {
                        // get the formatted output
                        var formattedOutput = formatter.Formatted(configuration, RxLogEntry.WithNoMeta(metric), FormatFlags.Instance);

                        sb.Append(formattedOutput);
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

    }
}
