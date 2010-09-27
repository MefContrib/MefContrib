using System.ComponentModel.Composition;

namespace MefContrib.Interception.Tests
{
    [Export]
    public class OrderProcessor
    {
        [Import]
        public ILogger Logger { get; set; }
    }
}