namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IInterceptionConfiguration
    {
        IExportedValueInterceptor Interceptor { get; }

        IEnumerable<IPartInterceptor> PartInterceptors { get; }

        IEnumerable<IExportHandler> Handlers { get; }
    }
}