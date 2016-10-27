using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Simple abstract formatter, which uses a default construct to make it easier to format items.
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    public abstract class SimpleFormatter<TLog> : RxLogFormatter<TLog>
    {
        private readonly bool _multiLine;

        protected SimpleFormatter(bool multiLine = false)
        {
            _multiLine = multiLine;
        }

        public override string Format(RxLoggerConfiguration configuration, RxLogEntryMeta meta, TLog instance, FormatFlags format)
        {
            Guard.NotNull(configuration);
            Guard.NotNull(instance);

            // right then, this is what we do
            // we format out the meta data that we want, and then just use a simple method to get the 
            // rest of the formatted text

            var sb = new StringBuilder();

            if (format == FormatFlags.IncludeMeta)
            {
                var formattedMeta = configuration.Formatting.FormatMeta(meta);

                if (_multiLine)
                {
                    sb.AppendLine(formattedMeta);
                }
                else
                {
                    sb.Append(formattedMeta + " ");
                }
            }

            BuildText(sb, configuration, instance);

            return sb.ToString();
        }

        public abstract void BuildText(StringBuilder sb, RxLoggerConfiguration configuration, TLog instance);
    }
}
