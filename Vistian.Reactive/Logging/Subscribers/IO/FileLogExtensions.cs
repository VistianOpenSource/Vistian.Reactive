using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Subscribers.IO
{
    public static class FileLogMixins
    {
        public static IObservable<Unit> Write(this IObservable<string> source, LogFileManager fileManager)
        {
            return FileLog.Write(source, fileManager);
        }
    }
}
