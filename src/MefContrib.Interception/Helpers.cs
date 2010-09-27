using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MefContrib.Interception
{
    public static class Helpers
    {
        public static void ShouldNotBeNull<T>(this T value, string argument)
        {
            if (Equals(value, null))
            {
                throw new ArgumentNullException(argument);
            }
        }
    }
}
