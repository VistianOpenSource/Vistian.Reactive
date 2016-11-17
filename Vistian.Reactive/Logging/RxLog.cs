using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reactive.Logging.Providers;

namespace Vistian.Reactive.Logging
{
    /// <summary>
    /// Main class for performance of logging
    /// </summary>
    public sealed class RxLog
    {
        /// <summary>
        /// The global, default RxLog.
        /// </summary>
        public static RxLog Default { get; private set; }

        /// <summary>
        /// The logging host.
        /// </summary>
        public IRxLogHost LogHost { get; set; }

        /// <summary>
        /// Get or set whether enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Create an instance associated with a specified <see cref="IRxLogHost"/>
        /// </summary>
        /// <param name="host"></param>
        public RxLog(IRxLogHost host)
        {
            Guard.NotNull(host);

            LogHost = host;
        }

        /// <summary>
        /// Publish a <see cref="RxLogEntry"/> to the host.
        /// </summary>
        /// <param name="entry"></param>
        public void Publish(RxLogEntry entry)
        {
            Guard.NotNull(entry);

            if (Enabled)
            {
                LogHost.Publish(entry);
            }
        }

        /// <summary>
        /// Set the default <see cref="RxLog"/> implementation.
        /// </summary>
        /// <param name="rxLog"></param>
        public static void SetDefault(RxLog rxLog)
        {
            if (RxLog.Default != null)
            {
                RxLog.Log(typeof(RxLog), Classified.Error("RxLog already has a default host assigned, request ignored."));
                return;
            }
            RxLog.Default = rxLog;
        }

        /// <summary>
        /// Set the default <see cref="RxLog"/> based upon a specified configuration.
        /// </summary>
        /// <param name="configuration"></param>
        public static void SetDefault(RxLoggerConfiguration configuration)
        {
            Guard.NotNull(configuration);

            var host = configuration.CreateHost();

            RxLog.SetDefault(new RxLog(host));
        }

        /// <summary>
        /// Log an entry to the default log writer.
        /// </summary>
        /// <param name="entry"></param>
        public static void Log(RxLogEntry entry)
        {
            Guard.NotNull(entry);

            Default?.Publish(entry);
        }


        /// <summary>
        /// Create and publish a log entry with specified <see cref="RxLogEntryMeta"/> and instance data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="meta"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static RxLogEntry<T> Log<T>(RxLogEntryMeta meta, T instance)
        {
            Guard.NotNull(instance);

            var entry = new RxLogEntry<T>(meta, instance);

            Log(entry);

            return entry;
        }

        /// <summary>
        /// Publish a lost entry using the specified meta and instance data.
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static RxLogEntry Log(RxLogEntryMeta meta, object instance)
        {
            Guard.NotNull(instance);

            var entry = new RxLogEntry(meta, instance);

            Log(entry);

            return entry;
        }

        /// <summary>
        /// Log information with automatic population of meta data.
        /// </summary>
        /// <typeparam name="THostType"></typeparam>
        /// <param name="instance"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        public static RxLogEntry Log<THostType>(object instance, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(instance);

            var meta = CreateMeta(typeof(THostType), callerMemberName, lineNo);

            return Log(meta, instance);
        }

        /// <summary>
        /// Log information with automatic population of meta data.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="instance"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        public static RxLogEntry Log(object host, object instance, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(host);
            Guard.NotNull(instance);

            var meta = CreateMeta(host.GetType(), callerMemberName, lineNo);

            return Log(meta, instance);
        }


        public static RxLogEntry<TInstanceType> Log<TInstanceType>(object host, TInstanceType instance,
            [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(host);
            Guard.NotNull(instance);

            var meta = CreateMeta(host.GetType(), callerMemberName, lineNo);

            return Log(meta, instance);

        }

        /// <summary>
        /// Create meta with the specified host type, calling member name and line number..
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        public static RxLogEntryMeta CreateMeta(Type type, string callerMemberName, int lineNo)
        {
            return Default?.LogHost?.Configuration.CreateMeta(type, callerMemberName, lineNo);
        }
    }
}
