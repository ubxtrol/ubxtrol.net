using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceProviderIsServiceDependencyResolution : FactoryDependencyResolution
    {
        private static readonly Lazy<IDependencyResolution> Cache = new Lazy<IDependencyResolution>(() => new ServiceProviderIsServiceDependencyResolution());

        public static IDependencyResolution Shared => ServiceProviderIsServiceDependencyResolution.Cache.Value;

        private ServiceProviderIsServiceDependencyResolution()
            : base(ServiceIdentity.From(0x0, typeof(IServiceProviderIsService), ServiceLifetime.Transient), ServiceProviderIsServiceDependencyResolution.ServiceCreation)
        { }

        private static IUbxtrolServiceRegistry ServiceCreation(IServiceProvider provider)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            return provider.GetService<IUbxtrolServiceRegistry>();
        }
    }
}
