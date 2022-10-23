using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class UbxtrolServiceRegistry : IServiceProviderIsService, IUbxtrolServiceRegistry
    {
        private readonly ServiceConfiguration configuration;

        public UbxtrolServiceRegistry(ServiceConfiguration configuration)
        {
            if (configuration == null)
                throw Error.ArgumentNull(nameof(configuration));

            this.configuration = configuration;
        }

        public bool IsRegistered(Type mServiceType)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            if (mServiceType.IsGenericTypeDefinition)
                return false;

            IReadOnlyDictionary<Type, IDependencyResolution> dependencies = this.configuration.Dependencies;
            if (dependencies.TryGetValue(mServiceType, out IDependencyResolution resolution))
                return !EmptyDependencyResolution.Shared.Equals(resolution);

            IReadOnlyDictionary<Type, ServiceRegistration> configuration = this.configuration.Configuration;
            if (configuration.ContainsKey(mServiceType))
                return true;

            if (!mServiceType.IsGenericType)
                return false;

            Type definition = mServiceType.GetGenericTypeDefinition();
            if (typeof(IEnumerable<>).Equals(definition))
                return true;

            return configuration.ContainsKey(definition);
        }

        public bool IsService(Type mServiceType)
        {
            return this.IsRegistered(mServiceType);
        }
    }
}
