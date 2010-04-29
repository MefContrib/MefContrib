namespace MefContrib.Hosting.Conventions.Tests
{
    using System.Collections.Generic;

    public interface IFakeInterface
    {
        string Name { get; set; }

        IEnumerable<int> Values { get; set; }
    }
}