using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Intermedia class indicating a current reads result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ReadInProgress<T>
    {
        /// <summary>
        /// How much was read
        /// </summary>
        public int AmountRead { get; set; }

        /// <summary>
        /// The offset from where the data was read
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The items read
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// The total amount of data available
        /// </summary>
        public int? Total { get; set; }

        public ReadInProgress()
        {
            Items = new List<T>();
        }
    }
}
