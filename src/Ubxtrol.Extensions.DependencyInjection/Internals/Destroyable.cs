using System;
using System.Threading.Tasks;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal sealed class Destroyable : IAsyncDisposable, IDisposable
    {
        private readonly object value;

        private static readonly ValueTask Completed = default;

        private Destroyable(object value)
        {
            if (value is null)
                throw Error.ArgumentNull(nameof(value));

            this.value = value;
        }

        public void Dispose()
        {
            IDisposable disposable = this.value as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                return;
            }

            IAsyncDisposable destroyable = this.value as IAsyncDisposable;
            if (destroyable == null)
                return;

            ValueTask result = destroyable.DisposeAsync();
            if (result.IsCompletedSuccessfully)
                return;

            result.AsTask().GetAwaiter().GetResult();
        }

        public ValueTask DisposeAsync()
        {
            IAsyncDisposable destroyable = this.value as IAsyncDisposable;
            if (destroyable != null)
                return destroyable.DisposeAsync();

            ValueTask result = Destroyable.Completed;
            IDisposable disposable = this.value as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            return result;
        }

        public static Destroyable From(object value)
        {
            Destroyable result = null;
            if (value is IAsyncDisposable || value is IDisposable)
                result = new Destroyable(value);

            return result;
        }

        public static void ThrowDisposed(Destroyable destroyable)
        {
            if (destroyable != null)
                destroyable.Dispose();

            throw Error.Disposed(nameof(IServiceProvider));
        }
    }
}
