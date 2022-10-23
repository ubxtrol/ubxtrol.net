using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务扩展方法静态包装类型.
    /// </summary>
    public static class UbxtrolServiceExtensions
    {
        /// <summary>
        /// 使用指定的服务描述符集合构建一个<see cref="UbxtrolServiceProvider"/>实例.
        /// </summary>
        /// <param name="collection">服务描述符集合.</param>
        /// <param name="configuration">配置信息初始化委托.</param>
        /// <returns>一个<see cref="UbxtrolServiceProvider"/>实例.</returns>
        public static UbxtrolServiceProvider BuildUbxtrolServiceProvider(this IServiceCollection collection, Action<UbxtrolServiceProviderOptions> configuration = null)
        {
            if (collection == null)
                throw Error.ArgumentNull(nameof(collection));

            UbxtrolServiceProviderOptions options = new UbxtrolServiceProviderOptions();
            if (configuration != null)
                configuration.Invoke(options);

            return collection.BuildUbxtrolServiceProvider(options);
        }

        /// <summary>
        /// 使用指定的服务描述符集合构建一个<see cref="UbxtrolServiceProvider"/>实例.
        /// </summary>
        /// <param name="collection">服务描述符集合.</param>
        /// <param name="options">服务提供对象配置信息.</param>
        /// <returns>一个<see cref="UbxtrolServiceProvider"/>实例.</returns>
        public static UbxtrolServiceProvider BuildUbxtrolServiceProvider(this IServiceCollection collection, UbxtrolServiceProviderOptions options)
        {
            if (collection == null)
                throw Error.ArgumentNull(nameof(collection));

            if (options == null)
                throw Error.ArgumentNull(nameof(options));

            UbxtrolServiceBuilder builder = UbxtrolServiceBuilder.From(collection, options);
            return builder.BuildServiceProvider();
        }

        /// <summary>
        /// 检查服务类型是否在<see cref="IServiceProvider"/>中注册.
        /// </summary>
        /// <param name="provider">服务提供对象.</param>
        /// <param name="mServiceType">需要检查的服务类型.</param>
        /// <returns>服务类型是否在<see cref="IServiceProvider"/>中注册.</returns>
        public static bool IsServiceRegistered(this IServiceProvider provider, Type mServiceType)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            IUbxtrolServiceRegistry registry = provider.GetRequiredService<IUbxtrolServiceRegistry>();
            return registry.IsRegistered(mServiceType);
        }

        /// <summary>
        /// 检查服务类型是否在<see cref="IServiceProvider"/>中注册.
        /// </summary>
        /// <typeparam name="TService">需要检查的服务类型.</typeparam>
        /// <param name="provider">服务提供对象.</param>
        /// <returns>服务类型是否在<see cref="IServiceProvider"/>中注册.</returns>
        public static bool IsServiceRegistered<TService>(this IServiceProvider provider)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            return provider.IsServiceRegistered(typeof(TService));
        }
    }
}
