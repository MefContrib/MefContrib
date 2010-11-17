using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

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
    }
}