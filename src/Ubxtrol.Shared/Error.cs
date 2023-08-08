using System;

namespace Ubxtrol
{
    internal static class Error
    {
        public static ArgumentException Argument(string message, string value) => new ArgumentException(message, value);

        public static ArgumentNullException ArgumentNull(string value) => new ArgumentNullException(value);

        public static ObjectDisposedException Disposed(string value) => new ObjectDisposedException(value);

        public static InvalidOperationException Invalid(string message) => new InvalidOperationException(message);
    }
}
