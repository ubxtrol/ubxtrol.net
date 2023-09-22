using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceContainer : IAsyncDisposable, IDisposable
    {
        private readonly IDictionary<ServiceKey, object> dependencies;

        private readonly StackBatch<Destroyable> disposable;

        private readonly object synchronization;

        public bool IsDisposed { get; private set; }

        public object Synchronization => this.synchronization;

        private IStackComponent<Destroyable> DisposeImpl()
        {
            IStackComponent<Destroyable> result = null;
            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    return result;

                result = this.disposable.Component;
                this.IsDisposed = true;
            }

            return result;
        }

        public ServiceContainer()
        {
            this.dependencies = new Dictionary<ServiceKey, object>(ServiceKeyEqualityComparer.Shared);
            this.disposable = new StackBatch<Destroyable>();
            this.synchronization = new object();
        }

        public void Dispose()
        {
            IStackComponent<Destroyable> component = this.DisposeImpl();
            while (component != null)
            {
                Destroyable current = component.Value;
                component = component.Component;
                current.Dispose();
            }

            this.disposable.Discard();
        }

        public async ValueTask DisposeAsync()
        {
            IStackComponent<Destroyable> component = this.DisposeImpl();
            while (component != null)
            {
                Destroyable current = component.Value;
                component = component.Component;

                ValueTask item = current.DisposeAsync();
                if (item.IsCompletedSuccessfully)
                    continue;

                await item.ConfigureAwait(false);
            }

            this.disposable.Discard();
        }

        public void Save(ServiceKey key, object value)
        {
            if (key == null)
                throw Error.ArgumentNull(nameof(key));

            this.dependencies.Add(key, value);
            this.TryBlock(value);
        }

        public object TryBlock(object result)
        {
            if (result == null)
                return result;

            Destroyable destroyable = Destroyable.From(result);
            if (destroyable == null)
                return result;

            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    Destroyable.ThrowDisposed(destroyable);

                this.disposable.Push(destroyable);
            }

            return result;
        }

        public bool TryGetService(ServiceKey key, out object value)
        {
            if (key == null)
                throw Error.ArgumentNull(nameof(key));

            if (this.IsDisposed)
                throw Error.Disposed(nameof(IServiceProvider));

            return this.dependencies.TryGetValue(key, out value);
        }
    }
}
