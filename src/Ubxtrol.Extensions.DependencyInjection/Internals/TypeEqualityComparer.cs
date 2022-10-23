using System;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class TypeEqualityComparer : IEqualityComparer<Type>
    {
        private static readonly Lazy<IEqualityComparer<Type>> Cache = new Lazy<IEqualityComparer<Type>>(() => new TypeEqualityComparer());

        public static IEqualityComparer<Type> Shared => TypeEqualityComparer.Cache.Value;

        public bool Equals(Type left, Type right)
        {
            if (left == null)
                throw Error.ArgumentNull(nameof(left));

            if (right == null)
                throw Error.ArgumentNull(nameof(right));

            return left.Equals(right);
        }

        public int GetHashCode(Type input)
        {
            if (input == null)
                throw Error.ArgumentNull(nameof(input));

            return input.GetHashCode();
        }
    }
}
