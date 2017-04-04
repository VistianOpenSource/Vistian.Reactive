using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Linq
{
    public static class ObservableSignalExtensions
    {
        /// <summary>
        /// Utility extension to turn any observable stream into one of <see cref="Unit"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<Unit> Signal<T>(this IObservable<T> source)
        {
            return source.Select(_ => Unit.Default);
        }         
    }
}
