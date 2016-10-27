using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Represents the 'unit' / 'type' of many of the metrics available.
    /// </summary>
    /// <remarks>
    /// Heavily derived from https://github.com/Recognos/Metrics.NET
    /// </remarks>
    public struct Unit 
    {
        public static readonly Unit None = new Unit(string.Empty);
        public static readonly Unit Requests = new Unit("Requests");
        public static readonly Unit Commands = new Unit("Commands");
        public static readonly Unit Calls = new Unit("Calls");
        public static readonly Unit Events = new Unit("Events");
        public static readonly Unit Errors = new Unit("Errors");
        public static readonly Unit Results = new Unit("Results");
        public static readonly Unit Items = new Unit("Items");
        public static readonly Unit MegaBytes = new Unit("Mb");
        public static readonly Unit KiloBytes = new Unit("Kb");
        public static readonly Unit Bytes = new Unit("bytes");
        public static readonly Unit Percent = new Unit("%");
        public static readonly Unit Threads = new Unit("Threads");

        public static Unit Custom(string name)
        {
            return new Unit(name);

        }

        public static implicit operator Unit(string name)
        {
            return Unit.Custom(name);
        }

        public readonly string Name;

        private Unit(string name)
        {
            Guard.NotNull(name);

            Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
