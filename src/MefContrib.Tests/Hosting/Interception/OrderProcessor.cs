using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Interception.Tests
{
    [Export]
    public class OrderProcessor
    {
        [Import]
        public ILogger Logger { get; set; }
    }
}