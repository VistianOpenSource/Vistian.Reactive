using System;
using System.Collections.Generic;
using Vistian.Reactive.Logging.Formatting;

namespace Vistian.Reactive.Logging.Configuration
{
    /// <summary>
    /// Configuration for the formatter
    /// </summary>
    public class LogFormattingConfiguration
    {
        /// <summary>
        /// Get the formatter resolver.
        /// </summary>
        public IFormatterResolver Resolver { get; private set; }

        /// <summary>
        /// Provides a default formatted single line string for the meta data.
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public virtual string FormatMeta(RxLogEntryMeta meta)
        {
            var formatter = new DefaultRxLogMetaFormatter();

            return formatter.Formatted(meta);
        }

        /// <summary>
        /// Create an instance with a specified <see cref="IFormatterResolver"/>
        /// </summary>
        /// <param name="resolver"></param>
        public LogFormattingConfiguration(IFormatterResolver resolver)
        {
            Resolver = resolver;
        }

        public IRxLogFormatter FormatterFor(RxLogEntry logEntry)
        {
            return Resolver.GetFor(logEntry);
        }

        /// <summary>
        /// Determine if there is a formatter for
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public bool HasFormatter(RxLogEntry logEntry)
        {
            return Resolver.HasFormatter(logEntry);
        }

        /// <summary>
        /// Create a default configuration , using the default formatters and attribute based 
        /// </summary>
        public static LogFormattingConfiguration Default => new LogFormattingConfiguration(new FormatterResolver(true, new DefaultRxLogFormatter(new DefaultRxLogMetaFormatter())));

        /// <summary>
        /// Create a default configuration that just uses attributes of message types to resolve their formatters.
        /// </summary>
        public static LogFormattingConfiguration AttributeBased => new LogFormattingConfiguration(new FormatterResolver(true, null));

        /// <summary>
        /// Create a default configuration using a specifed set of formatters.
        /// </summary>
        /// <param name="formatters"></param>
        /// <returns></returns>
        public static LogFormattingConfiguration Listed(IDictionary<Type, IRxLogFormatter> formatters)
        {
            var resolver = new FormatterResolver(false, specifiedFormatters: formatters);

            return new LogFormattingConfiguration(resolver);
        }
    }
}
