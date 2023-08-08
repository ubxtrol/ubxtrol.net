using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceContainer : IAsyncDisposable, IDisposable
    {
        private readonly IDictionary<ServiceKey, object> dependencies;

        private readonly LinkedList<object> disposable;

        private readonly object synchronization;

        public bool IsDisposed { get; private set; }

        public object Synchronization => this.synchronization;

        private LinkedListNode<object> ReadyToDispose()
        {
            LinkedListNode<object> result = null;
            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    return result;

                result = this.disposable.Last;
                this.IsDisposed = true;
            }

            return result;
        }

        public ServiceContainer()
        {
            this.dependencies = new Dictionary<ServiceKey, object>(ServiceKeyEqualityComparer.Shared);
            this.disposable = new LinkedList<object>();
            this.synchronization = new object();
        }

        public void Dispose()
        {
            LinkedListNode<object> node = this.ReadyToDispose();
            while (node != null)
            {
                IDisposable disposable = node.Value as IDisposable;
                if (disposable == null)
                    throw Error.Invalid("部分服务实例仅支持异步释放非托管资源!");

                disposable.Dispose(); node = node.Previous;
            }

            this.disposable.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            LinkedListNode<object> node = this.ReadyToDispose();
            while (node != null)
            {
                object current = node.Value;
                IAsyncDisposable disposable = current as IAsyncDisposable;
                if (disposable != null)
                    await disposable.DisposeAsync();
                else (current as IDisposable).Dispose();

                node = node.Previous;
            }

            this.disposable.Clear();
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

                this.disposable.AddLast(result);
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
