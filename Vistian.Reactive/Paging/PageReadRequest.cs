using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Represents a page read request.
    /// </summary>
    public struct PageReadRequest
    {
        /// <summary>
        /// The offset where the data is to be read from.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// How much data is to be read.
        /// </summary>
        public int Take { get; set; }
    }
}
