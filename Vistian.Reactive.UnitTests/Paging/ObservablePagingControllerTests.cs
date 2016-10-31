using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Paging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class ObservablePagingControllerTests
    {
        private ObservablePagingController<CollectionModel> _pagingController;
        private long _invokeCount;
        // so what tests do we need ?

        // verify that the read request goes through...
        // exception raized
        // retry occurs when exception told its okay
        // max size is passed through correctly 



        [Fact]
        public void ReadRequestPassedThroughTest()
        {
            PageReadRequest readRequest = default(PageReadRequest);

            Func<PageReadRequest, IObservable<PageReadResult<CollectionModel>>> func = (r) =>
            {
                readRequest = r;
                return Observable.Return(default(PageReadResult<CollectionModel>));
            };

            _pagingController = new ObservablePagingController<CollectionModel>(func);

            var offset = 10;
            var take = 10;

            var read = _pagingController.ReadPage(offset, take);

            Assert.Equal(offset,readRequest.Offset);
            Assert.Equal(take,readRequest.Take);
        }

        [Fact]
        public void ExceptionPassedToExceptionHandlerTest()
        {
            var exception = new InvalidOperationException();

            var readResult = new Subject<PageReadResult<CollectionModel>>();
            readResult.OnError(exception);

            Func<PageReadRequest, IObservable<PageReadResult<CollectionModel>>> func = (r) => readResult;

            var exceptionRaised = default(Exception);
            var errorRequest= default(PageReadRequest);
            var raised = false;

            Func<PageReadRequest, Exception, IObservable<bool>> exc = (pr, e) =>
            {
                raised = true;
                exceptionRaised = e;
                errorRequest = pr;
                return Observable.Return(false);
            };

            _pagingController = new ObservablePagingController<CollectionModel>(func,exc);

            var offset = 10;
            var take = 10;

            PageReadResult<CollectionModel> readValue = default(PageReadResult<CollectionModel>);

            var read = _pagingController.ReadPage(offset, take).Subscribe(v => readValue = v);

            Assert.True(raised);
            Assert.Same(exception,exceptionRaised);
            Assert.Equal(offset,errorRequest.Offset);
            Assert.Equal(take, errorRequest.Take);
        }



        /// <summary>
        /// Test request observable used for generating exceptions and results
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IObservable<PageReadResult<CollectionModel>> GetData(PageReadRequest request,Exception exception,PageReadResult<CollectionModel> result)
        {
            _invokeCount = 0;

            return Observable.Create<PageReadResult<CollectionModel>>(o =>
            {
                ++_invokeCount;

                if (_invokeCount == 1)
                {
                    o.OnError(exception);
                }
                else
                {
                    o.OnNext(result);
                    o.OnCompleted();
                }

                return Disposable.Empty;
            });
        }

        [Fact]
        public void ExceptionPassedAndRetryCorrectlyOccursTest()
        {
            var exception = new InvalidOperationException();


            var exceptionRaised = default(Exception);
            var errorRequest = default(PageReadRequest);
            var raised = false;


            var result = new PageReadResult<CollectionModel>(1,null,null);

            Func<PageReadRequest, Exception, IObservable<bool>> exc = (pr, e) =>
            {
                raised = true;
                exceptionRaised = e;
                errorRequest = pr;
                return Observable.Return(true);
            };

            Func<PageReadRequest, IObservable<PageReadResult<CollectionModel>>> func = (r) => this.GetData(r,exception,result);

            _pagingController = new ObservablePagingController<CollectionModel>(func, exc);

            var offset = 10;
            var take = 10;

            PageReadResult<CollectionModel> readValue = default(PageReadResult<CollectionModel>);

            var read = _pagingController.ReadPage(offset, take).Subscribe(v => readValue=v);

            Assert.True(raised);
            Assert.Same(exception, exceptionRaised);
            Assert.Equal(offset, errorRequest.Offset);
            Assert.Equal(take, errorRequest.Take);

            Assert.Equal(2,_invokeCount);

            Assert.Same(result,readValue);
        }

        [Fact]
        public void RequestsLargerThanMaxSizeAreTrimmedTest()
        {
            const int maxPageSize = 5;

            PageReadRequest readRequest = default(PageReadRequest);

            Func<PageReadRequest, IObservable<PageReadResult<CollectionModel>>> func = (r) =>
            {
                readRequest = r;
                return Observable.Return(default(PageReadResult<CollectionModel>));
            };

            _pagingController = new ObservablePagingController<CollectionModel>(func,maxPageSize:maxPageSize);

            var offset = 10;
            var take = 10;

            var read = _pagingController.ReadPage(offset, take);


            Assert.Equal(offset, readRequest.Offset);
            Assert.Equal( maxPageSize,readRequest.Take);

        }

    }
}
