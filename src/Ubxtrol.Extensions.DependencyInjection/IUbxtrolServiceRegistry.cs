using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务注册表接口定义.
    /// </summary>
    public interface IUbxtrolServiceRegistry
    {
        /// <summary>
        /// 检查服务类型是否在<see cref="IServiceProvider"/>中注册.
        /// </summary>
        /// <param name="mServiceType">需要检查的服务类型.</param>
        /// <returns>服务类型是否在<see cref="IServiceProvider"/>中注册.</returns>
        bool IsRegistered(Type mServiceType);
    }
}
