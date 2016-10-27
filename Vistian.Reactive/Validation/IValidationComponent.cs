using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// Core interface which all validation components must implement.
    /// </summary>
    public interface IValidationComponent
    {
        /// <summary>
        /// Get the Current,optional validation message
        /// </summary>
        ValidationText Text { get; }

        /// <summary>
        /// Get the current validation state
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Get the observable for validation state changes.
        /// </summary>
        IObservable<ValidationState> ValidationStatusChange { get; }
    }
}
