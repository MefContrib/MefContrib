using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Generics.Tests
{
    public class Repository<T> : IRepository<T>
    {
    }

    [Export(typeof(IMyRepository<>))]
    public class MyRepository<T> : IMyRepository<T>
    {
    }
}