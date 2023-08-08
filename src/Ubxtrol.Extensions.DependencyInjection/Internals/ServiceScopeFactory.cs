using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly ServiceProvider provider;

        public ServiceScopeFactory(ServiceProvider provider)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            this.provider = provider;
        }

        public IServiceScope CreateScope()
        {
            ServiceContainer container = this.provider.Container;
            if (container.IsDisposed)
                throw Error.Disposed(nameof(IServiceProvider));

            ScopedServiceProvider provider = new ScopedServiceProvider(this.provider);
            return new ServiceScope(provider);
        }
    }
}
