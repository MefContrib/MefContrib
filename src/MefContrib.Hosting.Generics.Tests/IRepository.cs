using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Generics.Tests.GenericCatalogSpecs
{
    [InheritedExport]
    public interface IRepository<T>
    {
    }
}