namespace Ubxtrol.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务提供对象配置信息.
    /// </summary>
    public class UbxtrolServiceProviderOptions
    {
        /// <summary>
        /// 获取或设置是否启用服务作用域验证.
        /// </summary>
        public bool EnableScopeValidation { get; set; }

        /// <summary>
        /// 获取或设置在构建时是否执行验证.
        /// </summary>
        public bool RunValidationOnBuild { get; set; }
    }
}
