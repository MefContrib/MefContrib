namespace MefContrib.Hosting.Conventions
{
    using System.Collections.Generic;
    using MefContrib.Hosting.Conventions.Configuration;

    public interface IPartRegistryLocator
    {
        IEnumerable<IPartRegistry<IContractService>> GetRegistries();
    }
}