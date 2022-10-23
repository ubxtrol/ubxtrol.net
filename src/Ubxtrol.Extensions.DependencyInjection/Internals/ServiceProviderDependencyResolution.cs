using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceProviderDependencyResolution : IDependencyResolution
    {
        private static readonly Lazy<IDependencyResolution> Cache = new Lazy<IDependencyResolution>(() => new ServiceProviderDependencyResolution());

        public static IDependencyResolution Shared => ServiceProviderDependencyResolution.Cache.Value;

        public void Resolve(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            context.Result = context.GetLifetimeProvider();
        }
    }
}
