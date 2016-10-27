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
    /// Default <see cref="Counter"/> formatter
    /// </summary>
    public class CounterFormatter : SimpleFormatter<Counter>
    {
        public override void BuildText(StringBuilder sb, RxLoggerConfiguration configuration, Counter counter)
        {
            sb.AppendLine($"{counter.Name} Counter");

            var totalAmount = counter.Instances.Values.Sum();

            sb.AppendLine($"Total {totalAmount}");

            if (counter.Instances.ContainsKey(string.Empty))
            {
                var noNameMatch = counter.Instances[string.Empty];

                sb.AppendLine($"Item (unnamed) = {noNameMatch} {counter.Unit.Name}");
            }

            foreach (var instance in counter.Instances)
            {
                if (!string.IsNullOrEmpty(instance.Key))
                {
                    sb.AppendLine($"Item {instance.Key} = {instance.Value} {counter.Unit.Name}");
                }
            }

        }
    }
}
