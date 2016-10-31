using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.Linq
{
    public static class RetryConditionExtensions
    {
        /// <summary>
        /// Retry the source using a separate Observable to determine whether to retry again or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="retryObservable">The observable factory used to determine whether to retry again or not. Number of retries & exception provided as parameters</param>
        /// <param name="executeScheduler">The scheduler to be used to observe the source on. If non specified MainThreadScheduler used</param>
        /// <param name="retryScheduler">The scheduler to use for the retry to be observed on. If non specified MainThreadScheduler used.</param>
        /// <returns></returns>
        public static IObservable<T> RetryX<T>(this IObservable<T> source,
            Func<int, Exception, IObservable<bool>> retryObservable, IScheduler executeScheduler = null,
            IScheduler retryScheduler = null)
        {
            if (retryObservable == null)
            {
                throw new ArgumentNullException(nameof(retryObservable));
            }

            if (executeScheduler == null)
            {
                executeScheduler = RxApp.MainThreadScheduler;
            }

            if (retryScheduler == null)
            {
                retryScheduler = RxApp.MainThreadScheduler;
            }

            // so, we need to subscribe to the sequence, if we get an error, then we do that again...
            return Observable.Create<T>(o =>
            {
                // whilst we are supposed to be running, we need to execute this
                var trySubject = new Subject<Exception>();

                // record number of times we retry
                var retryCount = 0;

                return trySubject.
                    AsObservable().
                    ObserveOn(retryScheduler).
                    SelectMany((e) => Observable.Defer(() => retryObservable(retryCount, e))). // select the retry logic
                    StartWith(true). // prime the pumps to ensure at least one execution
                    TakeWhile(shouldTry => shouldTry). // whilst we should try again
                    ObserveOn(executeScheduler).
                    Select(g => Observable.Defer(() => source.Materialize())). // get the result of the selector
                    Switch(). // always take the last one
                    Do((v) =>
                        {
                            switch (v.Kind)
                            {
                                case NotificationKind.OnNext:
                                    o.OnNext(v.Value);
                                    break;

                                case NotificationKind.OnError:
                                    ++retryCount;
                                    trySubject.OnNext(v.Exception);
                                    break;

                                case NotificationKind.OnCompleted:
                                    o.OnCompleted();
                                    trySubject.OnCompleted();
                                    break;
                            }
                        }
                    ).Subscribe();
            });
        }
    }
}
