using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reflection;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Default <see cref="IRxLogFormatter"/> implementation.
    /// </summary>
    /// <remarks>
    /// Default log formatting consistents of formatting the <see cref="RxLogEntryMeta"/> using a provided meta
    /// formatter and then, for each property that the object has, it writes out a simple PropertyName=Value.
    /// 
    /// </remarks>
    public class DefaultRxLogFormatter : IRxLogFormatter
    {
        /// <summary>
        /// The default meta formatter
        /// </summary>
        private readonly IRxLogMetaFormatter _metaFormatter;

        /// <summary>
        /// Whether we do a single property per line, or concat them all together
        /// </summary>
        private readonly bool _onePropertyPerLine;

        public DefaultRxLogFormatter(IRxLogMetaFormatter metaFormatter, bool onePropertyPerLine = false)
        {
            Guard.NotNull(metaFormatter);
            _metaFormatter = metaFormatter;
            _onePropertyPerLine = onePropertyPerLine;
        }

        public string Formatted(RxLoggerConfiguration configuration, RxLogEntry entry, FormatFlags flags)
        {
            Guard.NotNull(configuration);
            Guard.NotNull(entry);

            var sb = new StringBuilder();
            // right then, we need to format the core and then the rest of the properties for the actual log item... 
            // this is a straight wander of the properties of the item...

            if (flags == FormatFlags.IncludeMeta && entry.Meta != null)
            {
                sb.Append(_metaFormatter.Formatted(entry.Meta));
            }

            if (_onePropertyPerLine)
            {
                sb.AppendLine();
                sb.AppendLine(entry.Specifics.GetType().Name);
            }
            else
            {
                sb.Append(" ");
                sb.Append(entry.Specifics.GetType().Name);
                sb.Append(" ");
            }


            var attributes = entry.Specifics.Properties();
            foreach (var attribute in attributes)
            {
                if (_onePropertyPerLine)
                {
                    sb.Append(attribute.Key);
                    sb.Append("=");
                    sb.Append(attribute.Value);
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(" ");
                    sb.Append(attribute.Key);
                    sb.Append("=");
                    sb.Append(attribute.Value);

                }
            }

            return sb.ToString();
        }
    }
}
