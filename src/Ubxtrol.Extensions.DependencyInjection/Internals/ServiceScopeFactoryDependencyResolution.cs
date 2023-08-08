using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceScopeFactoryDependencyResolution : ServiceDependencyResolution
    {
        private static readonly Lazy<IDependencyResolution> Cache = new Lazy<IDependencyResolution>(() => new ServiceScopeFactoryDependencyResolution());

        public static IDependencyResolution Shared => ServiceScopeFactoryDependencyResolution.Cache.Value;

        private ServiceScopeFactoryDependencyResolution()
            : base(ServiceIdentity.From(0x0, typeof(IServiceScopeFactory), ServiceLifetime.Singleton))
        { }

        protected override void ExecuteServiceCreation(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            ServiceProvider provider = context.Provider.Provider;
            context.Result = new ServiceScopeFactory(provider);
        }
    }
}
