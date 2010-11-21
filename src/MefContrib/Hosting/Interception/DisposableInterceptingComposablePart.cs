namespace MefContrib.Hosting.Interception
{
    using System;
    using System.ComponentModel.Composition.Primitives;
    using System.Threading;

    /// <summary>
    /// Defines <see cref="InterceptingComposablePart"/> which is disposable.
    /// </summary>
    public class DisposableInterceptingComposablePart : InterceptingComposablePart, IDisposable
    {
        private int isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableInterceptingComposablePart"/> class.
        /// </summary>
        /// <param name="interceptedPart">The <see cref="ComposablePart"/> being intercepted.</param>
        /// <param name="valueInterceptor">The <see cref="IExportedValueInterceptor"/> instance.</param>
        public DisposableInterceptingComposablePart(ComposablePart interceptedPart, IExportedValueInterceptor valueInterceptor)
            : base(interceptedPart, valueInterceptor)
        {
        }

        void IDisposable.Dispose()
        {
            if (Interlocked.CompareExchange(ref this.isDisposed, 1, 0) == 0)
            {
                var disposable = (IDisposable) this.InterceptedPart;
                disposable.Dispose();
            }
        }
    }
}