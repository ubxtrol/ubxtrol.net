using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务构建接口定义.
    /// </summary>
    public interface IUbxtrolServiceBuilder
    {
        /// <summary>
        /// 构建一个<see cref="IServiceProvider"/>实例.
        /// </summary>
        /// <returns>一个<see cref="IServiceProvider"/>实例.</returns>
        IServiceProvider BuildServiceProvider();
    }
}
