using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reactive.Logging.Formatting;

// ReSharper disable once CheckNamespace
namespace Vistian.Reactive.Logging.Providers
{
    /// <summary>
    /// Simplistic formatter for <see cref="Classified"/> instances.
    /// </summary>
    public class ClassifiedFormatter : SimpleFormatter<Classified>
    {
        public override void BuildText(StringBuilder sb, RxLoggerConfiguration configuration, Classified instance)
        {
            sb.Append(instance.Message);
        }
    }
}
