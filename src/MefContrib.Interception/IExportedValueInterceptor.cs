namespace MefContrib.Interception
{
    public interface IExportedValueInterceptor
    {
        object Intercept(object value);
    }
}