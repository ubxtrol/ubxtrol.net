using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class StrictServiceScopeValidator : IServiceScopeValidator
    {
        private readonly ConcurrentDictionary<Type, Type> dependencies;

        private void OnDependencyResolutionCreated(ServiceScopeValidationContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            ServiceDependencyResolution resolution = context.Resolution as ServiceDependencyResolution;
            if (resolution == null)
                return;

            ServiceIdentity identity = resolution.Identity;
            switch (identity.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    context.SingletonServiceType = identity.Key.Value;
                    break;
                case ServiceLifetime.Scoped:
                    if (context.SingletonServiceType != null)
                        throw Error.Invalid($"依赖范围内服务[{identity.Key.Value}]的单例服务[{context.SingletonServiceType}]不能被创建!");

                    this.dependencies.TryAdd(context.ServiceType, identity.Key.Value);
                    break;
            }
            this.OnServiceDependencyResolutionCreated(context);
        }

        private void OnServiceDependencyResolutionCreated(ServiceScopeValidationContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            IEnumerableResolution resolution = context.Resolution as IEnumerableResolution;
            if (resolution == null)
                return;

            Type mSingletonServiceType = context.SingletonServiceType;
            using (IEnumerator<IDependencyResolution> enumerator = resolution.GetResolutionEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    context.Resolution = enumerator.Current;
                    this.OnDependencyResolutionCreated(context);
                    context.SingletonServiceType = mSingletonServiceType;
                }
            }
        }

        public StrictServiceScopeValidator()
        {
            this.dependencies = new ConcurrentDictionary<Type, Type>(TypeEqualityComparer.Shared);
        }

        public void BeforeDependencyResolutionResolved(Type mServiceType)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            if (!this.dependencies.TryGetValue(mServiceType, out Type value))
                return;

            if (mServiceType.Equals(value))
                throw Error.Invalid($"范围内的服务[{mServiceType}]不能从根服务提供对象创建!");

            throw Error.Invalid($"依赖范围内服务[{value}]的服务[{mServiceType}]不能从根服务提供对象创建!");
        }

        public void OnResolutionCreated(Type mServiceType, IDependencyResolution resolution)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            ServiceScopeValidationContext context = new ServiceScopeValidationContext(mServiceType);
            if (resolution != null)
                context.Resolution = resolution;

            this.OnDependencyResolutionCreated(context);
        }
    }
}
