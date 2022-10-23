using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 标识一个属性为依赖属性.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UbxtrolAutowiredAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置当依赖的服务不存在时是否忽略该属性的注入行为.
        /// </summary>
        public bool IgnoreWhenNotFound { get; set; }
    }
}
