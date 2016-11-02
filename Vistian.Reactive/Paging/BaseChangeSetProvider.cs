using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Vistian.Contract;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Base class providing enqueing of read requests and changesets for the read data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class BaseChangeSetProvider<TItem> : IPagedDataProvider<TItem>, IDisposable
    {
        public readonly IPagingController<TItem> PagingController;

        /// <summary>
        /// The actual queue
        /// </summary>
        private readonly OperationQueue _operationQueue;

        /// <summary>
        /// Used to cancel all outstanding operations
        /// </summary>
        private readonly Subject<Unit> _cancelAllPending = new Subject<Unit>();

        /// <summary>
        /// Are we currently busy.
        /// </summary>
        private readonly BehaviorSubject<bool> _busy = new BehaviorSubject<bool>(false);

        /// <summary>
        /// running count of inflight operations.
        /// </summary>
        private int _busyCount;

        /// <summary>
        /// The changeset of data changes
        /// </summary>
        private IObservable<IChangeSet<TItem>> _changeSet;

        /// <summary>
        /// The scheduler used to perform the update on
        /// </summary>
        private readonly IScheduler _updateScheduler;


        /// <summary>
        /// The actual reader of the pages
        /// </summary>
        private readonly PageReader<TItem> _pageReader;

        /// <summary>
        /// Disposable for subscriptions to the changesets
        /// </summary>
        private SingleAssignmentDisposable _changeSetDisposable;

        /// <summary>
        /// Busy observable, if any operations in play.
        /// </summary>
        public IObservable<bool> BusyObservable => _busy.AsObservable();

        /// <summary>
        /// Our changeset
        /// </summary>
        public IObservable<IChangeSet<TItem>> Changes => GetChangeSet();

        /// <summary>
        /// Get the total size of the data available.
        /// </summary>
        public int? Total { get; private set; }



        protected BaseChangeSetProvider(IPagingController<TItem> pagingController,
            IScheduler readScheduler = null,
            IScheduler updateScheduler = null)
        {
            Guard.NotNull(pagingController);

            PagingController = pagingController;

            _updateScheduler = updateScheduler ?? RxApp.MainThreadScheduler;
            readScheduler = readScheduler ?? RxApp.TaskpoolScheduler;

            _operationQueue = new OperationQueue(readScheduler);
            _pageReader = new PageReader<TItem>(pagingController, readScheduler);

            PagingController = pagingController;
        }

        /// <summary>
        /// Enqueue an operation, whilst maintaing a count of the inflight operations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observableFunc"></param>
        /// <returns>a HOT observable</returns>
        protected IObservable<T> Enqueue<T>(Func<IObservable<T>> observableFunc)
        {
            Guard.NotNull(observableFunc);

            if (Interlocked.Increment(ref _busyCount) == 1)
            {
                _busy.OnNext(true);
            }

            var operation = _operationQueue.Enqueue(observableFunc, _cancelAllPending.AsObservable());

            return operation.Finally(() =>
            {
                if (Interlocked.Decrement(ref _busyCount) == 0)
                {
                    _busy.OnNext(false);
                }
            });
        }

        /// <summary>
        /// Construct the change set.
        /// </summary>
        /// <returns></returns>
        protected IObservable<IChangeSet<TItem>> GetChangeSet()
        {
            if (_changeSet == null)
            {
                _changeSet = CreateChangeSet();

                _changeSetDisposable = new SingleAssignmentDisposable() { Disposable = _changeSet.Subscribe() };
            }
            return _changeSet;
        }

        /// <summary>
        /// Overloaded method responsible for creating the change set.
        /// </summary>
        /// <returns></returns>
        protected abstract IObservable<IChangeSet<TItem>> CreateChangeSet();

        /// <summary>
        /// Enqueue a read
        /// </summary>
        /// <param name="pageReadRequest"></param>
        /// <param name="replaceExisting"></param>
        /// <returns></returns>
        protected IObservable<PageReadResult<TItem>> EnqueueReadObservable(PageReadRequest pageReadRequest, bool replaceExisting = false)
        {
            return Enqueue(() =>
            {
                var alignedStart = 0;
                var alignedEnd = 0;

                if (GetReadRange(pageReadRequest, replaceExisting, ref alignedStart, ref alignedEnd))
                {
                    return _pageReader.ReadRangeObservable(alignedStart, alignedEnd).
                        ObserveOn(_updateScheduler).
                        Do(l => Total = l.Total).
                        Do(l => AddUpdate(l.Offset,l.Items, replaceExisting));
                }
                // no need to remotely read anything, just get on with it
                return Observable.Return(new PageReadResult<TItem>(pageReadRequest.Offset, this.Total, null));
            }
            );
        }

        /// <summary>
        /// Overloaded method used to calculate the range of reads required.
        /// </summary>
        /// <param name="pageReadRequest">Ther read request</param>
        /// <param name="forceReload">Should the data be always read?</param>
        /// <param name="alignedStart">output start of where data should be read from</param>
        /// <param name="alignedEnd">out put end of where data should be read up to</param>
        /// <returns>True if any data should infact be read.</returns>
        protected abstract bool GetReadRange(PageReadRequest pageReadRequest, bool forceReload, ref int alignedStart, ref int alignedEnd);

        /// <summary>
        /// Add or Update update the current cache with the specified list.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="items"></param>
        /// <param name="replaceAll">Indicates if all existing data should be replaced.</param>
        protected abstract void AddUpdate(int offset,List<TItem> items, bool replaceAll = false);

        /// <summary>
        /// Cancel all outstanding enqueued operations.
        /// </summary>
        protected void CancelPending()
        {
            _cancelAllPending.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _changeSetDisposable?.Dispose();
        }

        /// <summary>
        /// Get the size of chunks that will be used.
        /// </summary>
        public int MaxPageSize => this.PagingController.MaxPageSize;

        /// <summary>
        /// Read the data
        /// </summary>
        /// <param name="pageReadRequest"></param>
        /// <returns></returns>
        public IObservable<PageReadResult<TItem>> ReadPageObservable(PageReadRequest pageReadRequest)
        {
            Guard.NotNull(pageReadRequest);

            return EnqueueReadObservable(pageReadRequest);
        }
    }
}
