using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class DependencyResolveContext
    {
        private readonly ServiceProvider provider;

        public ServiceContainer Container { get; set; }

        public Nullable<ServiceLifetime> Lifetime { get; set; }

        public ServiceProvider Provider => this.provider;

        public object Result { get; set; }

        public DependencyResolveContext(ServiceProvider provider)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            this.provider = provider;
        }

        public ServiceProvider UseServiceProvider()
        {
            ServiceProvider result = this.provider;
            if (this.Lifetime.HasValue && this.Lifetime.Value == ServiceLifetime.Singleton)
                result = this.provider.Provider;

            return result;
        }
    }
}
