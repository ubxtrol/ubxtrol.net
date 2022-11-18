namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 后期构造类型定义.
    /// </summary>
    public interface IUbxtrolPostConstruct
    {
        /// <summary>
        /// 当依赖的服务已完成注入后触发.
        /// </summary>
        void OnServicesInjected();
    }
}
