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
    /// The default discrete formatter.
    /// </summary>
    public class DiscreteFormatter : SimpleFormatter<Discrete>
    {
        public override void BuildText(StringBuilder sb, RxLoggerConfiguration configuration, Discrete discrete)
        {
            sb.AppendLine($"{discrete.Name} {discrete.GetType()} Discrete {discrete.Unit.Name}");

            if (discrete.Count > 0)
            {
                sb.AppendLine($"Assigned = {discrete.Count} time(s)");

                sb.AppendLine($"Last Value = {discrete.LastValue} {discrete.Unit.Name}");
                sb.AppendLine($"At = {discrete.LastDateTimeOffset}");
            }
            else
            {
                sb.AppendLine("Never assigned");
            }
        }
    }
}
