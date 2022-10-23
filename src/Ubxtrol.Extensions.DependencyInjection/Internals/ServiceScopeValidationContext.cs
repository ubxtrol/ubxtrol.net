using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceScopeValidationContext
    {
        private readonly Type mServiceType;

        public IDependencyResolution Resolution { get; set; }

        public Type ServiceType => this.mServiceType;

        public Type SingletonServiceType { get; set; }

        public ServiceScopeValidationContext(Type mServiceType)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            this.mServiceType = mServiceType;
        }
    }
}
