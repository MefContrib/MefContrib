using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Generics.Tests
{
    [InheritedExport]
    public interface IRepository<T>
    {
    }
}