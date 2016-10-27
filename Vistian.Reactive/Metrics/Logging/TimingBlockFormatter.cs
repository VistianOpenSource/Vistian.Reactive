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
    /// Default <see cref="TimingBlock"/> formatter.
    /// </summary>
    public class TimingBlockFormatter : SimpleFormatter<TimingBlock>
    {
        public override void BuildText(StringBuilder sb, RxLoggerConfiguration configuration, TimingBlock instance)
        {
            if (string.IsNullOrEmpty(instance.Name))
            {
                sb.AppendLine($"unnamed Timer");
            }
            else
            {
                sb.AppendLine($"{instance.Name} Timer");
            }

            sb.AppendLine($"Total = {instance.Count}");

            if (instance.Count > 1)
            {
                sb.AppendLine($"Minimum = {instance.Min} ms");
                sb.AppendLine($"Maximum = {instance.Max} ms");
                sb.AppendLine($"Mean = {instance.Mean} ms");
            }

            sb.AppendLine($"Last = {instance.LastMilliseconds} ms");

            var entryCounter = 1;

            long last = -1;
            foreach (var entry in instance.Entries)
            {
                if (last < 0)
                {
                    last = entry.Milliseconds;
                }
                else
                {
                    last = entry.Milliseconds - last;
                }

                sb.AppendLine($"#{entryCounter,3} {entry.Label} =  {entry.Milliseconds} ms (+{last}) ms");

                last = entry.Milliseconds;

                ++entryCounter;
            }
        }
    }
}
