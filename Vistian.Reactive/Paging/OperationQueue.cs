using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Represents a simple queue of operations which are performed one at a time.
    /// </summary>
    public class OperationQueue : IDisposable
    {
        /// <summary>
        /// The scheduler on which the items are scheduled to run.
        /// </summary>
        private readonly IScheduler _scheduler;

        /// <summary>
        /// The operation qeuue
        /// </summary>
        private readonly Subject<QueueItem> _operationQueue = new Subject<QueueItem>();

        /// <summary>
        /// Enqueue a specified operation
        /// </summary>
        /// <param name="actionFunc"></param>
        public IObservable<T> Enqueue<T, TCancel>(Func<IObservable<T>> actionFunc, IObservable<TCancel> cancelObservable)
        {
            var queueItem = new QueueItem<T>(actionFunc, cancelObservable.Select(_ => Unit.Default));

            _operationQueue.OnNext(queueItem);

            return queueItem.Result;
        }

        private readonly SerialDisposable _queueDisposable = new SerialDisposable();


        /// <summary>
        /// Get a cancellation token and create one if doesn't exist.
        /// </summary>

        /// <summary>
        /// Create an instance for a specified scheduler.
        /// </summary>
        /// <param name="scheduler"></param>
        public OperationQueue(IScheduler scheduler)
        {
            _scheduler = scheduler;
            SetupQueue();
        }

        /// <summary>
        /// Setup the queue.
        /// </summary>
        private void SetupQueue()
        {
            var queueObservable = _operationQueue.
                                AsObservable().
                                ObserveOn(_scheduler).                                                          // observe on specified scheduler
                                Select(item => Observable.Defer(item.Execute).Catch(Observable.Return(item))).   // use defer approach 2 execute only when selected
                                Concat();                                                                       // enforces serial operation of enqueued items

            _queueDisposable.Disposable = queueObservable.Subscribe();
        }

        /// <summary>
        /// Close down the operation of the queue
        /// </summary>
        public void Close()
        {
            Flush();
            _queueDisposable.Disposable = Disposable.Empty;
        }

        /// <summary>
        /// Flush the queue
        /// </summary>
        public void Flush()
        {
            // if we have a cancellation token source then cancel all outstanding operations.
        }

        public void Dispose()
        {
            _queueDisposable?.Dispose();
        }
    }

    /// <summary>
    /// The internal queued item
    /// </summary>
    internal abstract class QueueItem
    {
        public abstract IObservable<QueueItem> Execute();
    }

    internal class QueueItem<T> : QueueItem
    {
        private readonly Func<IObservable<T>> _actionFunc;
        private readonly IObservable<Unit> _cancelObservable;

        public readonly ReplaySubject<T> Result = new ReplaySubject<T>();

        public QueueItem(Func<IObservable<T>> actionFunc, IObservable<Unit> cancelObservable)
        {
            _actionFunc = actionFunc;
            _cancelObservable = cancelObservable;
        }

        public override IObservable<QueueItem> Execute()
        {
            var ret = _actionFunc().Take(1).TakeUntil(_cancelObservable).Multicast(Result);

            ret.Connect();

            return ret.Select(_ => this);


        }
    }
}
