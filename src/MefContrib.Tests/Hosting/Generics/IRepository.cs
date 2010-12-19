using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Generics.Tests
{
    [InheritedExport]
    public interface IRepository<T>
    {
    }

    // My repository is not exported via InheritedExport attribute
    public interface IMyRepository<T>
    {
    }

    // Multi repository has two implementations
    public interface IMultiRepository<T>
    {
    }
}