using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Vistian.Reactive.Paging;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class SimpleBackingStoreCollectionTests
    {
        private TestChangeSetPagedDataProvider<CollectionModel> _changeSetProvider;
        private SimpleBackingStoreCollection<CollectionModel> _store;

        // we need to verify that change sets do correctly update the backing stores contents...
        // count and total go to the correct places

        public SimpleBackingStoreCollectionTests()
        {
            _changeSetProvider = new TestChangeSetPagedDataProvider<CollectionModel>();

            _store = new SimpleBackingStoreCollection<CollectionModel>(_changeSetProvider);
        }


        [Fact]
        public void InitialStateTest()
        {
            _changeSetProvider.SetTotal(23);

            Assert.Equal(23,_store.Total.Value);
            Assert.Equal(0,_store.Count);
        }

        [Fact]
        public void AddTest()
        {
            const int index = 0;
            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Add, item, index), }));

            Assert.Equal(1,_store.Count);
            Assert.Same(item,_store.Items[0]);
            Assert.Same(item,_store.Get(0));
        }

        [Fact]
        public void RemoveTest()
        {
            const int index = 0;
            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Add, item, index), }));

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Remove, item, index), }));
            Assert.Equal(0, _store.Count);
        }

        [Fact]
        public void ResetTest()
        {
            const int index = 0;
            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Add, item, index), }));

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Clear, new CollectionModel[] {item }, 0), }));
            Assert.Equal(0, _store.Count);
        }


        [Fact]
        public void ReplaceTest()
        {
            const int index = 0;
            var item = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Add, item, index), }));

            var newItem = new CollectionModel();

            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Replace, newItem, item, index, index), }));

            Assert.Equal(1, _store.Count);
            Assert.Same(newItem,_store.Items[0]);
            Assert.Same(newItem,_store.Get(0));
        }


        [Fact]
        public void ContainsTest()
        {
            var contains = _store.Contains(0);

            Assert.False(contains);

            const int index = 0;
            var item = new CollectionModel();
            _changeSetProvider.Publish(new ChangeSet<CollectionModel>(new[] { new Change<CollectionModel>(ListChangeReason.Add, item, index), }));

            contains = _store.Contains(0);
            Assert.True(contains);
        }

        [Fact]
        public void TotalsTest()
        {
            _changeSetProvider.SetTotal(121);
            var total = _store.Total;

            Assert.Equal(121,total);

            _changeSetProvider.SetTotal(200);

            total = _store.Total;

            Assert.Equal(200, total);

        }

    }
}
