using System;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Specification of a timing context.
    /// </summary>
    public interface ITimerContext : IDisposable
    {
        /// <summary>
        /// Get the timing block 
        /// </summary>
        TimingBlock TimingBlock { get; }
        /// <summary>
        /// Mark this timing.
        /// </summary>
        /// <param name="label"></param>
        void Mark(string label = null);

        /// <summary>
        /// End the block
        /// </summary>
        void End();
    }
}