namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;
    using MefContrib.Hosting.Interception.Configuration;

    /// <summary>
    /// Defines a composable parts catalog which enables open-generics support.
    /// </summary>
    public class GenericCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly InterceptingCatalog interceptingCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCatalog"/> class.
        /// </summary>
        /// <param name="registries"> A collection of <see cref="IGenericContractRegistry"/> instances.</param>
        public GenericCatalog(params IGenericContractRegistry[] registries)
            : this(new EmptyCatalog(), registries)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCatalog"/> class.
        /// </summary>
        /// <param name="catalog"><see cref="ComposablePartCatalog"/> from which <see cref="IGenericContractRegistry"/> instances will be retrieved.</param>
        /// <param name="registries">Additional registries.</param>
        public GenericCatalog(ComposablePartCatalog catalog, params IGenericContractRegistry[] registries)
        {
            var cfg = new InterceptionConfiguration()
                .AddHandler(new GenericExportHandler(registries));
            this.interceptingCatalog = new InterceptingCatalog(catalog, cfg);
        }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the <see cref="GenericCatalog"/>.</value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this.interceptingCatalog.Parts; }
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <returns>
        /// A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.
        /// </returns>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return this.interceptingCatalog.GetExports(definition);
        }
        
        #region INotifyComposablePartCatalogChanged Implementation

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog"/> has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add { this.interceptingCatalog.Changed += value; }
            remove { this.interceptingCatalog.Changed -= value; }
        }

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog"/> is changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add { this.interceptingCatalog.Changing += value; }
            remove { this.interceptingCatalog.Changing -= value; }
        }

        #endregion
    }
}