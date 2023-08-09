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

        private static void Destory(object value)
        {
            IAsyncDisposable disposable = value as IAsyncDisposable;
            if (disposable == null)
                return;

            ValueTask result = disposable.DisposeAsync();
            if (result.IsCompletedSuccessfully)
            {
                result.GetAwaiter().GetResult();
                return;
            }

            result.AsTask().GetAwaiter().GetResult();
        }

        private static async ValueTask DestoryAsync(ValueTask input, LinkedNode<object> node)
        {
            await input.ConfigureAwait(false);
            while (node != null)
            {
                object current = node.Value;
                IAsyncDisposable disposable = current as IAsyncDisposable;
                if (disposable != null)
                {
                    ValueTask item = disposable.DisposeAsync();
                    await item.ConfigureAwait(false);
                    node = node.Node;
                    continue;
                }

                ((IDisposable)current).Dispose();
                node = node.Node;
            }
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

                ServiceContainer.Destory(current);
                node = node.Node;
            }
        }

        public ValueTask DisposeAsync()
        {
            ValueTask result = default;
            LinkedNode<object> node = this.DisposeImpl();
            while (node != null)
            {
                object current = node.Value;
                IAsyncDisposable disposable = current as IAsyncDisposable;
                if (disposable != null)
                {
                    ValueTask item = disposable.DisposeAsync();
                    if (item.IsCompletedSuccessfully)
                    {
                        item.GetAwaiter().GetResult();
                        node = node.Node;
                        continue;
                    }

                    result = ServiceContainer.DestoryAsync(item, node.Node);
                    break;
                }

                ((IDisposable)current).Dispose();
                node = node.Node;
            }

            return result;
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

            bool mIsDisposed = false;
            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    mIsDisposed = true;
                else this.disposable.Append(result);
            }

            if (mIsDisposed)
            {
                IDisposable disposable = result as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
                else ServiceContainer.Destory(result);

                throw Error.Disposed(nameof(IServiceProvider));
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
