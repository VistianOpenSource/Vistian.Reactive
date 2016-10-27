using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Makes continguous requests from <see cref="IPagingController{TSource}"/> based upon supported pageSize.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class PageReader<TSource>
    {
        private readonly IPagingController<TSource> _pagingController;

        /// <summary>
        /// The scheduler we enqueue the reads from the paging controller on
        /// </summary>
        private readonly IScheduler _readScheduler;

        public PageReader(IPagingController<TSource> pagingController, IScheduler readScheduler = null)
        {
            _pagingController = pagingController;
            _readScheduler = readScheduler;
        }

        public IObservable<PageReadResult<TSource>> ReadRangeObservable(int alignedStart, int alignedEnd)
        {
            var pageSize = _pagingController.MaxPageSize;

            // generate a list of the offsets we need to load...
            var generator = pageSize == int.MaxValue ? Observable.Return(alignedStart, _readScheduler) :
                Observable.Generate(alignedStart, v => v < alignedEnd, value => value + pageSize, v => v, _readScheduler);

            var observable = generator.
                Select(requestedOffset => Observable.Defer(() => _pagingController.ReadPage(requestedOffset, pageSize))).   // read a page
                Concat().                                                                                                   // ensure one at a time of prior operation
                Scan(new ReadInProgress<TSource>(), (acc, result) =>                                                        // accumulate reads into a single result
                {
                    acc.AmountRead = result.AmountRead;
                    acc.Offset = result.Offset;
                    acc.Items.AddRange(result.Items);
                    acc.Total = result.Total;
                    return acc;
                }).
                SkipWhile(r => (r.AmountRead == pageSize) && (r.Offset + r.AmountRead < alignedEnd)).                         // keep going whilst more to come
                TakeLast(1).                                                                                                // take the single result
                Select(r => new PageReadResult<TSource>(alignedStart, r.Total, r.Items));                                   // shape the final result

            return observable;
        }
    }
}
