using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Xamarin;

namespace Vistian.Reactive.Logging
{
    /// <summary>
    /// Extensions enabling logging from within classes.
    /// </summary>
    /// 
    [Preserve(AllMembers = true)]
    public static class RxLogEntryMixins
    {
        public static RxLogEntry Log(this object publisher, object instance, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(publisher);

            var meta = RxLog.CreateMeta(publisher.GetType(), callerMemberName, lineNo);
            var entry = new RxLogEntry(meta, instance);

            RxLog.Log(entry);

            return entry;
        }


        public static RxLogEntry<T> Log<T>(this object publisher, T instance, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(publisher);
            Guard.NotNull(instance);

            var meta = RxLog.CreateMeta(publisher.GetType(), callerMemberName, lineNo);
            var entry = new RxLogEntry<T>(meta, instance);

            RxLog.Log(entry);

            return entry;
        }

    }
}
