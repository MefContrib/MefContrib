using System.ComponentModel.Composition;

namespace MefContrib.Interception.Tests.Generics
{
    [InheritedExport]
    public interface IRepository<T>
    {
    }
}