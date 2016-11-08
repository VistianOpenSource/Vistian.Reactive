using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;

namespace Vistian.Reactive.Paging.ChangeSetProviders
{
    /// <summary>
    /// An observable,changeset provider, backed by a <see cref="SourceCache{TObject,TKey}"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <remarks>This code may well be pending and update to Dynamic Data to ensure ordering is maintained.</remarks>
    public class SourceCacheBacked<TItem, TKey> : BaseChangeSetProvider<TItem>, IChangeSetPagedDataProvider<TItem>
    {
        /// <summary>
        /// Our cached list of results
        /// </summary>
        private readonly SourceCache<TItem, TKey> _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagingController"></param>
        /// <param name="keySelector"></param>
        /// <param name="readScheduler"></param>
        /// <param name="updateScheduler"></param>
        public SourceCacheBacked(IPagingController<TItem> pagingController,
            Func<TItem, TKey> keySelector,
            IScheduler readScheduler = null,
            IScheduler updateScheduler = null) : base(pagingController, readScheduler, updateScheduler)
        {
            _cache = new SourceCache<TItem, TKey>(keySelector);
        }

        protected override IObservable<IChangeSet<TItem>> CreateChangeSet()
        {
            return _cache.Connect().RemoveKey();
        }

        /// <summary>
        /// Force a reload of any cached data.
        /// </summary>
        /// <returns></returns>
        public IObservable<Unit> Reload()
        {
            // bin off any pending items...
            CancelPending();

            // build the request
            var pageRequest = new PageReadRequest() { Offset = 0, Take = _cache.Count };

            // and now do the update
            return EnqueueReadObservable(pageRequest, true).Select(_ => Unit.Default);
        }

        /// <summary>
        /// Smartly update the current cache with the specified list.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="items"></param>
        /// <param name="replaceAll"></param>
        protected override void AddUpdate(int offset,List<TItem> items, bool replaceAll = false)
        {
            _cache.Edit((c) =>
            {
                if (replaceAll)
                {
                    c.Clear();
                }
                c.AddOrUpdate(items);
            });
        }

        /// <summary>
        /// Get the read range.
        /// </summary>
        /// <param name="pageReadRequest"></param>
        /// <param name="forceReload"></param>
        /// <param name="alignedStart"></param>
        /// <param name="alignedEnd"></param>
        /// <returns></returns>
        protected override bool GetReadRange(PageReadRequest pageReadRequest, bool forceReload, ref int alignedStart, ref int alignedEnd)
        {
            return this.CalcReadRange(pageReadRequest, _cache.Count, ref alignedStart, ref alignedEnd);
        }


        public static SourceCacheBacked<TItem, TKey> FromObservable(
            Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunksObservable, Func<TItem, TKey> keySelector, int maxPageSize)
        {
            return new SourceCacheBacked<TItem, TKey>(new ObservablePagingController<TItem>(readChunksObservable, maxPageSize: maxPageSize), keySelector);
        }

        public static SourceCacheBacked<TItem, TKey> FromObservable(
            Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunksObservable, Func<TItem, TKey> keySelector, int maxPageSize, Func<PageReadRequest, Exception, IObservable<bool>> exceptionObservable)
        {
            return new SourceCacheBacked<TItem, TKey>(new ObservablePagingController<TItem>(readChunksObservable, maxPageSize: maxPageSize, exceptionObservable: exceptionObservable), keySelector);
        }
    }
}
