using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Formatting;
using Vistian.Reactive.Metrics.Logging;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// A discrete timing block.
    /// </summary>
    /// 
    [Formatter(typeof(TimingBlockFormatter))]
    public class TimingBlock
    {
        /// <summary>
        /// The list of individual timing entries.
        /// </summary>
        public List<TimerEntry> Entries { get; } = new List<TimerEntry>();

        /// <summary>
        /// The stopwatch used for the timing
        /// </summary>
        private readonly Stopwatch _sw;

        /// <summary>
        /// Are we currently active?
        /// </summary>
        private bool _isActive;

        /// <summary>
        /// Get the count of the total number of times this block has been used.
        /// </summary>
        public int Count { get; private set; }


        /// <summary>
        /// Get the name of this block
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// Get the last reading when this timing block was used.
        /// </summary>
        public long LastMilliseconds { get; private set; }

        /// <summary>
        /// Get the minimum time we have seen.
        /// </summary>
        public double Min { get; private set; }

        /// <summary>
        /// Get the mean time we have seen
        /// </summary>
        public double Mean { get; private set; }

        /// <summary>
        /// Get the maximum time we have seen.
        /// </summary>
        public double Max { get; private set; }

        /// <summary>
        /// Create a named timing block.
        /// </summary>
        /// <param name="name"></param>
        public TimingBlock(string name)
        {
            Name = name;
            _sw = new Stopwatch();
        }

        /// <summary>
        /// Add a timing with the specified name
        /// </summary>
        /// <param name="label"></param>
        public void Mark(string label)
        {
            var entry = new TimerEntry() { Label = label, Milliseconds = _sw.ElapsedMilliseconds };
            Entries.Add(entry);
        }

        /// <summary>
        /// Start the timing block.
        /// </summary>
        public void Begin()
        {
            // guard against a dubious call 
            if (_isActive)
            {
                return;
            }

            Entries.Clear();

            _sw.Reset();
            _sw.Start();

            _isActive = true;
        }

        /// <summary>
        /// End the timing block.
        /// </summary>
        public void End()
        {
            if (!_isActive)
            {
                return;
            }

            _sw.Stop();

            var totalMilliseconds = _sw.ElapsedMilliseconds;

            if (Count == 0)
            {
                LastMilliseconds = totalMilliseconds;
                Min = LastMilliseconds;
                Max = LastMilliseconds;
                Mean = LastMilliseconds;
            }
            else
            {
                Min = Math.Min(Min, totalMilliseconds);
                Max = Math.Max(Max, totalMilliseconds);

                Mean = ((Mean * Count) + totalMilliseconds) / (Count + 1);
                LastMilliseconds = totalMilliseconds;

            }
            _isActive = false;

            ++Count;
        }

        /// <summary>
        /// Create a <see cref="TimingBlock"/> along with an associated context.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static ITimerContext CreateContext(string label = "")
        {
            var timingBlock = new TimingBlock(label);

            var context = new TimerContext(timingBlock);

            return context;
        }
    }
}
