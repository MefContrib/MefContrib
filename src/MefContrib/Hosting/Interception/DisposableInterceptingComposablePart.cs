namespace MefContrib.Hosting.Interception
{
    using System;
    using System.ComponentModel.Composition.Primitives;
    using System.Threading;

    public class DisposableInterceptingComposablePart : InterceptingComposablePart, IDisposable
    {
        private int isDisposed;

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