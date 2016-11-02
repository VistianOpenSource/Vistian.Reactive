using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Contract
{
    /// <summary>
    /// Utility classes providing DEBUG traps for common check scenarios.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Ensures that the specified argument is not null.
        /// </summary>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        public static void NotNull<T>(T value, string parameterName = null)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }
        }


        /// <summary>
        /// Check that a string value isn't empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]

        public static void NotEmpty(string value, string parameterName = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("NotEmpty", parameterName);
            }
        }


        /// <summary>
        /// Check that is specified condition is true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="parameterName"></param>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        public static void True(Func<bool> condition, string parameterName = null)
        {
            if (!condition())
            {
                throw new ArgumentException(parameterName);
            }
        }

        /// <summary>
        /// Check that a specified type implements a specified type.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="type"></param>
        /// <param name="parameterName"></param>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        public static void Implements<TInterface>(Type type, string parameterName = null)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(TInterface)))
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}
