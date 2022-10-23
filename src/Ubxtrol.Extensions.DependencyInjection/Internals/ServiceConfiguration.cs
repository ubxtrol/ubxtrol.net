using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceConfiguration
    {
        private readonly IReadOnlyDictionary<Type, ServiceRegistration> configuration;

        private readonly ConcurrentDictionary<Type, IDependencyResolution> dependencies;

        private readonly IServiceScopeValidator validator;

        public IReadOnlyDictionary<Type, ServiceRegistration> Configuration => this.configuration;

        public IReadOnlyDictionary<Type, IDependencyResolution> Dependencies => this.dependencies;

        public IServiceScopeValidator Validator => this.validator;

        private IDependencyResolution CreateArrayDependencyResolution(Type mServiceType, ServiceDependencyChain chain)
        {
            int index = 0x0;
            ServiceDescription description = default;
            Stack<IDependencyResolution> dependencies = default;
            Type result = mServiceType.GenericTypeArguments.Single();
            ServiceIdentity identity = new ServiceIdentity(0x0, mServiceType);
            if (this.configuration.TryGetValue(result, out ServiceRegistration registration))
            {
                description = registration.Description;
                dependencies = new Stack<IDependencyResolution>();
                while (description != null)
                {
                    ServiceDescriptor descriptor = description.Descriptor;
                    if (descriptor.Lifetime > identity.Lifetime)
                        identity.Lifetime = descriptor.Lifetime;

                    IDependencyResolution resolution = this.CreateDependencyResolution(descriptor, chain, index);
                    description = description.Description;
                    dependencies.Push(resolution);
                    index += 0x1;
                }
                return new ArrayDependencyResolution(identity, result, dependencies.ToArray());
            }
            if (!result.IsGenericType)
                return new ArrayDependencyResolution(identity, result);

            Type definition = result.GetGenericTypeDefinition();
            if (!this.configuration.TryGetValue(definition, out registration))
                return new ArrayDependencyResolution(identity, result);

            description = registration.Description;
            Type[] arguments = result.GenericTypeArguments;
            dependencies = new Stack<IDependencyResolution>();
            while (description != null)
            {
                ServiceDescriptor descriptor = description.Descriptor;
                if (descriptor.Lifetime > identity.Lifetime)
                    identity.Lifetime = descriptor.Lifetime;

                Type implementation = descriptor.ImplementationType.MakeGenericType(arguments);
                IDependencyResolution resolution = this.CreateDependencyResolution(descriptor, chain, index, result, implementation);

                index += 0x1;
                dependencies.Push(resolution);
                description = description.Description;
            }
            return new ArrayDependencyResolution(identity, result, dependencies.ToArray());
        }

        private IDependencyResolution CreateConstructorDependencyResolution(ServiceIdentity identity, Type mImplementationType, ServiceDependencyChain chain)
        {
            ConstructorDescription description = this.LookupConstructorDescription(mImplementationType, chain);
            if (description == null)
                throw Error.Invalid($"无法解析类型[{mImplementationType}]的所有构造方法依赖项!");

            DynamicMethod result = ServiceCreationTools.CreateDynamicMethod();
            ILGenerator generator = result.GetILGenerator();
            generator.DeclareLocal(mImplementationType);

            int length = description.Parameters.Length;
            for (int index = 0x0; index < length; index += 0x1)
            {
                ParameterInfo current = description.Parameters[index];

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, index);
                generator.Emit(OpCodes.Ldelem_Ref);
                generator.Emit(OpCodes.Unbox_Any, current.ParameterType);
            }

            generator.Emit(OpCodes.Newobj, description.Constructor);
            generator.Emit(OpCodes.Stloc_0);

            List<IDependencyResolution> dependencies = new List<IDependencyResolution>();
            PropertyInfo[] properties = mImplementationType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            length = properties.Length;
            for (int index = 0x0; index < length; index += 0x1)
            {
                PropertyInfo current = properties[index];
                MethodInfo method = ServiceCreationTools.GetSetMethod(mImplementationType, current);
                if (method == null)
                    continue;

                UbxtrolAutowiredAttribute attribute = current.GetCustomAttribute<UbxtrolAutowiredAttribute>();
                if (attribute == null)
                    continue;

                IDependencyResolution resolution = this.GetResolution(current.PropertyType, chain);
                if (EmptyDependencyResolution.Shared.Equals(resolution))
                {
                    if (attribute.IgnoreWhenNotFound)
                        continue;

                    throw Error.Invalid($"无法解析类型[{mImplementationType}]的依赖属性[{current.Name}]!");
                }

                dependencies.Add(resolution);

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldc_I4, dependencies.Count - 0x1);
                generator.Emit(OpCodes.Ldelem_Ref);
                generator.Emit(OpCodes.Unbox_Any, current.PropertyType);
                generator.Emit(OpCodes.Callvirt, method);
            }

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            IDependencyResolution[] arguments = description.Dependencies.ToArray();
            ServiceCreation creation = ServiceCreationTools.CreateServiceCreation(result);
            return new ConstructorDependencyResolution(identity, creation, arguments, dependencies.ToArray());
        }

        private IDependencyResolution CreateDependencyResolution(ServiceDescriptor descriptor, ServiceDependencyChain chain, int index = 0x0, Type mServiceType = null, Type mImplementationType = null)
        {
            if (descriptor == null)
                throw Error.ArgumentNull(nameof(descriptor));

            if (descriptor.ImplementationInstance != null)
                return new InstanceDependencyResolution(descriptor.ImplementationInstance);

            if (mServiceType == null)
                mServiceType = descriptor.ServiceType;

            ServiceIdentity identity = ServiceIdentity.From(index, mServiceType, descriptor.Lifetime);
            if (descriptor.ImplementationFactory != null)
                return new FactoryDependencyResolution(identity, descriptor.ImplementationFactory);

            chain.ThrowCircularDependencyWith(mServiceType);
            try
            {
                if (mImplementationType == null)
                    mImplementationType = descriptor.ImplementationType;

                return this.CreateConstructorDependencyResolution(identity, mImplementationType, chain);
            }
            finally
            {
                chain.Remove(mServiceType);
            }
        }

        private IDependencyResolution CreateDependencyResolution(Type mServiceType, ServiceDependencyChain chain)
        {
            if (mServiceType.IsGenericTypeDefinition)
                return EmptyDependencyResolution.Shared;

            if (this.configuration.TryGetValue(mServiceType, out ServiceRegistration registration))
                return this.CreateDependencyResolution(registration.Descriptor, chain);

            if (!mServiceType.IsGenericType)
                return EmptyDependencyResolution.Shared;

            Type definition = mServiceType.GetGenericTypeDefinition();
            if (typeof(IEnumerable<>).Equals(definition))
                return this.CreateArrayDependencyResolution(mServiceType, chain);

            if (!this.configuration.TryGetValue(definition, out registration))
                return EmptyDependencyResolution.Shared;

            Type[] arguments = mServiceType.GenericTypeArguments;
            ServiceDescriptor descriptor = registration.Descriptor;
            Type mImplementationType = descriptor.ImplementationType.MakeGenericType(arguments);
            return this.CreateDependencyResolution(descriptor, chain, 0x0, mServiceType, mImplementationType);
        }

        private void CreateParameterDependencyResolution(ConstructorDescription description, ServiceDependencyChain chain, Nullable<bool> mThrowWhenNotFound = null)
        {
            int length = description.Parameters.Length;
            Type mImplementationType = description.Constructor.DeclaringType;
            for (int index = 0x0; index < length; index += 0x1)
            {
                ParameterInfo current = description.Parameters[index];
                IDependencyResolution resolution = this.GetResolution(current.ParameterType, chain);
                if (EmptyDependencyResolution.Shared.Equals(resolution))
                {
                    if (mThrowWhenNotFound.HasValue && mThrowWhenNotFound.Value)
                        throw Error.Invalid($"无法解析类型[{mImplementationType}]的依赖项[{current.ParameterType}]!");

                    description.UnResolved = true;
                    break;
                }
                description.Dependencies.Add(resolution);
            }
        }

        private ConstructorDescription LookupConstructorDescription(Type mImplementationType, ServiceDependencyChain chain)
        {
            ConstructorInfo[] constructors = mImplementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length < 0x1)
                throw Error.Invalid($"类型[{mImplementationType}]缺少公共构造方法!");

            ConstructorDescription result = default;
            if (constructors.Length < 0x2)
            {
                result = ConstructorDescription.From(constructors[0x0]);
                this.CreateParameterDependencyResolution(result, chain, true);
                return result;
            }
            ISet<Type> mSuperMap = default;
            IEnumerable<ConstructorDescription> descriptions = constructors.Select(current => ConstructorDescription.From(current));
            foreach (ConstructorDescription current in descriptions.OrderByDescending(x => x.Parameters.Length))
            {
                this.CreateParameterDependencyResolution(current, chain);
                if (current.UnResolved)
                    continue;

                if (result == null)
                {
                    result = current;
                    continue;
                }
                if (mSuperMap == null)
                    mSuperMap = new HashSet<Type>(result.Parameters.Select(x => x.ParameterType), TypeEqualityComparer.Shared);

                if (mSuperMap.IsSupersetOf(current.Parameters.Select(x => x.ParameterType)))
                    continue;

                throw Error.Invalid($"类型[{mImplementationType}]包含多个不明确的构造方法!");
            }
            return result;
        }

        private static ConcurrentDictionary<Type, IDependencyResolution> Initialize()
        {
            ConcurrentDictionary<Type, IDependencyResolution> result = new ConcurrentDictionary<Type, IDependencyResolution>(TypeEqualityComparer.Shared);
            result.TryAdd(typeof(IServiceProviderIsService), ServiceProviderIsServiceDependencyResolution.Shared);
            result.TryAdd(typeof(IUbxtrolServiceRegistry), UbxtrolServiceRegistryDependencyResolution.Shared);
            result.TryAdd(typeof(IServiceScopeFactory), ServiceScopeFactoryDependencyResolution.Shared);
            result.TryAdd(typeof(IServiceProvider), ServiceProviderDependencyResolution.Shared);
            return result;
        }

        public ServiceConfiguration(IReadOnlyDictionary<Type, ServiceRegistration> configuration, IServiceScopeValidator validator)
        {
            if (configuration == null)
                throw Error.ArgumentNull(nameof(configuration));

            if (validator == null)
                throw Error.ArgumentNull(nameof(validator));

            this.configuration = configuration;
            this.dependencies = ServiceConfiguration.Initialize();
            this.validator = validator;
        }

        public IDependencyResolution GetResolution(Type mServiceType, ServiceDependencyChain chain = null)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            if (this.dependencies.TryGetValue(mServiceType, out IDependencyResolution result))
                return result;

            if (chain == null)
                chain = new ServiceDependencyChain();

            result = this.CreateDependencyResolution(mServiceType, chain);
            this.validator.OnResolutionCreated(mServiceType, result);
            this.dependencies.TryAdd(mServiceType, result);
            return result;
        }
    }
}
