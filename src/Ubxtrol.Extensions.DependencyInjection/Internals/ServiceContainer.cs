using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceContainer : IAsyncDisposable, IDisposable
    {
        private readonly IDictionary<ServiceKey, object> dependencies;

        private readonly IList<object> disposable;

        private readonly object synchronization;

        public bool IsDisposed { get; private set; }

        public object Synchronization => this.synchronization;

        private IReadOnlyCollection<object> ReadyToDispose()
        {
            Stack<object> result = new Stack<object>();
            lock (this.synchronization)
            {
                if (this.IsDisposed)
                    return result;

                this.IsDisposed = true;
                foreach (object current in this.disposable)
                    result.Push(current);

                this.disposable.Clear();
            }
            return result;
        }

        public ServiceContainer()
        {
            this.dependencies = new Dictionary<ServiceKey, object>(ServiceKeyEqualityComparer.Shared);
            this.disposable = new List<object>();
            this.synchronization = new object();
        }

        public void Dispose()
        {
            IReadOnlyCollection<object> result = this.ReadyToDispose();
            if (result.Count < 0x1)
                return;

            foreach (object current in result)
            {
                IDisposable disposable = current as IDisposable;
                if (disposable == null)
                    throw Error.Invalid("部分服务实例仅支持异步释放非托管资源!");

                disposable.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            IReadOnlyCollection<object> result = this.ReadyToDispose();
            if (result.Count < 0x1)
                return;

            foreach (object current in result)
            {
                IAsyncDisposable disposable = current as IAsyncDisposable;
                if (disposable != null)
                    await disposable.DisposeAsync();
                else (current as IDisposable).Dispose();
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

            lock (this.synchronization)
            {
                if (!(result is IAsyncDisposable) && !(result is IDisposable))
                    return result;

                this.disposable.Add(result);
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
