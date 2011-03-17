using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MefContrib.Hosting.Conventions;

namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Diagnostics;

    public class ExportElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportElementCollection"/>
        /// </summary>
        public ExportElementCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExportElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public override ConfigurationElementCollectionType CollectionType
        {
            [DebuggerStepThrough]
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        protected override string ElementName
        {
            [DebuggerStepThrough]
            get { return "export"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var part = element as ExportElement;

            if (part != null)
            {
                return part.Member;
            }

            // TODO: Write exception message.
            throw new InvalidOperationException();
        }
    }
}