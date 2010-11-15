namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;
    using MefContrib.Hosting.Interception.Configuration;

    public class GenericCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly InterceptingCatalog interceptingCatalog;

        public GenericCatalog(params IGenericContractRegistry[] registries)
            : this(new EmptyCatalog(), registries)
        {
        }

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