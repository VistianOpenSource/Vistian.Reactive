using System;
using System.Reactive.Linq;
using Vistian.Contract;
using Vistian.Reactive.Linq;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Base paging controller,providing retry and error handling mechanisms.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class BasePagingController<TItem> : IPagingController<TItem>
    {
        protected BasePagingController(int maxPageSize)
        {
            Guard.True(() => maxPageSize > 0);
            MaxPageSize = maxPageSize;
        }

        /// <summary>
        /// Attempt to read the page, with optional retry capabilities.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public virtual IObservable<PageReadResult<TItem>> ReadPage(int offset, int take)
        {
            var request = new PageReadRequest() { Offset = offset, Take = Math.Min(take, MaxPageSize) };

            return this.ReadPage(request).RetryX((retryCount, ex) => OnErrorObservable(request, ex));
        }


        /// <summary>
        /// Method that will be overridden for reading page page requests.
        /// </summary>
        /// <param name="pageReadRequest"></param>
        /// <returns></returns>
        protected abstract IObservable<PageReadResult<TItem>> ReadPage(PageReadRequest pageReadRequest);

        /// <summary>
        /// Upon an exception, invoke the appropriate exeception factory.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected virtual IObservable<bool> OnErrorObservable(PageReadRequest request, Exception exception)
        {
            return DefaultErrorObservable(request, exception);
        }

        /// <summary>
        /// Default exception factory, just fail.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected static IObservable<bool> DefaultErrorObservable(PageReadRequest request, Exception exception)
        {
            return Observable.Return(false);
        }

        public int MaxPageSize { get; }
    }
}