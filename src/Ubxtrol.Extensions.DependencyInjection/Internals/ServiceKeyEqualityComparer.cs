using System;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceKeyEqualityComparer : IEqualityComparer<ServiceKey>
    {
        private static readonly Lazy<IEqualityComparer<ServiceKey>> Cache = new Lazy<IEqualityComparer<ServiceKey>>(() => new ServiceKeyEqualityComparer());

        public static IEqualityComparer<ServiceKey> Shared => ServiceKeyEqualityComparer.Cache.Value;

        public bool Equals(ServiceKey left, ServiceKey right)
        {
            if (left == null)
                throw Error.ArgumentNull(nameof(left));

            if (right == null)
                throw Error.ArgumentNull(nameof(right));

            bool result = left.Value.Equals(right.Value);
            return result && left.Index.Equals(right.Index);
        }

        public int GetHashCode(ServiceKey input)
        {
            if (input == null)
                throw Error.ArgumentNull(nameof(input));

            int result = input.Value.GetHashCode();
            return (result * 0x18D) ^ input.Index;
        }
    }
}
