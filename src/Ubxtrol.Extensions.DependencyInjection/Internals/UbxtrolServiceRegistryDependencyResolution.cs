using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class UbxtrolServiceRegistryDependencyResolution : ServiceDependencyResolution
    {
        private static readonly Lazy<IDependencyResolution> Cache = new Lazy<IDependencyResolution>(() => new UbxtrolServiceRegistryDependencyResolution());

        public static IDependencyResolution Shared => UbxtrolServiceRegistryDependencyResolution.Cache.Value;

        private UbxtrolServiceRegistryDependencyResolution()
            : base(ServiceIdentity.From(0x0, typeof(IUbxtrolServiceRegistry), ServiceLifetime.Singleton))
        { }

        protected override void ExecuteServiceCreation(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            ServiceConfiguration configuration = context.Provider.Configuration;
            context.Result = new UbxtrolServiceRegistry(configuration);
        }
    }
}
