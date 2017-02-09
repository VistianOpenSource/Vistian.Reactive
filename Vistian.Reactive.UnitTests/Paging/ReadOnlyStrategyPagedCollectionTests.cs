using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Moq;
using Vistian.Reactive.Paging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class ReadOnlyStrategyPagedCollectionTests
    {
        private Mock<ICollectionItemTouchedStrategy> _strategy;
        private TestChangeSetPagedDataProvider<CollectionModel> _changeSetProvider;
        private Mock<IPagedBackingStoreCollection<CollectionModel>> _backingStore;
        private StrategyBasedReadOnlyCollection<CollectionModel> _collection;

        public ReadOnlyStrategyPagedCollectionTests()
        {
            _strategy = new Moq.Mock<ICollectionItemTouchedStrategy>();

            _changeSetProvider = new TestChangeSetPagedDataProvider<CollectionModel>();

            _backingStore = new Moq.Mock<IPagedBackingStoreCollection<CollectionModel>>();
            _backingStore.Setup(p => p.ChangeSetProvider).Returns(_changeSetProvider);
            _backingStore.Setup(p => p.Count).Returns(0);

            _collection = new StrategyBasedReadOnlyCollection<CollectionModel>(_backingStore.Object, _strategy.Object);
        }

        [Fact]
        public void TouchIndexAndCountInvokeTheStrategyAndBackingStoreMethodsTest()
        {
            var testIndex = 1;

            var v = _collection[testIndex];

            var count = _collection.Count;
            
            _strategy.Verify(p => p.CountReferenced(),Times.Exactly(1));
            _strategy.Verify(p => p.IndexReferenced(testIndex), Times.Once);


            _backingStore.Verify(p => p.Count,Times.Once);
            _backingStore.Verify(p => p.Contains(testIndex),Times.Once);

            Assert.Null(v);
            Assert.Equal(0,count);
        }

        [Fact]
        public void AddedItemInBackingRaisesEventTest()
        {            
           NotifyCollectionChangedEventArgs receivedEventArgs = null;
           _collection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs args)
            {
                receivedEventArgs = args;
            };

            var index = 23;
            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new [] {new Change<CollectionModel>(ListChangeReason.Add,item,index), }) );
            
            Assert.NotNull(receivedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Add,receivedEventArgs.Action);
            Assert.Equal(index,receivedEventArgs.NewStartingIndex);
            Assert.Equal(1,receivedEventArgs.NewItems.Count);
            Assert.Same(item,receivedEventArgs.NewItems[0] );
        }


        [Fact]
        public void RemovedItemInBackingRaisesEventTest()
        {
            NotifyCollectionChangedEventArgs receivedEventArgs = null;
            _collection.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs args)
            {
                receivedEventArgs = args;
            };

            const int index = 23;
            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Remove, item, index), }));


            Assert.NotNull(receivedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Remove, receivedEventArgs.Action);
            Assert.Equal(index, receivedEventArgs.OldStartingIndex);
            Assert.Equal(1, receivedEventArgs.OldItems.Count);
            Assert.Same(item, receivedEventArgs.OldItems[0]);
        }


        [Fact]
        public void MovedItemInBackingRaisesEventTest()
        {
            NotifyCollectionChangedEventArgs receivedEventArgs = null;
            _collection.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs args)
            {
                receivedEventArgs = args;
            };

            const int oldIndex = 23;
            const int newIndex = 44;

            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Moved, item,item,newIndex,oldIndex) }));


            Assert.NotNull(receivedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Move, receivedEventArgs.Action);
            Assert.Equal(oldIndex, receivedEventArgs.OldStartingIndex);
            Assert.Equal(newIndex, receivedEventArgs.NewStartingIndex);
            Assert.Equal(1, receivedEventArgs.OldItems.Count);
            Assert.Same(item, receivedEventArgs.OldItems[0]);
            Assert.Equal(1, receivedEventArgs.NewItems.Count);
            Assert.Same(item, receivedEventArgs.NewItems[0]);
        }

        [Fact]
        public void ReplacedItemInBackingRaisesEventTest()
        {
            NotifyCollectionChangedEventArgs receivedEventArgs = null;
            _collection.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs args)
            {
                receivedEventArgs = args;
            };

            const int index = 23;
            var newItem = new CollectionModel();
            var oldItem = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Replace, newItem,oldItem,index,index), }));


            Assert.NotNull(receivedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Replace, receivedEventArgs.Action);
            Assert.Equal(index, receivedEventArgs.OldStartingIndex);
            Assert.Equal(1, receivedEventArgs.OldItems.Count);
            Assert.Same(oldItem, receivedEventArgs.OldItems[0]);
            Assert.Equal(1, receivedEventArgs.NewItems.Count);
            Assert.Same(newItem, receivedEventArgs.NewItems[0]);
        }


        [Fact]
        public void ResetInBackingRaisesEventTest()
        {
            NotifyCollectionChangedEventArgs receivedEventArgs = null;
            _collection.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs args)
            {
                receivedEventArgs = args;
            };

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Clear,new CollectionModel[] {},0),  }));

            Assert.NotNull(receivedEventArgs);
            Assert.Equal(NotifyCollectionChangedAction.Reset, receivedEventArgs.Action);
        }


    }
}
