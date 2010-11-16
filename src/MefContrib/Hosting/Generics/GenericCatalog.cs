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

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this.interceptingCatalog.Parts; }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return this.interceptingCatalog.GetExports(definition);
        }
        
        #region INotifyComposablePartCatalogChanged Implementation

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add { this.interceptingCatalog.Changed += value; }
            remove { this.interceptingCatalog.Changed -= value; }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add { this.interceptingCatalog.Changing += value; }
            remove { this.interceptingCatalog.Changing -= value; }
        }

        #endregion

        private class EmptyCatalog : ComposablePartCatalog
        {
            public override IQueryable<ComposablePartDefinition> Parts
            {
                get { return Enumerable.Empty<ComposablePartDefinition>().AsQueryable(); }
            }
        }
    }
}