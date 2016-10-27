using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Resolves an appropriate formatter for a <see cref="RxLog"/>
    /// </summary>
    public class FormatterResolver : IFormatterResolver
    {
        private readonly bool _useAttributes;
        private readonly IRxLogFormatter _defaultFormatter;

        /// <summary>
        /// The instances of all known so far <see cref="IRxLogFormatter"/> instances.
        /// </summary>
        private readonly Dictionary<Type, IRxLogFormatter> _formatterMap = new Dictionary<Type, IRxLogFormatter>();

        private static readonly Lazy<FormatterResolver> DefaultInstance = new Lazy<FormatterResolver>(() => new FormatterResolver(useAttributes: true));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAttributes"></param>
        /// <param name="defaultFormatter"></param>
        /// <param name="specifiedFormatters"></param>
        public FormatterResolver(bool useAttributes = true, IRxLogFormatter defaultFormatter = null, IDictionary<Type, IRxLogFormatter> specifiedFormatters = null)
        {
            _useAttributes = useAttributes;
            _defaultFormatter = defaultFormatter;

            if (specifiedFormatters != null)
            {
                foreach (var formatter in specifiedFormatters)
                {
                    _formatterMap[formatter.Key] = formatter.Value;
                }
            }
        }

        /// <summary>
        /// Get the default, reflection / attribute based formatter.
        /// </summary>
        public static FormatterResolver Default => DefaultInstance.Value;

        /// <summary>
        /// Get the formatter for a specified instance.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public IRxLogFormatter GetFor(RxLogEntry entry)
        {
            IRxLogFormatter formatterInstance;

            Guard.NotNull(entry);
            Guard.NotNull(entry.Specifics);

            if (_formatterMap.TryGetValue(entry.Specifics.GetType(), out formatterInstance))
            {
                return formatterInstance ?? _defaultFormatter;
            }

            if (_useAttributes)
            {
                TryResolveByAttribute(entry);

                formatterInstance = _formatterMap[entry.Specifics.GetType()];

                if (formatterInstance != null)
                {
                    return formatterInstance;
                }
            }

            return _defaultFormatter;
        }

        /// <summary>
        /// Determine if there is a formatter for
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public bool HasFormatter(RxLogEntry logEntry)
        {
            IRxLogFormatter formatterInstance;

            Guard.NotNull(logEntry);
            Guard.NotNull(logEntry.Specifics);

            if (_formatterMap.TryGetValue(logEntry.Specifics.GetType(), out formatterInstance))
            {
                return formatterInstance != null;
            }

            if (_useAttributes)
            {
                TryResolveByAttribute(logEntry);

                formatterInstance = _formatterMap[logEntry.Specifics.GetType()];

                if (formatterInstance != null)
                {
                    return true;
                }
            }

            return _defaultFormatter != null;
        }

        private void TryResolveByAttribute(RxLogEntry instance)
        {
            Guard.NotNull(instance);

            var rootType = instance.Specifics.GetType();
            var attribute = rootType.GetTypeInfo().GetCustomAttributes(typeof(FormatterAttribute), true).Cast<FormatterAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                _formatterMap[rootType] = null;
            }
            else
            {
                _formatterMap[rootType] = (IRxLogFormatter)Activator.CreateInstance(attribute.FormatterType);
            };
        }
    }
}
