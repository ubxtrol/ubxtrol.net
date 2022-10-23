using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceIdentity
    {
        private readonly ServiceKey key;

        public ServiceKey Key => this.key;

        public ServiceLifetime Lifetime { get; set; }

        public ServiceIdentity(int index, Type value)
        {
            if (value == null)
                throw Error.ArgumentNull(nameof(value));

            this.key = new ServiceKey(index, value);
        }

        public static ServiceIdentity From(int index, Type value, ServiceLifetime lifetime)
        {
            ServiceIdentity result = new ServiceIdentity(index, value);
            result.Lifetime = lifetime;
            return result;
        }
    }
}
