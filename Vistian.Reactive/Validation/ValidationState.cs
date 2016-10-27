using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// Represents the validation state of a validation component.
    /// </summary>
    public sealed class ValidationState
    {
        /// <summary>
        /// The associated component
        /// </summary>
        public IValidationComponent Component { get; }

        /// <summary>
        /// Get whether the validation is currently valid or not.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Get the validation text.
        /// </summary>
        public ValidationText Text { get; }


        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="text"></param>
        /// <param name="component"></param>
        public ValidationState(bool isValid, string text, IValidationComponent component) : this(isValid, new ValidationText(text), component)
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="text"></param>
        /// <param name="component"></param>
        public ValidationState(bool isValid, ValidationText text, IValidationComponent component)
        {
            IsValid = isValid;
            Text = text;
            Component = component;
        }
    }
}
