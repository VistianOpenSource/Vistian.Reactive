using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Vistian.Reactive.Paging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class CollectionItemTouchedStrategyTests
    {
        private Mock<IPagedBackingStoreCollection<CollectionModel>> _backingStore;
        private CollectionItemTouchedStrategy<CollectionModel> _cits;
        private Mock<IChangeSetPagedDataProvider<CollectionModel>> _csp;

        public CollectionItemTouchedStrategyTests()
        {
            _backingStore = new Moq.Mock<IPagedBackingStoreCollection<CollectionModel>>();
            _backingStore.Setup(p => p.Count).Returns(0);

            _csp = new Moq.Mock<IChangeSetPagedDataProvider<CollectionModel>>();

            _csp.Setup(c => c.MaxPageSize).Returns(25);

            _cits = new CollectionItemTouchedStrategy<CollectionModel>(_backingStore.Object,0,Scheduler.Immediate,Scheduler.Immediate);


            _backingStore.Setup(p => p.ChangeSetProvider).Returns(_csp.Object);

        }
        [Fact]
        public void CountCorrectlyQueuedTest()
        {
            _cits.CountReferenced();

            _csp.Verify(c => c.ReadPageObservable(It.IsAny<PageReadRequest>()));
        }

        [Fact]
        public void IndexCorrectlyQeueuedTest()
        {
            _cits.IndexReferenced(10);


            // need to verify that it correctly requests the right page
            _csp.Verify(c => c.ReadPageObservable(It.Is((PageReadRequest p) => p.Offset==0 && p.Take==25)));
        }

        [Fact]
        public void IndexOffsetCorrectlyQeueuedTest()
        {
            _cits.IndexReferenced(26);

            // need to verify that it correctly requests the right page
            _csp.Verify(c => c.ReadPageObservable(It.Is((PageReadRequest p) => p.Offset == 25 && p.Take == 25)));
        }

        [Fact]
        public void MultipleRequestsBundledToAlignedPagesTest()
        {
            _cits.IndexReferenced(10);
            _cits.IndexReferenced(20);

            // need to verify that it correctly requests the right page
            _csp.Verify(c => c.ReadPageObservable(It.Is((PageReadRequest p) => p.Offset == 0 && p.Take == 25)),Times.Once);
        }

    }
}
