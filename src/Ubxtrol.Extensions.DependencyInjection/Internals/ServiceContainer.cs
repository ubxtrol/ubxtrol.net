using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceContainer : IAsyncDisposable, IDisposable
    {
        private readonly IDictionary<ServiceKey, object> dependencies;

        private readonly LinkedBatch<object> disposable;

        private readonly object synchronization;

        public bool IsDisposed { get; private set; }

        public object Synchronization => this.synchronization;

        private LinkedNode<object> DisposeImpl()
        {
            LinkedNode<object> result = null;
            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    return result;

                result = this.disposable.Node;
                if (result != null)
                    this.disposable.Discard();

                this.IsDisposed = true;
            }

            return result;
        }

        public ServiceContainer()
        {
            this.dependencies = new Dictionary<ServiceKey, object>(ServiceKeyEqualityComparer.Shared);
            this.disposable = new LinkedBatch<object>();
            this.synchronization = new object();
        }

        public void Dispose()
        {
            LinkedNode<object> node = this.DisposeImpl();
            while (node != null)
            {
                object current = node.Value;
                IDisposable disposable = current as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                    node = node.Node;
                    continue;
                }

                ValueTask item = ((IAsyncDisposable)current).DisposeAsync();
                if (item.IsCompleted)
                {
                    node = node.Node;
                    continue;
                }

                item.AsTask().GetAwaiter().GetResult();
                node = node.Node;
            }
        }

        public async ValueTask DisposeAsync()
        {
            LinkedNode<object> node = this.DisposeImpl();
            while (node != null)
            {
                object current = node.Value;
                IAsyncDisposable disposable = current as IAsyncDisposable;
                if (disposable != null)
                {
                    await disposable.DisposeAsync();
                    node = node.Node;
                    continue;
                }

                ((IDisposable)current).Dispose();
                node = node.Node;
            }
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

            if (!(result is IAsyncDisposable) && !(result is IDisposable))
                return result;

            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    throw Error.Disposed(nameof(IServiceProvider));

                this.disposable.Append(result);
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
