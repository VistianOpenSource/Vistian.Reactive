using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Vistian.Reactive.Paging;

namespace Vistian.Reactive.UnitTests.Paging
{
    public class TestChangeSetPagedDataProvider<T> : IChangeSetPagedDataProvider<T> where T:class
    {
        private readonly Subject<IChangeSet<T>> _subject = new Subject<IChangeSet<T>>();

        public IObservable<PageReadResult<T>> ReadPageObservable(PageReadRequest pageReadRequest)
        {
            throw new NotImplementedException();
        }

        public int MaxPageSize { get; }
        public int? Total { get; private set; }

        public IObservable<IChangeSet<T>> Changes => _subject.AsObservable();

        public void SetTotal(int total)
        {
            Total = total;
        }

        public void Publish(IChangeSet<T> changeSet)
        {
            _subject.OnNext(changeSet);
        }
    }
}