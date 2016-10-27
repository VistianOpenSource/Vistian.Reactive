using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reactive.Logging.Formatting;

namespace Vistian.Reactive.Metrics.Logging
{
    /// <summary>
    /// Default <see cref="Timer"/> formatter
    /// </summary>
    public class TimerFormatter : SimpleFormatter<Timer>
    {
        readonly TimingBlockFormatter _blockFormatter = new TimingBlockFormatter();

        public override void BuildText(StringBuilder sb, RxLoggerConfiguration configuration, Timer timer)
        {
            sb.AppendLine($"{timer.Name} Timer");

            var totalAmount = timer.Entries.Count;

            sb.AppendLine($"Total {totalAmount}");

            foreach (var instance in timer.Entries)
            {
                _blockFormatter.BuildText(sb, configuration, instance.Value);
            }
        }
    }
}
