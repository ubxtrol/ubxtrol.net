using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal interface IServiceScopeValidator
    {
        void BeforeDependencyResolutionResolved(Type mServiceType);

        void OnResolutionCreated(Type mServiceType, IDependencyResolution resolution);
    }
}
