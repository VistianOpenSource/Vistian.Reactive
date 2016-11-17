using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Providers
{
    public static class ThrowExtensions
    {
        /// <summary>
        /// Simple extension to force a throw once logged.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="exception"></param>
        public static void Throw(this RxLogEntry<Classified> entry,
            Func<RxLogEntry<Classified>, Exception> exception)
        {
            throw exception(entry);
        }
    }
}
