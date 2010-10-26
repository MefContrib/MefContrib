namespace MefContrib.Hosting.Interception
{
    /// <summary>
    /// Represents an empty interceptor.
    /// </summary>
    public class EmptyInterceptor : IExportedValueInterceptor
    {
        /// <summary>
        /// Gets the default instance of the <see cref="EmptyInterceptor"/> class.
        /// </summary>
        public static readonly IExportedValueInterceptor Default = new EmptyInterceptor();

        /// <summary>
        /// Intercepts an exported value.
        /// </summary>
        /// <param name="value">The value to be intercepted.</param>
        /// <returns>Intercepted value.</returns>
        public object Intercept(object value)
        {
            return value;
        }
    }
}