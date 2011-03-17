
using MefContrib.Hosting.Conventions;

namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    
    public class MetadataElementCollection : ConfigurationElementCollection
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new MetadataElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected override object GetElementKey(ConfigurationElement element)
        {
            MetadataElement metadataElement =
                element as MetadataElement;

            if (metadataElement != null)
            {
                return metadataElement.Name;
            }

            throw new InvalidOperationException();
        }
    }
}