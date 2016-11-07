using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Vistian.Reactive.ReactiveUI;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Paged ChangeSet Provider which just passes through changes, the assumption being the underlying paging controller is a completely read only source.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class PagedObservablePassthroughChangeSetProvider<TItem> : BaseChangeSetProvider<TItem>, IChangeSetPagedDataProvider<TItem>
    {
        public PagedObservablePassthroughChangeSetProvider(IPagingController<TItem> pagingController, IScheduler readScheduler = null, IScheduler updateScheduler = null) : base(pagingController, readScheduler, updateScheduler)
        {
        }

        private int _offset = 0;


        private Subject<IChangeSet<TItem>> _changeSetSubject = new Subject<IChangeSet<TItem>>();
        /// <summary>
        /// Create a change set
        /// </summary>
        /// <returns></returns>
        protected override IObservable<IChangeSet<TItem>> CreateChangeSet()
        {
            return _changeSetSubject.AsObservable();
        }

        /// <summary>
        /// Calculate the read range.
        /// </summary>
        /// <param name="pageReadRequest"></param>
        /// <param name="forceReload"></param>
        /// <param name="alignedStart"></param>
        /// <param name="alignedEnd"></param>
        /// <returns></returns>
        /// <remarks>
        /// Be considerable to a forced reload to ensure the complete data is re-read.</remarks>
        protected override bool GetReadRange(PageReadRequest pageReadRequest, bool forceReload, ref int alignedStart, ref int alignedEnd)
        {
            return this.CalcReadRange(pageReadRequest, forceReload ? 0 : _offset, ref alignedStart, ref alignedEnd);
        }

        /// <summary>
        /// Apply retrieved data to the underlying <see cref="KeyedReactiveList{TItem,TKey}"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="items"></param>
        /// <param name="replaceAll"></param>
        protected override void AddUpdate(int offset, List<TItem> items, bool replaceAll = false)
        {
            if (replaceAll)
            {

                // need to generate updates from zero.
                //_changeSetSubject.OnNext(new ChangeSet<TItem>(new Change<TItem>(ListChangeReason.Clear)));
                //_list.SmartUpdate(items);//

                _offset = items.Count;
            }
            else
            {

                var trulyNewContentOffset = 0;

                if (offset < _offset)
                {
                    trulyNewContentOffset = _offset;

                    var startOffset = offset;
                    // we need to do a replacement for certain items and then do an add range...
                    for (var index = 0; index < _offset - startOffset; ++index)
                    {
                        // need to generate replace changesets here...

                  //      _list[startOffset + index] = items[index];
                    }
                }

                // need to generate add change sets here
                //_list.AddRange(items.GetRange(trulyNewContentOffset, offset + items.Count - _offset));

                _offset = Math.Max(_offset, offset + items.Count);
            }
        }

        /// <summary>
        /// For a reload of the existing, underlying data.
        /// </summary>
        public void Reload()
        {
            // cancel all pending reads, this should improve performance
            this.CancelPending();

            // build the request
             var pageRequest = new PageReadRequest() { Offset = 0, Take = _offset };

            // and now do the update
            EnqueueReadObservable(pageRequest, true);
        }
    }

}
