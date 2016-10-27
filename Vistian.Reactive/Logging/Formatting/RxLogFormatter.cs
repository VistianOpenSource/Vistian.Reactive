using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Type safe, generic implementation for building basis of a log formatter.
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    public abstract class RxLogFormatter<TLog> : RxLogFormatter
    {
        protected override string Format(RxLoggerConfiguration configuration, RxLogEntry entry, FormatFlags format)
        {
            return this.Format(configuration, entry.Meta, (TLog)entry.Specifics, format);
        }

        public abstract string Format(RxLoggerConfiguration configuration, RxLogEntryMeta meta, TLog instance, FormatFlags format);
    }

    /// <summary>
    /// Base class providing a platform for building a formatter.
    /// </summary>
    public abstract class RxLogFormatter : IRxLogFormatter
    {
        protected abstract string Format(RxLoggerConfiguration configuration, RxLogEntry entry, FormatFlags format);
        public string Formatted(RxLoggerConfiguration configuration, RxLogEntry entry, FormatFlags format)
        {
            return Format(configuration, entry, format);
        }
    }
}
