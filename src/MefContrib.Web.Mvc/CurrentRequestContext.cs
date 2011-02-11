using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using MefContrib.Web.Mvc.Internal;

namespace MefContrib.Web.Mvc
{
    /// <summary>
    /// CurrentRequestContext
    /// </summary>
    public static class CurrentRequestContext
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public static IDictionary Items
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items;
                } 
                else if (WcfOperationContext.Current != null)
                {
                    return WcfOperationContext.Current.Items;
                }
                return null;
            }
        }
    }
}
