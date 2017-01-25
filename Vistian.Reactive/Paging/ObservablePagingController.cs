using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Wrapper class for observable paged readers, implementing <see cref="IPagingController{TItem}"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ObservablePagingController<TItem> : BasePagingController<TItem>
    {
        private readonly Func<PageReadRequest, IObservable<PageReadResult<TItem>>> _readChunkObservable;
        private readonly Func<PageReadRequest, Exception, IObservable<bool>> _exceptionObservable;

        public ObservablePagingController(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunkObservable,
                                            Func<PageReadRequest, Exception, IObservable<bool>> exceptionObservable = null,
                                            int maxPageSize = 10):base(maxPageSize)
        {
            Guard.NotNull(readChunkObservable);

            _readChunkObservable = readChunkObservable;
            _exceptionObservable = exceptionObservable;
        }

        /// <summary>
        /// Upon an exception, invoke the appropriate exeception factory.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override IObservable<bool> OnErrorObservable(PageReadRequest request, Exception exception)
        {
            return _exceptionObservable != null ? _exceptionObservable(request, exception) : DefaultErrorObservable(request,exception);
        }

        protected override IObservable<PageReadResult<TItem>> ReadPage(PageReadRequest pageReadRequest)
        {
            return _readChunkObservable(pageReadRequest);
        }
    }
}
