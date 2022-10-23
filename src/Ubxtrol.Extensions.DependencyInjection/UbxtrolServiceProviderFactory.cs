using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务提供对象实例工厂.
    /// </summary>
    public class UbxtrolServiceProviderFactory : IServiceProviderFactory<IUbxtrolServiceBuilder>
    {
        private readonly UbxtrolServiceProviderOptions options;

        /// <summary>
        /// 获取服务提供对象配置信息.
        /// </summary>
        public UbxtrolServiceProviderOptions Options => this.options;

        /// <summary>
        /// 初始化一个<see cref="UbxtrolServiceProviderFactory"/>实例.
        /// </summary>
        public UbxtrolServiceProviderFactory()
        {
            this.options = new UbxtrolServiceProviderOptions();
        }

        /// <summary>
        /// 创建一个<see cref="IUbxtrolServiceBuilder"/>实例.
        /// </summary>
        /// <param name="collection">服务描述符集合.</param>
        /// <returns>一个<see cref="IUbxtrolServiceBuilder"/>实例.</returns>
        public IUbxtrolServiceBuilder CreateBuilder(IServiceCollection collection)
        {
            if (collection == null)
                throw Error.ArgumentNull(nameof(collection));

            return UbxtrolServiceBuilder.From(collection, this.options);
        }

        /// <summary>
        /// 创建一个<see cref="IServiceProvider"/>实例.
        /// </summary>
        /// <param name="builder">服务构建对象.</param>
        /// <returns>一个<see cref="IServiceProvider"/>实例.</returns>
        public IServiceProvider CreateServiceProvider(IUbxtrolServiceBuilder builder)
        {
            if (builder == null)
                throw Error.ArgumentNull(nameof(builder));

            return builder.BuildServiceProvider();
        }
    }
}
