using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace MefContrib.Interception
{
    public interface IExportHandler
    {
        void Initialize(ComposablePartCatalog catalog);
        IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports);
    }
}
