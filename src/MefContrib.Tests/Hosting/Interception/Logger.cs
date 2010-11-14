using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Interception.Tests
{
    public interface ILogger
    {
        void Log(string message);
    }

    [Export(typeof(ILogger))]
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            
        }
    }
}