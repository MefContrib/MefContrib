using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace MefContrib.Web.Mvc.Internal
{
    /// <summary>
    /// WcfOperationContext extension
    /// </summary>
    internal class WcfOperationContext
        : IExtension<OperationContext>
    {
        private readonly IDictionary items;

        private WcfOperationContext()
        {
            items = new Hashtable();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IDictionary Items
        {
            get { return items; }
        }

        /// <summary>
        /// Gets the current WcfOperationContext.
        /// </summary>
        /// <value>The current WcfOperationContext.</value>
        public static WcfOperationContext Current
        {
            get
            {
                WcfOperationContext context = OperationContext.Current.Extensions.Find<WcfOperationContext>();
                if (context == null)
                {
                    context = new WcfOperationContext();
                    OperationContext.Current.Extensions.Add(context);
                }
                return context;
            }
        }

        /// <summary>
        /// Attaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Attach(OperationContext owner) { }

        /// <summary>
        /// Detaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Detach(OperationContext owner) { }
    }
}
