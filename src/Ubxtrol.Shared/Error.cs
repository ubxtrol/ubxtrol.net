using System;

namespace Ubxtrol
{
    internal static class Error
    {
        public static ArgumentException Argument(string message, string value)
        {
            return new ArgumentException(message, value);
        }

        public static ArgumentNullException ArgumentNull(string value)
        {
            return new ArgumentNullException(value);
        }

        public static ObjectDisposedException Disposed(string value)
        {
            return new ObjectDisposedException(value);
        }

        public static InvalidOperationException Invalid(string message)
        {
            return new InvalidOperationException(message);
        }
    }
}
