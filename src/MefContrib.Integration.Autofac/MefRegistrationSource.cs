#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Containers;
    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;

    /// <summary>
    ///   An <see cref = "IRegistrationSource" /> implementation that retrieves exports from MEF and provides them to Autofac as services.
    /// </summary>
    public class MefRegistrationSource : IRegistrationSource
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MefRegistrationSource" /> class.
        /// </summary>
        /// <param name = "container">The container.</param>
        public MefRegistrationSource(CompositionContainer container)
        {
            Container = container;
        }

        #endregion

        #region Implementation of IRegistrationSource

        /// <summary>
        ///   Retrieve registrations for an unregistered service, to be used
        ///   by the container.
        /// </summary>
        /// <param name = "service">The service that was requested.</param>
        /// <param name = "registrationAccessor">A function that will return existing registrations for a service.</param>
        /// <returns>Registrations providing the service.</returns>
        /// <remarks>
        ///   If the source is queried for service s, and it returns a component that implements both s and s', then it
        ///   will not be queried again for either s or s'. This means that if the source can return other implementations
        ///   of s', it should return these, plus the transitive closure of other components implementing their
        ///   additional services, along with the implementation of s. It is not an error to return components
        ///   that do not implement <paramref name = "service" />.
        /// </remarks>
        IEnumerable<IComponentRegistration> IRegistrationSource.RegistrationsFor(Service service,
                                                                                 Func
                                                                                     <Service,
                                                                                     IEnumerable<IComponentRegistration>
                                                                                     > registrationAccessor)
        {
            var typedService = service as IServiceWithType;
            // If the requested service is not a typed service or if any non-MEF registrations exist for the service, don't bother going to MEF.
            if (typedService == null /*|| registrationAccessor(service).Any()*/)
                yield break;
            var serviceType = typedService.ServiceType;
            // Handling the enumerable scenario here but really it should be handled by the CollectionRegistrationSource, 
            // but just in case the it was excluded from the container...
            Type elementType = null;
            if (serviceType.IsGenericTypeDefinedBy(typeof (IEnumerable<>)))
            {
                elementType = serviceType.GetGenericArguments()[0];
            }
            else if (serviceType.IsArray)
            {
                elementType = serviceType.GetElementType();
            }
            var keyedService = typedService as KeyedService;
            var serviceName = keyedService != null ? keyedService.ServiceKey.ToString() : null;
            var lazyExports = ContainerServices.ResolveAll(Container, elementType ?? serviceType, serviceName);
            if (elementType != null)
            {
                yield return
                    RegistrationBuilder.ForDelegate(elementType.MakeArrayType(),
                                                    (c, p) =>
                                                        {
                                                            var instances =
                                                                lazyExports.Select(export => export.Value).ToArray();
                                                            var array = Array.CreateInstance(elementType,
                                                                                             instances.Length);
                                                            instances.CopyTo(array, 0);
                                                            return array;
                                                        })
                        .ExternallyOwned()
                        .WithMetadata("Source", "MEF")
                        .CreateRegistration();
            }
            else
            {
                foreach (var lazyExport in lazyExports)
                {
                    var export = lazyExport;
                    yield return RegistrationBuilder.ForDelegate(serviceType, (c, p) => export.Value)
                        .PreserveExistingDefaults()
                        .ExternallyOwned()
                        .WithMetadata("Source", "MEF")
                        .As(service)
                        .CreateRegistration();
                }
            }
        }

        /// <summary>
        ///   Gets whether the registrations provided by this source are 1:1 adapters on top
        ///   of other components (I.e. like Meta, Func or Owned.)
        /// </summary>
        /// <value></value>
        bool IRegistrationSource.IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        #endregion

        #region Properties

        internal CompositionContainer Container { get; private set; }

        #endregion
    }
}