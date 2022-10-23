using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class EmptyDependencyResolution : IDependencyResolution
    {
        private static readonly Lazy<IDependencyResolution> Cache = new Lazy<IDependencyResolution>(() => new EmptyDependencyResolution());

        public static IDependencyResolution Shared => EmptyDependencyResolution.Cache.Value;

        public void Resolve(DependencyResolveContext context)
        { }
    }
}
