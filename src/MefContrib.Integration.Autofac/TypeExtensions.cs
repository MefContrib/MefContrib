#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac
{
    using System;

    internal static class TypeExtensions
    {
        #region Methods

        public static bool IsGenericTypeDefinedBy(this Type @this, Type openGeneric)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("this");
            }
            if (openGeneric == null)
            {
                throw new ArgumentNullException("openGeneric");
            }
            return (@this.IsGenericType && (@this.GetGenericTypeDefinition() == openGeneric));
        }

        #endregion
    }
}