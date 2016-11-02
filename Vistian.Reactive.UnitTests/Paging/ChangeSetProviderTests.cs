using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Moq;
using Vistian.Reactive.Paging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class ChangeSetProviderTests
    {
        private Mock<IPagingController<CollectionModel>> _pagingController;
        private TestBaseChangeSetProvider<CollectionModel> _changeSetController;
        private const int DefaultPageSize = 25;
        // so what do we need to test


        public ChangeSetProviderTests()
        {
            var scheduler = Scheduler.Immediate;

            _pagingController = new Mock<IPagingController<CollectionModel>>();
            _changeSetController = new TestBaseChangeSetProvider<CollectionModel>(_pagingController.Object,readScheduler:scheduler,updateScheduler:scheduler);

            _pagingController.Setup(p => p.MaxPageSize).Returns(DefaultPageSize);
        }

        [Fact]
        public void BaseControllerSetupCorrectyTest()
        {
            Assert.Equal(DefaultPageSize,_changeSetController.MaxPageSize);
            
            _pagingController.Verify(p => p.MaxPageSize,Times.Once);
        }

        [Fact]
        public void NormalReadFunctionsCorrectlyTest()
        {
            var offset = 0;
            var take = 10;
            var total = 99;

            _changeSetController.SetAlignedValues(offset,offset+take,true);

            var requestRequest = new PageReadRequest() {Offset = offset, Take = take};

            var result = default(PageReadResult<CollectionModel>);

            var items = new List<CollectionModel>() {new CollectionModel(), new CollectionModel()};

            var res = new PageReadResult<CollectionModel>(offset,5, items);
            res.Total = total;

            var pcReturn = Observable.Return(res);
            _pagingController.Setup(p => p.ReadPage(offset, take)).Returns(pcReturn);

            var readPage = _changeSetController.ReadPageObservable(requestRequest).Subscribe(r => result=r);

            Assert.Equal(res.Offset,result.Offset);
            Assert.Equal(res.Total, result.Total);
            Assert.Equal(res.Items[0],result.Items[0]);
            Assert.Equal(res.Items[1], result.Items[1]);
            Assert.Equal(total,_changeSetController.Total);

            _pagingController.Verify(p => p.ReadPage(offset,take),Times.Once);
        }

        [Fact]
        public void NothingToReadMeansNoPagingRequestTest()
        {
            var offset = 0;
            var take = 10;
            var total = 99;

            _changeSetController.SetAlignedValues(offset, offset,false);

            var requestRequest = new PageReadRequest() { Offset = offset, Take = take };

            var result = default(PageReadResult<CollectionModel>);


            var items = new List<CollectionModel>() { new CollectionModel(), new CollectionModel() };

            var res = new PageReadResult<CollectionModel>(offset, 5, items) {Total = total};

            var pcReturn = Observable.Return(res);
            _pagingController.Setup(p => p.ReadPage(offset, take)).Returns(pcReturn);

            var busyStates = new List<bool>();

            _changeSetController.BusyObservable.Subscribe(b => busyStates.Add(b));

            var readPage = _changeSetController.ReadPageObservable(requestRequest).Subscribe(r => result = r);

            Assert.Equal(res.Offset, result.Offset);
            Assert.Equal(0, result.AmountRead);
            Assert.Null(result.Total);          // we don't know the total since never read..
            Assert.Equal(3,busyStates.Count);
            Assert.False(busyStates[0]);
            Assert.True(busyStates[1]);
            Assert.False(busyStates[2]);

            _pagingController.Verify(p => p.ReadPage(offset, take), Times.Never);

        }
    }


    public class TestBaseChangeSetProvider<T> : BaseChangeSetProvider<T>
    {
        private int _start;
        private int _end;
        private bool _isTrue;

        public TestBaseChangeSetProvider(IPagingController<T> pagingController, IScheduler readScheduler = null, IScheduler updateScheduler = null) : base(pagingController, readScheduler, updateScheduler)
        {
        }

        protected override IObservable<IChangeSet<T>> CreateChangeSet()
        {
            throw new NotImplementedException();
        }


        public void SetAlignedValues(int start, int end,bool isTrue)
        {
            _start = start;
            _end = end;
            _isTrue = isTrue;
        }

        protected override bool GetReadRange(PageReadRequest pageReadRequest, bool forceReload, ref int alignedStart, ref int alignedEnd)
        {
            alignedStart = _start;
            alignedEnd = _end;
            return _isTrue;
        }

        protected override void AddUpdate(int offset,List<T> items, bool replaceAll = false)
        {
        }
    }

    
}
