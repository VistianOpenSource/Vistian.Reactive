using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Xamarin;

namespace Vistian.Reactive.Logging
{
    /// <summary>
    /// Class representing a single log entry
    /// </summary>
	[Preserve]
    public class RxLogEntry
    {
        public RxLogEntry(RxLogEntryMeta meta, object specifics)
        {
            Guard.NotNull(specifics);

            Meta = meta;
            Specifics = specifics;
        }

        /// <summary>
        /// Get the core details surrounding this log entry.
        /// </summary>
        public RxLogEntryMeta Meta { get; private set; }

        /// <summary>
        /// Get or set the actual log instance
        /// </summary>
        public object Specifics { get; }

        /// <summary>
        /// Determine if this log entry is of a specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool InstanceOfType<T>()
        {
            return Specifics.GetType() == typeof(T);
        }

        public static RxLogEntry WithNoMeta(object specifics)
        {
            return new RxLogEntry(null, specifics);
        }
    }

    [Preserve]
    public class RxLogEntry<TInstance> : RxLogEntry
    {
        public RxLogEntry(RxLogEntryMeta meta, TInstance specifics) : base(meta, specifics)
        {
        }

        /// <summary>
        /// Get or set the actual log instance
        /// </summary>
        public TInstance Instance => (TInstance)Specifics;
    }
}
