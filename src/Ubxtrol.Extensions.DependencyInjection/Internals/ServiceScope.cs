using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceScope : IAsyncDisposable, IServiceScope
    {
        private readonly ScopedServiceProvider provider;

        public IServiceProvider ServiceProvider => this.provider;

        public ServiceScope(ScopedServiceProvider provider)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            this.provider = provider;
        }

        public void Dispose() => this.provider.Dispose();

        public ValueTask DisposeAsync() => this.provider.DisposeAsync();
    }
}
