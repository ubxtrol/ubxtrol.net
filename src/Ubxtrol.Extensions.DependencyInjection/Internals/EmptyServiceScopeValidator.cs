using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class EmptyServiceScopeValidator : IServiceScopeValidator
    {
        private static readonly Lazy<IServiceScopeValidator> Cache = new Lazy<IServiceScopeValidator>(() => new EmptyServiceScopeValidator());

        public static IServiceScopeValidator Shared => EmptyServiceScopeValidator.Cache.Value;

        public void BeforeDependencyResolutionResolved(Type mServiceType)
        { }

        public void OnResolutionCreated(Type mServiceType, IDependencyResolution resolution)
        { }
    }
}
