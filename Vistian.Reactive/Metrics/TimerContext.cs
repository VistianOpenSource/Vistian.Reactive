using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Metrics
{
    /// <summary>
    /// Create a disposable context for a timing context.
    /// </summary>
    public class TimerContext : ITimerContext
    {
        /// <summary>
        /// Our timing block
        /// </summary>
        private readonly TimingBlock _block;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        public TimerContext(TimingBlock block)
        {
            Guard.NotNull(block);

            _block = block;

            block.Begin();
        }

        public TimingBlock TimingBlock => _block;

        /// <summary>
        /// Mark this timing.
        /// </summary>
        /// <param name="label"></param>
        public void Mark(string label = null)
        {
            label = label ?? string.Empty;

            _block.Mark(label);
        }

        /// <summary>
        /// End the block
        /// </summary>
        public void End()
        {
            _block.End();
        }

        /// <summary>
        /// Dispose of the timing context.
        /// </summary>
        public void Dispose()
        {
            End();
        }
    }

}
