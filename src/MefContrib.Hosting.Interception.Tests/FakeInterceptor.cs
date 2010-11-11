namespace MefContrib.Hosting.Interception.Tests
{
    public class FakeInterceptor : IExportedValueInterceptor
    {
        public object Intercept(object value)
        {
            return value;
        }
    }
}