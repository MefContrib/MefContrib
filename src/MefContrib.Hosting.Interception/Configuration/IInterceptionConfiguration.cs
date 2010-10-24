namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IInterceptionConfiguration
    {
        IExportedValueInterceptor Interceptor { get; }

        IEnumerable<IPartInterceptionCriteria> InterceptionCriteria { get; }

        IEnumerable<IExportHandler> Handlers { get; }
    }
}