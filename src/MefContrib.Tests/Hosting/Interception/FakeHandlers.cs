using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace MefContrib.Hosting.Interception.Tests
{
    public class FakeExportHandler1 : IExportHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            yield break;
        }
    }

    public class FakeExportHandler2 : IExportHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            yield break;
        }
    }

    public class FakePartHandler1 : IPartHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<ComposablePartDefinition> GetParts(IEnumerable<ComposablePartDefinition> parts)
        {
            yield break;
        }

        public event EventHandler<PartHandlerChangedEventArgs> Changed;
    }

    public class FakePartHandler2 : IPartHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<ComposablePartDefinition> GetParts(IEnumerable<ComposablePartDefinition> parts)
        {
            yield break;
        }

        public event EventHandler<PartHandlerChangedEventArgs> Changed;
    }

    public class RecomposablePartHandler : IPartHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
            Changed += delegate { };
        }

        public IEnumerable<ComposablePartDefinition> GetParts(IEnumerable<ComposablePartDefinition> parts)
        {
            return parts;
        }

        public void AddParts(ComposablePartCatalog catalog)
        {
            Changed(this, new PartHandlerChangedEventArgs(catalog.Parts, Enumerable.Empty<ComposablePartDefinition>()));
        }

        public void RemoveParts(ComposablePartCatalog catalog)
        {
            Changed(this, new PartHandlerChangedEventArgs(Enumerable.Empty<ComposablePartDefinition>(), catalog.Parts));
        }

        public event EventHandler<PartHandlerChangedEventArgs> Changed;
    }
}