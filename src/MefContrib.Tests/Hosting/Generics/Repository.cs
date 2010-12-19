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

    [Export(typeof(IMultiRepository<>))]
    public class MultiRepository1<T> : IMultiRepository<T>
    {
    }

    [Export(typeof(IMultiRepository<>))]
    public class MultiRepository2<T> : IMultiRepository<T>
    {
    }
}