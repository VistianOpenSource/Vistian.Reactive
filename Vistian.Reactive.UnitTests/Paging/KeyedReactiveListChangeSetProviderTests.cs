using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Vistian.Reactive.Paging;
using Vistian.Reactive.Paging.ChangeSetProviders;
using Vistian.Reactive.ReactiveUI;
using Xunit;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class KeyedReactiveListChangeSetProviderTests
    {
        private readonly Mock<IPagingController<CollectionModel>> _pagingController;
        private KeyedReactiveListBacked<CollectionModel, string> _provider;
        private List<CollectionModel> _itemFirstSet;
        private List<CollectionModel> _itemSecondSet;
        private List<CollectionModel> _itemReplacementFirstSet;
        private List<CollectionModel> _itemReplacementSecondSet;

        public const int DefaultPageSize = 2;

        public KeyedReactiveListChangeSetProviderTests()
        {
            var scheduler = Scheduler.Immediate;

            _pagingController = new Mock<IPagingController<CollectionModel>>();

            _pagingController.Setup(p => p.MaxPageSize).Returns(DefaultPageSize);

            _provider = new KeyedReactiveListBacked<CollectionModel,string>(_pagingController.Object,(m) => m.Key,scheduler,scheduler);

            _itemFirstSet = new List<CollectionModel>()
            {
                new CollectionModel() {Key = "Key#1",Instance = "1"},
                new CollectionModel() {Key = "Key#2",Instance="1"}
            };

            _itemSecondSet = new List<CollectionModel>()
            {
                new CollectionModel() {Key = "Key#3",Instance="1"},
                new CollectionModel() {Key = "Key#4",Instance="1"}
            };

            _itemReplacementFirstSet = new List<CollectionModel>()
            {
                new CollectionModel() {Key = "Key#1",Instance = "2"},
                new CollectionModel() {Key = "Key#2",Instance="2"}
            };

            _itemReplacementSecondSet = new List<CollectionModel>()
            {
                new CollectionModel() {Key = "Key#3",Instance="2"},
            };
        }


        [Fact]
        public void ListAddedToCorrectlyTest()
        {
            _pagingController.Setup(p => p.ReadPage(0,DefaultPageSize)).Returns(Observable.Return(new PageReadResult<CollectionModel>(0,_itemFirstSet.Count,_itemFirstSet)));

            // already tested read returns, just need to check the backing list is correct
            _provider.ReadPageObservable(new PageReadRequest() {Offset = 0, Take = DefaultPageSize}).Subscribe();

            Assert.Same(_itemFirstSet[0],_provider.Items[0]);
            Assert.Same(_itemFirstSet[1], _provider.Items[1]);
            Assert.Equal(_itemFirstSet.Count,_provider.Items.Count);
        }

        [Fact]
        public void ListCorrectlyAppendsToExistingTest()
        {
            _pagingController.Setup(p => p.ReadPage(0, DefaultPageSize)).Returns(Observable.Return(new PageReadResult<CollectionModel>(0, null, _itemFirstSet)));
            _pagingController.Setup(p => p.ReadPage(_itemFirstSet.Count, DefaultPageSize)).Returns(Observable.Return(new PageReadResult<CollectionModel>(_itemFirstSet.Count, null, _itemSecondSet)));

            // already tested read returns, just need to check the backing list is correct
            _provider.ReadPageObservable(new PageReadRequest() { Offset = 0, Take = DefaultPageSize }).Subscribe();
            _provider.ReadPageObservable(new PageReadRequest() { Offset = _itemSecondSet.Count, Take = DefaultPageSize }).Subscribe();


            Assert.Same(_itemFirstSet[0], _provider.Items[0]);
            Assert.Same(_itemFirstSet[1], _provider.Items[1]);
            Assert.Same(_itemSecondSet[0], _provider.Items[2]);
            Assert.Same(_itemSecondSet[1], _provider.Items[3]);

            Assert.Equal(_itemFirstSet.Count+_itemSecondSet.Count, _provider.Items.Count);
        }

        [Fact]
        public void ListCorrectlyDealsWithOutOfExistingRangeReadsTest()
        {
            _pagingController.Setup(p => p.ReadPage(0, _itemFirstSet.Count)).Returns(Observable.Return(new PageReadResult<CollectionModel>(0,null, _itemFirstSet)));
            _pagingController.Setup(p => p.ReadPage(_itemFirstSet.Count,_itemSecondSet.Count)).Returns(Observable.Return(new PageReadResult<CollectionModel>(_itemFirstSet.Count, null, _itemSecondSet)));

            // already tested read returns, just need to check the backing list is correct
            _provider.ReadPageObservable(new PageReadRequest() { Offset = _itemSecondSet.Count, Take = DefaultPageSize }).Subscribe();

            Assert.Same(_itemFirstSet[0], _provider.Items[0]);
            Assert.Same(_itemFirstSet[1], _provider.Items[1]);
            Assert.Same(_itemSecondSet[0], _provider.Items[2]);
            Assert.Same(_itemSecondSet[1], _provider.Items[3]);

            Assert.Equal(_itemFirstSet.Count + _itemSecondSet.Count, _provider.Items.Count);
        }


        [Fact]
        public void ReloadReplacesDataCorrectlyTest()
        {
            // setup for consequetive invocations to return differing set - explicit assumption our code under test doesn't call more than once !
            var x= _pagingController.SetupSequence(p => p.ReadPage(0, DefaultPageSize)).Returns(Observable.Return(new PageReadResult<CollectionModel>(0, null, _itemFirstSet))).Returns(Observable.Return(new PageReadResult<CollectionModel>(0, null, _itemReplacementFirstSet)));
            _pagingController.SetupSequence(p => p.ReadPage(DefaultPageSize, DefaultPageSize)).Returns(Observable.Return(new PageReadResult<CollectionModel>(_itemFirstSet.Count, null, _itemSecondSet))).Returns(Observable.Return(new PageReadResult<CollectionModel>(_itemReplacementFirstSet.Count, null, _itemReplacementSecondSet)));

            // already tested read returns, just need to check the backing list is correct
            _provider.ReadPageObservable(new PageReadRequest() { Offset = _itemSecondSet.Count, Take = DefaultPageSize }).Subscribe();

            _provider.Reload();

            Assert.Same(_itemReplacementFirstSet[0], _provider.Items[0]);
            Assert.Same(_itemReplacementFirstSet[1], _provider.Items[1]);
            Assert.Same(_itemReplacementSecondSet[0], _provider.Items[2]);


            Assert.Equal(_itemReplacementFirstSet.Count + _itemReplacementSecondSet.Count, _provider.Items.Count);


        }

    }
}
