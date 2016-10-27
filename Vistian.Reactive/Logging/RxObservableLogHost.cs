﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging
{
    /// <summary>
    /// The Log host.
    /// </summary>
    public class RxObservableLogHost : IDisposable, IRxLogHost
    {
        /// <summary>
        /// The backing subject for all of the logging details.
        /// </summary>
        private readonly ISubject<RxLogEntry, RxLogEntry> _syncsubject;

        /// <summary>
        /// 
        /// </summary>
        private readonly ISubject<RxLogEntry, RxLogEntry> _pumpSubject;

        /// <summary>
        /// The things we need to dispose of
        /// </summary>
        private readonly CompositeDisposable _disposables;

        /// <summary>
        /// The message observable
        /// </summary>
        private readonly IObservable<RxLogEntry> _messageObservable;


        /// <summary>
        /// Create an instance using the specified configuration
        /// </summary>
        /// <param name="configuration"></param>
        public RxObservableLogHost(RxLoggerConfiguration configuration)
        {
            Configuration = configuration;

            _disposables = new CompositeDisposable();

            var pump = new Subject<RxLogEntry>();

            _pumpSubject = Subject.Synchronize(pump, Configuration.Host.PublishScheduler);

            //TODO: perhaps need to 'wind in' the duplicate sync subjects, only need one really, that which 
            // starts the process...

            var log = new ReplaySubject<RxLogEntry>(Configuration.Host.ReplaySize);

            // ensure we marshall the log requests to the appropriate scheduler...
            _syncsubject = Subject.Synchronize(log, Configuration.Host.PublishScheduler);

            // setup the subscription
            /*
            var prePumpSubscription = Configuration.Host.PrePumpObservable(_pumpSubject).
                Do(entry => { _syncsubject.OnNext(entry); }).
                Catch(Observable.Empty<RxLogEntry>()).
                Retry().
                Subscribe();
            */

            var prePumpSubscription = _pumpSubject.
                Do(Enqueue).
                Select(_ => Unit.Default).
                Catch((Exception e) => this.OnError(e)).
                Retry().
                Subscribe();

            _disposables.Add(prePumpSubscription);

            // Create a single one
            _messageObservable = log.Publish().RefCount();

            // and now build all of the subscribers
            BuildSubscribers(_messageObservable);
        }

        /// <summary>
        /// Initialize the publish pipeline and the subscribers
        /// </summary>
        private void Initialize()
        {
        }

        /// <summary>
        /// Get the configuration for this log host.
        /// </summary>
        public RxLoggerConfiguration Configuration { get; }

        /// <summary>
        /// Public a <see cref="RxLog"/> to subscribers.
        /// </summary>
        /// <param name="entry"></param>
        public void Publish(RxLogEntry entry)
        {
            Initialize();

            _pumpSubject.OnNext(entry);
        }

        /// <summary>
        /// Internal method to enqueue a message
        /// </summary>
        /// <param name="entry"></param>
        internal void Enqueue(RxLogEntry entry)
        {
            _syncsubject.OnNext(entry);
        }


        /// <summary>
        /// Build all of the subscriptions.
        /// </summary>
        /// <param name="logSource"></param>
        public void BuildSubscribers(IObservable<RxLogEntry> logSource)
        {
            foreach (var subscriber in Configuration.Subscribers)
            {
                // we need to add some error handling around this, to ensure we don't c**k up...
                _disposables.Add(subscriber(Configuration, logSource).Catch((Exception ex) => OnError(ex)).Retry().Subscribe());
            }
        }

        /// <summary>
        /// Invoke the error function in the 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private IObservable<Unit> OnError(Exception exception)
        {
            return Configuration.Host.OnError(exception);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
