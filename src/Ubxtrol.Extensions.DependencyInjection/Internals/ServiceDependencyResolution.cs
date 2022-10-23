using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal abstract class ServiceDependencyResolution : IDependencyResolution
    {
        private readonly ServiceIdentity identity;

        public ServiceIdentity Identity => this.identity;

        protected ServiceDependencyResolution(ServiceIdentity identity)
        {
            if (identity == null)
                throw Error.ArgumentNull(nameof(identity));

            this.identity = identity;
        }

        protected abstract void ExecuteServiceCreation(DependencyResolveContext context);

        public void Resolve(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            Nullable<ServiceLifetime> lifetime = context.Lifetime;
            try
            {
                context.Lifetime = this.identity.Lifetime;
                ServiceContainer container = context.Provider.Container;
                switch (context.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        container = context.Container;
                        break;
                    case ServiceLifetime.Transient:
                        this.ExecuteServiceCreation(context);
                        container.TryBlock(context.Result);
                        return;
                }
                ServiceKey key = this.identity.Key;
                lock (container.Synchronization)
                {
                    if (container.TryGetService(key, out object result))
                    {
                        context.Result = result;
                        return;
                    }
                    this.ExecuteServiceCreation(context);
                    container.Save(key, context.Result);
                }
            }
            finally
            {
                context.Lifetime = lifetime;
            }
        }
    }
}
