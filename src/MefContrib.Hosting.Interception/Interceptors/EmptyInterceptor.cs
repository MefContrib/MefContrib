namespace MefContrib.Hosting.Interception
{
    public class EmptyInterceptor : IExportedValueInterceptor
    {
        public static readonly IExportedValueInterceptor Default = new EmptyInterceptor();

        public object Intercept(object value)
        {
            return value;
        }
    }
}