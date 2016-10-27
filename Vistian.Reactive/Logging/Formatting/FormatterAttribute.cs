using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Logging.Formatting
{
    /// <summary>
    /// Attribute which can be applied to instances used with logging to help
    /// in specifying a default formatter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class FormatterAttribute : Attribute
    {
        public FormatterAttribute(Type formatterType)
        {
            Guard.NotNull(formatterType);
            Guard.Implements<IRxLogFormatter>(formatterType);

            FormatterType = formatterType;
        }

        /// <summary>
        /// Get the formatter type.MUST implement <see cref="IRxLogFormatter"></see>
        /// </summary>
        public Type FormatterType { get; }
    }
}
