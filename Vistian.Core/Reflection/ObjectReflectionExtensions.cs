using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reflection
{
    public static class ObjectReflectionExtensions
    {
        /// <summary>
        /// Given an object instance return a dictionary of the public property values.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Properties(this object instance)
        {
            Guard.NotNull(instance);

            // we will now use reflection 
            var attributes = new Dictionary<string, object>();

            var ti = instance.GetType().GetTypeInfo();

            var properties = ti.DeclaredProperties;

            foreach (var property in properties)
            {
                var value = property.GetValue(instance);

                attributes[property.Name] = value;
            }

            return attributes;
        }
    }
}
