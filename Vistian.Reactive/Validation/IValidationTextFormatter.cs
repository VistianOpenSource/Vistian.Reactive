using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// Specification for a <see cref="ValidationText"/> formatter.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface IValidationTextFormatter<out TOut>
    {
        TOut Format(ValidationText validationText);
    }
}
