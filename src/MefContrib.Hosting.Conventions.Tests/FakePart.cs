namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;

    public class FakePart : IFakeInterface
    {
        public FakePart()
        {
        }

        public FakePart(int a, string[] b)
        {
            
        }

        public int Count;

        public string Name { get; set; }

        public Func<string, string, object> Delegate { get; set; }

        public IEnumerable<int> Values { get; set; }

        public void DoWork()
        {
        }
    }
}