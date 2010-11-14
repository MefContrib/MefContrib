namespace MefContrib.Hosting.Interception
{
    /// <summary>
    /// Represents an interceptor which can intercept exported values.
    /// </summary>
    public interface IExportedValueInterceptor
    {
        /// <summary>
        /// Intercepts an exported value.
        /// </summary>
        /// <param name="value">The value to be intercepted.</param>
        /// <returns>Intercepted value.</returns>
        object Intercept(object value);
    }
}