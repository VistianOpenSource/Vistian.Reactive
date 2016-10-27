using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Simple strategy used in the collection wrappers to handle referencing of count and items.
    /// </summary>
    public interface ICollectionItemTouchedStrategy
    {
        /// <summary>
        /// Record a specific index in the collection has been publically referenced
        /// </summary>
        /// <param name="index"></param>
        void IndexReferenced(int index);

        /// <summary>
        /// Record that the count of the collection has been publically referenced.
        /// </summary>
        void CountReferenced();
    }
}
