using System.Collections.Generic;
using System.Reflection;
using MefContrib.Hosting.Conventions;
using MefContrib.Hosting.Conventions.Configuration;

namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class ImportElementCollection : ConfigurationElementCollection
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            [DebuggerStepThrough]
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string ElementName
        {
            [DebuggerStepThrough]
            get { return "import"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var part = element as ImportElement;

            if (part != null)
            {
                return part.Member;
            }

            throw new InvalidOperationException();
        }
    }
}