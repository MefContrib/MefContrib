using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Interception.Tests.Generics
{
    [InheritedExport]
    public interface IRepository<T>
    {
    }
}