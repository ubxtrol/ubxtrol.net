using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class UbxtrolServiceBuilder : IUbxtrolServiceBuilder
    {
        private readonly IReadOnlyDictionary<Type, ServiceRegistration> configuration;

        private readonly UbxtrolServiceProviderOptions options;

        IServiceProvider IUbxtrolServiceBuilder.BuildServiceProvider()
        {
            return this.BuildServiceProvider();
        }

        private UbxtrolServiceBuilder(IReadOnlyDictionary<Type, ServiceRegistration> configuration, UbxtrolServiceProviderOptions options)
        {
            if (configuration == null)
                throw Error.ArgumentNull(nameof(configuration));

            this.configuration = configuration;
            this.options = options;
        }

        private void CreateDependencyResolution(ServiceConfiguration configuration)
        {
            foreach (KeyValuePair<Type, ServiceRegistration> current in this.configuration)
            {
                Type mServiceType = current.Key;
                if (mServiceType.IsGenericTypeDefinition)
                    continue;

                ServiceDescription description = current.Value.Description;
                if (description.Description != null)
                    mServiceType = typeof(IEnumerable<>).MakeGenericType(mServiceType);

                configuration.GetResolution(mServiceType);
            }
        }

        public UbxtrolServiceProvider BuildServiceProvider()
        {
            IServiceScopeValidator validator = EmptyServiceScopeValidator.Shared;
            if (this.options.EnableScopeValidation)
                validator = new StrictServiceScopeValidator();

            ServiceConfiguration configuration = new ServiceConfiguration(this.configuration, validator);
            if (this.options.RunValidationOnBuild)
                this.CreateDependencyResolution(configuration);

            return new UbxtrolServiceProvider(configuration);
        }

        public static UbxtrolServiceBuilder From(IServiceCollection collection, UbxtrolServiceProviderOptions options)
        {
            if (collection == null)
                throw Error.ArgumentNull(nameof(collection));

            if (options == null)
                throw Error.ArgumentNull(nameof(options));

            Dictionary<Type, ServiceRegistration> configuration = new Dictionary<Type, ServiceRegistration>(TypeEqualityComparer.Shared);
            foreach (ServiceDescriptor current in collection)
            {
                if (current.ServiceType.IsGenericTypeDefinition)
                {
                    Type implementation = current.ImplementationType;
                    if (implementation == null || !implementation.IsGenericTypeDefinition)
                        throw Error.Argument($"泛型服务[{current.ServiceType}]需要配置泛型实现类型!", nameof(collection));

                    if (implementation.IsAbstract || implementation.IsInterface)
                        throw Error.Argument($"泛型实现类型[{implementation}]无法实例化!", nameof(collection));
                }
                else if (current.ImplementationFactory == null && current.ImplementationInstance == null)
                {
                    Type implementation = current.ImplementationType;
                    if (implementation == null)
                        throw Error.Argument($"服务[{current.ServiceType}]需要配置实现类型!", nameof(collection));

                    if (implementation.IsAbstract || implementation.IsInterface)
                        throw Error.Argument($"实现类型[{implementation}]无法实例化!", nameof(collection));
                }
                if (!configuration.TryGetValue(current.ServiceType, out ServiceRegistration registration))
                    configuration.Add(current.ServiceType, registration = new ServiceRegistration());

                registration.Append(current);
            }
            return new UbxtrolServiceBuilder(configuration, options);
        }
    }
}
