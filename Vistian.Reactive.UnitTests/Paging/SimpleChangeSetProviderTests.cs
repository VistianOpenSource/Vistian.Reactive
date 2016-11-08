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
using Vistian.Reactive.Paging.ChangeSetProviders;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class SimpleChangeSetProviderTests
    {
        private readonly Mock<IPagingController<CollectionModel>> _pagingController;
        private readonly Simple<CollectionModel> _provider;
        private readonly List<CollectionModel> _itemFirstSet;
        private readonly List<CollectionModel> _itemAppendFirstSet;

        public const int DefaultPageSize = 5;

        public SimpleChangeSetProviderTests()
        {
            var scheduler = Scheduler.Immediate;

            _pagingController = new Mock<IPagingController<CollectionModel>>();

            _pagingController.Setup(p => p.MaxPageSize).Returns(DefaultPageSize);

            _provider = new Simple<CollectionModel>(_pagingController.Object, scheduler, scheduler);

            _itemFirstSet = new List<CollectionModel>()
            {
                new CollectionModel() {Key = "Key#1", Instance = "1"},
                new CollectionModel() {Key = "Key#2", Instance = "1"}
            };


            _itemAppendFirstSet = new List<CollectionModel>()
            {
                new CollectionModel() {Key = "Key#3", Instance = "1"},
            };
        }

         [Fact]
        public void AdditionsCreateCorrectChangeSets()
         {
            var changes = new List<IChangeSet<CollectionModel>>();

             _pagingController.Setup(p => p.ReadPage(0, DefaultPageSize))
                 .Returns(() => Observable.Return(new PageReadResult<CollectionModel>(0,null, _itemFirstSet))).Verifiable();

             _provider.Changes.Subscribe(c => changes.Add(c));

            _provider.ReadPageObservable(new PageReadRequest() { Offset = 0, Take = DefaultPageSize }).Subscribe();

            var changeSet = changes[0];

            var index = 0;
            Assert.Equal(_itemFirstSet.Count, changeSet.TotalChanges);
            foreach (var change in changeSet)
            {
                Assert.Equal(ListChangeReason.Add, change.Reason);
                var item = change.Item.Current;

                Assert.Same(_itemFirstSet[index], item);
                Assert.Equal(index, change.Item.CurrentIndex);
                ++index;
            }

            _pagingController.Verify();

        }

        [Fact]
        public void AppendsCreatesNewAdditionsTest()
        {
            var changes = new List<IChangeSet<CollectionModel>>();

            _pagingController.Setup(p => p.ReadPage(0, _itemFirstSet.Count))
                .Returns(() => Observable.Return(new PageReadResult<CollectionModel>(0, null, _itemFirstSet))).Verifiable();

            _pagingController.Setup(p => p.ReadPage(_itemFirstSet.Count, _itemAppendFirstSet.Count))
                .Returns(() => Observable.Return(new PageReadResult<CollectionModel>(0, null, _itemAppendFirstSet))).Verifiable();

            _provider.Changes.Subscribe(c => changes.Add(c));

            _provider.ReadPageObservable(new PageReadRequest() { Offset = 0, Take = _itemFirstSet.Count }).Subscribe();

            // clear down changes list, not interested in verifying the additions, another test has done this.

            changes.Clear();

            _provider.ReadPageObservable(new PageReadRequest() { Offset = _itemFirstSet.Count, Take = _itemAppendFirstSet.Count }).Subscribe();

            Assert.Equal(1, changes.Count);

            var changeSet = changes[0];

            var index = 0;

            // verify the addition(s)

            foreach (var change in changeSet)
            {
                Assert.Equal(ListChangeReason.Add, change.Reason);
                var item = change.Item.Current;

                Assert.Same(_itemAppendFirstSet[index], item);
                Assert.Equal(index+_itemFirstSet.Count, change.Item.CurrentIndex);
                ++index;
            }

            _pagingController.Verify();
        }

        [Fact]
        public void ReloadCreatesClearAndAdditionsTest()
        {
            var changes = new List<IChangeSet<CollectionModel>>();

            _pagingController.Setup(p => p.ReadPage(0, _itemFirstSet.Count))
                .Returns(() => Observable.Return(new PageReadResult<CollectionModel>(0, null, _itemFirstSet))).Verifiable();

            _provider.Changes.Subscribe(c => changes.Add(c));

            _provider.ReadPageObservable(new PageReadRequest() { Offset = 0, Take = _itemFirstSet.Count }).Subscribe();

            // now clear, since not interested in whats happened so far
            changes.Clear();

            _provider.Reload();
            
            Assert.Equal(1,changes.Count);

            var first = changes[0].First();

            Assert.Equal(ListChangeReason.Clear, first.Reason);

            var index = 0;

            foreach (var change in changes[0].Select(c => c).Skip(1).ToList())
            {
                Assert.Equal(ListChangeReason.Add,change.Reason);

                var item = change.Item.Current;

                Assert.Same(_itemFirstSet[index],item);
                Assert.Equal(index,change.Item.CurrentIndex);

                ++index;
            }

        }
    }

}
