using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// An enhanced paged data provider that additionally provides data updates through <see cref="IChangeSet"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChangeSetPagedDataProvider<T> : IPagedDataProvider<T>
    {
        /// <summary>
        /// Get an observable for the underlying data changes.
        /// </summary>
        IObservable<IChangeSet<T>> Changes { get; }
    }

}
