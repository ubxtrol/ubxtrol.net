using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务提供对象的默认实现.
    /// </summary>
    public class UbxtrolServiceProvider : IAsyncDisposable, IDisposable, IServiceProvider, ISupportRequiredService
    {
        private readonly ServiceProvider provider;

        object ISupportRequiredService.GetRequiredService(Type mServiceType)
        {
            return this.provider.GetRequiredService(mServiceType);
        }

        internal UbxtrolServiceProvider(ServiceConfiguration configuration)
        {
            if (configuration == null)
                throw Error.ArgumentNull(nameof(configuration));

            this.provider = new ServiceProvider(configuration);
        }

        /// <summary>
        /// 释放当前对象的非托管资源.
        /// </summary>
        public void Dispose()
        {
            this.provider.Dispose();
        }

        /// <summary>
        /// 异步释放当前对象的非托管资源.
        /// </summary>
        /// <returns>一个<see cref="ValueTask"/>实例.</returns>
        public ValueTask DisposeAsync()
        {
            return this.provider.DisposeAsync();
        }

        /// <summary>
        /// 获取指定类型的服务实例.
        /// </summary>
        /// <param name="mServiceType">服务类型.</param>
        /// <returns>该类型的服务实例.</returns>
        public object GetService(Type mServiceType)
        {
            return this.provider.GetService(mServiceType);
        }
    }
}
