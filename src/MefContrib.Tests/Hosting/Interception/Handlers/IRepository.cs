using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Interception.Tests.Handlers
{
    [InheritedExport]
    public interface IRepository<T>
    {
    }
}