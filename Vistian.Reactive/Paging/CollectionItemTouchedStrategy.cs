using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Helper method to instigate requests for pages based upon recorded touches.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class CollectionItemTouchedStrategy<TItem> : ICollectionItemTouchedStrategy
    {
        private readonly IPagedBackingStoreCollection<TItem> _pagedBackingStoreCollection;

        /// <summary>
        /// How long we allow to bundle together requests
        /// </summary>
        private readonly int _bundleTicks;

        /// <summary>
        /// Scheduler upon which touches are recorded
        /// </summary>
        private readonly IScheduler _uiScheduler;

        /// <summary>
        /// Scheduler on which the reads will be kicked off on.
        /// </summary>
        private readonly IScheduler _readScheduler;

        /// <summary>
        /// Those indexes which have been touched.
        /// </summary>
        private HashSet<int> _indexesTouched;

        /// <summary>
        /// Records first time anything has been referenced, used for generating an initial read.
        /// </summary>
        private bool _firstTime = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagedBackingStoreCollection"></param>
        /// <param name="bundleTicks">The number of ticks to wait before kicking of reads of data</param>
        /// <param name="uiScheduler">The scheduler upon which the reads are kicked off</param>
        /// <param name="readScheduler">The scheduler the reads are performed upon</param>
        public CollectionItemTouchedStrategy(IPagedBackingStoreCollection<TItem> pagedBackingStoreCollection,
            int bundleTicks = 0,
            IScheduler uiScheduler = null,
            IScheduler readScheduler = null)
        {
            _pagedBackingStoreCollection = pagedBackingStoreCollection;
            _bundleTicks = bundleTicks;
            _uiScheduler = uiScheduler ?? RxApp.MainThreadScheduler;
            _readScheduler = readScheduler ?? RxApp.TaskpoolScheduler;
        }

        /// <summary>
        /// The index has been touched.
        /// </summary>
        /// <param name="index"></param>
        public void IndexReferenced(int index)
        {
            _firstTime = false;

            ProcessTouch(index);
        }


        /// <summary>
        /// The count on the collection has been referenced.
        /// </summary>
        public void CountReferenced()
        {
            if (!_firstTime) return;

            _firstTime = false;

            // generate a 'fake' touch for the first possible index to kick off any reads that could be relevant
            ProcessTouch(0);
        }


        /// <summary>
        /// Process a touch for a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>
        /// Attempt to bundle up the touches we have made for a more optimal set of requets.
        /// requests Are scheduled onto the UI to ensure that we don't get any list update
        /// issues.
        /// </remarks>
        private void ProcessTouch(int index)
        {
            if (_indexesTouched == null)
            {
                _indexesTouched = new HashSet<int> {index};

                _uiScheduler.Schedule(index, TimeSpan.FromTicks(_bundleTicks), (s, i) => this.InternalTouched());
            }
            else
            {
                _indexesTouched.Add(index);
            }
        }

        /// <summary>
        /// Schedule the paging operations, ensuring we optimally 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// There is an IMPLICIT assumption here that this runs on the same thread as that 
        /// populating the requested</remarks>
        private IDisposable InternalTouched()
        {
            var queue = _indexesTouched;

            // clear down old items, this will allow for others to be queued up...
            _indexesTouched = null;

            return CreateOffsets(queue).
                ObserveOn(_readScheduler).
                Select(o => _pagedBackingStoreCollection.ChangeSetProvider.CreateReadRequest<TItem>(o)).
                Select(pr => _pagedBackingStoreCollection.ChangeSetProvider.ReadPageObservable(pr)).
                Subscribe();
        }

        /// <summary>
        /// Create those indexes which are going to be requested.
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        protected virtual IObservable<int> CreateOffsets(HashSet<int> indexes)
        {
            var maxPageSize = _pagedBackingStoreCollection.ChangeSetProvider.MaxPageSize;

            var offsetsToRead = new HashSet<int>();

            foreach (var index in indexes)
            {
                // work out the aligned base that this falls into
                // we potentially align upwards to force the read of the page following, ensuring a contiguous run.
                var alignedOffset = (maxPageSize == int.MaxValue) ? index : ((int)Math.Round(((float)index) / maxPageSize)) * maxPageSize;

                // if don't have a total, or we are below the total amount then enqueue the read...
                if (!_pagedBackingStoreCollection.Total.HasValue || alignedOffset < _pagedBackingStoreCollection.Total)
                {
                    offsetsToRead.Add(alignedOffset);
                }
            }

            return offsetsToRead.ToObservable();
        }
    }
}
