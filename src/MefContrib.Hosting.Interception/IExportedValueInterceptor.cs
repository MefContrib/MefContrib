namespace MefContrib.Hosting.Interception
{
    public interface IExportedValueInterceptor
    {
        object Intercept(object value);
    }
}