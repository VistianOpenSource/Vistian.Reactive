using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Linq;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Wrapper class for observable paged readers, implementing <see cref="IPagingController{TItem}"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ObservablePagingController<TItem> : IPagingController<TItem>
    {
        private readonly Func<PageReadRequest, IObservable<PageReadResult<TItem>>> _readChunkObservable;
        private readonly Func<PageReadRequest, Exception, IObservable<bool>> _exceptionObservable;

        public ObservablePagingController(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunkObservable,
                                            Func<PageReadRequest, Exception, IObservable<bool>> exceptionObservable = null,
                                            int maxPageSize = 10)
        {
            Guard.NotNull(readChunkObservable);
            Guard.True(() => maxPageSize > 0);

            MaxPageSize = maxPageSize;
            _readChunkObservable = readChunkObservable;
            _exceptionObservable = exceptionObservable ?? DefaultErrorObservable;
        }

        /// <summary>
        /// Attempt to read the page, with optional retry capabilities.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public virtual IObservable<PageReadResult<TItem>> ReadPage(int offset, int take)
        {
            var request = new PageReadRequest() { Offset = offset, Take = Math.Min(take,MaxPageSize) };

            return _readChunkObservable(request).RetryX((retryCount, ex) => OnErrorObservable(request, ex));
        }

        /// <summary>
        /// Upon an exception, invoke the appropriate exeception factory.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected IObservable<bool> OnErrorObservable(PageReadRequest request, Exception exception)
        {
            return _exceptionObservable(request, exception);
        }

        /// <summary>
        /// Default exception factory, just fail.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static IObservable<bool> DefaultErrorObservable(PageReadRequest request, Exception exception)
        {
            return Observable.Return(false);
        }
        /// <summary>
        /// Get the maximum page size
        /// </summary>
        public int MaxPageSize { get; }
    }
}
