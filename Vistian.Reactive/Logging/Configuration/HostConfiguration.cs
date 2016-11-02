using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Vistian.Contract;

namespace Vistian.Reactive.Logging.Configuration
{
    /// <summary>
    /// Main log host configuration details.
    /// </summary>
    public class HostConfiguration
    {
        /// <summary>
        /// Get the scheduler upon which all logging events are synchronized with
        /// </summary>
        public IScheduler PublishScheduler { get; }

        /// <summary>
        /// Get the size of the buffer used for logging.
        /// </summary>
        public int ReplaySize { get; }

        /// <summary>
        /// The observable used for any processing prior to being enqueued.
        /// </summary>
        /// <remarks>
        /// Default behavior is straight passthrough.</remarks>
        public Func<IObservable<RxLogEntry>, IObservable<RxLogEntry>> PrePumpObservable { get; }

        /// <summary>
        /// Get or set the error function.
        /// </summary>
        public Func<Exception, IObservable<Unit>> Errored { get;  }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishScheduler"></param>
        /// <param name="replaySize"></param>
        /// <param name="prePumpObservable"></param>
        /// <param name="errored"></param>
        public HostConfiguration(IScheduler publishScheduler = null, int replaySize = 1,
            Func<IObservable<RxLogEntry>, IObservable<RxLogEntry>> prePumpObservable = null,Func<Exception,IObservable<Unit>> errored=null)
        {
            PublishScheduler = publishScheduler ?? TaskPoolScheduler.Default;

            PrePumpObservable = prePumpObservable ?? ((o) => o);

            Errored = errored ?? ((e) => Observable.Return(Unit.Default));

            ReplaySize = replaySize;
        }

        /// <summary>
        /// Create the host.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public virtual RxObservableLogHost CreateHost(RxLoggerConfiguration configuration)
        {
            Guard.NotNull(configuration);

            var host = new RxObservableLogHost(configuration);
            return host;
        }

        /// <summary>
        /// Invoked from the observable host when an exception is received.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public IObservable<Unit> OnError(Exception exception)
        {
            return Errored(exception);
        }

        public static HostConfiguration Default => new HostConfiguration(null, 1, null);
    }
}
