using System;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceDependencyChain
    {
        private readonly ISet<Type> chain;

        public ServiceDependencyChain()
        {
            this.chain = new HashSet<Type>(TypeEqualityComparer.Shared);
        }

        public void Remove(Type mServiceType)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            this.chain.Remove(mServiceType);
        }

        public void ThrowCircularDependencyWith(Type mServiceType)
        {
            if (mServiceType == null)
                throw Error.ArgumentNull(nameof(mServiceType));

            if (this.chain.Contains(mServiceType))
                throw Error.Invalid($"检测到服务[{mServiceType}]前后存在循环依赖!");

            this.chain.Add(mServiceType);
        }
    }
}
