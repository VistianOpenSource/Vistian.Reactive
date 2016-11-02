using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Xamarin
{
    /// <summary>
    /// Duplicate of Xamarin Preserve Attribute.
    /// </summary>
    /// <remarks>
    /// Why is it here? Good question. We seemed to go through a phase of this not being available in the core tooling with Xamarin so...</remarks>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public sealed class PreserveAttribute : Attribute
    {
        //
        // Fields
        //
        public bool AllMembers;

        public bool Conditional;
    }
}
