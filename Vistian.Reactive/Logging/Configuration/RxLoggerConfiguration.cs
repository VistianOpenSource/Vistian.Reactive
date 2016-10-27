using System;
using System.Collections.Generic;
using System.Reactive;
using Vistian.Contract;

namespace Vistian.Reactive.Logging.Configuration
{
    /// <summary>
    /// Main configuration file for <see cref="RxLog"/>
    /// </summary>
    public class RxLoggerConfiguration
    {
        /// <summary>
        /// Get the host configuration
        /// </summary>
        public HostConfiguration Host { get; }

        /// <summary>
        /// Get the formatting configuration.
        /// </summary>
        public LogFormattingConfiguration Formatting { get; }

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="hostConfiguration"></param>
        /// <param name="formattingConfiguration"></param>
        /// <param name="logSubscribers"></param>
        public RxLoggerConfiguration(HostConfiguration hostConfiguration, LogFormattingConfiguration formattingConfiguration,
                                    IEnumerable<Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>> logSubscribers)
        {
            Guard.NotNull(hostConfiguration);
            Guard.NotNull(formattingConfiguration);

            Host = hostConfiguration;
            Formatting = formattingConfiguration;

            AddSubscribers(logSubscribers);
        }

        /// <summary>
        /// Create an observable log host
        /// </summary>
        /// <returns></returns>
        public RxObservableLogHost CreateHost()
        {
            var logHost = Host.CreateHost(this);

            return logHost;
        }

        /// <summary>
        /// Create the meta data for an entry.
        /// </summary>
        /// <param name="callingClass"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        public virtual RxLogEntryMeta CreateMeta(Type callingClass, string memberName, int lineNo)
        {
            return new RxLogEntryMeta(callingClass, memberName, lineNo);
        }

        /// <summary>
        /// The list of subscribers.
        /// </summary>
        public List<Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>> Subscribers = new List<Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>>();

        /// <summary>
        /// Add a subscriber
        /// </summary>
        /// <param name="observerFactory"></param>
        public void AddSubscriber(Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>> observerFactory)
        {
            Guard.NotNull(observerFactory);

            Subscribers.Add(observerFactory);
        }

        /// <summary>
        /// Add subscribers.
        /// </summary>
        /// <param name="subscribers"></param>
        public void AddSubscribers(IEnumerable<Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                AddSubscriber(subscriber);
            }
        }

        /// <summary>
        /// Create a default configuration with specified subscribers.
        /// </summary>
        /// <param name="logFormattingConfiguration"></param>
        /// <param name="logSubscribers"></param>
        /// <param name="hostConfiguration"></param>
        /// <returns></returns>
        public static RxLoggerConfiguration Create(HostConfiguration hostConfiguration = null, LogFormattingConfiguration logFormattingConfiguration = null, params Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>[] logSubscribers)
        {
            hostConfiguration = hostConfiguration ?? HostConfiguration.Default;
            logFormattingConfiguration = logFormattingConfiguration ?? LogFormattingConfiguration.Default;

            return new RxLoggerConfiguration(hostConfiguration, logFormattingConfiguration, logSubscribers);
        }

        /// <summary>
        /// Create a default configuration with specified subscribers.
        /// </summary>
        /// <param name="logSubscribers"></param>
        /// <returns></returns>
        public static RxLoggerConfiguration Create(params Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>[] logSubscribers)
        {
            return Create(null, null, logSubscribers);
        }

        /// <summary>
        /// Create a configuration with a specified <see cref="HostConfiguration"/> and subscribers
        /// </summary>
        /// <param name="hostConfiguration"></param>
        /// <param name="logSubscribers"></param>
        /// <returns></returns>
        public static RxLoggerConfiguration Create(HostConfiguration hostConfiguration, params Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>>[] logSubscribers)
        {
            return Create(hostConfiguration, null, logSubscribers);
        }
    }
}
