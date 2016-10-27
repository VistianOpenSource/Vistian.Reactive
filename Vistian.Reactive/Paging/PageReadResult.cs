using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// The result of a paged read
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PageReadResult<T>
    {
        public PageReadResult(int offset, int? total, List<T> items)
        {
            Offset = offset;
            Total = total;
            Items = items;
            AmountRead = items?.Count ?? 0;
        }

        /// <summary>
        /// Get the amount actually read
        /// </summary>
        public int AmountRead { get; set; }

        /// <summary>
        /// Get the total amount of data available.
        /// </summary>
        /// <remarks>
        /// No value implies not known.
        /// Int32.MaxValue implies keep going
        /// A value less than Int32.MaxValue is total amount of data available
        /// </remarks>
        /// 
        public int? Total { get; set; }

        /// <summary>
        /// Get the offset from where this data was read from
        /// </summary>
        public int Offset { get; set; }


        /// <summary>
        /// The items that were read.
        /// </summary>
        public List<T> Items { get; }
    }
}
